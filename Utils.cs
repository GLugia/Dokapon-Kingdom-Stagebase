using CharaReader.data;
using System;
using System.Linq;
using System.Reflection;

namespace CharaReader
{
	public static unsafe class Utils
	{
		public static string ToHexString(this object val, bool add_0x = true)
		{
			Type type = val.GetType();
			if (type.IsArray)
			{
				string arr_ret = "{ ";
				foreach (object obj in val as Array)
				{
					arr_ret += obj.ToHexString(add_0x) + " ";
				}
				return arr_ret + "}";
			}
			if (!(type.IsPrimitive && type.IsValueType))
			{
				return val.ToString();
			}
			string ret = (add_0x ? "0x" : "") + ((string)type.GetMethod("ToString", new Type[] { typeof(string) }).Invoke(val, new object[] { "x" })).ToUpperInvariant();
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
			while ((add_0x ? ret.Length - 2 : ret.Length) < size)
			{
				ret = ret.Insert(add_0x ? 2 : 0, "0");
			}
			return ret;
		}

		public static string ArrayToString(this object obj)
		{
			if (obj.GetType().IsArray)
			{
				Array? nobj = obj as Array;
				string ret = "{ ";
				for (int i = 0; i < nobj?.Length; i++)
				{
					ret += nobj.GetValue(i)?.ArrayToString();
					if (i != nobj.Length - 1)
					{
						ret += ", ";
					}
				}
				return ret + " }";
			}
			if (obj.GetType().IsPrimitive)
			{
				return $"{obj.ToHexString(false)}";
			}
			return obj.ToString();
		}

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
				foreach (var item in list)
				{
					ret += Size(item);
				}
				return ret;
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields)
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

		private static int ObjectSize<T>(this T obj)
		{
			obj ??= Activator.CreateInstance<T>();
			string name = obj.ToString();
			if (obj is string)
			{
				name = "string";
			}
			name = name[(name.LastIndexOf('.') + 1)..].ToLowerInvariant();
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

		public static bool TryGetField(this object obj, string name, out dynamic value)
		{
			FieldInfo info = obj.GetType().GetFields().FirstOrDefault(a => a.Name == name);
			value = info?.GetValue(obj);
			return info != null;
		}

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
			end += alignment - (end % alignment);
			return Program.shift_jis.GetString(data.AsSpan()[offset..end]);
		}

		/// <summary>
		/// Appends offsets of string values found in <paramref name="data"/> between <paramref name="start_offset"/> and <paramref name="end_offset"/>.
		/// If no <paramref name="end_offset"/> is given, reads until a 32bit value equals 0.
		/// Then sets <paramref name="description"/>.description to the bytes between <paramref name="start_offset"/> and <paramref name="end_offset"/>.
		/// </summary>
		/// <typeparam name="T">The alignment type of this read. Must be a primitive type.</typeparam>
		/// <param name="data">The array of bytes to read from.</param>
		/// <param name="origin_offset">The original offset from the file of the first index in <paramref name="data"/>.</param>
		/// <param name="description">The description to set.</param>
		/// <param name="start_offset">The file offset referenced as the first index of this read.</param>
		/// <param name="end_offset">The file offset referenced as the last index of this read.</param>
		public static void ReadDescription_String(this byte[] data, int origin_offset, ref Description description, int start_offset, int? end_offset = null, int alignment = sizeof(int))
		{
			int offset = start_offset - origin_offset;
			int end;
			if (end_offset != null)
			{
				end = end_offset.Value - origin_offset;
			}
			else
			{
				for (end = offset; end < data.Length - sizeof(int); end += sizeof(int))
				{
					if (BitConverter.ToUInt32(data, end) == 0)
					{
						break;
					}
				}
				if (end == data.Length - sizeof(int))
				{
					end = data.Length - 1;
				}
			}
			description.description = data[offset..end];
			for (; offset < end; offset += alignment - (offset % alignment))
			{
				Array.Resize(ref description.ptrs, description.ptrs.Length + 1);
				// the offset is based on the original data length. this is intended.
				// 0-index is description.ptrs[i] - description.ptrs[0]
				// this assumes pointers exist to begin with.
				description.ptrs[^1] = offset;
				for (; offset < end; offset++)
				{
					if (data[offset] == 0)
					{
						break;
					}
				}
			}
		}

		public static void ReadDescription_Array(this byte[] data, int origin_offset, ref Description description, int start_offset, int? end_offset, dynamic separator, int alignment = sizeof(int))
		{
			// get the 0-based-index to reference the data array
			int offset = start_offset - origin_offset;
			int end;
			int size = Size(separator);
			if (end_offset != null)
			{
				// this results to a value within the bounds of the data array
				end = end_offset.Value - origin_offset;
			}
			else
			{
				// if no end_offset is given, scan the array for the given separator
				for (end = offset; end < data.Length - 1; end++)
				{
					if (DynamicPeek(data, end, separator) == separator)
					{
						end += size;
						break;
					}
				}
			}
			// append the data to the description array
			Array.Resize(ref description.description, description.description.Length + (end - offset));
			data[offset..end].CopyTo(description.description, description.description.Length - (end - offset));
			Console.Out.WriteLine($"{offset.ToHexString()}:{end.ToHexString()}->{start_offset.ToHexString()}:{(end + origin_offset).ToHexString()}");
			for (; offset < end; offset += alignment - (offset % alignment))
			{
				Array.Resize(ref description.ptrs, description.ptrs.Length + 1);
				description.ptrs[^1] = offset;
				for (; offset + alignment < end; offset++)
				{
					dynamic value = DynamicPeek(data, offset, separator);
					if (value == separator)
					{
						offset += size;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Reads the <paramref name="data"/> from the next 0x03 object. This 0x03 object is specifically designed to contain
		/// raw 32bit values. These values are read and stored in <paramref name="description"/>. The data array in <paramref name="description"/>
		/// is completely ignored in this method.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="origin_offset"></param>
		/// <param name="description"></param>
		/// <param name="start_offset"></param>
		public static void ReadDescription_Value(this byte[] data, int origin_offset, ref Description description, int start_offset)
		{
			int offset = start_offset - origin_offset;
			while (offset + sizeof(int) < data.Length)
			{
				Array.Resize(ref description.ptrs, description.ptrs.Length + 1);
				description.ptrs[^1] = offset;
				offset += sizeof(int);
			}
			description.description = data;
		}

		/// <summary>
		/// Reads the <paramref name="data"/> from the next 0x03 object. This 0x03 object is specifically designed to contain
		/// offset pointers to indexes withing a following array of bytes. This method takes that array of bytes from <paramref name="data"/>
		/// and moves it to <paramref name="description"/> while also storing the offsets to each important index within the array.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="origin_offset"></param>
		/// <param name="description"></param>
		/// <param name="start_offset"></param>
		public static void ReadDescription_PointerArray(this byte[] data, int origin_offset, ref Description description, int start_offset)
		{
			int offset = start_offset - origin_offset;
			int end = data.Length - 1;
			int temp_pointer;
			int[] different_pointers = Array.Empty<int>();
			while (offset < end && (temp_pointer = BitConverter.ToInt32(data, offset)) != 0)
			{
				Array.Resize(ref description.ptrs, description.ptrs.Length + 1);
				description.ptrs[^1] = temp_pointer - origin_offset;
				if (Array.IndexOf(different_pointers, temp_pointer - origin_offset) == -1)
                {
					Array.Resize(ref different_pointers, different_pointers.Length + 1);
					different_pointers[^1] = temp_pointer - origin_offset;
                }
				offset += sizeof(int);
			}
			for (int i = 0; i < different_pointers.Length - 1; i++)
			{
				Array.Resize(ref description.description, description.description.Length + (different_pointers[i + 1] - different_pointers[i]));
				data[different_pointers[i]..different_pointers[i + 1]].CopyTo(description.description, description.description.Length - (different_pointers[i + 1] - different_pointers[i]));
			}
			for (offset = different_pointers[^1]; offset < end; offset++)
            {
				if (data[offset] == 0xFF)
                {
					break;
                }
			}
			offset++;
			Array.Resize(ref description.description, description.description.Length + (offset - different_pointers[^1]));
			data[different_pointers[^1]..offset].CopyTo(description.description, description.description.Length - (offset - different_pointers[^1]));
		}

		public static dynamic DynamicPeek<T>(byte[] data, int offset)
		{
			return typeof(T).Name.ToLowerInvariant() switch
			{
				"char" => Program.shift_jis.GetChars(data, offset, 1)[0],
				"byte" => data[offset],
				"sbyte" => (sbyte)data[offset],
				"bool" => BitConverter.ToBoolean(data, offset),
				"uint16" => BitConverter.ToUInt16(data, offset),
				"int16" => BitConverter.ToInt16(data, offset),
				"uint32" => BitConverter.ToUInt32(data, offset),
				"int32" => BitConverter.ToInt32(data, offset),
				"uint64" => BitConverter.ToUInt64(data, offset),
				"int64" => BitConverter.ToInt64(data, offset),
				"single" => BitConverter.ToSingle(data, offset),
				"double" => BitConverter.ToDouble(data, offset),
				_ => throw new Exception($"Invalid generic type: {typeof(T)}")
			};
		}

		public static dynamic DynamicPeek(byte[] data, int offset, dynamic type)
		{
			return type.GetType().Name.ToLowerInvariant() switch
			{
				"char" => Program.shift_jis.GetChars(data, offset, 1)[0],
				"byte" => data[offset],
				"sbyte" => (sbyte)data[offset],
				"bool" => BitConverter.ToBoolean(data, offset),
				"uint16" => BitConverter.ToUInt16(data, offset),
				"int16" => BitConverter.ToInt16(data, offset),
				"uint32" => BitConverter.ToUInt32(data, offset),
				"int32" => BitConverter.ToInt32(data, offset),
				"uint64" => BitConverter.ToUInt64(data, offset),
				"int64" => BitConverter.ToInt64(data, offset),
				"single" => BitConverter.ToSingle(data, offset),
				"double" => BitConverter.ToDouble(data, offset),
				_ => throw new Exception($"Invalid type: {type.GetType().Name}")
			};
		}

		public static Array ConvertTo(byte[] data, dynamic type)
		{
			Array ret;
			switch (type)
			{
				case null: return null;
				case bool:
					{
						ret = Array.CreateInstance(typeof(bool), data.Length / sizeof(bool));
						for (int i = 0; i < data.Length - sizeof(bool); i += sizeof(bool))
						{
							ret.SetValue(BitConverter.ToBoolean(data.AsSpan()[i..(i + sizeof(bool))]), i / sizeof(bool));
						}
						return ret;
					}
				case char:
					{
						ret = Array.CreateInstance(typeof(char), data.Length / sizeof(char));
						for (int i = 0; i < data.Length - sizeof(char); i += sizeof(char))
						{
							ret.SetValue(Program.shift_jis.GetString(data.AsSpan()[i..(i + sizeof(char))])[0], i / sizeof(char));
						}
						return ret;
					}
				case byte: return data;
				case sbyte:
					{
						ret = Array.CreateInstance(typeof(sbyte), data.Length / sizeof(sbyte));
						for (int i = 0; i < data.Length - sizeof(sbyte); i += sizeof(sbyte))
						{
							ret.SetValue((sbyte)data[i], i / sizeof(sbyte));
						}
						return ret;
					}
				case ushort:
					{
						ret = Array.CreateInstance(typeof(ushort), data.Length / sizeof(ushort));
						for (int i = 0; i < data.Length - sizeof(ushort); i += sizeof(ushort))
						{
							ret.SetValue(BitConverter.ToUInt16(data.AsSpan()[i..(i + sizeof(ushort))]), i / sizeof(ushort));
						}
						return ret;
					}
				case short:
					{
						ret = Array.CreateInstance(typeof(short), data.Length / sizeof(short));
						for (int i = 0; i < data.Length - sizeof(short); i += sizeof(short))
						{
							ret.SetValue(BitConverter.ToInt16(data.AsSpan()[i..(i + sizeof(short))]), i / sizeof(short));
						}
						return ret;
					}
				case uint:
					{
						ret = Array.CreateInstance(typeof(uint), data.Length / sizeof(uint));
						for (int i = 0; i < data.Length - sizeof(uint); i += sizeof(uint))
						{
							ret.SetValue(BitConverter.ToUInt32(data.AsSpan()[i..(i + sizeof(uint))]), i / sizeof(uint));
						}
						return ret;
					}
				case int:
					{
						ret = Array.CreateInstance(typeof(int), data.Length / sizeof(int));
						for (int i = 0; i < data.Length - sizeof(int); i += sizeof(int))
						{
							ret.SetValue(BitConverter.ToInt32(data.AsSpan()[i..(i + sizeof(int))]), i / sizeof(int));
						}
						return ret;
					}
				case ulong:
					{
						ret = Array.CreateInstance(typeof(ulong), data.Length / sizeof(ulong));
						for (int i = 0; i < data.Length - sizeof(ulong); i += sizeof(ulong))
						{
							ret.SetValue(BitConverter.ToUInt64(data.AsSpan()[i..(i + sizeof(ulong))]), i / sizeof(ulong));
						}
						return ret;
					}
				case long:
					{
						ret = Array.CreateInstance(typeof(long), data.Length / sizeof(long));
						for (int i = 0; i < data.Length - sizeof(long); i += sizeof(long))
						{
							ret.SetValue(BitConverter.ToInt64(data.AsSpan()[i..(i + sizeof(long))]), i / sizeof(long));
						}
						return ret;
					}
				case float:
					{
						ret = Array.CreateInstance(typeof(float), data.Length / sizeof(float));
						for (int i = 0; i < data.Length - sizeof(float); i += sizeof(float))
						{
							ret.SetValue(BitConverter.ToSingle(data.AsSpan()[i..(i + sizeof(float))]), i / sizeof(float));
						}
						return ret;
					}
				case double:
					{
						ret = Array.CreateInstance(typeof(double), data.Length / sizeof(double));
						for (int i = 0; i < data.Length - sizeof(double); i += sizeof(double))
						{
							ret.SetValue(BitConverter.ToDouble(data.AsSpan()[i..(i + sizeof(double))]), i / sizeof(double));
						}
						return ret;
					}
				default: throw new Exception($"Unhandled type: {type.GetType().Name}");
			}
		}
	}
}
