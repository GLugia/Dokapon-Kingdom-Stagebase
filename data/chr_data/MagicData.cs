using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class MagicData : BaseData
	{
		public MagicType[] magic_base;
		public MagicType[] magic_elements;
		public Magic[] offensive_magic;
		public string[] offensive_descriptions;
		public Magic[] defensive_magic;
		public string[] defensive_descriptions;
		public MagicItem[] item_magic;
		public string[] item_descriptions;
		public int unk_9f;
		public MagicUnk_D1[] unk_d1;
		public byte[] unk_d4;
		public byte[] unk_d5;
		public Ability[] darkling_abilities;
		public string[] darkling_ability_descriptions;
		public Ability[] field_abilities;
		public string[] field_ability_descriptions;
		public Ability[] battle_abilities;
		public string[] battle_ability_descriptions;

		public MagicData(DataReader reader) : base(reader)
		{
			int end;
			int table_id;
			dynamic magic_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x71: offensive_descriptions = reader.ReadDescriptions(); break;
					case 0x8A: magic_base = reader.ReadStructs<MagicType>(table_id); break;
					case 0x8B: magic_elements = reader.ReadStructs<MagicType>(table_id); break;
					case 0x70: offensive_magic = reader.ReadStructs<Magic>(table_id); break;
					case 0x73: defensive_descriptions = reader.ReadDescriptions(); break;
					case 0x72: defensive_magic = reader.ReadStructs<Magic>(table_id); break;
					case 0x75: item_descriptions = reader.ReadDescriptions(); break;
					case 0x74: item_magic = reader.ReadStructs<MagicItem>(table_id); break;
					case 0x9F: unk_9f = reader.ReadInt32(); break; 
					case 0xD1: unk_d1 = reader.ReadStructs<MagicUnk_D1>(table_id); break;
					case 0xD4:
						{
							magic_id = reader.ReadInt32();
							end = reader.ReadInt32();
							unk_d4 = reader.ReadBytes(magic_id);
							reader.offset = end;
							break;
						}
					case 0xD5:
						{
							magic_id = reader.ReadInt32();
							end = reader.ReadInt32();
							unk_d5 = reader.ReadBytes(magic_id);
							reader.offset = end;
							break;
						}
					case 0xD8: darkling_ability_descriptions = reader.ReadDescriptions(); break;
					case 0xD9: darkling_abilities = reader.ReadStructs<Ability>(table_id); break;
					case 0x7E: field_ability_descriptions = reader.ReadDescriptions(); break;
					case 0x7B: field_abilities = reader.ReadStructs<Ability>(table_id); break;
					case 0x7D: battle_ability_descriptions = reader.ReadDescriptions(); break;
					case 0x7A: battle_abilities = reader.ReadStructs<Ability>(table_id); break;
					case 0x03:
						{
							reader.offset = reader.ReadInt32();
							finished = true;
							break;
						}
					default:
						{
							throw new Exception($"Unknown table {(reader.offset - sizeof(int)).ToHexString()}->{((byte)table_id).ToHexString()}");
						}
				}
			}
		}

		/*   offensive
		 * 0x71 - des_ptr
		 *   base
		 * 0x8A - int
		 *   element
		 * 0x8B - int
		 *   offensive
		 * 0x70 - short
		 *   defensive
		 * 0x73 - des_ptr
		 * 0x72 - short
		 *   items
		 * 0x75 - des_ptr
		 * 0x74 - short
		 * 0xD1 - int
		 * 0xD4
		 * 0xD5
		 *   darkling
		 * 0xD8 - des_ptr
		 * 0xD9 - byte
		 *   field
		 * 0x7E - des_ptr
		 * 0x7B - byte
		 *   battle
		 * 0x7D - des_ptr
		 * 0x7A - byte
		 *   descriptions
		 * 0x03
		 */
		public override void Write(DataWriter writer)
		{
			writer.ReservePointer(0x71, "off_des_ptr", 2);
			writer.WriteStructs(0x8A, magic_base);
			writer.WriteStructs(0x8B, magic_elements);
			writer.WriteStructs(0x70, offensive_magic);
			writer.ReservePointer(0x73, "def_des_ptr", 2);
			writer.WriteStructs(0x72, defensive_magic);
			writer.ReservePointer(0x75, "item_des_ptr", 2);
			writer.WriteStructs(0x74, item_magic);

			writer.Write(0x9F); // ????
			writer.Write(unk_9f);

			writer.WriteStructs(0xD1, unk_d1);

			writer.Write(0xD4);
			writer.Write(unk_d4.Length);
			int d4_end_ptr = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(unk_d4);
			while (writer.offset % 4 != 0)
			{
				writer.offset++;
			}
			int temp_pos = writer.offset;
			writer.offset = d4_end_ptr;
			writer.Write(temp_pos);
			writer.offset = temp_pos;

			writer.Write(0xD5);
			writer.Write(unk_d5.Length);
			int d5_end_ptr = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(unk_d5);
			while (writer.offset % 4 != 0)
			{
				writer.offset++;
			}
			temp_pos = writer.offset;
			writer.offset = d5_end_ptr;
			writer.Write(temp_pos);
			writer.offset = temp_pos;

			writer.ReservePointer(0xD8, "drk_des_ptr", 2);
			writer.WriteStructs(0xD9, darkling_abilities);
			writer.ReservePointer(0x7E, "fld_des_ptr", 2);
			writer.WriteStructs(0x7B, field_abilities);
			writer.ReservePointer(0x7D, "btl_des_ptr", 2);
			writer.WriteStructs(0x7A, battle_abilities);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("drk_des_ptr");
			writer.WriteDescriptions(darkling_ability_descriptions);
			writer.WritePointer("drk_des_ptr");
			writer.WritePointer("off_des_ptr");
			writer.WriteDescriptions(offensive_descriptions);
			writer.WritePointer("off_des_ptr");
			writer.WritePointer("def_des_ptr");
			writer.WriteDescriptions(defensive_descriptions);
			writer.WritePointer("def_des_ptr");
			writer.WritePointer("item_des_ptr");
			writer.WriteDescriptions(item_descriptions);
			writer.WritePointer("item_des_ptr");
			writer.WritePointer("fld_des_ptr");
			writer.WriteDescriptions(field_ability_descriptions);
			writer.WritePointer("fld_des_ptr");
			writer.WritePointer("btl_des_ptr");
			writer.WriteDescriptions(battle_ability_descriptions);
			writer.WritePointer("btl_des_ptr");
			writer.WritePointer("des_end_ptr");
		}
	}
}
