using CharaReader.data.chr_data;
using System;

namespace CharaReader.data
{
	public class CHR
	{
		public string class_data_start;
		public string class_data_end;

		public Weapon[] weapons;
		public WeaponModel[] weapon_models;
		public string[][] weapon_descriptions;

		public Shield[] shields;
		public ShieldModel[] shield_models;
		public string[] shield_descriptions;

		public Accessory[] accessories;
		public string[] accessory_descriptions;

		public Hair[] hair;
		public HairModel[] hair_models;
		public string[] hair_descriptions;

		public ItemType[] item_types;
		public Item[] items;
		public ItemGift[] item_gifts;
		public ItemUnk_D7[] item_data_d7;
		public ItemUnk_D0[] item_data_d0;
		public ItemFunc[] item_funcs;
		public string[] item_descriptions;

		public MagicType[] magic_base;
		public MagicType[] magic_elements;
		public Magic[] magic_off;
		public string[] magic_off_descriptions;
		public Magic[] magic_def;
		public string[] magic_def_descriptions;
		public MagicItem[] magic_items;
		public string[] magic_item_descriptions;
		public int magic_unk_9f;
		public MagicUnk_D1[] magic_unk_d1;
		public byte[] magic_unk_d4;
		public byte[] magic_unk_d5;
		public Ability[] ability_darkling;
		public string[] ability_darkling_descriptions;
		public Ability[] ability_field;
		public string[] ability_field_descriptions;
		public Ability[] ability_battle;
		public string[] ability_battle_descriptions;

		public Job[] jobs;
		public JobSkills[][] job_skills;
		public JobStats[][] job_stats;
		public JobMastery[][] job_mastery;
		public JobBag[][] job_bag_sizes;
		public JobUnk_3E[][] job_unk_3E;
		public JobMoney[][] job_money;
		public JobUnk_D6[] job_unk_d6;
		public JobModel[][][] job_models;
		public JobModel_0_3[][] job_model_0_3s;
		public JobModel_4_7[][] job_model_4_7s;
		public JobUnk_43[][] job_unk_43;
		public JobUnk_38[][] job_unk_38;
		public ushort[][] job_unk_39;
		public ushort[][] job_unk_8F;
		public string[] job_descriptions;

		public StatusPermanent[] status_permanent;
		public StatusBattle[] status_battle;
		public StatusField[] status_field;
		public StatusUnk_8E[] status_unk_8E;
		public StatusUnk_9A[][] status_unk_9A;
		public StatusUnk_9B[] status_unk_9B;
		public int length;

		public CHR(DataReader reader)
		{
			string name = string.Join("", reader.ReadChars(4));
			if (name != "@CHR")
			{
				throw new Exception($"Expected '@CHR' got {name}");
			}
			length = reader.ReadInt32();
			reader.offset = reader.ReadInt32();
			int table_id = reader.ReadInt32();
			int end;
			if (table_id == 0x01)
			{
				end = reader.ReadInt32();
				class_data_start = reader.ReadString();
				reader.offset = end;
			}

			weapon_descriptions = Array.Empty<string[]>();
			job_unk_39 = Array.Empty<ushort[]>();
			job_unk_8F = Array.Empty<ushort[]>();
			dynamic item_id;
			dynamic gender_id;
			dynamic extra_id;
			int temp;
			int offset;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x03: reader.offset = reader.ReadInt32(); break;

					case 0x5A:
					case 0x63:
						{
							Array.Resize(ref weapon_descriptions, weapon_descriptions.Length + 1);
							weapon_descriptions[^1] = reader.ReadDescriptions();
							break;
						}
					case 0x58: weapons = reader.ReadStructs<Weapon>(table_id); break;
					case 0x59: weapon_models = reader.ReadStructs<WeaponModel>(table_id); break;

					case 0x60: shield_descriptions = reader.ReadDescriptions(); break;
					case 0x5E: shields = reader.ReadStructs<Shield>(table_id); break;
					case 0x5F: shield_models = reader.ReadStructs<ShieldModel>(table_id); break;

					case 0x65: accessory_descriptions = reader.ReadDescriptions(); break;
					case 0x64: accessories = reader.ReadStructs<Accessory>(table_id); break;

					case 0x95: hair_descriptions = reader.ReadDescriptions(); break;
					case 0x96: hair = reader.ReadStructs<Hair>(table_id); break;
					case 0x97: hair_models = reader.ReadStructs<HairModel>(table_id); break;

					case 0x6A: item_descriptions = reader.ReadDescriptions(); break;
					case 0x89: item_types = reader.ReadStructs<ItemType>(table_id); break;
					case 0x69: items = reader.ReadStructs<Item>(table_id); break;
					case 0x6C: item_gifts = reader.ReadStructs<ItemGift>(table_id); break;
					case 0xD7: item_data_d7 = reader.ReadStructs<ItemUnk_D7>(table_id); break;
					case 0xD0: item_data_d0 = reader.ReadStructs<ItemUnk_D0>(table_id); break;
					case 0x6B: item_funcs = reader.ReadStructs<ItemFunc>(table_id); break;

					case 0x71: magic_off_descriptions = reader.ReadDescriptions(); break;
					case 0x8A: magic_base = reader.ReadStructs<MagicType>(table_id); break;
					case 0x8B: magic_elements = reader.ReadStructs<MagicType>(table_id); break;
					case 0x70: magic_off = reader.ReadStructs<Magic>(table_id); break;
					case 0x73: magic_def_descriptions = reader.ReadDescriptions(); break;
					case 0x72: magic_def = reader.ReadStructs<Magic>(table_id); break;
					case 0x75: magic_item_descriptions = reader.ReadDescriptions(); break;
					case 0x74: magic_items = reader.ReadStructs<MagicItem>(table_id); break;
					case 0x9F: magic_unk_9f = reader.ReadInt32(); break;
					case 0xD1: magic_unk_d1 = reader.ReadStructs<MagicUnk_D1>(table_id); break;
					case 0xD4:
						{
							item_id = reader.ReadInt32();
							end = reader.ReadInt32();
							magic_unk_d4 = reader.ReadBytes(item_id);
							reader.offset = end;
							break;
						}
					case 0xD5:
						{
							item_id = reader.ReadInt32();
							end = reader.ReadInt32();
							magic_unk_d5 = reader.ReadBytes(item_id);
							reader.offset = end;
							break;
						}
					case 0xD8: ability_darkling_descriptions = reader.ReadDescriptions(); break;
					case 0xD9: ability_darkling = reader.ReadStructs<Ability>(table_id); break;
					case 0x7E: ability_field_descriptions = reader.ReadDescriptions(); break;
					case 0x7B: ability_field = reader.ReadStructs<Ability>(table_id); break;
					case 0x7D: ability_battle_descriptions = reader.ReadDescriptions(); break;
					case 0x7A: ability_battle = reader.ReadStructs<Ability>(table_id); break;

					case 0x3D: job_descriptions = reader.ReadDescriptions(); break;
					case 0x42: jobs = reader.ReadStructs<Job>(table_id); break;
					case 0x3C: job_skills = reader.ReadStructs2<JobSkills>(table_id); break;
					case 0x40: job_stats = reader.ReadStructs2<JobStats>(table_id); break;
					case 0x3B: job_mastery = reader.ReadStructs2<JobMastery>(table_id); break;
					case 0x44: job_bag_sizes = reader.ReadStructs2<JobBag>(table_id); break;
					case 0x3E: job_unk_3E = reader.ReadStructs2<JobUnk_3E>(table_id); break;
					case 0x2E: job_money = reader.ReadStructs2<JobMoney>(table_id); break;
					case 0xD6: job_unk_d6 = reader.ReadStructs<JobUnk_D6>(table_id); break;
					case 0x41: job_models = reader.ReadStructs3<JobModel>(table_id); break;
					case 0x45: job_model_0_3s = reader.ReadStructs2<JobModel_0_3>(table_id); break;
					case 0x29: job_model_4_7s = reader.ReadStructs2<JobModel_4_7>(table_id); break;
					case 0x43: job_unk_43 = reader.ReadStructs2<JobUnk_43>(table_id); break;
					case 0x38: job_unk_38 = reader.ReadStructs2<JobUnk_38>(table_id); break;
					case 0x39:
						{
							// reads to 0xFFFF
							item_id = reader.ReadInt32();
							if (item_id > job_unk_39.Length - 1)
							{
								Array.Resize(ref job_unk_39, item_id + 1);
								job_unk_39[item_id] = Array.Empty<ushort>();
							}
							temp = reader.ReadInt32();
							offset = reader.offset;
							reader.offset = temp;
							while (reader.offset < reader.length)
							{
								Array.Resize(ref job_unk_39[item_id], job_unk_39[item_id].Length + 1);
								job_unk_39[item_id][^1] = reader.ReadUInt16();
								if (job_unk_39[item_id][^1] == ushort.MaxValue)
								{
									reader.ReadInt16();
									break;
								}
							}
							reader.offset = offset;
							break;
						}
					case 0x8F:
						{
							// reads to 0xFFFF
							item_id = reader.ReadInt32();
							if (item_id > job_unk_8F.Length - 1)
							{
								Array.Resize(ref job_unk_8F, item_id + 1);
								job_unk_8F[item_id] = Array.Empty<ushort>();
							}
							temp = reader.ReadInt32();
							offset = reader.offset;
							reader.offset = temp;
							while (reader.offset < reader.length)
							{
								Array.Resize(ref job_unk_8F[item_id], job_unk_8F[item_id].Length + 1);
								job_unk_8F[item_id][^1] = reader.ReadUInt16();
								if (job_unk_8F[item_id][^1] == ushort.MaxValue)
								{
									reader.ReadInt16();
									break;
								}
							}
							reader.offset = offset;
							break;
						}
					default:
						{
							reader.offset -= sizeof(int);
							finished = true;
							break;
						}
				}
			}
			finished = false;
			table_id = reader.ReadInt32();
			if (table_id == 0x01)
			{
				end = reader.ReadInt32();
				class_data_end = reader.ReadString();
				reader.offset = end;
			}

			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{

					case 0x81: status_permanent = reader.ReadStructs<StatusPermanent>(table_id); break;
					case 0x82: status_battle = reader.ReadStructs<StatusBattle>(table_id); break;
					case 0x83: status_field = reader.ReadStructs<StatusField>(table_id); break;
					case 0x8E: status_unk_8E = reader.ReadStructs<StatusUnk_8E>(table_id); break;
					case 0x9A: status_unk_9A = reader.ReadStructs2<StatusUnk_9A>(table_id); break;
					case 0x9B: status_unk_9B = reader.ReadStructs<StatusUnk_9B>(table_id); break;

					case 0x9E: finished = true; break;

					default: throw new Exception($"Unhandled table {(reader.offset - sizeof(int)).ToHexString()}->{((byte)table_id).ToHexString()}");
				}
			}
		}

		public void Write(DataWriter writer)
		{
			writer.ReservePointer(0x52484340, "chr_file_len_ptr");
			writer.Write(0x30);
			writer.offset = 0x30;

			writer.ReservePointer(0x01, "class_name_ptr");
			writer.Write(class_data_start);
			writer.WritePointer("class_name_ptr");

			writer.ReservePointer(0x5A, "des_1_pos", 2);
			writer.ReservePointer(0x63, "des_0_pos", 2);
			writer.WriteStructs(0x58, weapons);
			writer.WriteStructs(0x59, weapon_models);
			writer.ReservePointer(0x03, "des_len_pos");
			writer.Write(0);
			writer.WritePointer("des_0_pos");
			writer.WriteDescriptions(weapon_descriptions[1]);
			writer.WritePointer("des_0_pos");
			writer.WritePointer("des_1_pos");
			writer.WriteDescriptions(weapon_descriptions[0]);
			writer.WritePointer("des_1_pos");
			writer.WritePointer("des_len_pos");

			writer.ReservePointer(0x60, "des_ptr_pos", 2);
			writer.WriteStructs(0x5E, shields);
			writer.WriteStructs(0x5F, shield_models);
			writer.ReservePointer(0x03, "des_end_pos");
			writer.Write(0);
			writer.WritePointer("des_ptr_pos");
			writer.WriteDescriptions(shield_descriptions);
			writer.WritePointer("des_ptr_pos");
			writer.WritePointer("des_end_pos");

			writer.ReservePointer(0x65, "acc_des_ptr", 2);
			writer.WriteStructs(0x64, accessories);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("acc_des_ptr");
			writer.WriteDescriptions(accessory_descriptions);
			writer.WritePointer("acc_des_ptr");
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x95, "hair_des_ptr", 2);
			writer.WriteStructs(0x96, hair);
			writer.WriteStructs(0x97, hair_models);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("hair_des_ptr");
			writer.WriteDescriptions(hair_descriptions);
			writer.WritePointer("hair_des_ptr");
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x6A, "des_ptr_pos", 2);
			writer.WriteStructs(0x89, item_types);
			writer.WriteStructs(0x69, items);
			writer.WriteStructs(0x6C, item_gifts);
			writer.WriteStructs(0xD7, item_data_d7);
			writer.WriteStructs(0xD0, item_data_d0);
			writer.WriteStructs(0x6B, item_funcs);
			writer.ReservePointer(0x03, "des_end_pos");
			writer.Write(0);
			writer.WritePointer("des_ptr_pos");
			writer.WriteDescriptions(item_descriptions);
			writer.WritePointer("des_ptr_pos");
			writer.WritePointer("des_end_pos");

			writer.ReservePointer(0x71, "off_des_ptr", 2);
			writer.WriteStructs(0x8A, magic_base);
			writer.WriteStructs(0x8B, magic_elements);
			writer.WriteStructs(0x70, magic_off);
			writer.ReservePointer(0x73, "def_des_ptr", 2);
			writer.WriteStructs(0x72, magic_def);
			writer.ReservePointer(0x75, "item_des_ptr", 2);
			writer.WriteStructs(0x74, magic_items);

			writer.Write(0x9F); // ????
			writer.Write(magic_unk_9f);

			writer.WriteStructs(0xD1, magic_unk_d1);

			writer.Write(0xD4);
			writer.Write(magic_unk_d4.Length);
			int d4_end_ptr = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(magic_unk_d4);
			while (writer.offset % 4 != 0)
			{
				writer.offset++;
			}
			int temp_pos = writer.offset;
			writer.offset = d4_end_ptr;
			writer.Write(temp_pos);
			writer.offset = temp_pos;

			writer.Write(0xD5);
			writer.Write(magic_unk_d5.Length);
			int d5_end_ptr = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(magic_unk_d5);
			while (writer.offset % 4 != 0)
			{
				writer.offset++;
			}
			temp_pos = writer.offset;
			writer.offset = d5_end_ptr;
			writer.Write(temp_pos);
			writer.offset = temp_pos;

			writer.ReservePointer(0xD8, "drk_des_ptr", 2);
			writer.WriteStructs(0xD9, ability_darkling);
			writer.ReservePointer(0x7E, "fld_des_ptr", 2);
			writer.WriteStructs(0x7B, ability_field);
			writer.ReservePointer(0x7D, "btl_des_ptr", 2);
			writer.WriteStructs(0x7A, ability_battle);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("drk_des_ptr");
			writer.WriteDescriptions(ability_darkling_descriptions);
			writer.WritePointer("drk_des_ptr");
			writer.WritePointer("off_des_ptr");
			writer.WriteDescriptions(magic_off_descriptions);
			writer.WritePointer("off_des_ptr");
			writer.WritePointer("def_des_ptr");
			writer.WriteDescriptions(magic_def_descriptions);
			writer.WritePointer("def_des_ptr");
			writer.WritePointer("item_des_ptr");
			writer.WriteDescriptions(magic_item_descriptions);
			writer.WritePointer("item_des_ptr");
			writer.WritePointer("fld_des_ptr");
			writer.WriteDescriptions(ability_field_descriptions);
			writer.WritePointer("fld_des_ptr");
			writer.WritePointer("btl_des_ptr");
			writer.WriteDescriptions(ability_battle_descriptions);
			writer.WritePointer("btl_des_ptr");
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x30, "job_des_ptr", 2);
			writer.WriteStructs(0x42, jobs);
			writer.WriteStructs(0x3C, job_skills);
			writer.WriteStructs(0x40, job_stats);
			writer.WriteStructs(0x3B, job_mastery);
			writer.WriteStructs(0x44, job_bag_sizes);
			writer.WriteStructs(0x3E, job_unk_3E);
			writer.WriteStructs(0x2E, job_money);
			writer.WriteStructs(0xD6, job_unk_d6);
			writer.WriteStructs(0x41, job_models);
			writer.WriteStructs(0x45, job_model_0_3s);
			writer.WriteStructs(0x29, job_model_4_7s);
			writer.WriteStructs(0x43, job_unk_43);
			writer.WriteStructs(0x38, job_unk_38);
			for (int i = 0; i < job_unk_39.Length; i++)
			{
				writer.ReservePointer(0x39, $"39_ptr_{i}", 2);
			}
			for (int i = 0; i < job_unk_8F.Length; i++)
			{
				writer.ReservePointer(0x8F, $"8F_ptr_{i}", 2);
			}
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			for (int i = 0; i < job_unk_39.Length; i++)
			{
				writer.WritePointerID($"39_ptr_{i}", i);
				for (int j = 0; j < job_unk_39[i].Length; j++)
				{
					writer.Write(job_unk_39[i][j]);
				}
				writer.Write((short)0);
			}
			for (int i = 0; i < job_unk_8F.Length; i++)
			{
				writer.WritePointerID($"8F_ptr_{i}", i);
				for (int j = 0; j < job_unk_8F[i].Length; j++)
				{
					writer.Write(job_unk_8F[i][j]);
				}
				writer.Write((short)0);
			}
			writer.WritePointer("job_des_ptr");
			writer.WriteDescriptions(job_descriptions);
			writer.WritePointer("job_des_ptr");
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x01, "class_name_ptr");
			writer.Write(class_data_end);
			writer.WritePointer("class_name_ptr");

			writer.WriteStructs(0x81, status_permanent);
			writer.WriteStructs(0x82, status_battle);
			writer.WriteStructs(0x83, status_field);
			writer.WriteStructs(0x8E, status_unk_8E);
			writer.WriteStructs(0x9A, status_unk_9A);
			writer.WriteStructs(0x9B, status_unk_9B);

			// more goes here

			writer.WritePointer("chr_file_len_ptr");
		}
	}
}
