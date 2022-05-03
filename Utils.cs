using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CharaReader
{
	public static unsafe class Utils
	{
		/// <summary>
		/// Converts a given primitive value to a hexadecimal string.
		/// </summary>
		/// <param name="val">The value to convert.</param>
		/// <param name="add_0x">Whether or not to include '0x'. ie. '0xFF' or 'FF'.</param>
		/// <returns></returns>
		public static string ToHexString(this object val, bool add_0x = true)
		{
			// get the type of the object
			Type type = val.GetType();
			// if the object is an array
			if (type.IsArray)
			{
				return $"{{ {string.Join(", ", ((object[])val).Select(a => a.ToHexString(add_0x)))} }}";
			}
			int size = val switch
			{
				char or byte or sbyte => 1,
				ushort or short => 2,
				uint or int or float or bool => 4,
				ulong or long or double => 8,
				_ => 0
			};
			if (size == 0)
			{
				return val.ToString();
			}
			if (val is float f)
			{
				return BitConverter.ToString(BitConverter.GetBytes(f).Reverse().ToArray()).Replace("-", "");
			}
			else if (val is double d)
			{
				return BitConverter.ToString(BitConverter.GetBytes(d).Reverse().ToArray()).Replace("-", "");
			}
			byte[] bytes = new byte[size];
			for (int i = 0; i < size; i++)
			{
				bytes[i] = (byte)(((dynamic)val >> (8 * i)) & 0xFF);
			}
			return $"{(add_0x ? "0x" : "")}{BitConverter.ToString(bytes.Reverse().ToArray()).Replace("-", "")}";
		}

		public static string ConvertToString(this object obj)
		{
			int indent = 0;
			string ret = RecursiveToString(obj, ref indent);
			return ret;
		}

		private static string RecursiveToString(object obj, ref int indentation)
		{
			string indent = BuildIndent(indentation);
			if (obj == null)
			{
				return "NULL";
			}
			Type obj_type = obj.GetType();
			if (obj_type.IsArray)
			{
				if (obj_type.GetElementType() == typeof(char))
				{
					return Program.shift_jis.GetString(((char[])obj).Select(a => (byte)a).ToArray());
				}

				Array nobj = obj as Array;

				if (nobj.Length > 0xFF)
				{
					return $"{obj_type.GetElementType().Name}[{nobj.Length:X}]";
				}

				string ret = $"{obj_type.GetElementType().Name}[{nobj.Length:X}]\n{indent}{{\n";
				indentation++;
				indent = BuildIndent(indentation);
				for (int i = 0; i < nobj?.Length; i++)
				{
					ret += $"{indent}{RecursiveToString(nobj.GetValue(i), ref indentation)}";
					if (i != nobj.Length - 1)
					{
						ret += $",\n";
					}
				}
				indentation--;
				indent = BuildIndent(indentation);
				ret += $"\n{indent}}}";
				return ret;
			}
			else if (obj_type.Name.StartsWith("List"))
			{
				return RecursiveToString(((dynamic)obj).ToArray(), ref indentation);
			}
			else if (obj_type.Name.StartsWith("Dictionary"))
			{
				string ret = $"\n{indent}{{\n";
				indentation++;
				indent = BuildIndent(indentation);
				for (int i = 0; i < ((dynamic)obj).Count; i++)
				{
					ret += $"{indent}{RecursiveToString(Enumerable.ElementAt(((dynamic)obj).Keys, i), ref indentation)}->{RecursiveToString(Enumerable.ElementAt(((dynamic)obj).Values, i), ref indentation)}";
					if (i != ((dynamic)obj).Count - 1)
					{
						ret += $",\n";
					}
				}
				indentation--;
				indent = BuildIndent(indentation);
				return ret + $"\n{indent}}}";
			}
			else if (obj_type == typeof(string))
			{
				return (string)obj;
			}
			else if (obj_type.IsPrimitive)
			{
				if (obj_type != typeof(char))
				{
					return obj.ToHexString();
				}
				else
				{
					return Program.shift_jis.GetString(new byte[] { (byte)(char)obj });
				}
			}
			else if (!obj_type.IsAbstract && !obj_type.IsEnum)
			{
				List<string> field_data = new();
				TypedReference tref = __makeref(obj);
				string ret = $"{obj_type.Name}\n{indent}{{\n";
				indentation++;
				indent = BuildIndent(indentation);
				FieldInfo[] fields = obj_type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					ret += $"{indent}{fields[i].Name} = {RecursiveToString(fields[i].GetValueDirect(tref), ref indentation)}";
					if (i != fields.Length - 1)
					{
						ret += $",\n";
					}
				}
				indentation--;
				indent = BuildIndent(indentation);
				ret += $"\n{indent}}}";
				return ret;
			}
			return obj.ToString();
		}

		private static string BuildIndent(int indentation)
		{
			string indent = "";
			for (int i = 0; i < indentation; i++)
			{
				indent += "    ";
			}
			return indent;
		}

		/// <summary>
		/// Calculates the size of an object.
		/// </summary>
		/// <param name="obj">The object in question.</param>
		/// <returns>The size of the object given.</returns>
		public static int Size(this object obj)
		{
			Type type = obj.GetType();
			int ret = 0;
			if (type.IsPrimitive)
			{
				if (type.IsArray)
				{
					foreach (dynamic val in obj as Array)
					{
						ret += Size(val);
					}
					return ret;
				}
				return ObjectSize(type);
			}
			else if (type.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase)
				|| obj is string)
			{
				return ObjectSize(obj);
			}
			else if (type.IsArray)
			{
				foreach (object val in obj as Array)
				{
					ret += Size(val);
				}
				return ret;
			}
			else if (obj is System.Collections.IList list)
			{
				foreach (object item in list)
				{
					ret += Size(item);
				}
				return ret;
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				if (field.FieldType.IsArray)
				{
					Array data = (Array)field.GetValue(obj);
					foreach (object val in data ?? Array.Empty<object>())
					{
						ret += Size(val);
					}
					ret += Size(field.FieldType.GetElementType());
				}
				else if (field.FieldType.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase))
				{
					string data = (string)field.GetValue(obj) ?? "";
					ret += ObjectSize(data);
				}
				else if ((field.FieldType.BaseType?.Name.Equals("Ptr_Custom`1")).GetValueOrDefault())
				{
					object val = field.GetValue(obj);
					if (val == null)
					{
						val = IntPtr.Zero;
					}
					ret += ObjectSize((IntPtr)val);
				}
				else
				{
					object val = field.GetValue(obj);
					if (val != null)
					{
						ret += Size(val);
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Calculates dynamic object sizes based on the name of the type given. Expects primitives but not direct values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private static int ObjectSize<T>(this T obj)
		{
			// if the object given is null, create a default instance
			obj ??= Activator.CreateInstance<T>();
			// get the object as a string
			string name = obj.ToString();
			// strings are direct values so if the object is a string, set the name to a string
			if (obj is string)
			{
				name = "string";
			}
			else if (obj is IntPtr)
			{
				name = "intptr";
			}
			// ignore any and all namespace inclusions
			name = name[(name.LastIndexOf('.') + 1)..].ToLowerInvariant();
			// set the return variable based on the name of the object
			int ret = name switch
			{
				"bool" => sizeof(bool),
				"bool[]" => sizeof(bool) * (((obj as bool[])?.Length ?? 0) + 1),
				"sbyte" => sizeof(sbyte),
				"sbyte[]" => sizeof(sbyte) * (((obj as sbyte[])?.Length ?? 0) + 1),
				"byte" => sizeof(byte),
				"byte[]" => sizeof(byte) * (((obj as byte[])?.Length ?? 0) + 1),
				"int16" => sizeof(short),
				"int16[]" => sizeof(short) * (((obj as short[])?.Length ?? 0) + 1),
				"uint16" => sizeof(ushort),
				"uint16[]" => sizeof(ushort) * (((obj as ushort[])?.Length ?? 0) + 1),
				"int32" => sizeof(int),
				"int32[]" => sizeof(int) * (((obj as int[])?.Length ?? 0) + 1),
				"uint32" => sizeof(uint),
				"uint32[]" => sizeof(uint) * (((obj as uint[])?.Length ?? 0) + 1),
				"int64" => sizeof(long),
				"int64[]" => sizeof(long) * (((obj as long[])?.Length ?? 0) + 1),
				"uint64" => sizeof(ulong),
				"uint64[]" => sizeof(ulong) * (((obj as ulong[])?.Length ?? 0) + 1),
				"single" => sizeof(float),
				"single[]" => sizeof(float) * (((obj as float[])?.Length ?? 0) + 1),
				"double" => sizeof(double),
				"double[]" => sizeof(double) * (((obj as double[])?.Length ?? 0) + 1),
				"char" => sizeof(char) / 2,
				"char[]" => (sizeof(char) / 2) * (((obj as char[])?.Length ?? 0) + 1),
				"string" => (sizeof(char) / 2) * (((obj as string)?.Length ?? 0) + 1),
				"string[]" => ((obj as string[])?.Sum(a => (a ?? "").ObjectSize()) ?? (sizeof(char) / 2)),
				"intptr" => sizeof(int),
				_ => throw new Exception($"Unhandled type: {obj}")
			};
			return ret;
		}

		public static string Align(this string value, int alignment)
		{
			while (alignment - (value.Length % alignment) != alignment)
			{
				value += '\0';
			}
			return value;
		}

		/// <summary>
		/// Reads a string of bytes from the given byte array.
		/// </summary>
		/// <param name="data">The array to read from.</param>
		/// <param name="offset">The index of the array to start reading from.</param>
		/// <param name="alignment">The alignment of the string.</param>
		/// <returns>A Shift-JIS string</returns>
		public static string ReadString(this byte[] data, int offset, int alignment = sizeof(int))
		{
			int end;
			for (end = offset; end < data.Length - 1; end++)
			{
				if (data[end] == 0)
				{
					break;
				}
			}
			if (end + (alignment - (end % alignment)) < data.Length)
			{
				end += alignment - (end % alignment);
			}
			else
			{
				end = data.Length;
			}
			return Program.shift_jis.GetString(data.AsSpan()[offset..end]);
		}

		public static bool TypeToEnum<T>(object o, out T value) where T : struct
		{
			string name = o.GetType().Name;
			string real_name = "";
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsUpper(name[i]))
				{
					real_name += '_';
				}
				real_name += char.ToUpper(name[i]);
			}
			return Enum.TryParse(real_name, out value);
		}

		public static bool EnumToType<T>(in T value, out Type o) where T : struct
		{
			// store the name of the value
			string name = $"{value}";
			string real_name = "";
			string[] sections = name.Split('_');
			for (int i = 0; i < sections.Length; i++)
			{
				if (sections[i].Length == 0)
				{
					real_name += "_";
					continue;
				}
				else if (byte.TryParse(sections[i], NumberStyles.HexNumber, null, out _))
				{
					real_name += sections[i];
				}
				else
				{
					real_name += $"{char.ToUpperInvariant(sections[i][0])}{sections[i][1..].ToLowerInvariant()}";
				}

				if (i + 1 < sections.Length)
				{
					real_name += "_";
				}
			}
			o = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(a => a.IsClass && !a.IsAbstract && a.Name == real_name);
			return o != null;
		}
	}
}
