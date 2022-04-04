using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CharaReader.data
{
	public class DataWriter
	{
		private string _file;
		private byte[] _data;
		public ReadOnlySpan<byte> data => _data.AsSpan();
		public int offset;
		public string offset_hex => offset.ToHexString();
		public int length => _data?.Length ?? -1;
		private Dictionary<string, Pointer> reserved_offsets;

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
			_file = null;
			offset = 0;
			reserved_offsets.Clear();
			reserved_offsets = null;
			_data = null;
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
			if (obj.Equals(default))
			{
				return;
			}
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

		public void WriteStructs<T>(int id, in T[] objs) where T : struct
		{
			if (objs == null)
			{
				return;
			}
			TypedReference tref;
			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i].Equals(default(T)))
				{
					continue;
				}
				tref = __makeref(objs[i]);
				Write(id);
				foreach (FieldInfo field in objs[i].GetType().GetFields())
				{
					Write(field.GetValueDirect(tref));
				}
			}
		}

		public void WriteStructs<T>(int id, in T[][] objs) where T : struct
		{
			if (objs == null)
			{
				return;
			}
			TypedReference tref;
			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i] == null)
				{
					continue;
				}
				for (int j = 0; j < objs[i].Length; j++)
				{
					if (objs[i][j].Equals(default(T)))
					{
						continue;
					}
					tref = __makeref(objs[i][j]);
					Write(id);
					foreach (FieldInfo field in objs[i][j].GetType().GetFields())
					{
						Write(field.GetValueDirect(tref));
					}
				}
			}
		}

		public void WriteStructs<T>(int id, in T[][][] objs) where T : struct
		{
			if (objs == null)
			{
				return;
			}
			TypedReference tref;
			for (int i = 0; i < objs.Length; i++)
			{
				if (objs[i] == null)
				{
					continue;
				}
				for (int j = 0; j < objs[i].Length; j++)
				{
					if (objs[i][j] == null)
					{
						continue;
					}
					for (int k = 0; k < objs[i][j].Length; k++)
					{
						if (objs[i][j][k].Equals(default(T)))
						{
							continue;
						}
						tref = __makeref(objs[i][j][k]);
						Write(id);
						foreach (FieldInfo field in objs[i][j][k].GetType().GetFields())
						{
							Write(field.GetValueDirect(tref));
						}
					}
				}
			}
		}

		public void WriteDescriptions(object obj)
		{
			switch (obj)
			{
				case null:
					{
						break;
					}
				case Array arr:
					{
						foreach (object val in arr)
						{
							if (val == null)
							{
								continue;
							}
							WriteDescriptions(val);
						}
						break;
					}
				default:
					{
						Write(obj);
						break;
					}
			}
		}

		public void ReservePointer(int id, string name, int count = 1)
		{
			Write(id);
			reserved_offsets.Add(name, new Pointer
			{
				offset = offset,
				index = 0,
				count = count
			});
			offset += count * sizeof(int);
		}

		public void WritePointer(string name)
		{
			if (!reserved_offsets.TryGetValue(name, out Pointer ptr))
			{
				throw new KeyNotFoundException($"No pointer of name '{name}' was found.");
			}
			int temp_offset = offset;
			offset = ptr.offset + (ptr.index * sizeof(int));
			ptr.index++;
			if (ptr.index > ptr.count - 1)
			{
				reserved_offsets.Remove(name);
			}
			Write(temp_offset);
			offset = temp_offset;
		}

		public void WritePointerID(string name, dynamic id)
		{
			if (!reserved_offsets.TryGetValue(name, out Pointer ptr))
			{
				throw new KeyNotFoundException($"No pointer of name '{name}' was found.");
			}
			int temp_offset = offset;
			offset = ptr.offset + (ptr.index * sizeof(int));
			ptr.index += 2;
			if (ptr.index > ptr.count - 1)
			{
				reserved_offsets.Remove(name);
			}
			Write(id);
			Write(temp_offset);
			offset = temp_offset;
		}
	}

	public class Pointer
	{
		public int offset;
		public int index;
		public int count;
	}
}
