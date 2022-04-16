using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CharaReader.data
{
	public class DataWriter
	{
		/// <summary>
		/// The file this buffer will write to.
		/// </summary>
		private string _file;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] _data;
		/// <summary>
		/// A public, readonly instance of the buffer.
		/// </summary>
		public ReadOnlySpan<byte> data => _data.AsSpan();
		/// <summary>
		/// The current offset of the buffer.
		/// </summary>
		public int offset;
		/// <summary>
		/// The current offset of the buffer in hex.
		/// </summary>
		public string offset_hex => offset.ToHexString();
		/// <summary>
		/// The current length of the buffer. Defaults to -1 if the buffer is <see langword="null"/>.
		/// </summary>
		public int length => _data?.Length ?? -1;
		/// <summary>
		/// A list of named offsets reserved for pointer use.
		/// </summary>
		private Dictionary<string, Pointer> reserved_offsets;

		/// <summary>
		/// Initializes the necessary fields for creating a new instance of <see cref="DataWriter"/>.
		/// </summary>
		/// <param name="file">The file to read from.</param>
		public DataWriter(string file)
		{
			_file = file;
			_data = Array.Empty<byte>();
			offset = 0;
			reserved_offsets = new();
		}

		/// <summary>
		/// Close this reader.
		/// </summary>
		public void Close()
		{
			// align the data to 16
			if (_data.Length % 16 != 0)
			{
				Array.Resize(ref _data, _data.Length + (16 - (_data.Length % 16)));
			}
			// write the data to file
			File.WriteAllBytes(_file, _data);
			// null the file name
			_file = null;
			// zero the offset
			offset = 0;
			// clear any reserved offsets
			reserved_offsets.Clear();
			// null the list
			reserved_offsets = null;
			// null the data
			_data = null;
		}

		/// <summary>
		/// Handles writing primitive objects that may not be immediately known. ie. dynamics.
		/// </summary>
		/// <param name="val">The value to write.</param>
		/// <param name="alignment">The alignment of the data.</param>
		/// <exception cref="Exception">If <paramref name="val"/> is not a primitive type.</exception>
		public void Write(object val, int alignment = sizeof(int))
		{
			switch (val)
			{
				case bool: Write((bool)val); break;
				case char: Write((char)val); break;
				case char[]: Write((char[])val); break;
				case string: Write((string)val, alignment); break;
				case sbyte: Write((sbyte)val); break;
				case byte: Write((byte)val); break;
				case byte[]: Write((byte[])val); break;
				case short: Write((short)val); break;
				case ushort: Write((ushort)val); break;
				case int: Write((int)val); break;
				case uint: Write((uint)val); break;
				case long: Write((long)val); break;
				case ulong: Write((ulong)val); break;
				case float: Write((float)val); break;
				case double: Write((double)val); break;
				// if it's null or unhandled, throw an error
				case null:
				default: throw new Exception($"Unhandled type: {val.GetType()}::{val}");
			}
		}

		/// <summary>
		/// Writes a b8 value to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
		public void Write(bool val)
		{
			// if the offset is larger than the length, resize to fit
			if (offset > data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			// set the value at the current offset to the value given
			_data[offset] = (byte)(val ? 1 : 0);
			// increment the offset
			offset++;
		}

		/// <summary>
		/// Writes a char value to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
		public void Write(char val)
		{
			if (offset > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			_data[offset] = (byte)val;
			offset++;
		}

		/// <summary>
		/// Writes an array of chars to the buffer.
		/// </summary>
		/// <param name="val">The array to write to the buffer.</param>
		public void Write(char[] val)
		{
			if (offset + val.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + val.Length);
			}
			val.CopyTo(_data, offset);
			offset += val.Length;
		}

		/// <summary>
		/// Writes a string to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
		/// <param name="alignment">The alignment of this data.</param>
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

		/// <summary>
		/// Writes an s8 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
		public void Write(sbyte val)
		{
			if (offset > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			_data[offset] = (byte)val;
			offset++;
		}

		/// <summary>
		/// Writes a u8 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
		public void Write(byte val)
		{
			if (offset > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + 1);
			}
			_data[offset] = val;
			offset++;
		}

		/// <summary>
		/// Writes a u8 array to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
		public void Write(byte[] val)
		{
			if (offset + val.Length > _data.Length - 1)
			{
				Array.Resize(ref _data, offset + val.Length + 1);
			}
			val.CopyTo(_data, offset);
			offset += val.Length;
		}

		/// <summary>
		/// Writes an s16 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
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

		/// <summary>
		/// Writes a u16 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
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

		/// <summary>
		/// Writes an s32 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
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

		/// <summary>
		/// Writes a u32 to the buffer.
		/// </summary>
		/// <param name="val"></param>
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

		/// <summary>
		/// Writes an s64 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
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

		/// <summary>
		/// Writes a u64 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
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

		/// <summary>
		/// Writes an f32 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
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

		/// <summary>
		/// Writes an f64 to the buffer.
		/// </summary>
		/// <param name="val">The value to write to the buffer.</param>
		public void Write(double val)
        {
			byte[] data = BitConverter.GetBytes(val);
			if (offset + data.Length > _data.Length - 1)
            {
				Array.Resize(ref _data, offset + data.Length + 1);
            }
			data.CopyTo(_data, offset);
			offset += data.Length;
        }

		/// <summary>
		/// Writes an array of structs.
		/// </summary>
		/// <typeparam name="T">The type of struct to write.</typeparam>
		/// <param name="id">The id of this section.</param>
		/// <param name="objs">The array of structs to write.</param>
		public void WriteStructs<T>(int id, in T[] objs) where T : struct
		{
			// if the object given is null, return early
			if (objs == null)
			{
				return;
			}
			// initialize a typed reference
			TypedReference tref;
			// iterate over the items in this array
			for (int i = 0; i < objs.Length; i++)
			{
				// if it is a default instance of this struct, skip it
				if (objs[i].Equals(default(T)))
				{
					continue;
				}
				// make a reference to this struct
				tref = __makeref(objs[i]);
				// write the id
				Write(id);
				// iterate over the fields in this struct
				foreach (FieldInfo field in objs[i].GetType().GetFields())
				{
					// write the dynamic value of this field
					Write(field.GetValueDirect(tref));
				}
			}
		}

		/// <summary>
		/// Writes a double array of structs.
		/// </summary>
		/// <typeparam name="T">The type of struct to write.</typeparam>
		/// <param name="id">The id of this section.</param>
		/// <param name="objs">The array of structs to write.</param>
		public void WriteStructs<T>(int id, in T[][] objs) where T : struct
		{
			// if the object given is null, return early
			if (objs == null)
			{
				return;
			}
			// initialize a typed reference
			TypedReference tref;
			// iterate over the double array
			for (int i = 0; i < objs.Length; i++)
			{
				// if the array at this index is null, skip it
				if (objs[i] == null)
				{
					continue;
				}
				// iterate over the items in this array
				for (int j = 0; j < objs[i].Length; j++)
				{
					// if it is a default instance of this struct, skip it
					if (objs[i][j].Equals(default(T)))
					{
						continue;
					}
					// make a reference to this struct
					tref = __makeref(objs[i][j]);
					// write the id
					Write(id);
					// iterate over the fields in this struct
					foreach (FieldInfo field in objs[i][j].GetType().GetFields())
					{
						// write the dynamic value of this field
						Write(field.GetValueDirect(tref));
					}
				}
			}
		}

		/// <summary>
		/// Writes a triple array of structs.
		/// </summary>
		/// <typeparam name="T">The type of struct to write.</typeparam>
		/// <param name="id">The id of this section.</param>
		/// <param name="objs">The array of structs to write.</param>
		public void WriteStructs<T>(int id, in T[][][] objs) where T : struct
		{
			// if the object given is null, return early
			if (objs == null)
			{
				return;
			}
			// initialize a typed reference
			TypedReference tref;
			// iterate over the triple array
			for (int i = 0; i < objs.Length; i++)
			{
				// if the double array at this index is null, skip it
				if (objs[i] == null)
				{
					continue;
				}
				// iterate over the double array
				for (int j = 0; j < objs[i].Length; j++)
				{
					// if the array at this index is null, skip it
					if (objs[i][j] == null)
					{
						continue;
					}
					// iterate over the items in this array
					for (int k = 0; k < objs[i][j].Length; k++)
					{
						// if it is a default instance of this struct, skip it
						if (objs[i][j][k].Equals(default(T)))
						{
							continue;
						}
						// make a reference to this struct
						tref = __makeref(objs[i][j][k]);
						// write the id
						Write(id);
						// iterate over the fields in this struct
						foreach (FieldInfo field in objs[i][j][k].GetType().GetFields())
						{
							// write the dynamic value of this field
							Write(field.GetValueDirect(tref));
						}
					}
				}
			}
		}

		/// <summary>
		/// Writes the current offset to all reserved pointers starting with <paramref name="base_name"/> and ending with
		/// <paramref name="ending"/>. This is not the best way to do this but it is the easiest. This only writes one
		/// offset per pointer before removing it from the list.
		/// </summary>
		/// <param name="base_name">A constant string used in the start of reserved pointers' names.</param>
		/// <param name="ending">Any object that a reserved pointer's name can end with.</param>
		public void WriteAllPointers(string base_name, object ending) // ending can be an int offset or 'end'
		{
			// allocate a new list of strings
			List<string> keys_to_remove = new();
			// initialize an s32 value
			int old_offset;
			// iterate the currently reserved offsets
			foreach ((string key, Pointer ptr) in reserved_offsets)
			{
				// if the current object's key contains the given parameters
				if (key.StartsWith($"{base_name}_") && key.EndsWith($"_{ending}"))
				{
					// mark the current offset
					old_offset = offset;
					// jump to the pointer's offset
					offset = ptr.offset;
					// write the marked offset
					Write(old_offset);
					// save this pointer's key
					keys_to_remove.Add(key);
					// jump back to the original offset
					offset = old_offset;
				}
			}
			// iterate over the saved keys
			foreach (string key in keys_to_remove)
			{
				// remove them from the reserved offsets
				reserved_offsets.Remove(key);
			}
		}

		/// <summary>
		/// Handles writing to all pointers that reference an offset comparative to the origin offset of the byte array.
		/// This is used when a 0x03 object contains multiple <see cref="Description"/> objects.
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

		/// <summary>
		/// Writes an array of <see cref="Description"/>s by scanning through the given <paramref name="descriptions"/> array.
		/// </summary>
		/// <param name="base_name">The start of the names of pointers refering to this array of <see cref="Description"/>s</param>
		/// <param name="descriptions">The <see cref="Description"/> array to write to the buffer.</param>
		/// <param name="separator">The primitive value to write between each object. (ie. 0xFF or '\0')</param>
		public void WriteDescriptions(string base_name, Description[] descriptions, dynamic separator)
		{
			// initialize an array of bytes
			byte[] total = Array.Empty<byte>();
			// get the size of the dynamic separator parameter
			int separator_size = Utils.Size(separator);
			// iterate through the descriptions array
			for (int i = 0; i < descriptions.Length; i++)
			{
				// if the description in this index is null or empty, skip it
				if (descriptions[i] == null || (descriptions[i] != null && (descriptions[i].ptrs.Length == 0 || descriptions[i].description.Length == 0)))
				{
					continue;
				}
				// initialize a value to remember the last index in the 'total' array
				int index = 0;
				// iterate through the pointers in the current description
				for (int j = 0; j < descriptions[i].ptrs.Length; j++)
				{
					// find the end of the data in the current description
					int end;
					for (end = index + 1; end + separator_size < descriptions[i].description.Length; end++)
					{
						// if the dynamic value at the current end index is the target separator
						if (Utils.DynamicPeek(descriptions[i].description, end, separator) == separator)
						{
							// push the index to include the separator
							end += separator_size;
							// then stop scanning
							break;
						}
					}
					// if the sum of the end index and the separator index is larger than the length of the current description
					if (end + separator_size >= descriptions[i].description.Length)
					{
						// set it to the length
						end = descriptions[i].description.Length;
					}
					// take the bytes from the current index to the end index
					byte[] data = descriptions[i].description[index..end];
					// if the sum of the current pointer and the length of the previous array is more than the 'total' array can hold
					if (descriptions[i].ptrs[j] + data.Length > total.Length - 1)
					{
						// resize it to fit
						Array.Resize(ref total, descriptions[i].ptrs[j] + data.Length);
					}
					// copy the 'data' array's contents to the 'total' array starting at the current pointer's index
					data.CopyTo(total, descriptions[i].ptrs[j]);
					// set the next index
					index = end;
				}
			}
			// once all descriptions are accounted for
			for (int i = 0; i < total.Length; i++)
			{
				// write any and all reserved pointers that point to this index
				WriteAllPointers(base_name, i);
				// then write the byte at this index
				Write(total[i]);
			}
		}

		/// <summary>
		/// Handles writing the stored byte array in a <see cref="Description"/>.
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

		/// <summary>
		/// Writes an id to file, gives it a name, then reserves 4 bytes per <paramref name="count"/>.
		/// Offsets can be written to this named pointer as many times as <paramref name="count"/> allows.
		/// Once <paramref name="count"/> is reached, it's removed from the <see cref="reserved_offsets"/> list.
		/// </summary>
		/// <param name="id">The id of this object to write to file.</param>
		/// <param name="name">The name of this pointer.</param>
		/// <param name="count">The amount of reservations to make.</param>
		public void ReservePointer(dynamic id, string name, int count = 1)
		{
			// write the id immediately
			Write(id);
			// reserve a new pointer
			reserved_offsets.Add(name, new Pointer
			{
				// mark the offset
				offset = offset,
				// init the index to 0
				index = 0,
				// set the count
				count = count
			});
			// skip the next 4 bytes per count
			offset += count * sizeof(int);
		}

		/// <summary>
		/// Reserves a pointer similar to <see cref="ReservePointer(dynamic, string, int)"/> without writing an id.
		/// </summary>
		/// <param name="name">The name of this pointer.</param>
		/// <param name="count">The amoutn of reservations to make.</param>
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

		/// <summary>
		/// Reserves an array of pointers separated by a 32bit boolean and an array id. Reserves in a specific style:
		/// "<paramref name="name"/>_X_<paramref name="ptrs"/>[X]" where 'X' is the current index of the <paramref name="ptrs"/>.
		/// </summary>
		/// <param name="name">The name of this pointer.</param>
		/// <param name="ptrs">The array of pointers to reserve.</param>
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

		/// <summary>
		/// Writes the current offset to the target named pointer's next available index.
		/// </summary>
		/// <param name="name">The name of the pointer to write to.</param>
		/// <param name="remove">Whether or not you wish to remove this pointer. Even if its lifespan is finished.</param>
		/// <exception cref="KeyNotFoundException">No pointer of the target name was found.</exception>
		public void WritePointer(string name, bool remove = true)
		{
			if (!reserved_offsets.TryGetValue(name, out Pointer ptr))
			{
				throw new KeyNotFoundException($"No pointer of name '{name}' was found.");
			}
			// mark the current offset
			int temp_offset = offset;
			// set the offset to the sum of the pointer's marked offset and the index
			offset = ptr.offset + (ptr.index * sizeof(int));
			// increment the index early
			ptr.index++;
			// if we can remove this pointer and its index is larger than its count
			if (remove && ptr.index > ptr.count - 1)
			{
				// remove it
				reserved_offsets.Remove(name);
			}
			// write the marked offset
			Write(temp_offset);
			// jump back to the marked offset
			offset = temp_offset;
		}

		/// <summary>
		/// This is entirely unused but I'm leaving it here just in case it's ever useful. It simply writes the <paramref name="forced_offset"/>
		/// to the target named pointer.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="forced_offset"></param>
		/// <exception cref="KeyNotFoundException"></exception>
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

		/// <summary>
		/// Writes the <paramref name="id"/> and <see cref="offset"/> to the target named pointer's next available index.
		/// Pointers reserved should reserve 2 indexes each before using this method.
		/// </summary>
		/// <param name="name">The name of the pointer to write to.</param>
		/// <param name="id">The id to give this pointer.</param>
		/// <param name="remove">Whether or not you wish to remove this pointer. Even if its lifespan is finished.</param>
		/// <exception cref="KeyNotFoundException">No pointer of the target name was found.</exception>
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

	/// <summary>
	/// A class used in reserving specific offsets for marking offset positions.
	/// </summary>
	public class Pointer
	{
		/// <summary>
		/// The origin offset of this pointer.
		/// </summary>
		public int offset;
		/// <summary>
		/// The current index of this pointer. (<see cref="offset"/> + (<see cref="index"/> * sizeof(int))
		/// </summary>
		public int index;
		/// <summary>
		/// The amount of offsets reserved.
		/// </summary>
		public int count;
	}
}
