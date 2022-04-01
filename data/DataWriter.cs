using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CharaReader.data
{
	public class DataWriter
	{
		private readonly string _file;
		private byte[] _data;
		public ReadOnlySpan<byte> data => _data.AsSpan();
		public int offset;
		public int length => _data?.Length ?? -1;
		public Dictionary<string, int> reserved_offsets;

		public DataWriter(string file)
		{
			_file = file;
			_data = Array.Empty<byte>();
			offset = 0;
			reserved_offsets = new();
		}

		public void Close()
		{
			Array.Resize(ref _data, _data.Length + (_data.Length % 16));
			File.WriteAllBytes(_file, _data);
		}

		public void Write(object val)
		{
			switch (val)
			{
				case null:
					{
						break;
					}
				case bool:
					{
						Write((bool)val);
						break;
					}
				case char:
					{
						Write((char)val);
						break;
					}
				case char[]:
					{
						Write((char[])val);
						break;
					}
				case string:
					{
						Write((string)val);
						break;
					}
				case sbyte:
					{
						Write((sbyte)val);
						break;
					}
				case byte:
					{
						Write((byte)val);
						break;
					}
				case byte[]:
					{
						Write((byte[])val);
						break;
					}
				case short:
					{
						Write((short)val);
						break;
					}
				case ushort:
					{
						Write((ushort)val);
						break;
					}
				case int:
					{
						Write((int)val);
						break;
					}
				case uint:
					{
						Write((uint)val);
						break;
					}
				case long:
					{
						Write((long)val);
						break;
					}
				case ulong:
					{
						Write((ulong)val);
						break;
					}
				case float:
					{
						Write((float)val);
						break;
					}
				default:
					{
						throw new Exception($"Unhandled type: {val.GetType()}::{val}");
					}
			}
		}

		public void Write(bool val)
		{
			if (offset > data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			_data[offset] = (byte)(val ? 1 : 0);
			offset++;
		}

		public void Write(char val)
		{
			if (offset > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			_data[offset] = (byte)val;
			offset++;
		}

		public void Write(char[] val)
		{
			if (offset + val.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + val.Length);
			}
			val.CopyTo(_data, offset);
			offset += val.Length;
		}

		public void Write(string val)
		{
			byte[] data = Program.shift_jis.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length);
			}
			data.CopyTo(_data, offset);
			offset += data.Length + (offset % 4);
		}

		public void Write(sbyte val)
		{
			if (offset > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			_data[offset] = (byte)val;
			offset++;
		}

		public void Write(byte val)
		{
			if (offset > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			_data[offset] = val;
			offset++;
		}

		public void Write(byte[] val)
		{
			if (offset + val.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + val.Length + 1);
			}
			val.CopyTo(_data, offset);
			offset += val.Length;
		}

		public void Write(short val)
		{
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length);
			}
			data.CopyTo(_data, offset);
			offset += data.Length;
		}

		public void Write(ushort val)
		{
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length);
			}
			data.CopyTo(_data, offset);
			offset += data.Length;
		}

		public void Write(int val)
		{
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length + 1);
			}
			data.CopyTo(_data, offset);
			offset += data.Length;
		}

		public void Write(uint val)
		{
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length + 1);
			}
			data.CopyTo(_data, offset);
			offset += data.Length;
		}

		public void Write(long val)
		{
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length + 1);
			}
			data.CopyTo(_data, offset);
			offset += data.Length;
		}

		public void Write(ulong val)
		{
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length + 1);
			}
			data.CopyTo(_data, offset);
			offset += data.Length;
		}

		public void Write(float val)
		{
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length + 1);
			}
			data.CopyTo(_data, offset);
			offset += data.Length;
		}

		public void WriteStruct<T>(T obj) where T : struct
		{
			TypedReference tref = __makeref(obj);
			object val;
			foreach (FieldInfo field in obj.GetType().GetFields())
			{
				val = field.GetValueDirect(tref);
				if (val != null)
				{
					Write(val);
				}
			}
		}
	}
}
