using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class MagicData : BaseData
	{
		public string[] magic_base;
		public string[] magic_elements;
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
		public AbilityDarkling[] darkling_abilities;
		public string[] darkling_ability_descriptions;
		public AbilityField[] field_abilities;
		public string[] field_ability_descriptions;
		public AbilityBattle[] battle_abilities;
		public string[] battle_ability_descriptions;

		public MagicData(DataReader reader) : base(reader)
		{
			magic_base = Array.Empty<string>();
			magic_elements = Array.Empty<string>();
			offensive_magic = Array.Empty<Magic>();
			offensive_descriptions = Array.Empty<string>();
			defensive_magic = Array.Empty<Magic>();
			defensive_descriptions = Array.Empty<string>();
			item_magic = Array.Empty<MagicItem>();
			item_descriptions = Array.Empty<string>();
			unk_d1 = Array.Empty<MagicUnk_D1>();
			unk_d4 = Array.Empty<byte>();
			unk_d5 = Array.Empty<byte>();
			darkling_abilities = Array.Empty<AbilityDarkling>();
			darkling_ability_descriptions = Array.Empty<string>();
			field_abilities = Array.Empty<AbilityField>();
			field_ability_descriptions = Array.Empty<string>();
			battle_abilities = Array.Empty<AbilityBattle>();
			battle_ability_descriptions = Array.Empty<string>();
			int start;
			int end;
			int temp_offset;
			int table_id;
			dynamic magic_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x71:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref offensive_descriptions, offensive_descriptions.Length + 1);
								offensive_descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x8A:
						{
							magic_id = reader.ReadInt32();
							if (magic_id > magic_base.Length - 1)
							{
								Array.Resize(ref magic_base, magic_id + 1);
							}
							magic_base[magic_id] = reader.ReadString();
							break;
						}
					case 0x8B:
						{
							magic_id = reader.ReadInt32();
							if (magic_id > magic_elements.Length - 1)
							{
								Array.Resize(ref magic_elements, magic_id + 1);
							}
							magic_elements[magic_id] = reader.ReadString();
							break;
						}
					case 0x70:
						{
							magic_id = reader.ReadInt16();
							if (magic_id > offensive_magic.Length - 1)
							{
								Array.Resize(ref offensive_magic, magic_id + 1);
							}
							offensive_magic[magic_id] = reader.ReadStruct<Magic>();
							break;
						}
					case 0x73:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref defensive_descriptions, defensive_descriptions.Length + 1);
								defensive_descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x72:
						{
							magic_id = reader.ReadInt16();
							if (magic_id > defensive_magic.Length - 1)
							{
								Array.Resize(ref defensive_magic, magic_id + 1);
							}
							defensive_magic[magic_id] = reader.ReadStruct<Magic>();
							break;
						}
					case 0x75:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref item_descriptions, item_descriptions.Length + 1);
								item_descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x74:
						{
							magic_id = reader.ReadInt16();
							if (magic_id > item_magic.Length - 1)
							{
								Array.Resize(ref item_magic, magic_id + 1);
							}
							item_magic[magic_id] = reader.ReadStruct<MagicItem>();
							break;
						}
					case 0x9F:
						{
							unk_9f = reader.ReadInt32();
							break;
						}
					case 0xD1:
						{
							magic_id = reader.ReadByte();
							if (magic_id > unk_d1.Length - 1)
							{
								Array.Resize(ref unk_d1, magic_id + 1);
							}
							unk_d1[magic_id] = reader.ReadStruct<MagicUnk_D1>();
							break;
						}
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
					case 0xD8:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref darkling_ability_descriptions, darkling_ability_descriptions.Length + 1);
								darkling_ability_descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0xD9:
						{
							magic_id = reader.ReadByte();
							if (magic_id > darkling_abilities.Length - 1)
							{
								Array.Resize(ref darkling_abilities, magic_id + 1);
							}
							darkling_abilities[magic_id] = reader.ReadStruct<AbilityDarkling>();
							break;
						}
					case 0x7E:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref field_ability_descriptions, field_ability_descriptions.Length + 1);
								field_ability_descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x7B:
						{
							magic_id = reader.ReadByte();
							if (magic_id > field_abilities.Length - 1)
							{
								Array.Resize(ref field_abilities, magic_id + 1);
							}
							field_abilities[magic_id] = reader.ReadStruct<AbilityField>();
							break;
						}
					case 0x7D:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref battle_ability_descriptions, battle_ability_descriptions.Length + 1);
								battle_ability_descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x7A:
						{
							magic_id = reader.ReadByte();
							if (magic_id > battle_abilities.Length - 1)
							{
								Array.Resize(ref battle_abilities, magic_id + 1);
							}
							battle_abilities[magic_id] = reader.ReadStruct<AbilityBattle>();
							break;
						}
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
			writer.Write(0x71);
			int off_des_ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (int i = 1; i < magic_base.Length; i++)
			{
				writer.Write(0x8A);
				writer.Write(i);
				writer.Write(magic_base[i]);
			}
			for (int i = 1; i < magic_elements.Length; i++)
			{
				writer.Write(0x8B);
				writer.Write(i);
				writer.Write(magic_elements[i]);
			}
			for (short i = 1; i < offensive_magic.Length; i++)
			{
				writer.Write(0x70);
				writer.Write(i);
				writer.WriteStruct(offensive_magic[i]);
			}
			writer.Write(0x73);
			int def_des_ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (short i = 1; i < defensive_magic.Length; i++)
			{
				writer.Write(0x72);
				writer.Write(i);
				writer.WriteStruct(defensive_magic[i]);
			}
			writer.Write(0x75);
			int item_des_ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (short i = 1; i < item_magic.Length; i++)
			{
				writer.Write(0x74);
				writer.Write(i);
				writer.WriteStruct(item_magic[i]);
			}
			writer.Write(0x9F);
			writer.Write(unk_9f);
			for (byte i = 1; i < unk_d1.Length; i++)
			{
				writer.Write(0xD1);
				writer.Write(i);
				writer.WriteStruct(unk_d1[i]);
			}

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

			writer.Write(0xD8);
			int drk_des_ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (byte i = 1; i < darkling_abilities.Length; i++)
			{
				writer.Write(0xD9);
				writer.Write(i);
				writer.WriteStruct(darkling_abilities[i]);
			}
			writer.Write(0x7E);
			int fld_des_ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (byte i = 1; i < field_abilities.Length; i++)
			{
				writer.Write(0x7B);
				writer.Write(i);
				writer.WriteStruct(field_abilities[i]);
			}
			writer.Write(0x7D);
			int btl_des_ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (byte i = 1; i < battle_abilities.Length; i++)
			{
				writer.Write(0x7A);
				writer.Write(i);
				writer.WriteStruct(battle_abilities[i]);
			}
			writer.Write(0x03);
			int des_end_pos = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(0);
			temp_pos = writer.offset;
			for (int i = 0; i < darkling_ability_descriptions.Length; i++)
			{
				writer.Write(darkling_ability_descriptions[i]);
			}
			int ret_pos = writer.offset;
			writer.offset = drk_des_ptr;
			writer.Write(temp_pos);
			writer.Write(ret_pos);
			writer.offset = ret_pos;
			temp_pos = writer.offset;
			for (int i = 0; i < offensive_descriptions.Length; i++)
			{
				writer.Write(offensive_descriptions[i]);
			}
			ret_pos = writer.offset;
			writer.offset = off_des_ptr;
			writer.Write(temp_pos);
			writer.Write(ret_pos);
			writer.offset = ret_pos;
			temp_pos = writer.offset;
			for (int i = 0; i < defensive_descriptions.Length; i++)
			{
				writer.Write(defensive_descriptions[i]);
			}
			ret_pos = writer.offset;
			writer.offset = def_des_ptr;
			writer.Write(temp_pos);
			writer.Write(ret_pos);
			writer.offset = ret_pos;
			temp_pos = writer.offset;
			for (int i = 0; i < item_descriptions.Length; i++)
			{
				writer.Write(item_descriptions[i]);
			}
			ret_pos = writer.offset;
			writer.offset = item_des_ptr;
			writer.Write(temp_pos);
			writer.Write(ret_pos);
			writer.offset = ret_pos;
			temp_pos = writer.offset;
			for (int i = 0; i < field_ability_descriptions.Length; i++)
			{
				writer.Write(field_ability_descriptions[i]);
			}
			ret_pos = writer.offset;
			writer.offset = fld_des_ptr;
			writer.Write(temp_pos);
			writer.Write(ret_pos);
			writer.offset = ret_pos;
			temp_pos = writer.offset;
			for (int i = 0; i < battle_ability_descriptions.Length; i++)
			{
				writer.Write(battle_ability_descriptions[i]);
			}
			ret_pos = writer.offset;
			writer.offset = btl_des_ptr;
			writer.Write(temp_pos);
			writer.Write(ret_pos);
			writer.offset = des_end_pos;
			writer.Write(ret_pos);
			writer.offset = ret_pos;
		}
	}
}
