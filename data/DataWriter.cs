using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			if (_data.Length % 16 != 0)
			{
				Array.Resize(ref _data, _data.Length + (16 - (_data.Length % 16)));
			}
			File.WriteAllBytes(_file, _data);
			_file = null;
			offset = 0;
			reserved_offsets.Clear();
			reserved_offsets = null;
			_data = null;
		}

		public void Write(object val, int alignment = sizeof(int))
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
						Write((string)val, alignment);
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

		public void Write(string val, int alignment = sizeof(int))
		{
			byte[] data = Program.shift_jis.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + data.Length);
			}
			data.CopyTo(_data, offset);
			offset += data.Length + (offset % alignment);
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

		public void Write<T>(Span<T> val)
		{
			if (offset + val.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + val.Length + 1);
			}
			val.ToArray().CopyTo(_data, offset);
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

		public void WriteAllPointers(string base_name, object ending) // ending can be an int offset or 'end'
		{
			List<string> keys_to_remove = new();
			int old_offset;
			foreach ((string key, Pointer ptr) in reserved_offsets)
			{
				if (key.StartsWith($"{base_name}_") && key.EndsWith($"_{ending}"))
				{
					old_offset = offset;
					offset = ptr.offset;
					Write(old_offset);
					keys_to_remove.Add(key);
					offset = old_offset;
				}
			}
			foreach (string key in keys_to_remove)
			{
				reserved_offsets.Remove(key);
			}
		}

		/// <summary>
		/// Handles writing to all pointers that reference an offset comparative to the origin offset of the byte array.
		/// </summary>
		/// <param name="base_name">The base name of all pointers referencing this Description.</param>
		/// <param name="description">The description to write.</param>
		/// <param name="origin_offset">The offset of the first byte in this 0x03 object.</param>
		public void WriteDescriptions(string base_name, Description description, int origin_offset)
		{
			if (description.description.Length <= 0)
			{
				return;
			}
			int real_offset = offset - origin_offset;
			for (int i = 0; i < description.description.Length; i++)
			{
				WriteAllPointers(base_name, i + real_offset);
				Write(description.description[i]);
			}
		}

		public void WriteDescriptions(string base_name, Description[] descriptions, dynamic separator)
		{
			byte[] total = Array.Empty<byte>();
			int separator_size = Utils.Size(separator);
			for (int i = 0; i < descriptions.Length; i++)
			{
				if (descriptions[i] == null || (descriptions[i] != null && (descriptions[i].ptrs.Length == 0 || descriptions[i].description.Length == 0)))
				{
					continue;
				}
				int index = 0;
				for (int j = 0; j < descriptions[i].ptrs.Length; j++)
				{
					int end;
					for (end = index + 1; end + separator_size < descriptions[i].description.Length; end++)
					{
						dynamic value = Utils.DynamicPeek(descriptions[i].description, end, separator);
						if (value == separator)
						{
							end += separator_size;
							break;
						}
					}
					if (end + separator_size >= descriptions[i].description.Length)
					{
						end = descriptions[i].description.Length;
					}
					byte[] data = descriptions[i].description[index..end];
					if (descriptions[i].ptrs[j] + data.Length > total.Length - 1)
					{
						Array.Resize(ref total, descriptions[i].ptrs[j] + data.Length);
					}
					data.CopyTo(total, descriptions[i].ptrs[j]);
					index = end;
				}
			}
			for (int i = 0; i < total.Length; i++)
			{
				WriteAllPointers(base_name, i);
				Write(total[i]);
			}
		}

		/// <summary>
		/// Handles writing the stored byte array in a Description
		/// </summary>
		/// <param name="description">The Description to write.</param>
		public void WriteDescriptions(Description description)
		{
			if (description.description.Length <= 0)
			{
				return;
			}
			Write(description.description);
		}

		public void ReservePointer(dynamic id, string name, int count = 1)
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

		public void ReservePointerNoID(string name, int count = 1)
		{
			reserved_offsets.Add(name, new Pointer
			{
				offset = offset,
				index = 0,
				count = count
			});
			offset += count * sizeof(int);
		}

		public void ReservePointerArray(string name, int[] ptrs)
		{
			for (int i = 0; i < ptrs.Length; i++)
			{
				Write(1); // declare that there is another pointer
				Write(i); // the id of this pointer
				reserved_offsets.Add($"{name}_{i}_{ptrs[i]}", new Pointer
				{
					offset = offset,
					index = 0,
					count = 1
				});
				offset += sizeof(int);
			}
			Write(0);
		}

		public void WritePointer(string name, bool remove = true)
		{
			if (!reserved_offsets.TryGetValue(name, out Pointer ptr))
			{
				throw new KeyNotFoundException($"No pointer of name '{name}' was found.");
			}
			int temp_offset = offset;
			offset = ptr.offset + (ptr.index * sizeof(int));
			ptr.index++;
			if (remove && ptr.index > ptr.count - 1)
			{
				reserved_offsets.Remove(name);
			}
			Write(temp_offset);
			offset = temp_offset;
		}

		public void ForcePointer(string name, int forced_offset)
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
			Write(forced_offset);
			offset = temp_offset;
		}

		public void WritePointerID(string name, dynamic id, bool remove = true)
		{
			if (!reserved_offsets.TryGetValue(name, out Pointer ptr))
			{
				throw new KeyNotFoundException($"No pointer of name '{name}' was found.");
			}
			int temp_offset = offset;
			offset = ptr.offset + (ptr.index * sizeof(int));
			ptr.index += 2;
			if (remove && ptr.index > ptr.count - 1)
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
