using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.testing
{
	public unsafe class Testing
	{
		// stagebase
		private byte[] _stagebase;
		public FilePtr stagebase;
		public long offset;
		public FilePtr[] files;
		// a pointer array for each type of object referenced
		private SortedDictionary<DataType, List<IntPtr>> data_type_ptrs;

		public Testing()
		{
			Print("Loading Stagebase to memory...");
			// read the bytes from the file
			_stagebase = File.ReadAllBytes("STAGEBASE.DAT");
			// allocate enough space in memory to contain the bytes from stagebase
			IntPtr stagebase_ptr = Marshal.AllocHGlobal(_stagebase.Length);
			// copy the bytes from stagebase to memory
			Marshal.Copy(_stagebase, 0, stagebase_ptr, _stagebase.Length);
			// NOTE: Never null the original _stagebase array. Doing so destroys the pointer to it.
			// create a new FilePtr instance for stagebase
			stagebase = new()
			{
				// store the origin position of the file in memory
				origin = new((long)stagebase_ptr),
				// store the ending position of the file in memory
				length = new((long)stagebase_ptr + _stagebase.Length),
				// this isn't needed but we zero it anyway
				header = IntPtr.Zero
			};
			Print($"Stagebase loaded to 0x{stagebase.origin:x} with a length of 0x{(long)stagebase.length - (long)stagebase.origin:x}\n");
			// initialize the FilePtr array
			files = Array.Empty<FilePtr>();
			// read every file in stagebase
			for (offset = (long)stagebase.origin; offset < (long)stagebase.length;)
			{
				Print("Loading file...");
				Array.Resize(ref files, files.Length + 1);
				// create a new instance
				files[^1] = new()
				{
					// store the current offset as the origin
					origin = new IntPtr(offset),
					// store the next 4 bytes as the length
					length = new IntPtr(offset + sizeof(int)),
					// store the next 4 bytes as the header's ending position
					header = new IntPtr(offset + (sizeof(int) * 2))
				};
				// read the file size
				int length = Marshal.ReadInt32(files[^1].length);
				// align it to 16
				length += 16 - (length % 16);
				// increment the offset by the length of the file
				offset += length;
				Print($"{Program.shift_jis.GetString(BitConverter.GetBytes(Marshal.ReadInt32(files[^1].origin)))} loaded to 0x{(long)files[^1].origin:x} with a length of 0x{length:x}\n");
			}
			Print("Initializing data...");
			// initialize the dictionary of types
			data_type_ptrs = new SortedDictionary<DataType, List<IntPtr>>();
			// iterate over all data types
			for (byte i = 1; i < (byte)DataType.COUNT; i++)
			{
				// initialize each ID's list
				data_type_ptrs.Add((DataType)i, new List<IntPtr>());
			}
			Print("Done.\n");
			// used for each object's type ID
			DataType data_type;
			bool finished = false;
			// iterate through the files in memory
			for (int i = 0; i < files.Length; i++)
			{
				Print($"Loading {Program.shift_jis.GetString(BitConverter.GetBytes(Marshal.ReadInt32(files[i].origin)))}...\n");
				// read from the offset of this file's header to the end of the file's data
				for (offset = Marshal.ReadInt32(files[i].header); offset < Marshal.ReadInt32(files[i].length);)
				{
					// get the type of this object
					data_type = (DataType)Marshal.ReadInt32(files[i].origin, (int)offset);
					if (data_type == (DataType)0x88)
					{
						finished = true;
						break;
					}
					offset += sizeof(int);
					// if the type is valid (not NONE aka 0 aka null)
					if (data_type_ptrs.TryGetValue(data_type, out List<IntPtr> ptrs))
					{
						// create a pointer to the object's data and add it to the list
						ptrs.Add(GetObjectPtr(files[i].origin, data_type));
					}
				}
				if (finished)
				{
					break;
				}
			}
			Print("Done.");
		}

		/// <summary>
		/// Read a <see cref="DataType"/> from memory.
		/// This exists because <see cref="Marshal.PtrToStructure{T}(IntPtr)"/> does not read everything we want.
		/// It will probably add the instance to a Dictionary at some point down the road.
		/// </summary>
		/// <param name="ptr">The origin position to add to the offset.</param>
		/// <param name="data_type">The ID of the type of data to read.</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private IntPtr GetObjectPtr(IntPtr ptr, DataType data_type)
		{
			IntPtr ret = new((long)ptr + offset);
			GetObjectFromPtr(ret, data_type);
			return ret;
		}

		public dynamic GetObjectFromPtr(IntPtr ptr, DataType data_type)
		{
			dynamic result = Activator.CreateInstance(NewDataType(data_type));
			dynamic temp;
			int offset = 0;
			foreach (FieldInfo field in result.GetType().GetFields())
			{
				switch (field.FieldType.Name.ToLowerInvariant())
				{
					case "byte":
						temp = Marshal.ReadByte(ptr, (int)offset);
						offset++;
						break;
					case "sbyte":
						temp = (sbyte)Marshal.ReadByte(ptr, (int)offset);
						offset++;
						break;
					case "uint16":
						temp = (ushort)Marshal.ReadInt16(ptr, (int)offset);
						offset += sizeof(ushort);
						break;
					case "int16":
						temp = Marshal.ReadInt16(ptr, (int)offset);
						offset += sizeof(short);
						break;
					case "uint32":
						temp = (uint)Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(uint);
						break;
					case "int32":
						temp = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						break;
					case "uint64":
						temp = (ulong)Marshal.ReadInt64(ptr, (int)offset);
						offset += sizeof(ulong);
						break;
					case "int64":
						temp = Marshal.ReadInt64(ptr, (int)offset);
						offset += sizeof(long);
						break;
					case "single":
						temp = BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(ptr, (int)offset)));
						offset += sizeof(float);
						break;
					case "double":
						temp = BitConverter.ToDouble(BitConverter.GetBytes(Marshal.ReadInt32(ptr, (int)offset)));
						offset += sizeof(double);
						break;
					case "string":
						// allocate an array of 4 bytes
						byte[] array = new byte[sizeof(int)];
						// the array's current index
						int index = 0;
						// the next byte to read from memory
						byte array_temp;
						// store the current offset
						int temp_offset = (int)offset;
						// read until 0
						while ((array_temp = Marshal.ReadByte(ptr, temp_offset)) != 0)
						{
							// if the current index is too large
							if (index >= array.Length)
							{
								// allocate another 4 bytes
								Array.Resize(ref array, array.Length + sizeof(int));
							}
							// set the index to the read value
							array[index++] = array_temp;
							// increment the temporary offset
							temp_offset++;
						}
						// translate the bytes to a string
						temp = Program.shift_jis.GetString(array);
						// increment the real offset by the length of the string
						offset += array.Length;
						break;
					case "intptr":
						temp = new IntPtr((long)ptr + Marshal.ReadInt32(ptr, (int)offset));
						offset += sizeof(int);
						break;
					default:
						throw new Exception($"Unhandled type: {field.FieldType}");
				}
				field.SetValue(result, temp);
			}
			this.offset += offset;
			return result;
		}

		public IntPtr SetObjectToPtr(IntPtr ptr, object obj)
		{
			if (obj == null)
			{
				throw new NullReferenceException();
			}
			byte[] bytes;
			int temp_offset = 0;
			foreach (FieldInfo field in obj.GetType().GetFields())
			{
				bytes = field.FieldType.Name.ToLowerInvariant() switch
				{
					"sbyte" or "byte" => BitConverter.GetBytes((byte)field.GetValue(obj)),
					"int16" or "uint16" => BitConverter.GetBytes((short)field.GetValue(obj)),
					"int32" or "uin32" => BitConverter.GetBytes((int)field.GetValue(obj)),
					"int64" or "uint64" => BitConverter.GetBytes((long)field.GetValue(obj)),
					"single" => BitConverter.GetBytes((float)field.GetValue(obj)),
					"double" => BitConverter.GetBytes((double)field.GetValue(obj)),
					"string" => Program.shift_jis.GetBytes((string)field.GetValue(obj) ?? ""),
					"intptr" => BitConverter.GetBytes((long)(IntPtr)field.GetValue(obj)),
					_ => throw new Exception($"Unhandled type: {field.FieldType}")
				};
				foreach (byte b in bytes)
				{
					Marshal.WriteByte(ptr, temp_offset, b);
					temp_offset++;
				}
			}
			return ptr;
		}

		public IntPtr CreateNewObject(DataType data_type)
		{
			if (data_type_ptrs.TryGetValue(data_type, out List<IntPtr> ptrs))
			{
				dynamic instance = Activator.CreateInstance(NewDataType(data_type));
				IntPtr ret = Marshal.AllocHGlobal(Utils.Size(instance));
				ret = SetObjectToPtr(ret, instance);
				ptrs.Add(ret);
				return ret;
			}
			throw new KeyNotFoundException($"{data_type}");
		}

		/// <summary>
		/// Used to print to console a little easier
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		private void Print<T>(in T obj) where T : notnull
		{
			Console.Out.Write(obj);
		}

		/// <summary>
		/// Convert a <see cref="DataType"/> ID to a <see cref="Type"/> instance.
		/// </summary>
		/// <param name="data_type">The ID to convert.</param>
		/// <returns></returns>
		/// <exception cref="Exception">If the ID is invalid or unhandled.</exception>
		public static Type NewDataType(DataType data_type) =>
			data_type switch
			{
				DataType.FILE_LABEL => typeof(FileLabel),
				DataType.STAGE => typeof(Stage),
				DataType.LOCATION => typeof(Location),
				DataType.WEAPONS => null,
				DataType.UNK_6E => typeof(Unk_6E),
				_ => throw new Exception($"Unhandled data type: {data_type}")
			};

		public static DataType GetDataType<T>(in T obj) where T : notnull =>
			obj switch
			{
				FileLabel => DataType.FILE_LABEL,
				Stage => DataType.STAGE,
				Location => DataType.LOCATION,
				Unk_6E => DataType.UNK_6E,
				null => DataType.WEAPONS,
				_ => throw new Exception($"Unhandled object type: {obj}")
			};
	}

	public enum DataType : byte
	{
		NONE,
		FILE_LABEL = 0x01,
		STAGE = 0x05,
		LOCATION = 0x37,
		WEAPONS = 0x58,
		UNK_6E = 0x6E,
		COUNT = 0xFF
	}
}
