using CharaReader.testing.data_types;
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
		public FilePtr[] stagebase_files;
		// a pointer array for each type of object referenced
		private SortedDictionary<DataType, List<dynamic>> default_types;
		private SortedDictionary<IntPtr, Array> ptr_references;

		public Testing()
		{
			// initialize and allocate stagebase data
			LoadStagebase();
			// iterate through the files in memory
			for (int i = 0; i < stagebase_files.Length; i++)
			{
				Print($"Loading {Program.shift_jis.GetString(BitConverter.GetBytes(Marshal.ReadInt32(stagebase_files[i].origin)))}...\n");
				// read all default objects from this file
				if (!ReadFile(stagebase_files[i]))
				{
					break;
				}
			}
			Print($"Testing getting objects from the dictionary:\n");
			Print($"\t{default_types[DataType.LOCATION][0].name}\n");
			Print($"Testing setting an object then getting it for printing:\n");
			default_types[DataType.LOCATION][0].name = "HAHAHAHAHAHAHA";
			Print($"\t{default_types[DataType.LOCATION][0].name}\n");
			Print("Done.");
		}

		private void LoadStagebase()
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
			stagebase_files = Array.Empty<FilePtr>();
			// read every file in stagebase
			for (long offset = (long)stagebase.origin; offset < (long)stagebase.length;)
			{
				Print("Loading file...");
				Array.Resize(ref stagebase_files, stagebase_files.Length + 1);
				// create a new instance
				stagebase_files[^1] = new()
				{
					// store the current offset as the origin
					origin = new IntPtr(offset),
					// store the next 4 bytes as the length
					length = new IntPtr(offset + sizeof(int)),
					// store the next 4 bytes as the header's ending position
					header = new IntPtr(offset + (sizeof(int) * 2))
				};
				// read the file size
				int length = Marshal.ReadInt32(stagebase_files[^1].length);
				// align it to 16
				length += 16 - (length % 16);
				// increment the offset by the length of the file
				offset += length;
				Print($"{Program.shift_jis.GetString(BitConverter.GetBytes(Marshal.ReadInt32(stagebase_files[^1].origin)))} loaded to 0x{(long)stagebase_files[^1].origin:x} with a length of 0x{length:x}\n");
			}
			Print("Initializing data...");
			// initialize the dictionary of types
			ptr_references = new SortedDictionary<IntPtr, Array>();
			default_types = new SortedDictionary<DataType, List<object>>();
			// iterate over all data types
			for (byte i = 1; i < (byte)DataType.COUNT; i++)
			{
				// initialize each ID's list
				default_types.Add((DataType)i, new List<object>());
			}
			Print("Done.\n");
		}

		private bool ReadFile(FilePtr ptr)
		{
			int data_type;
			for (long offset = Marshal.ReadInt32(ptr.header); offset < Marshal.ReadInt32(ptr.length);)
			{
				data_type = Marshal.ReadInt32(ptr.origin, (int)offset);
				offset += sizeof(int);
				Print($"Reading {(DataType)data_type}...");
				switch (data_type)
				{
					case 0x03: ReadDataFromPtr(ptr.origin, ref offset); break;
					default:
						if (!ReadObjectFromPtr(ptr.origin, ref offset, (DataType)data_type, out dynamic result))
						{
							string message = $"Invalid DataType: 0x{(byte)data_type:X} at 0x{offset - sizeof(int):X}";
							string top = "\n\n....... ";
							for (int i = 0; i < message.Length; i++)
							{
								top += "v";
							}
							top += " .......\n";
							string bottom = "....... ";
							for (int i = 0; i < message.Length; i++)
							{
								bottom += "^";
							}
							bottom += " .......\n";
							Print(top);
							Print($">>>>>>> {message} <<<<<<<\n");
							Print(bottom);
							return false;
						}
						else if (result != null)
						{
							default_types[(DataType)data_type].Add(result);
						}
						break;
				}
				Print("Done.\n");
			}
			return true;
		}

		public bool ReadObjectFromPtr(IntPtr ptr, ref long offset, DataType data_type, out dynamic result)
		{
			// if the object already exists as a pointer to an array of bytes
			if (ptr_references.TryGetValue((IntPtr)((long)ptr + offset - sizeof(int)), out Array arr))
			{
				// increment the offset by the length of the array
				offset += arr.Length - sizeof(int);
				// set the result to null
				result = null;
				// return true to continue reading
				return true;
			}
			Type type = NewDataType(data_type);
			// if the type is invalid
			if (type == null)
			{
				// set the result to null
				result = null;
				// return false to stop reading
				return false;
			}
			result = Activator.CreateInstance(type);
			dynamic temp;
			dynamic value;
			Array data;
			foreach (FieldInfo field in result.GetType().GetFields())
			{
				switch (field.FieldType.Name.ToLowerInvariant())
				{
					case "byte":
						temp = Marshal.ReadByte(ptr, (int)offset);
						offset++;
						break;
					case "byte[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(byte), value - offset);
						for (; offset < value; offset++)
						{
							data.SetValue(Marshal.ReadByte(ptr, (int)offset), offset - temp);
						}
						temp = data;
						break;
					case "sbyte":
						temp = (sbyte)Marshal.ReadByte(ptr, (int)offset);
						offset++;
						break;
					case "sbyte[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(sbyte), value - offset);
						for (; offset < value; offset++)
						{
							data.SetValue((sbyte)Marshal.ReadByte(ptr, (int)offset), offset - temp);
						}
						temp = data;
						break;
					case "uint16":
						temp = (ushort)Marshal.ReadInt16(ptr, (int)offset);
						offset += sizeof(ushort);
						break;
					case "uint16[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(ushort), (value - offset) / sizeof(ushort));
						for (; offset < value; offset += sizeof(ushort))
						{
							data.SetValue((ushort)Marshal.ReadInt16(ptr, (int)offset), (offset - temp) / sizeof(ushort));
						}
						temp = data;
						break;
					case "int16":
						temp = Marshal.ReadInt16(ptr, (int)offset);
						offset += sizeof(short);
						break;
					case "int16[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(short), (value - offset) / sizeof(short));
						for (; offset < value; offset += sizeof(short))
						{
							data.SetValue((short)Marshal.ReadInt16(ptr, (int)offset), (offset - temp) / sizeof(short));
						}
						temp = data;
						break;
					case "uint32":
						temp = (uint)Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(uint);
						break;
					case "uint32[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(uint), (value - offset) / sizeof(uint));
						for (; offset < value; offset += sizeof(uint))
						{
							data.SetValue((uint)Marshal.ReadInt32(ptr, (int)offset), (offset - temp) / sizeof(uint));
						}
						temp = data;
						break;
					case "int32":
						temp = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						break;
					case "int32[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(int), (value - offset) / sizeof(int));
						for (; offset < value; offset += sizeof(int))
						{
							data.SetValue(Marshal.ReadInt32(ptr, (int)offset), (offset - temp) / sizeof(int));
						}
						temp = data;
						break;
					case "uint64":
						temp = (ulong)Marshal.ReadInt64(ptr, (int)offset);
						offset += sizeof(ulong);
						break;
					case "uint64[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(ulong), (value - offset) / sizeof(ulong));
						for (; offset < value; offset += sizeof(ulong))
						{
							data.SetValue((ulong)Marshal.ReadInt64(ptr, (int)offset), (offset - temp) / sizeof(ulong));
						}
						temp = data;
						break;
					case "int64":
						temp = Marshal.ReadInt64(ptr, (int)offset);
						offset += sizeof(long);
						break;
					case "int64[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(long), (value - offset) / sizeof(long));
						for (; offset < value; offset += sizeof(long))
						{
							data.SetValue(Marshal.ReadInt64(ptr, (int)offset), (offset - temp) / sizeof(long));
						}
						temp = data;
						break;
					case "single":
						temp = BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(ptr, (int)offset)));
						offset += sizeof(float);
						break;
					case "single[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(float), (value - offset) / sizeof(float));
						for (; offset < value; offset += sizeof(float))
						{
							data.SetValue(BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(ptr, (int)offset))), (offset - temp) / sizeof(float));
						}
						temp = data;
						break;
					case "double":
						temp = BitConverter.ToDouble(BitConverter.GetBytes(Marshal.ReadInt32(ptr, (int)offset)));
						offset += sizeof(double);
						break;
					case "double[]":
						value = Marshal.ReadInt32(ptr, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(double), (value - offset) / sizeof(double));
						for (; offset < value; offset += sizeof(double))
						{
							data.SetValue(BitConverter.ToDouble(BitConverter.GetBytes(Marshal.ReadInt32(ptr, (int)offset))), (offset - temp) / sizeof(double));
						}
						temp = data;
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
						do
						{
							array_temp = Marshal.ReadByte(ptr, temp_offset);
							// increment the temporary offset
							temp_offset++;
							// if the current index is too large
							if (index >= array.Length)
							{
								// allocate another 4 bytes
								Array.Resize(ref array, array.Length + sizeof(int));
							}
							// set the index to the read value
							array[index++] = array_temp;
						}
						while (array_temp != 0);
						// translate the bytes to a string
						temp = Program.shift_jis.GetString(array);
						// increment the real offset by the length of the string
						offset += array.Length;
						break;
					case "intptr":
						temp = new IntPtr((long)ptr + Marshal.ReadInt32(ptr, (int)offset));
						offset += sizeof(int);
						continue;
					case "arrayptr":
						temp = new ArrayPtr();
						temp.start = new IntPtr((long)ptr + Marshal.ReadInt32(ptr, (int)offset));
						offset += sizeof(int);
						temp.end = new IntPtr((long)ptr + Marshal.ReadInt32(ptr, (int)offset));
						offset += sizeof(int);
						data = Array.CreateInstance(typeof(byte), (int)((long)temp.end - (long)temp.start));
						for (long i = (long)temp.start; i < (long)temp.end; i++)
						{
							data.SetValue(Marshal.ReadByte((IntPtr)i, 0), i - (long)temp.start);
						}
						ptr_references.Add(temp.start, data);
						break;
					default:
						throw new Exception($"Unhandled type: {field.FieldType.Name.ToLowerInvariant()}");
				}
				field.SetValue(result, temp);
			}
			return true;
		}

		public void ReadDataFromPtr(IntPtr ptr, ref long offset)
		{
			int end = Marshal.ReadInt32(ptr, (int)offset);
			offset += sizeof(int) * 2;
			long origin = offset;
			while (offset < end)
			{
				IntPtr pos = new((long)ptr + offset);
				while (Marshal.ReadByte(ptr, (int)offset) != 0)
				{
					offset++;
				}
				ptr_references[pos] = new byte[(int)(offset - origin)];
				for (int i = 0; i < ptr_references[pos].Length; i++)
				{
					ptr_references[pos].SetValue(Marshal.ReadByte(pos, i), i);
				}
				offset += sizeof(int) - (offset % sizeof(int));
				origin = offset;
			}
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
		public static Type NewDataType(DataType data_type)
		{
			// try to parse the object using the name of the DataType
			if (EnumToType(data_type, out Type type))
			{
				// return the type from Assembly
				return type;
			}
			// fallback for DataTypes that aren't directly translated
			return data_type switch
			{
				DataType.PTR => typeof(IntPtr),
				_ => null
			};
		}

		public static DataType GetDataType<T>(in T obj) where T : notnull
		{
			// try to parse the DataType using the name of the object
			if (TypeToEnum(obj, out DataType data_type))
			{
				return data_type;
			}
			// fallback for types that aren't directly translated
			return obj switch
			{
				IntPtr => DataType.PTR,
				_ => throw new InvalidCastException($"{obj} is not a DataType")
			};
		}

		public static bool TypeToEnum<T>(object o, out T value) where T : struct
		{
			string name = o.GetType().Name;
			string real_name = "";
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsUpper(name[i]))
				{
					real_name += '_';
				}
				real_name += char.ToUpper(name[i]);
			}
			return Enum.TryParse(real_name, out value);
		}

		public static bool EnumToType<T>(in T value, out Type o) where T : struct
		{
			// store the name of the value
			string name = $"{value}";
			string real_name = "";
			// noun-ify the name (SPACE -> Space, weapon -> Weapon, file_LABEL -> File_Label)
			if (name.Contains("UNK"))
			{
				real_name = string.Join("", "Unk", name[3..]);
			}
			else
			{
				for (int i = 0; i < name.Length; i++)
				{
					// if this index is an underscore
					if (name[i] == '_' && i + 1 < name.Length)
					{
						// append it and increment the index
						real_name += name[i++];
						// append the next char as uppercase
						real_name += char.ToUpperInvariant(name[i]);
					}
					// if it's not the first index
					else if (i != 0)
					{
						real_name += char.ToLowerInvariant(name[i]);
					}
					// if it is the first index
					else
					{
						real_name += char.ToUpperInvariant(name[i]);
					}
				}
			}
			o = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(a => a.IsClass && !a.IsAbstract && a.Name == real_name);
			return o != null;
		}
	}

	public enum DataType : byte
	{
		NONE,
		FILE_LABEL = 0x01,
		PTR = 0x03,
		STAGE = 0x05,
		UNK_2B = 0x2B,
		UNK_2F = 0x2F,
		LOCATION = 0x37,
		WEAPONS = 0x58,
		UNK_66 = 0x66,
		TEMPLE = 0x67,
		UNK_68 = 0x68,
		TOWN = 0x6D,
		UNK_6E = 0x6E,
		UNK_6F = 0x6F,
		UNK_7F = 0x7F,
		SPACE_EFFECT = 0x86,
		SPACE = 0x87,
		SPACE_DATA = 0x88,
		UNK_93 = 0x93,
		UNK_94 = 0x94,
		UNK_DA = 0xDA,
		UNK_DB = 0xDB,
		UNK_E0 = 0xE0,
		COUNT = 0xFF
	}
}
