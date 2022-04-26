using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
				// allocate a return string for an array type
				string arr_ret = "{ ";
				// iterate the contents
				foreach (object obj in val as Array)
				{
					// recursively append the value as hex
					arr_ret += obj.ToHexString(add_0x) + " ";
				}
				// return the resulting string
				return arr_ret + "}";
			}
			// if the object is not primitive or a value type
			if (!(type.IsPrimitive && type.IsValueType))
			{
				// return a direct string result
				return val.ToString();
			}
			// allocate the return string and attempt to call ToString("x") which converts the type to hex for us
			string ret = (add_0x ? "0x" : "") + ((string)type.GetMethod("ToString", new Type[] { typeof(string) }).Invoke(val, new object[] { "X" })).ToUpperInvariant();
			// determine the alignment size based on the object
			int size = val switch
			{
				char => 2,
				sbyte => 2,
				byte => 2,
				short => 4,
				ushort => 4,
				int => 8,
				uint => 8,
				long => 16,
				ulong => 16,
				float => 8,
				double => 16,
				_ => 0
			};
			// while the length of the string is smaller than the alignment size
			while ((add_0x ? ret.Length - 2 : ret.Length) < size)
			{
				// insert a 0
				ret = ret.Insert(add_0x ? 2 : 0, "0");
			}
			// return the resulting string
			return ret;
		}

		/// <summary>
		/// Converts an object to a dynamic string.
		/// </summary>
		/// <param name="obj">The object to convert.</param>
		/// <returns>A string value.</returns>
		public static string ArrayToString(this object obj)
		{
			Type obj_type = obj.GetType();
			// if the object is an array
			if (obj_type.IsArray)
			{
				// cast it to the default array class
				Array nobj = obj as Array;
				// initialize a return value
				string ret = "{ ";
				// iterate the contents
				for (int i = 0; i < nobj?.Length; i++)
				{
					// recursively append the resulting string
					ret += nobj.GetValue(i)?.ArrayToString();
					// if this index is not the last
					if (i != nobj.Length - 1)
					{
						// append a comma separator
						ret += ", ";
					}
				}
				// return the resulting string
				return ret + " }";
			}
			// if the object is a primitive type
			if (obj_type.IsPrimitive)
			{
				// return the object as hex
				return $"{obj.ToHexString(false)}";
			}
			if (!obj_type.IsAbstract && !obj_type.IsEnum)
			{
				List<string> field_data = new();
				TypedReference tref = __makeref(obj);
				foreach (FieldInfo field in obj_type.GetFields())
				{
					field_data.Add($"{field.Name,-16} = {field.GetValueDirect(tref)}");
				}
				return $"{obj_type.Name}\n{{\n\t{string.Join("\n\t", field_data)}\n}}\n";
			}
			// return the default string
			return obj.ToString();
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
				_ => throw new Exception($"Unhandled type: {obj}")
			};
			return ret;
		}

		/// <summary>
		/// Attempts to get a field from an object.
		/// </summary>
		/// <param name="obj">The object whats field to query.</param>
		/// <param name="name">The name of the field to query.</param>
		/// <param name="value">The field's value if it exists.</param>
		/// <returns><see langword="false"/> if the field does not exist. <see langword="true"/> otherwise.</returns>
		public static bool TryGetField(this object obj, string name, out dynamic value)
		{
			// attempt to find the first instance of the field with the same name
			FieldInfo info = obj.GetType().GetFields().FirstOrDefault(a => a.Name == name);
			// set the out parameter to the resulting value of the field obtained
			value = info?.GetValue(obj);
			// return if the field existed
			return info != null;
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
	}
}
