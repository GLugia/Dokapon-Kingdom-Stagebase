using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CharaReader
{
	public static unsafe class Utils
	{
		public static byte[] Read(this BinaryReader reader, int length, string name, byte print_type = 1)
		{
			Console.Out.Write($"Reading {name}... ");
			if (print_type > 2)
			{
				Console.Out.Write('\n');
			}
			byte[] ret = reader.ReadBytes(length);
			if (print_type > 0 && print_type < 3)
			{
				string results = "{ ";
				bool flag = false;
				foreach (byte b in ret)
				{
					if (flag)
					{
						results += ",";
					}
					results += b.ToString("x");
					flag = true;
				}
				Console.Out.Write(results + " }");
				Console.Out.Write(" / ");
			}
			object converted = "";
			switch (print_type)
			{
				case 1:
					{
						int val = BitConverter.ToInt32(ret);
						string add = "";
						if (val == 0x00)
						{
							val = ret.Length;
							add = "0x00 * ";
						}
						converted = $"{add}0x{val:x}";
						break;
					}
				case 3:
				case 2:
					{
						converted = Encoding.UTF8.GetString(ret);
						break;
					}
			}
			Console.Out.Write(converted);
			Console.Out.Write('\n');
			return ret;
		}

		public static void Write(this BinaryWriter writer, byte[] bytes, string name)
		{
			Console.Out.Write($"Writing {name}...");
			writer.Write(bytes);
			writer.Flush();
			Console.Out.Write("Done\n");
		}

		public static object ReadStruct(this BinaryReader reader)
		{
			int position = (int)reader.BaseStream.Position;
			int table_id = reader.ReadInt32();
			if (table_id == 0)
			{
				return null;
			}
			if (table_id > 0xFF)
			{
				Console.Out.WriteLine($"Invalid table_id at {position.ToHexString()}: {table_id.ToHexString()}");
				;
			}
			if (Structs.structs[table_id] == null)
			{
				Console.Out.WriteLine($"Null table_id at {position.ToHexString()}: {((byte)table_id).ToHexString()}");
				Console.Out.WriteLine($"Similar structs:");
				int total = 0;
				while (reader.ReadNextStructWithId((byte)table_id, out byte[] data))
				{
					Console.Out.WriteLine($"\t{{ {string.Join(" ", data.Select(a => (char)a))} }}");
					total += data.Length;
				}
				reader.BaseStream.Position -= total;
				Console.Out.Flush();
				throw new Exception();
			}
			Type target = Structs.structs[table_id];
			// Console.Out.WriteLine($"{target}: {position.ToHexString()}");
			object ret = Activator.CreateInstance(Structs.structs[table_id]);
			Type type = ret.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			if (fields.Length == 0)
			{
				;
			}
			fields[0].SetValue(ret, table_id);
			object read_value;
			string type_name;
			for (int i = 1; i < fields.Length; i++)
			{
				type_name = fields[i].FieldType.Name.ToLowerInvariant();
				read_value = type_name switch
				{
					"byte" => reader.ReadByte(),
					"byte[]" => reader.ReadBytes((int)fields.FirstOrDefault(a => a.Name == "end_of_data").GetValue(ret) - (int)reader.BaseStream.Position),
					"int16" => reader.ReadInt16(),
					"int32" => reader.ReadInt32(),
					"int32[]" => reader.ReadZeroTerminatedInts(),
					"int64" => reader.ReadInt64(),
					"single" => reader.ReadSingle(),
					"double" => reader.ReadDouble(),
					"char[]" => reader.ReadZeroTerminatedChars(),
					"string" => reader.ReadAlignedString(),
					"pointer[]" => reader.ReadZeroTerminatedPointers(),
					_ => throw new Exception($"Unhandled type: {type_name}")
				};
				// Console.Out.WriteLine($"\t{fields[i].FieldType} {fields[i].Name}: {read_value.ArrayToString()}");
				fields[i].SetValue(ret, read_value);
			}
			return ret;
		}

		public static int[] ReadZeroTerminatedInts(this BinaryReader reader)
		{
			int[] ret = Array.Empty<int>();
			int temp;
			while ((temp = reader.ReadInt32()) != 0)
			{
				Array.Resize(ref ret, ret.Length + 1);
				ret[^1] = temp;
			}
			return ret;
		}

		public static Pointer[] ReadZeroTerminatedPointers(this BinaryReader reader)
		{
			Pointer[] pointers = Array.Empty<Pointer>();
			while (reader.ReadInt32() == 1)
			{
				Array.Resize(ref pointers, pointers.Length + 1);
				pointers[^1] = new Pointer(reader.ReadInt32(), reader.ReadInt32());
			}
			return pointers;
		}

		private static bool ReadNextStructWithId(this BinaryReader reader, byte id, out byte[] data)
		{
			data = Array.Empty<byte>();
			byte temp;
			for (int i = 0; i < 64; i++)
			{
				Array.Resize(ref data, data.Length + 1);
				temp = reader.ReadByte();
				if (temp == id)
				{
					return true;
				}
				data[^1] = temp;
			}
			reader.BaseStream.Position -= data.Length;
			return false;
		}

		public static char[] ReadZeroTerminatedChars(this BinaryReader reader)
		{
			char[] ret = Array.Empty<char>();
			while (true)
			{
				if (reader.BaseStream.Position % 4 == 0 && ret.Length > 0 && (ret[^1] == '\0' || ret[^1] == 0))
				{
					break;
				}
				Array.Resize(ref ret, ret.Length + 1);
				ret[^1] = reader.ReadChar();
			}
			return ret;
		}

		public static string ReadAlignedString(this BinaryReader reader)
		{
			string ret = "";
			while (ret.Length < 64)
			{
				if (ret.Length > 0 && (ret[^1] == '\0' || ret[^1] == 0))
				{
					break;
				}
				ret += string.Join("", reader.ReadChars(4));
			}
			return ret;
		}

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
			Type type;
			if (obj != null)
			{
				if (obj is Type)
				{
					type = obj as Type;
				}
				else
				{
					type = obj.GetType();
				}
			}
			else
			{
				type = typeof(T);
			}
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
				return ObjectSize(obj);
			}
			if (type.IsArray)
			{
				return ObjectSize(obj);
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields)
			{
				if (field.FieldType.IsArray)
				{
					ret += Size(field.GetValue(obj));
				}
				else
				{
					ret += Size(field.FieldType);
				}
			}
			if (obj is System.Collections.IList list)
			{
				foreach (var item in list)
				{
					ret += Size(item);
				}
			}
			return ret;
		}

		private static int ObjectSize<T>(this T obj)
		{
			obj ??= Activator.CreateInstance<T>();
			string name = obj.ToString();
			name = name[(name.LastIndexOf('.') + 1)..].ToLowerInvariant();
			return name switch
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
				"char" => sizeof(char),
				"char[]" => sizeof(char) * (((obj as char[])?.Length ?? 0) + 1),
				"string" => sizeof(char) * (((obj as string)?.Length ?? 0) + 1),
				"pointer[]" => sizeof(Pointer) * (((obj as Pointer[])?.Length ?? 0) + 1),
				_ => throw new Exception($"Unhandled type: {obj}")
			};
		}
	}
}
