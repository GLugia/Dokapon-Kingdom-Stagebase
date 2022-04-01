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
	}
}
