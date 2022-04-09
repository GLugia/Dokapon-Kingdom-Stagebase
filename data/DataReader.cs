using System;
using System.IO;
using System.Reflection;

namespace CharaReader.data
{
	public class DataReader
	{
		private byte[] _data;
		public ReadOnlySpan<byte> data => _data.AsSpan();
		public int offset;
		public string offset_hex => offset.ToHexString();
		public int length => _data?.Length ?? -1;
		public string length_hex => length.ToHexString();

		public DataReader(string file)
		{
			_data = File.ReadAllBytes(file);
			offset = 0;
		}

		public void Close()
		{
			_data = null;
			offset = 0;
		}

		public bool ReadBool()
		{
			bool ret = _data[offset] > 0;
			offset++;
			return ret;
		}

		public char ReadChar()
		{
			string ret = Program.shift_jis.GetString(_data, offset, 1);
			offset++;
			return ret[0];
		}

		public char[] ReadChars(int amount)
		{
			string ret = Program.shift_jis.GetString(_data, offset, amount);
			offset += amount;
			return ret.ToCharArray();
		}

		public string ReadString(int alignment = sizeof(int))
		{
			byte[] val = Array.Empty<byte>();
			while (val.Length < 256 && offset < _data.Length)
			{
				if (val.Length > 0 && (val[^1] == '\0' || val[^1] == 0))
				{
					break;
				}
				Array.Resize(ref val, val.Length + alignment);
				_data[offset..(offset + alignment)].CopyTo(val, val.Length - alignment);
				offset += alignment;
			}
			return Program.shift_jis.GetString(val);
		}

		public sbyte ReadSByte()
		{
			sbyte ret = (sbyte)_data[offset];
			offset += sizeof(sbyte);
			return ret;
		}

		public byte ReadByte()
		{
			byte ret = _data[offset];
			offset += sizeof(byte);
			return ret;
		}

		public byte[] ReadBytes(int amount)
		{
			byte[] ret = _data[offset..(offset + amount)];
			offset += amount;
			return ret;
		}

		public short ReadInt16()
		{
			short ret = BitConverter.ToInt16(_data.AsSpan()[offset..(offset + sizeof(short))]);
			offset += sizeof(short);
			return ret;
		}

		public ushort ReadUInt16()
		{
			ushort ret = BitConverter.ToUInt16(_data.AsSpan()[offset..(offset + sizeof(ushort))]);
			offset += sizeof(ushort);
			return ret;
		}

		public uint ReadUInt32()
		{
			uint ret = BitConverter.ToUInt32(_data.AsSpan()[offset..(offset + sizeof(uint))]);
			offset += sizeof(uint);
			return ret;
		}

		public int ReadInt32()
		{
			int ret = BitConverter.ToInt32(_data.AsSpan()[offset..(offset + sizeof(int))]);
			offset += sizeof(int);
			return ret;
		}

		public long ReadInt64()
		{
			long ret = BitConverter.ToInt64(_data.AsSpan()[offset..(offset + sizeof(long))]);
			offset += sizeof(long);
			return ret;
		}

		public ulong ReadUInt64()
		{
			ulong ret = BitConverter.ToUInt64(_data.AsSpan()[offset..(offset + sizeof(ulong))]);
			offset += sizeof(ulong);
			return ret;
		}

		public float ReadSingle()
		{
			float ret = BitConverter.ToSingle(_data.AsSpan()[offset..(offset + sizeof(float))]);
			offset += sizeof(float);
			return ret;
		}

		public double ReadDouble()
		{
			double ret = BitConverter.ToSingle(_data.AsSpan()[offset..(offset + sizeof(double))]);
			offset += sizeof(double);
			return ret;
		}

		public T[] ReadStructs<T>(int id) where T : struct
		{
			T[] ret = Array.Empty<T>();
			T temp;
			do
			{
				temp = ReadStruct<T>();
				if (!temp.TryGetField("item_id", out dynamic item_id))
				{
					throw new Exception("Struct does not contain the correct ids.");
				}
				if (item_id > ret.Length - 1)
				{
					Array.Resize(ref ret, item_id + 1);
				}
				ret[item_id] = temp;
			}
			while (ReadInt32() == id);
			offset -= sizeof(int);
			return ret;
		}

		public T[][] ReadStructs2<T>(int id) where T : struct
		{
			T[][] ret = Array.Empty<T[]>();
			T temp;
			do
			{
				temp = ReadStruct<T>();
				if (!temp.TryGetField("item_id", out dynamic item_id)
					|| !temp.TryGetField("gender_id", out dynamic gender_id))
				{
					throw new Exception("Struct does not contain the correct ids.");
				}
				if (item_id > ret.Length - 1)
				{
					Array.Resize(ref ret, item_id + 1);
					ret[item_id] = Array.Empty<T>();
				}
				if (gender_id > ret[item_id].Length - 1)
				{
					Array.Resize(ref ret[item_id], gender_id + 1);
				}
				ret[item_id][gender_id] = temp;
			}
			while (ReadInt32() == id);
			offset -= sizeof(int);
			return ret;
		}

		public T[][][] ReadStructs3<T>(int id) where T : struct
		{
			T[][][] ret = Array.Empty<T[][]>();
			T temp;
			do
			{
				temp = ReadStruct<T>();
				if (!temp.TryGetField("item_id", out dynamic item_id)
					|| !temp.TryGetField("gender_id", out dynamic gender_id)
					|| !temp.TryGetField("extra_id", out dynamic extra_id))
				{
					throw new Exception("Struct does not contain the correct ids.");
				}
				if (item_id > ret.Length - 1)
				{
					Array.Resize(ref ret, item_id + 1);
					ret[item_id] = Array.Empty<T[]>();
				}
				if (gender_id > ret[item_id].Length - 1)
				{
					Array.Resize(ref ret[item_id], gender_id + 1);
					ret[item_id][gender_id] = Array.Empty<T>();
				}
				if (extra_id > ret[item_id][gender_id].Length - 1)
				{
					Array.Resize(ref ret[item_id][gender_id], extra_id + 1);
				}
				ret[item_id][gender_id][extra_id] = temp;
			}
			while (ReadInt32() == id);
			offset -= sizeof(int);
			return ret;
		}

		public T ReadStruct<T>() where T : struct
		{
			T ret = Activator.CreateInstance<T>();
			TypedReference tref = __makeref(ret);
			foreach (FieldInfo info in ret.GetType().GetFields())
			{
				switch (info.FieldType.Name.ToLowerInvariant())
				{
					case "bool":
						{
							info.SetValueDirect(tref, ReadBool());
							break;
						}
					case "char":
						{
							info.SetValueDirect(tref, ReadChar());
							break;
						}
					case "char[]":
						{
							info.SetValueDirect(tref, ReadString().ToCharArray());
							break;
						}
					case "string":
						{
							info.SetValueDirect(tref, ReadString());
							break;
						}
					case "byte":
						{
							info.SetValueDirect(tref, ReadByte());
							break;
						}
					case "sbyte":
						{
							info.SetValueDirect(tref, ReadSByte());
							break;
						}
					case "uint16":
						{
							info.SetValueDirect(tref, ReadUInt16());
							break;
						}
					case "int16":
						{
							info.SetValueDirect(tref, ReadInt16());
							break;
						}
					case "uint32":
						{
							info.SetValueDirect(tref, ReadUInt32());
							break;
						}
					case "int32":
						{
							int val = ReadInt32();
							info.SetValueDirect(tref, val);
							break;
						}
					case "uint64":
						{
							info.SetValueDirect(tref, ReadUInt64());
							break;
						}
					case "int64":
						{
							info.SetValueDirect(tref, ReadInt64());
							break;
						}
					case "single":
						{
							info.SetValueDirect(tref, ReadSingle());
							break;
						}
					default:
						{
							throw new Exception($"Unhandled type '{info.FieldType}' in struct '{ret}'");
						}
				}
			}
			return ret;
		}
	}
}
