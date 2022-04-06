using CharaReader.data.bas_data;
using System;
using System.Linq;

namespace CharaReader.data
{
	public class BAS
	{
		public string base_data_start;
		public string base_data_end;
		public Stage[] stages;
		public Unk_6E[] unk_6E;
		public Location[] locations;
		public string[] space_descriptions;
		public Space[] spaces;
		public Temple[] temples;
		public Unk_68[] unk_68;
		public int unk_66;
		public Town[] towns;
		public Unk_6F[] unk_6F;
		public Unk_93[] unk_93;
		public Unk_2B[] unk_2B;
		public ushort[] unk_DB;
		public Unk_DA[] unk_DA;
		public byte[] unk_E0;
		public Unk_2F[][] unk_2F;
		public ChestStatusEffects[][] chest_status_effects;
		public ushort[][] unk_7F;
		public byte[] unk_94;
		public byte[][] unk_85;
		public byte[][][] unk_78;
		public Unk_AC[] unk_AC;
		public int length;

		public BAS(DataReader reader)
		{
			string name = string.Join("", reader.ReadChars(4));
			if (name != "@BAS")
			{
				throw new Exception($"Expected '@BAS' got '{name}'");
			}
			length = reader.ReadInt32();
			reader.offset = reader.ReadInt32();
			int table_id = reader.ReadInt32();
			int end;
			if (table_id == 0x01)
			{
				end = reader.ReadInt32();
				base_data_start = reader.ReadString();
				reader.offset = end;
			}

			stages = Array.Empty<Stage>();
			unk_85 = new byte[2][];
			unk_AC = Array.Empty<Unk_AC>();
			dynamic item_id;
			int temp;
			int offset;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x01:
						{
							end = reader.ReadInt32();
							base_data_end = reader.ReadString();
							reader.offset = end;
							break;
						}
					case 0x03: reader.offset = reader.ReadInt32(); break;
					case 0x05:
						{
							item_id = reader.ReadInt32();
							if (item_id > stages.Length - 1)
							{
								Array.Resize(ref stages, item_id + 1);
							}
							temp = reader.ReadInt32();
							offset = reader.offset;
							reader.offset = temp;
							stages[item_id] = new()
							{
								item_id = item_id,
								file = reader.ReadString()
							};
							reader.offset = offset;
							break;
						}
					case 0x6E: unk_6E = reader.ReadStructs<Unk_6E>(table_id); break;
					case 0x37: locations = reader.ReadStructs<Location>(table_id); break;
					case 0x88: space_descriptions = reader.ReadDescriptions(); break;
					case 0x87: spaces = reader.ReadStructs<Space>(table_id); break;
					case 0x67: temples = reader.ReadStructs<Temple>(table_id); break;
					case 0x68: unk_68 = reader.ReadStructs<Unk_68>(table_id); break;
					case 0x66: unk_66 = reader.ReadInt32(); break;
					case 0x6D: towns = reader.ReadStructs<Town>(table_id); break;
					case 0x6F: unk_6F = reader.ReadStructs<Unk_6F>(table_id); break;
					case 0x93: unk_93 = reader.ReadStructs<Unk_93>(table_id); break;
					case 0x2B: unk_2B = reader.ReadStructs<Unk_2B>(table_id); break;
					case 0xDB:
						{
							reader.ReadInt32();
							int outer_end = reader.ReadInt32();
							reader.ReadInt32();
							unk_DB = Array.Empty<ushort>();
							while (reader.offset < outer_end)
							{
								Array.Resize(ref unk_DB, unk_DB.Length + 1);
								unk_DB[^1] = reader.ReadUInt16();
							}
							break;
						}
					case 0xDA: unk_DA = reader.ReadStructs<Unk_DA>(table_id); break;
					case 0xE0:
						{
							end = reader.ReadInt32();
							temp = reader.ReadInt32();
							unk_E0 = reader.ReadBytes(end - reader.offset);
							reader.offset = temp;
							break;
						}
					case 0x2F: unk_2F = reader.ReadStructs2<Unk_2F>(table_id); break;
					case 0x86: chest_status_effects = reader.ReadStructs2<ChestStatusEffects>(table_id); break;
					case 0x7F:
						{
							int[] offsets = Array.Empty<int>();
							do
							{
								item_id = reader.ReadInt32();
								if (item_id > offsets.Length - 1)
								{
									Array.Resize(ref offsets, item_id + 1);
								}
								offsets[item_id] = reader.ReadInt32();
							}
							while (reader.ReadInt32() == table_id);
							reader.offset -= sizeof(int);
							offset = reader.offset;
							unk_7F = Array.Empty<ushort[]>();
							for (int i = 0; i < offsets.Length; i++)
							{
								if (i > unk_7F.Length - 1)
								{
									Array.Resize(ref unk_7F, i + 1);
									unk_7F[i] = Array.Empty<ushort>();
								}
								reader.offset = offsets[i];
								unk_7F[i] = new ushort[reader.ReadUInt16() + 1];
								for (int j = 0; j < unk_7F[i].Length; j++)
								{
									unk_7F[i][j] = reader.ReadUInt16();
								}
							}
							reader.offset = offset;
							break;
						}
					case 0x94:
						{
							temp = reader.ReadInt32();
							end = reader.ReadInt32();
							reader.offset = temp;
							unk_94 = reader.ReadBytes(end - temp);
							break;
						}
					case 0x85:
						{
							temp = reader.ReadInt32();
							end = reader.ReadInt32();
							offset = reader.offset;
							reader.offset = temp;
							unk_85[0] = reader.ReadBytes(end - temp);
							reader.offset = offset;
							temp = end;
							end = reader.ReadInt32();
							reader.offset = temp;
							unk_85[1] = reader.ReadBytes(end - temp);
							break;
						}
					case 0x78:
						{
							unk_78 = Array.Empty<byte[][]>();
							int[] offsets = Array.Empty<int>();
							do
							{
								item_id = reader.ReadInt32();
								if (item_id > offsets.Length - 1)
								{
									Array.Resize(ref offsets, item_id + 1);
								}
								offsets[item_id] = reader.ReadInt32();
								Array.Resize(ref unk_78, item_id + 1);
								unk_78[item_id] = Array.Empty<byte[]>();
							}
							while (reader.ReadInt32() == table_id);
							reader.offset -= sizeof(int);
							offset = reader.offset;
							for (int i = 0; i < offsets.Length; i++)
							{
								if (offsets[i] == 0)
								{
									continue;
								}
								reader.offset = offsets[i];
								int[] sub_offsets = Array.Empty<int>();
								dynamic temp_offset;
								while ((temp_offset = reader.ReadInt32()) != 0)
								{
									Array.Resize(ref sub_offsets, sub_offsets.Length + 1);
									sub_offsets[^1] = temp_offset;
								}
								Array.Resize(ref unk_78[i], sub_offsets.Length);
								for (int j = 0; j < sub_offsets.Length - 1; j++)
								{
									if (sub_offsets[j] == 0)
									{
										continue;
									}
									reader.offset = sub_offsets[j];
									unk_78[i][j] = reader.ReadBytes(sub_offsets[j + 1] - sub_offsets[j]);
								}
								unk_78[i][^1] = Array.Empty<byte>();
								while ((temp_offset = reader.ReadInt16()) != 0)
								{
									Array.Resize(ref unk_78[i][^1], unk_78[i][^1].Length + sizeof(short));
									BitConverter.GetBytes(temp_offset).CopyTo(unk_78[i][^1], unk_78[i][^1].Length - sizeof(short));
								}
							}
							reader.offset = offset;
							break;
						}
					case 0xAC:
						{
							item_id = reader.ReadInt32();
							temp = reader.ReadInt32();
							offset = reader.offset;
							reader.offset = temp;
							if (item_id > unk_AC.Length - 1)
							{
								Array.Resize(ref unk_AC, item_id + 1);
							}
							unk_AC[item_id].unk_00 = reader.ReadBytes(0x20);
							unk_AC[item_id].file_names = Array.Empty<string>();
							unk_AC[item_id].unk_01 = Array.Empty<byte[]>();
							while (reader.ReadInt32() == 1)
							{
								temp = reader.ReadInt16() + 1;
								reader.offset += sizeof(short);
								if (temp > unk_AC[item_id].file_names.Length - 1)
								{
									Array.Resize(ref unk_AC[item_id].file_names, temp + 1);
								}
								unk_AC[item_id].file_names[temp] = reader.ReadString();
							}
							while (reader.ReadInt32() == 1)
							{
								temp = reader.ReadInt16();
								if (temp > unk_AC[item_id].unk_01.Length - 1)
								{
									Array.Resize(ref unk_AC[item_id].unk_01, temp + 1);
								}
								unk_AC[item_id].unk_01[temp] = reader.ReadBytes(0xE);
							}
							reader.offset = offset;
							break;
						}
					case 0x79:
						{
							finished = true;
							break;
						}
					default: throw new Exception($"Unknown table {(reader.offset - sizeof(int)).ToHexString()}->{((byte)table_id).ToHexString()}");
				}
			}
		}

		public void Write(DataWriter writer)
		{
			writer.ReservePointer(0x53414240, "bas_file_len_ptr");
			writer.Write(0x30);
			writer.offset = 0x30;

			writer.ReservePointer(0x01, "base_name_ptr");
			writer.Write(base_data_start);
			writer.WritePointer("base_name_ptr");

			for (int i = 0; i < stages.Length; i++)
			{
				writer.ReservePointer(0x05, $"stage_ptr_{i}", 2);
			}

			writer.WriteStructs(0x6E, unk_6E);
			writer.WriteStructs(0x37, locations);
			writer.ReservePointer(0x88, "des_ptr", 2);
			writer.WriteStructs(0x87, spaces);
			writer.WriteStructs(0x67, temples);
			writer.WriteStructs(0x68, unk_68);
			writer.Write(0x66);
			writer.Write(unk_66);
			writer.WriteStructs(0x6D, towns);
			writer.WriteStructs(0x6F, unk_6F);
			writer.WriteStructs(0x93, unk_93);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("des_ptr");
			writer.WriteDescriptions(space_descriptions);
			writer.WritePointer("des_ptr");
			writer.WritePointer("des_end_ptr");
			writer.WriteStructs(0x2B, unk_2B);

			writer.Write(0xDB);
			writer.Write((byte)0x55);
			writer.Write((byte)0x13);
			writer.offset += 2;
			int ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (int i = 0; i < unk_DB.Length; i++)
			{
				writer.Write(unk_DB[i]);
			}
			int end = writer.offset;
			writer.offset = ptr;
			writer.Write(end);
			writer.Write(end);
			writer.offset = end;

			for (int i = 0; i < unk_DA.Length; i++)
			{
				writer.Write(0xDA);
				writer.Write(unk_DA[i].item_id);
				writer.Write(unk_DA[i].unk_00);
			}

			writer.ReservePointer(0xE0, "E0_ptr", 2);
			writer.Write(unk_E0);
			writer.WritePointer("E0_ptr");
			writer.offset += writer.offset % 4;
			writer.WritePointer("E0_ptr");

			writer.WriteStructs(0x2F, unk_2F);
			writer.WriteStructs(0x86, chest_status_effects);

			// 0x7F goes here but idk how to write it
			for (int i = 0; i < unk_7F.Length; i++)
			{
				writer.ReservePointer(0x7F, $"7F_ptr_{i}", 2);
			}

			writer.ReservePointer(0x94, "94_ptr", 2);
			writer.WritePointer("94_ptr");
			writer.Write(unk_94);
			writer.WritePointer("94_ptr");

			writer.ReservePointer(0x85, "85_ptr", 3);
			writer.WritePointer("85_ptr");
			writer.Write(unk_85[0]);
			writer.WritePointer("85_ptr");
			writer.Write(unk_85[1]);
			writer.WritePointer("85_ptr");

			for (int i = 0; i < unk_78.Length; i++)
			{
				if (unk_78[i] == null || unk_78[i].Length == 0)
				{
					continue;
				}
				writer.ReservePointer(0x78, $"78_ptr_{i}", 2);
			}

			writer.ReservePointer(0x03, "des_ptr");
			writer.Write(0);

			for (int i = 0; i < unk_7F.Length; i++)
			{
				writer.WritePointerID($"7F_ptr_{i}", i);
				writer.Write((ushort)(unk_7F[i].Length - 1));
				for (int j = 0; j < unk_7F[i].Length; j++)
				{
					writer.Write(unk_7F[i][j]);
				}
			}

			for (int i = 0; i < unk_78.Length; i++)
			{
				if (unk_78[i] == null || unk_78[i].Length == 0)
				{
					continue;
				}
				writer.WritePointerID($"78_ptr_{i}", i);
				writer.ReservePointerNoID("78_sub_ptr", unk_78[i].Length);
				writer.Write(0);
				for (int j = 0; j < unk_78[i].Length; j++)
				{
					writer.WritePointer("78_sub_ptr");
					writer.Write(unk_78[i][j]);
				}
				writer.offset += 8 - (writer.offset % 8);
			}

			writer.WritePointer("des_ptr");

			for (int i = 0; i < unk_AC.Length; i++)
			{
				if (unk_AC[i].file_names == null)
				{
					continue;
				}
				writer.ReservePointer(0xAC, $"AC_ptr_{i}", 2);
			}

			writer.ReservePointer(0x03, "des_ptr");
			writer.Write(0);
			for (int i = 0x1E; i < unk_AC.Length; i++)
			{
				if (unk_AC[i].unk_01.Length == 0)
				{
					continue;
				}
				writer.WritePointerID($"AC_ptr_{i}", i);
				writer.Write(unk_AC[i].unk_00);
				for (ushort j = 0; j < unk_AC[i].file_names.Length; j++)
				{
					writer.Write(1);
					writer.Write((short)(j - 1));
					writer.offset += sizeof(short);
					writer.Write(unk_AC[i].file_names[j]);
				}
				writer.Write(0);
				for (short j = 0; j < unk_AC[i].unk_01.Length; j++)
				{
					writer.Write(1);
					writer.Write(j);
					writer.Write(unk_AC[i].unk_01[j]);
				}
				writer.Write(0);
			}
			int last_null = 0;
			for (int i = 0x1E; i < unk_AC.Length; i++)
			{
				if (unk_AC[i].unk_01.Length != 0)
				{
					continue;
				}
				writer.WritePointerID($"AC_ptr_{i}", i);
				last_null = i;
			}
			writer.Write(unk_AC[last_null].unk_00);
			writer.Write(0);
			writer.Write(0);
			writer.WritePointer("des_ptr");

			writer.ReservePointer(0x01, "base_name_ptr");
			writer.Write(base_data_end);
			writer.WritePointer("base_name_ptr");

			int offset = writer.offset;
			writer.offset += sizeof(int);
			int length = 0;
			writer.Write(stages.FirstOrDefault().file[0]);
			writer.offset += 4 - (writer.offset % 4);
			writer.Write(0xF);
			string last_file = "";
			for (int i = 0; i < stages.Length; i++)
			{
				if (stages[i].file == last_file)
				{
					continue;
				}
				last_file = stages[i].file;
				writer.WritePointerID($"stage_ptr_{i}", i);
				writer.Write(stages[i].file);
				length += stages[i].file.Length - 2;
			}
			writer.offset -= last_file.Length;
			for (int i = stages.Length - 1; i > -1; i--)
			{
				if (stages[i].file != last_file)
				{
					continue;
				}
				try
				{
					writer.WritePointerID($"stage_ptr_{i}", i);
				}
				catch (Exception)
				{
					break;
				}
			}
			int temp = writer.offset;
			writer.offset = offset;
			writer.Write((length / 2) + 1);
			writer.offset = temp;
			writer.offset += last_file.Length;
		}
	}
}
