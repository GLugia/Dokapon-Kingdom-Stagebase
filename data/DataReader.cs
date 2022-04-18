using System;
using System.IO;
using System.Reflection;

namespace CharaReader.data
{
	public class DataReader
	{
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
		/// The length of the buffer.
		/// </summary>
		public int length => _data?.Length ?? -1;
		/// <summary>
		/// The length of the buffer in hex.
		/// </summary>
		public string length_hex => length.ToHexString();

		/// <summary>
		/// Initializes the necessary fields for creating a new instance of <see cref="DataReader"/>.
		/// </summary>
		/// <param name="file">The file to read from.</param>
		public DataReader(string file)
		{
			// read the entire file given
			_data = File.ReadAllBytes(file);
			// init offset to 0
			offset = 0;
		}

		/// <summary>
		/// Close this reader.
		/// </summary>
		public void Close()
		{
			// null the buffer
			_data = null;
			// reset the offset
			offset = 0;
		}

		/// <summary>
		/// Reads the next byte as a boolean.
		/// </summary>
		public bool ReadBool()
		{
			bool ret = _data[offset] > 0;
			offset++;
			return ret;
		}

		/// <summary>
		/// Reads the next byte as a char.
		/// </summary>
		public char ReadChar()
		{
			string ret = Program.shift_jis.GetString(_data, offset, 1);
			offset++;
			return ret[0];
		}

		/// <summary>
		/// Reads an array of bytes as char values.
		/// </summary>
		/// <param name="amount">The amount of bytes to read.</param>
		public char[] ReadChars(int amount)
		{
			string ret = Program.shift_jis.GetString(_data, offset, amount);
			offset += amount;
			return ret.ToCharArray();
		}

		/// <summary>
		/// Reads a string value ending in '\0'.
		/// </summary>
		/// <param name="alignment">The alignment of this string. Defaults to 'sizeof(int)'./></param>
		public string ReadString(int alignment = sizeof(int))
		{
			int index = offset;
			while (_data[offset] != '\0' && _data[offset] != 0)
			{
				offset++;
			}
			string ret = Program.shift_jis.GetString(_data[index..offset]);
			offset += alignment - (offset % alignment);
			return ret;
		}

		/// <summary>
		/// Reads the next byte as an sbyte.
		/// </summary>
		public sbyte ReadSByte()
		{
			sbyte ret = (sbyte)_data[offset];
			offset += sizeof(sbyte);
			return ret;
		}

		/// <summary>
		/// Reads the next byte.
		/// </summary>
		public byte ReadByte()
		{
			byte ret = _data[offset];
			offset += sizeof(byte);
			return ret;
		}

		/// <summary>
		/// Reads an array of bytes.
		/// </summary>
		/// <param name="amount">The amount of bytes to read.</param>
		public byte[] ReadBytes(int amount)
		{
			byte[] ret = _data[offset..(offset + amount)];
			offset += amount;
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a short.
		/// </summary>
		public short ReadInt16()
		{
			short ret = BitConverter.ToInt16(_data.AsSpan()[offset..(offset + sizeof(short))]);
			offset += sizeof(short);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a ushort.
		/// </summary>
		/// <returns></returns>
		public ushort ReadUInt16()
		{
			ushort ret = BitConverter.ToUInt16(_data.AsSpan()[offset..(offset + sizeof(ushort))]);
			offset += sizeof(ushort);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as an int.
		/// </summary>
		public int ReadInt32()
		{
			int ret = BitConverter.ToInt32(_data.AsSpan()[offset..(offset + sizeof(int))]);
			offset += sizeof(int);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a uint.
		/// </summary>
		public uint ReadUInt32()
		{
			uint ret = BitConverter.ToUInt32(_data.AsSpan()[offset..(offset + sizeof(uint))]);
			offset += sizeof(uint);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a long.
		/// </summary>
		/// <returns></returns>
		public long ReadInt64()
		{
			long ret = BitConverter.ToInt64(_data.AsSpan()[offset..(offset + sizeof(long))]);
			offset += sizeof(long);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a ulong.
		/// </summary>
		/// <returns></returns>
		public ulong ReadUInt64()
		{
			ulong ret = BitConverter.ToUInt64(_data.AsSpan()[offset..(offset + sizeof(ulong))]);
			offset += sizeof(ulong);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a float.
		/// </summary>
		/// <returns></returns>
		public float ReadSingle()
		{
			float ret = BitConverter.ToSingle(_data.AsSpan()[offset..(offset + sizeof(float))]);
			offset += sizeof(float);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a double.
		/// </summary>
		/// <returns></returns>
		public double ReadDouble()
		{
			double ret = BitConverter.ToSingle(_data.AsSpan()[offset..(offset + sizeof(double))]);
			offset += sizeof(double);
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as an array of structs. Structs given must have a field named 'item_id'.
		/// </summary>
		/// <typeparam name="T">The type of struct to create.</typeparam>
		/// <param name="id">The id of this struct.</param>
		/// <returns></returns>
		/// <exception cref="Exception">The format of this structs' fields is incorrect.</exception>
		public T[] ReadStructs<T>(int id)
		{
			// initialize an empty array of structs
			T[] ret = Array.Empty<T>();
			// initialize a temporary struct
			T temp;
			do
			{
				// read the struct
				temp = ReadStruct<T>();
				// if the struct does not contain the required field 'item_id'
				if (!temp.TryGetField("item_id", out dynamic item_id))
				{
					// error out
					throw new Exception("Struct does not contain the correct ids.");
				}
				// put this struct in the array using the item_id as the index
				if (item_id > ret.Length - 1)
				{
					Array.Resize(ref ret, item_id + 1);
				}
				ret[item_id] = temp;
			}
			// if the next s32 value is the same as the given id, repeat above
			while (ReadInt32() == id);
			// otherwise, pretend it didn't read that s32
			offset -= sizeof(int);
			// return the array generated
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a double array of structs. Structs given must have the fields named
		/// 'item_id' AND 'gender_id'.
		/// </summary>
		/// <typeparam name="T">The type of struct to create.</typeparam>
		/// <param name="id">The id of this struct.</param>
		/// <returns></returns>
		/// <exception cref="Exception">The format of this structs' fields is incorrect.</exception>
		public T[][] ReadStructs2<T>(int id)
		{
			// initialize a double array of structs
			T[][] ret = Array.Empty<T[]>();
			// initialize a temporary struct
			T temp;
			do
			{
				// read the struct
				temp = ReadStruct<T>();
				// if the struct does not contain the required fields
				if (!temp.TryGetField("item_id", out dynamic item_id)
					|| !temp.TryGetField("gender_id", out dynamic gender_id))
				{
					// error out
					throw new Exception("Struct does not contain the correct ids.");
				}
				// resize and initialize array data
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
			// if the next s32 value is the same as the given id, repeat above
			while (ReadInt32() == id);
			// otherwise, pretend it didn't read that s32
			offset -= sizeof(int);
			// return the double array generated
			return ret;
		}

		/// <summary>
		/// Reads the next bytes as a triple array of structs. Structs given must have the fields named
		/// 'item_id', 'gender_id', AND 'extra_id'.
		/// </summary>
		/// <typeparam name="T">The type of struct to create.</typeparam>
		/// <param name="id">The id of this struct.</param>
		/// <returns></returns>
		/// <exception cref="Exception">The format of this structs' fields is incorrect.</exception>
		public T[][][] ReadStructs3<T>(int id)
		{
			// initialize a double array of structs
			T[][][] ret = Array.Empty<T[][]>();
			// initialize a temporary struct
			T temp;
			do
			{
				// read the struct
				temp = ReadStruct<T>();
				// if the struct does not contain the required fields
				if (!temp.TryGetField("item_id", out dynamic item_id)
					|| !temp.TryGetField("gender_id", out dynamic gender_id)
					|| !temp.TryGetField("extra_id", out dynamic extra_id))
				{
					// error out
					throw new Exception("Struct does not contain the correct ids.");
				}
				// resize and initialize array data
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
			// if the next s32 value is the same as the given id, repeat above
			while (ReadInt32() == id);
			// otherwise, pretend it didn't read that s32
			offset -= sizeof(int);
			// return the triple array generated
			return ret;
		}

		/// <summary>
		/// Dynamically reads the given struct type. This does not handle reading <see cref="Description"/>
		/// field types as they are too complex to handle dynamically.
		/// </summary>
		/// <typeparam name="T">The type of struct to read.</typeparam>
		/// <returns></returns>
		/// <exception cref="Exception">If a non-primitive field type exists.</exception>
		public T ReadStruct<T>()
		{
			// create an instance of the given struct type
			T ret = Activator.CreateInstance<T>();
			// make a typed reference to the struct created
			// this lets us change the fields of this struct directly and would not work otherwise
			TypedReference tref = __makeref(ret);
			// iterate all fields in this struct
			foreach (FieldInfo info in ret.GetType().GetFields())
			{
				// depending on the lowercase name of the type of this field,
				// set the field's value to the value read from the buffer.
				switch (info.FieldType.Name.ToLowerInvariant())
				{
					case "bool": info.SetValueDirect(tref, ReadBool()); break;
					case "char": info.SetValueDirect(tref, ReadChar()); break;
					case "char[]": info.SetValueDirect(tref, ReadString().ToCharArray()); break;
					case "string": info.SetValueDirect(tref, ReadString()); break;
					case "byte": info.SetValueDirect(tref, ReadByte()); break;
					case "sbyte": info.SetValueDirect(tref, ReadSByte()); break;
					case "uint16": info.SetValueDirect(tref, ReadUInt16()); break;
					case "int16": info.SetValueDirect(tref, ReadInt16()); break;
					case "uint32": info.SetValueDirect(tref, ReadUInt32()); break;
					case "int32": info.SetValueDirect(tref, ReadInt32()); break;
					case "uint64": info.SetValueDirect(tref, ReadUInt64()); break;
					case "int64": info.SetValueDirect(tref, ReadInt64()); break;
					case "single": info.SetValueDirect(tref, ReadSingle()); break;
					default: throw new Exception($"Unhandled type '{info.FieldType}' in struct '{ret}'");
				}
			}
			return ret;
		}
	}
}
