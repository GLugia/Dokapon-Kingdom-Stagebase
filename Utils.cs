﻿using System;
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

		public static int Size<T>(this T obj)
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

		public static string ReadString(this byte[] data, int offset, byte separator = 0)
		{
			int end;
			for (end = offset; end < data.Length - 1; end++)
			{
				if (data[end] == separator)
				{
					break;
				}
			}
			return Program.shift_jis.GetString(data.AsSpan()[offset..end]);
		}

		public static void SetPointers2(this byte[] data, int origin_offset, ref int[] ref_ptrs, int start_offset, int end_value, byte separator = 0, int alignment = sizeof(int))
		{
			int offset = start_offset - origin_offset;
			int temp;
			while ((temp = BitConverter.ToInt32(data.AsSpan()[offset..(offset + sizeof(int))])) != end_value)
			{
				offset += alignment;
				if (offset == data.Length - sizeof(int))
				{
					offset += sizeof(int);
					break;
				}
			}
			SetPointers(data, origin_offset, ref ref_ptrs, start_offset, offset + origin_offset, separator, alignment);
		}

		public static void SetPointers(this byte[] data, int origin_offset, ref int[] ref_ptrs, int start_offset, int end_offset, byte separator = 0, int alignment = sizeof(int))
		{
			int offset = start_offset - origin_offset;
			int end = end_offset - origin_offset;
			while (offset < end)
			{
				Array.Resize(ref ref_ptrs, ref_ptrs.Length + 1);
				ref_ptrs[^1] = offset;
				while (offset < end)
				{
					if (data[offset] == separator)
					{
						break;
					}
					offset++;
				}
				offset += alignment - (offset % alignment);
			}
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
