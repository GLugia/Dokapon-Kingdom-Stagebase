using CharaReader.data.ptrs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CharaReader.data
{
	public unsafe class Stagebase
	{
		// stagebase
		private byte[] _stagebase;
		private IntPtr stagebase_ptr;
		private Ptr_File stagebase;
		private Ptr_File[] stagebase_files;
		// a pointer array for each type of object referenced
		private SortedDictionary<DataType, List<dynamic>> _struct_data;
		public IReadOnlyDictionary<DataType, List<dynamic>> struct_data => _struct_data;

		/* TODO: Change Value type to an abstract class type
		 * this will let us get the size of the object for skipping
		 * its contents but also let us use named fields to access
		 * the existing bytes
		 */
		private SortedDictionary<IntPtr, byte[]> _ptr_data;
		public IReadOnlyDictionary<IntPtr, byte[]> ptr_data => _ptr_data;

		public Stagebase()
		{
			Print("Loading Stagebase to memory...");
			// read the bytes from the file
			_stagebase = File.ReadAllBytes("STAGEBASE.DAT");
			// allocate enough space in memory to contain the bytes from stagebase
			stagebase_ptr = Marshal.AllocHGlobal(_stagebase.Length);
			// copy the bytes from stagebase to memory
			Marshal.Copy(_stagebase, 0, stagebase_ptr, _stagebase.Length);
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
			Marshal.FreeHGlobal(stagebase_ptr);
			Print("Done.");
		}

		private void LoadStagebase()
		{
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
			stagebase_files = Array.Empty<Ptr_File>();
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
			_ptr_data = new SortedDictionary<IntPtr, byte[]>();
			_struct_data = new SortedDictionary<DataType, List<object>>();
			// iterate over all data types
			for (byte i = 1; i < (byte)DataType.COUNT; i++)
			{
				// initialize each ID's list
				_struct_data.Add((DataType)i, new List<object>());
			}
			Print("Done.\n");
		}

		private bool ReadFile(Ptr_File ptr)
		{
			int data_type;
			for (long offset = Marshal.ReadInt32(ptr.header);
				offset < Marshal.ReadInt32(ptr.length);)
			{
				IntPtr position = (IntPtr)((long)ptr.origin + offset);
				// if this offset is a referenced ptr, we want to skip over its contained data.
				if (_ptr_data.ContainsKey(position))
				{
					if (_ptr_data[position] == null)
					{
						throw new NullReferenceException($"{position}");
					}
					// skip the data held in this array
					Print($"\tSkipping locked offset: 0x{offset:X}\n");
					offset += _ptr_data[position].Length;
					// restart the loop to check if the next offset is also a referenced ptr
					continue;
				}
				// if this offset is not a referenced ptr, re-align the offset to int
				if (offset % sizeof(int) != 0)
				{
					offset += sizeof(int) - (offset % sizeof(int));
				}
				// read the next ID
				data_type = Marshal.ReadInt32(ptr.origin, (int)offset);
				offset += sizeof(int);
				Print($"Reading {(DataType)data_type} at 0x{offset - sizeof(int):X}...");
				switch (data_type)
				{
					// if the ID read is 0x03, take as many ptrs from this section as possible
					// most will have already been finished but this makes sure there
					// isn't any unused data left out
					case 0x03:
						offset = Marshal.ReadInt32(ptr.origin, (int)offset);
						break;

					// if the ID read is not 0x03, try to read it as a struct object
					default:
						if (!ReadObjectFromPtr(ptr.origin, ref offset, (DataType)data_type, out dynamic result))
						{
							// create an easily visible message explaining the ID
							// of the object we failed to read and list the position
							// it's at
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
						// if the object's read was successful and the result given is not null
						else if (result != null)
						{
							// add it to the list of similar objects
							_struct_data[(DataType)data_type].Add(result);
						}
						break;
				}
				Print("Done.\n");
			}
			return true;
		}

		public bool ReadObjectFromPtr(IntPtr origin, ref long offset, DataType data_type, out dynamic result)
		{
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
			dynamic temp_value;
			dynamic temp_end;
			Array data;
			foreach (FieldInfo field in result.GetType().GetFields())
			{
				switch (field.FieldType.Name.ToLowerInvariant())
				{
					case "char[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(byte), temp_value - offset);
						for (; offset < temp_value; offset++)
						{
							data.SetValue(Marshal.ReadByte(origin, (int)offset), offset - temp);
						}
						temp = Program.shift_jis.GetChars((byte[])data);
						break;
					case "byte":
						temp = Marshal.ReadByte(origin, (int)offset);
						offset++;
						break;
					case "byte[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						if (temp_value > offset)
						{
							temp_value -= offset;
						}
						temp_end = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						data = Array.CreateInstance(typeof(byte), temp_value);
						for (int i = 0; i < temp_value; i++)
						{
							data.SetValue(Marshal.ReadByte(origin, (int)offset), i);
							offset++;
						}
						temp = data;
						offset = temp_end;
						break;
					case "sbyte":
						temp = (sbyte)Marshal.ReadByte(origin, (int)offset);
						offset++;
						break;
					case "sbyte[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(sbyte), temp_value - offset);
						for (; offset < temp_value; offset++)
						{
							data.SetValue((sbyte)Marshal.ReadByte(origin, (int)offset), offset - temp);
						}
						temp = data;
						break;
					case "uint16":
						temp = (ushort)Marshal.ReadInt16(origin, (int)offset);
						offset += sizeof(ushort);
						break;
					case "uint16[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp_end = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(ushort), (temp_value - offset) / sizeof(ushort));
						for (; offset < temp_value; offset += sizeof(ushort))
						{
							data.SetValue((ushort)Marshal.ReadInt16(origin, (int)offset), (offset - temp) / sizeof(ushort));
						}
						temp = data;
						offset = temp_end;
						break;
					case "int16":
						temp = Marshal.ReadInt16(origin, (int)offset);
						offset += sizeof(short);
						break;
					case "int16[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(short), (temp_value - offset) / sizeof(short));
						for (; offset < temp_value; offset += sizeof(short))
						{
							data.SetValue(Marshal.ReadInt16(origin, (int)offset), (offset - temp) / sizeof(short));
						}
						temp = data;
						break;
					case "uint32":
						temp = (uint)Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(uint);
						break;
					case "uint32[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(uint), 99);
						offset = temp_value;
						for (int i = 0; i < data.Length; i++)
						{
							data.SetValue((uint)Marshal.ReadInt32(origin, (int)offset), i);
							offset += sizeof(uint);
						}
						offset = temp;
						temp = data;
						break;
					case "int32":
						temp = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						break;
					case "int32[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(int), (temp_value - offset) / sizeof(int));
						for (; offset < temp_value; offset += sizeof(int))
						{
							data.SetValue(Marshal.ReadInt32(origin, (int)offset), (offset - temp) / sizeof(uint));
						}
						temp = data;
						break;
					case "uint64":
						temp = (ulong)Marshal.ReadInt64(origin, (int)offset);
						offset += sizeof(ulong);
						break;
					case "uint64[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(ulong), (temp_value - offset) / sizeof(ulong));
						for (; offset < temp_value; offset += sizeof(ulong))
						{
							data.SetValue((ulong)Marshal.ReadInt64(origin, (int)offset), (offset - temp) / sizeof(ulong));
						}
						temp = data;
						break;
					case "int64":
						temp = Marshal.ReadInt64(origin, (int)offset);
						offset += sizeof(long);
						break;
					case "int64[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(long), (temp_value - offset) / sizeof(long));
						for (; offset < temp_value; offset += sizeof(long))
						{
							data.SetValue(Marshal.ReadInt64(origin, (int)offset), (offset - temp) / sizeof(long));
						}
						temp = data;
						break;
					case "single":
						temp = BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(origin, (int)offset)));
						offset += sizeof(float);
						break;
					case "single[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(float), (temp_value - offset) / sizeof(float));
						for (; offset < temp_value; offset += sizeof(float))
						{
							data.SetValue(BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(origin, (int)offset))), (offset - temp) / sizeof(float));
						}
						temp = data;
						break;
					case "double":
						temp = BitConverter.ToDouble(BitConverter.GetBytes(Marshal.ReadInt32(origin, (int)offset)));
						offset += sizeof(double);
						break;
					case "double[]":
						temp_value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						temp = offset; // store origin of data
						data = Array.CreateInstance(typeof(double), (temp_value - offset) / sizeof(double));
						for (; offset < temp_value; offset += sizeof(double))
						{
							data.SetValue(BitConverter.ToDouble(BitConverter.GetBytes(Marshal.ReadInt32(origin, (int)offset))), (offset - temp) / sizeof(double));
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
							array_temp = Marshal.ReadByte(origin, temp_offset);
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
					default:
						if (field.FieldType.BaseType != null && field.FieldType.BaseType.Name == "Ptr_Custom`1")
						{
							object[] args = new object[] { origin, offset };
							temp = Activator.CreateInstance(field.FieldType, args);
							offset = (long)args[1];
							Dictionary<IntPtr, byte[]> handler = temp.Handle();
							foreach ((IntPtr pos, byte[] arr) in handler)
							{
								if (!_ptr_data.ContainsKey(pos))
								{
									Print($"Locking 0x{(long)pos - (long)origin:X} to {arr.ArrayToString()}...");
									_ptr_data.Add(pos, arr);
								}
								else
								{
									Print($"Offset 0x{(long)pos - (long)origin:X} is already locked...");
								}
							}
							break;
						}
						throw new Exception($"Unhandled type: {field.FieldType.Name.ToLowerInvariant()}");
				}
				field.SetValue(result, temp);
			}
			return true;
		}

		/// <summary>
		/// Used to print to console a little easier
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		private static void Print<T>(in T obj) where T : notnull
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
			return null;
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
			string[] sections = name.Split('_');
			for (int i = 0; i < sections.Length; i++)
			{
				if (sections[i].Length == 0)
				{
					real_name += "_";
					continue;
				}
				else if (byte.TryParse(sections[i], NumberStyles.HexNumber, null, out _))
				{
					real_name += sections[i];
				}
				else
				{
					real_name += $"{char.ToUpperInvariant(sections[i][0])}{sections[i][1..].ToLowerInvariant()}";
				}

				if (i + 1 < sections.Length)
				{
					real_name += "_";
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
		UNK_17 = 0x17,
		UNK_1C = 0x1C,
		UNK_1D = 0x1D,
		UNK_1E = 0x1E,
		JOB_MODEL_4_7 = 0x29,
		UNK_2A = 0x2A,
		UNK_2B = 0x2B,
		UNK_2C = 0x2C,
		UNK_2D = 0x2D,
		JOB_MONEY = 0x2E,
		UNK_2F = 0x2F,
		LOCATION = 0x37,
		JOB_38 = 0x38,
		JOB_39 = 0x39,
		NPC_NAME = 0x3A,
		JOB_MASTERY = 0x3B,
		JOB_SKILLS = 0x3C,
		JOB_DESCRIPTIONS = 0x3D,
		JOB_3E = 0x3E,
		UNK_3F = 0x3F,
		JOB_STATS = 0x40,
		JOB_MODEL = 0x41,
		JOB = 0x42,
		JOB_43 = 0x43,
		JOB_BAG = 0x44,
		JOB_MODEL_0_3 = 0x45,
		UNK_46 = 0x46,
		UNK_47 = 0x47,
		UNK_48 = 0x48,
		UNK_49 = 0x49,
		NPC_TEXT_LIST = 0x4A,
		NPC_TEXT = 0x4B,
		UNK_4C = 0x4C,
		UNK_4D = 0x4D,
		UNK_4E = 0x4E,
		UNK_4F = 0x4F,
		NPC_ENEMY = 0x50,
		NPC_ENEMY_MODEL = 0x51,
		UNK_52 = 0x52,
		NPC_ENEMY_DROP_TABLE = 0x53,
		UNK_54 = 0x54,
		NPC = 0x55,
		NPC_MODEL = 0x56,
		NPC_MODEL_0 = 0x57,
		WEAPON = 0x58,
		WEAPON_MODEL = 0x59,
		WEAPON_DESCRIPTIONS = 0x5A,
		UNK_5B = 0x5B,
		UNK_5C = 0x5C,
		UNK_5D = 0x5D,
		SHIELD = 0x5E,
		SHIELD_MODEL = 0x5F,
		SHIELD_DESCRIPTIONS = 0x60,
		NPC_ENEMY_MODEL_0 = 0x61,
		UNK_62 = 0x62,
		WEAPON_CLASS_BONUS = 0x63,
		ACCESSORY = 0x64,
		ACCESSORY_DESCRIPTIONS = 0x65,
		UNK_66 = 0x66,
		TEMPLE = 0x67,
		UNK_68 = 0x68,
		ITEM = 0x69,
		ITEM_DESCRIPTIONS = 0x6A,
		ITEM_FUNC = 0x6B,
		ITEM_GIFT = 0x6C,
		TOWN = 0x6D,
		UNK_6E = 0x6E,
		UNK_6F = 0x6F,
		MAGIC_OFF = 0x70,
		MAGIC_OFF_DESCRIPTIONS = 0x71,
		MAGIC_DEF = 0x72,
		MAGIC_DEF_DESCRIPTIONS = 0x73,
		MAGIC_ITEM = 0x74,
		MAGIC_ITEM_DESCRIPTIONS = 0x75,
		UNK_76 = 0x76,
		UNK_77 = 0x77,
		UNK_78 = 0x78,
		UNK_79 = 0x79,
		ABILITY_BATTLE = 0x7A,
		ABILITY_FIELD = 0x7B,
		NPC_ENEMY_DESCRIPTIONS = 0x7C,
		ABILITY_BATTLE_DESCRIPTIONS = 0x7D,
		ABILITY_FIELD_DESCRIPTIONS = 0x7E,
		UNK_7F = 0x7F,
		STATUS_PERMANENT = 0x81,
		STATUS_BATTLE = 0x82,
		STATUS_FIELD = 0x83,
		UNK_84 = 0x84,
		UNK_85 = 0x85,
		SPACE_EFFECT = 0x86,
		SPACE = 0x87,
		SPACE_DATA = 0x88,
		ITEM_TYPE = 0x89,
		MAGIC_TYPE = 0x8A,
		MAGIC_ELEMENT = 0x8B,
		UNK_8C = 0x8C,
		STATUS_8E = 0x8E,
		JOB_8F = 0x8F,
		UNK_93 = 0x93,
		UNK_94 = 0x94,
		HAIR_DESCRIPTIONS = 0x95,
		HAIR = 0x96,
		HAIR_MODEL = 0x97,
		STATUS_9A = 0x9A,
		STATUS_9B = 0x9B,
		UNK_9C = 0x9C,
		UNK_9D = 0x9D,
		UNK_9E = 0x9E,
		MAGIC_9F = 0x9F,
		UNK_A1 = 0xA1,
		UNK_A2 = 0xA2,
		UNK_A3 = 0xA3,
		UNK_A4 = 0xA4,
		UNK_A5 = 0xA5,
		UNK_AC = 0xAC,
		UNK_AD = 0xAD,
		UNK_AE = 0xAE, // battle system text
		UNK_AF = 0xAF, // battle system text part 2
		UNK_BF = 0xBF,
		UNK_C0 = 0xC0,
		ITEM_D0 = 0xD0,
		MAGIC_D1 = 0xD1,
		UNK_D2 = 0xD2,
		UNK_D3 = 0xD3,
		MAGIC_D4 = 0xD4,
		MAGIC_D5 = 0xD5,
		JOB_D6 = 0xD6,
		ITEM_D7 = 0xD7,
		ABILITY_DARKLING_DESCRIPTIONS = 0xD8,
		ABILITY_DARKLING = 0xD9,
		UNK_DA = 0xDA,
		UNK_DB = 0xDB,
		UNK_DF = 0xDF,
		UNK_E0 = 0xE0,
		UNK_E1 = 0xE1,
		UNK_E2 = 0xE2,
		COUNT = 0xFF
	}
}
