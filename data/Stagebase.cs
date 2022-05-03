using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using static CharaReader.Utils;

namespace CharaReader.data
{
	public unsafe class Stagebase
	{
		private StreamWriter logger;
		// stagebase
		private byte[] _stagebase;
		private IntPtr stagebase_ptr;
		private FilePtr stagebase;
		private FilePtr[] stagebase_files;
		// a pointer array for each type of object referenced
		private SortedDictionary<DataType, List<dynamic>> _struct_data;
		public IReadOnlyDictionary<DataType, List<dynamic>> struct_data => _struct_data;

		/* TODO: Change Value type to an abstract class type
		 * this will let us get the size of the object for skipping
		 * its contents but also let us use named fields to access
		 * the existing bytes
		 */
		private SortedDictionary<IntPtr, PtrData> _ptr_data;
		public IReadOnlyDictionary<IntPtr, PtrData> ptr_data => _ptr_data;

		public Stagebase()
		{
			logger = new(File.Create("DKSBE.log"), Program.shift_jis)
			{
				AutoFlush = true,
				NewLine = "\n"
			};
			Print("Loading Stagebase to memory...");
			// read the bytes from the file
			_stagebase = File.ReadAllBytes("STAGEBASE.DAT");
			// allocate enough space in memory to contain the bytes from stagebase
			stagebase_ptr = Marshal.AllocHGlobal(_stagebase.Length);
			// copy the bytes from stagebase to memory
			Marshal.Copy(_stagebase, 0, stagebase_ptr, _stagebase.Length);
			// initialize and allocate stagebase data
			LoadStagebase();

			Print("Done.\n");
		}

		internal void ParseObjects()
		{
			// iterate through the files in memory
			for (int i = 0; i < stagebase_files.Length; i++)
			{
				Print($"\nLoading {Program.shift_jis.GetString(BitConverter.GetBytes(Marshal.ReadInt32(stagebase_files[i].origin)))}...\n");
				// read all default objects from this file
				if (!ReadFile(stagebase_files[i]))
				{
					break;
				}
			}
			Marshal.FreeHGlobal(stagebase_ptr);
		}

		internal void Close()
		{
			logger.Flush();
			logger.Close();
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
			_ptr_data = new SortedDictionary<IntPtr, PtrData>();
			_struct_data = new SortedDictionary<DataType, List<object>>();
			// iterate over all data types
			for (byte i = 1; i < (byte)DataType.COUNT; i++)
			{
				// initialize each ID's list
				_struct_data.Add((DataType)i, new List<object>());
			}
			_stagebase = null;
			Print("Done.\n");
		}

		private bool ReadFile(FilePtr ptr)
		{
			int data_type;
			for (long offset = Marshal.ReadInt32(ptr.header);
				offset < Marshal.ReadInt32(ptr.length);)
			{
				// re-align the offset to int
				if (offset % sizeof(int) != 0)
				{
					offset += sizeof(int) - (offset % sizeof(int));
					continue;
				}
				IntPtr position = (IntPtr)((long)ptr.origin + offset);
				// if this offset is a referenced ptr, we want to skip over its contained data.
				if (_ptr_data.ContainsKey(position))
				{
					if (_ptr_data[position] == null || _ptr_data[position].ref_obj == null)
					{
						Print($">>>>>>>> ERROR: {(long)position:X} / {offset:X} <<<<<<<<");
						return false;
					}
					// skip the data held in this array
					Print($"Skipping locked offset: 0x{offset:X}\n");
					offset += _ptr_data[position].size;
					// restart the loop to check if the next offset is also a referenced ptr
					continue;
				}

				// read until the next 03 object is found and store it's ending offset
				long ret_offset = offset;
				int id_of_03;
				int end_of_03;
				int zero_of_03;
				do
				{
					id_of_03 = Marshal.ReadInt32(ptr.origin, (int)offset);
					end_of_03 = Marshal.ReadInt32(ptr.origin, (int)offset + sizeof(int));
					zero_of_03 = Marshal.ReadInt32(ptr.origin, (int)offset + sizeof(long));
					offset += sizeof(int);
				}
				while ((id_of_03 != 0x03
					|| end_of_03 <= offset
					|| zero_of_03 != 0)
					&& offset < Marshal.ReadInt32(ptr.length));
				if (offset >= Marshal.ReadInt32(ptr.length))
				{
					end_of_03 = Marshal.ReadInt32(ptr.length);
				}
				offset = ret_offset;

				// read the next ID
				data_type = Marshal.ReadInt32(ptr.origin, (int)offset);
				offset += sizeof(int);
				Print($"Reading {Enum.GetName(typeof(DataType), (byte)data_type) ?? ((byte)data_type).ToHexString()} at 0x{offset - sizeof(int):X}...");
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
						if (!EnumToType((DataType)data_type, out Type type) || !TryReadClass(ptr.origin, ref offset, end_of_03, type, out dynamic result))
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
							Print($"Value is {Utils.ConvertToString(result)}... ");
						}
						break;
				}
				Print("Done.\n\n");
			}
			return true;
		}

		public bool TryReadClass(IntPtr origin, ref long offset, int end_of_03, Type type, out dynamic result)
		{
			// store the offset of this type
			long origin_offset = offset;
			// create a new instance of the type
			result = Activator.CreateInstance(type);
			// the type we create is not always a ref value so we need to make one
			// this won't throw an error if the type is already a ref value
			TypedReference tref = __makeref(result);
			// dynamics are not ref values for some reason so we need to store the resulting value in an object
			object ref_value;
			// iterate over the fields in this instance
			foreach (FieldInfo field in result.GetType().GetFields())
			{
				// attempt to read the data at the current offset as the field's type
				if (!TryReadField(origin, ref offset, end_of_03, field.FieldType, out dynamic value))
				{
					// if we fail, end the recursion
					return false;
				}
				// store the resulting value in the object
				ref_value = value;
				// set the field's value to the object
				field.SetValueDirect(tref, ref_value);
			}
			return true;
		}

		public bool TryReadField(IntPtr origin, ref long offset, int end_of_03, Type type, out dynamic value)
		{
			// if the type is a primitive value or a string, read it as such
			if ((type.IsPrimitive || type.Name.Equals("String")) && TryReadPrimitive(origin, ref offset, type, out value))
			{
				return true;
			}
			// if the type is an array or some form of an array type, read it as such
			else if ((type.IsArray || type.Name.Contains("List") || type.Name.Contains("Dictionary")) && TryReadArray(origin, ref offset, end_of_03, type, out value))
			{
				return true;
			}
			// if the type is a struct, read it as a class immediately
			else if (type.IsValueType && !type.IsEnum && TryReadClass(origin, ref offset, end_of_03, type, out value))
			{
				return true;
			}
			// if the type is a class
			else if (type.IsClass)
			{
				// read the pointer
				int target_offset = Marshal.ReadInt32(origin, (int)offset);
				offset += sizeof(int);
				// check if the object at the target offset exists already
				IntPtr ptr = new((long)origin + target_offset);
				if (_ptr_data.ContainsKey(ptr))
				{
					// if it does, point to it and stop here
					value = _ptr_data[ptr].ref_obj;
					return true;
				}
				// if it doesn't, store the current offset
				long ret_offset = offset;
				// jump to the pointer
				offset = target_offset;
				// and try to read it as a class
				if (!TryReadClass(origin, ref offset, end_of_03, type, out value))
				{
					// if it fails, stop the recursion
					return false;
				}
				// if it succeeds, add the resulting value to the list of pointers
				_ptr_data[ptr] = new PtrData(value, offset - target_offset);
				// and jump back to the stored offset
				offset = ret_offset;
				return true;
			}
			// if the type is anything else
			else
			{
				// set the value to null
				value = null;
				// and stop the recursion
				return false;
			}
		}

		public bool TryReadArray(IntPtr origin, ref long offset, int end_of_03, Type type, out dynamic value)
		{
			switch (type.Name.ToLowerInvariant())
			{
				case "list_s`1":
					{
						// read all the pointers to types in the list
						List<int> ptrs = new();
						do
						{
							ptrs.Add(Marshal.ReadInt32(origin, (int)offset));
							offset += sizeof(int);
						}
						while (offset < ptrs[0] && Marshal.ReadInt32(origin, (int)offset) != 0);
						// initialize the list
						dynamic list = Activator.CreateInstance(type);
						// iterate over the pointers
						for (int i = 0; i < ptrs.Count; i++)
						{
							// check if the object at this position was already created
							IntPtr ptr = new((long)origin + ptrs[i]);
							if (_ptr_data.ContainsKey(ptr))
							{
								// if it does, reference it and skip over its contents
								Print($"Skipping locked offset: 0x{offset:X}\n");
								list.Add(_ptr_data[ptr].ref_obj);
								offset += _ptr_data[ptr].size;
								continue;
							}
							// jump to the pointer's position
							offset = ptrs[i];
							// attempt to read the data recursively
							if (!TryReadField(origin, ref offset, end_of_03, type.GetGenericArguments()[0], out dynamic obj))
							{
								// if it fails, the return chain begins
								value = null;
								return false;
							}
							offset += sizeof(int) - (offset % sizeof(int));
							_ptr_data[ptr] = new PtrData(obj, offset - ptrs[i]);
							// if it succeeds, add it to the list
							list.Add(obj);
						}
						value = list;
						return true;
					}
				case "list_se`1":
					{
						int start = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						int end = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						long ret_offset = offset;
						dynamic list = Activator.CreateInstance(type);
						long previous_offset;
						for (offset = start; offset < end && offset < end_of_03;)
						{
							if (offset % sizeof(int) != 0)
							{
								offset += sizeof(int) - (offset % sizeof(int));
							}
							IntPtr ptr = new((long)origin + offset);
							if (_ptr_data.ContainsKey(ptr))
							{
								Print($"Skipping locked offset: 0x{offset:X}\n");
								list.Add(_ptr_data[ptr].ref_obj);
								offset += _ptr_data[ptr].size;
								continue;
							}
							previous_offset = offset;
							if (!TryReadField(origin, ref offset, end_of_03, type.GetGenericArguments()[0], out dynamic obj))
							{
								value = null;
								return false;
							}
							offset += sizeof(int) - (offset % sizeof(int));
							_ptr_data[ptr] = new PtrData(obj, offset - previous_offset);
							list.Add(obj);
						}
						offset = ret_offset;
						value = list;
						return true;
					}
				case "list_sp`1":
					{
						int start = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						int end = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						dynamic list = Activator.CreateInstance(type);
						long previous_offset;
						for (offset = start; offset < end && offset < end_of_03;)
						{
							IntPtr ptr = new((long)origin + offset);
							if (_ptr_data.ContainsKey(ptr))
							{
								Print($"Skipping locked offset: 0x{offset:X}\n");
								list.Add(_ptr_data[ptr].ref_obj);
								offset += _ptr_data[ptr].size;
								continue;
							}
							previous_offset = offset;
							if (!TryReadField(origin, ref offset, end_of_03, type.GetGenericArguments()[0], out dynamic obj))
							{
								value = null;
								return false;
							}
							_ptr_data[ptr] = new PtrData(obj, offset - previous_offset);
							list.Add(obj);
						}
						offset = end;
						value = list;
						return true;
					}
				case "list_ep`1":
					{
						int end = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						int padded = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						dynamic list = Activator.CreateInstance(type);
						long previous_offset;
						while (offset < end && offset < end_of_03)
						{
							IntPtr ptr = new((long)origin + offset);
							if (_ptr_data.ContainsKey(ptr))
							{
								Print($"Skipping locked offset: 0x{offset:X}\n");
								list.Add(_ptr_data[ptr].ref_obj);
								offset += _ptr_data[ptr].size;
								continue;
							}
							previous_offset = offset;
							if (!TryReadField(origin, ref offset, end_of_03, type.GetGenericArguments()[0], out dynamic obj))
							{
								value = null;
								return false;
							}
							_ptr_data[ptr] = new PtrData(obj, offset - previous_offset);
							list.Add(obj);
						}
						offset = padded;
						value = list;
						return true;
					}
				case "list_ce`1":
					{
						int count = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						int end = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						dynamic list = Activator.CreateInstance(type);
						for (int i = 0; i < count; i++)
						{
							if (!TryReadField(origin, ref offset, end_of_03, type.GetGenericArguments()[0], out dynamic obj))
							{
								value = null;
								return false;
							}
							list.Add(obj);
						}
						offset = end;
						value = list;
						return true;
					}
				case "list_c`1":
					{
						long ret_offset = offset + sizeof(int);
						offset = Marshal.ReadInt32(origin, (int)offset);
						Type generic = type.GetGenericArguments()[0];
						if (!TryReadPrimitive(origin, ref offset, generic, out dynamic count))
						{
							value = null;
							return false;
						}
						dynamic list = Activator.CreateInstance(type);
						for (int i = 0; i < count; i++)
						{
							if (!TryReadField(origin, ref offset, end_of_03, generic, out dynamic obj))
							{
								value = null;
								return false;
							}
							list.Add(obj);
						}
						value = list;
						offset = ret_offset;
						return true;
					}
				case "list_e`1":
					{
						int end = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						dynamic list = Activator.CreateInstance(type);
						while (offset < end && offset < end_of_03)
						{
							if (!TryReadField(origin, ref offset, end_of_03, type.GetGenericArguments()[0], out dynamic obj))
							{
								value = null;
								return false;
							}
							list.Add(obj);
						}
						offset = end;
						value = list;
						return true;
					}
				case "list`1":
					{
						// read all the pointers to types in the list
						List<int> ptrs = new();
						do
						{
							ptrs.Add(Marshal.ReadInt32(origin, (int)offset));
							offset += sizeof(int);
						}
						while (offset < ptrs[0] && Marshal.ReadInt32(origin, (int)offset) != 0);
						// initialize the list
						dynamic list = Activator.CreateInstance(type);
						// iterate over the pointers
						for (int i = 0; i < ptrs.Count - 1; i++)
						{
							IntPtr ptr = new((long)origin + ptrs[i]);
							if (_ptr_data.ContainsKey(ptr))
							{
								Print($"Skipping locked offset: 0x{offset:X}\n");
								list.Add(_ptr_data[ptr].ref_obj);
								offset += _ptr_data[ptr].size;
								continue;
							}
							// jump to the pointer's position
							offset = ptrs[i];
							// attempt to read the data recursively
							if (!TryReadField(origin, ref offset, end_of_03, type.GetGenericArguments()[0], out dynamic obj))
							{
								// if it fails, the return chain begins
								value = null;
								return false;
							}
							_ptr_data[ptr] = new PtrData(obj, offset - ptrs[i]);
							// if it succeeds, add it to the list
							list.Add(obj);
						}
						// jump to the last indexed position
						offset = ptrs[^1];
						value = list;
						return true;
					}
				case "dictionary`2":
					{
						dynamic dictionary = Activator.CreateInstance(type);
						while (offset < end_of_03)
						{
							int existing_instance = Marshal.ReadInt32(origin, (int)offset);
							offset += sizeof(int);
							if (existing_instance == 0)
							{
								break;
							}
							Type[] generics = type.GetGenericArguments();
							if (!TryReadField(origin, ref offset, end_of_03, generics[0], out dynamic key))
							{
								value = null;
								return false;
							}
							if (!TryReadField(origin, ref offset, end_of_03, generics[1], out dynamic val))
							{
								value = null;
								return false;
							}
							dictionary.Add(key, val);
						}
						value = dictionary;
						return true;
					}
				case "list_max_terminated`1":
					{
						IntPtr ptr = new((long)origin + offset);
						if (_ptr_data.ContainsKey(ptr))
						{
							Print($"Skipping locked offset: 0x{offset:X}\n");
							value = _ptr_data[ptr].ref_obj;
							offset += _ptr_data[ptr].size;
							return true;
						}
						Type generic = type.GetGenericArguments()[0];
						FieldInfo mv_field = generic.GetField("MaxValue");
						dynamic max_value;
						if (mv_field != null && mv_field.IsLiteral && !mv_field.IsInitOnly)
						{
							max_value = mv_field.GetRawConstantValue();
						}
						else
						{
							value = null;
							return false;
						}
						dynamic list = Activator.CreateInstance(type);
						long previous_offset = offset;
						while (offset < end_of_03)
						{
							if (!TryReadField(origin, ref offset, end_of_03, generic, out dynamic obj))
							{
								value = null;
								return false;
							}
							list.Add(obj);
							if (obj == max_value)
							{
								break;
							}
						}
						_ptr_data[ptr] = new PtrData(list, offset - previous_offset);
						value = list;
						return true;
					}
				default:
					{
						Type element_type = type.GetElementType();
						dynamic list = Activator.CreateInstance(typeof(List<>).MakeGenericType(element_type));
						long previous_offset = offset;
						do
						{
							if (!TryReadField(origin, ref offset, end_of_03, element_type, out dynamic obj))
							{
								value = null;
								return false;
							}
							if (obj.Equals(0) && list.Count > 0)
							{
								break;
							}
							list.Add(obj);
						}
						while (offset < end_of_03);
						value = list.ToArray();
						return true;
					}
			}
		}

		public bool TryReadPrimitive(IntPtr origin, ref long offset, Type type, out dynamic value)
		{
			switch (type.Name.ToLowerInvariant())
			{
				case "byte":
					{
						value = Marshal.ReadByte(origin, (int)offset);
						offset++;
						return true;
					}
				case "sbyte":
					{
						value = (sbyte)Marshal.ReadByte(origin, (int)offset);
						offset++;
						return true;
					}
				case "bool":
					{
						value = Marshal.ReadInt32(origin, (int)offset) == 1;
						offset++;
						return true;
					}
				case "uint16":
					{
						value = (ushort)Marshal.ReadInt16(origin, (int)offset);
						offset += sizeof(ushort);
						return true;
					}
				case "int16":
					{
						value = Marshal.ReadInt16(origin, (int)offset);
						offset += sizeof(short);
						return true;
					}
				case "uint32":
					{
						value = (uint)Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(uint);
						return true;
					}
				case "int32":
					{
						value = Marshal.ReadInt32(origin, (int)offset);
						offset += sizeof(int);
						return true;
					}
				case "uint64":
					{
						value = (ulong)Marshal.ReadInt64(origin, (int)offset);
						offset += sizeof(ulong);
						return true;
					}
				case "int64":
					{
						value = Marshal.ReadInt64(origin, (int)offset);
						offset += sizeof(long);
						return true;
					}
				case "char":
					{
						byte[] bytes = new byte[] { Marshal.ReadByte(origin, (int)offset) };
						offset++;
						value = Program.shift_jis.GetChars(bytes)[0];
						return true;
					}
				case "string":
					{
						byte[] bytes = Array.Empty<byte>();
						byte temp_byte;
						while (true)
						{
							temp_byte = Marshal.ReadByte(origin, (int)offset);
							offset++;
							if (temp_byte == 0)
							{
								while (Marshal.ReadByte(origin, (int)offset) == 0 && offset % sizeof(int) != 0)
								{
									offset++;
								}
								break;
							}
							Array.Resize(ref bytes, bytes.Length + 1);
							bytes[^1] = temp_byte;
						}
						value = Program.shift_jis.GetString(bytes);
						return true;
					}
				case "single":
					{
						value = Convert.ToSingle(Marshal.ReadInt32(origin, (int)offset));
						offset += sizeof(float);
						return true;
					}
				case "double":
					{
						value = Convert.ToDouble(Marshal.ReadInt64(origin, (int)offset));
						offset += sizeof(double);
						return true;
					}
			}
			value = null;
			return false;
		}

		/// <summary>
		/// Used to print to console a little easier
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		private void Print<T>(in T obj) where T : notnull
		{
			string str = Utils.ConvertToString(obj);
			logger.Write(str);
			Console.Out.Write(str);
		}
	}

	public enum SubDataType : byte
	{
		NONE,
		UNK_01 = 0x01,
		UNK_03 = 0x03,
		UNK_06 = 0x06,
		UNK_07 = 0x07,
		UNK_09 = 0x09,
		UNK_0C = 0x0C,
		UNK_11 = 0x11,
		UNK_1A = 0x1A,
		COUNT = 0xFF
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
		SPACE_DESCRIPTIONS = 0x88,
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
		//UNK_AE = 0xAE, // battle system text
		//UNK_AF = 0xAF, // battle system text part 2
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
	/* 
	 */
}
