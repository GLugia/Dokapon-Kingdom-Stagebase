using CharaReader.data.chr_data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharaReader.data
{
	public class CHR
	{
		public string class_data_start;
		public string class_data_end;

		public byte[][] descriptions;
		public List<Action<byte[], int>> description_ptr_handlers;

		public Weapon[] weapons;
		public WeaponModel[] weapon_models;
		public Description weapon_descriptions;
		public Description weapon_bonus_descriptions;

		public Shield[] shields;
		public ShieldModel[] shield_models;
		public Description shield_descriptions;

		public Accessory[] accessories;
		public Description accessory_descriptions;

		public Hair[] hair;
		public HairModel[] hair_models;
		public Description hair_descriptions;

		public ItemType[] item_types;
		public Item[] items;
		public ItemGift[] item_gifts;
		public ItemUnk_D7[] item_data_d7;
		public ItemUnk_D0[] item_data_d0;
		public ItemFunc[] item_funcs;
		public Description item_descriptions;

		public MagicType[] magic_base;
		public MagicType[] magic_elements;
		public Magic[] magic_off;
		public Description magic_off_descriptions;
		public Magic[] magic_def;
		public Description magic_def_descriptions;
		public MagicItem[] magic_items;
		public Description magic_item_descriptions;
		public int magic_unk_9f;
		public MagicUnk_D1[] magic_unk_d1;
		public byte[] magic_unk_d4;
		public byte[] magic_unk_d5;
		public Ability[] ability_darkling;
		public Description ability_darkling_descriptions;
		public Ability[] ability_field;
		public Description ability_field_descriptions;
		public Ability[] ability_battle;
		public Description ability_battle_descriptions;

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
		public Description job_descriptions;

		public StatusPermanent[] status_permanent;
		public StatusBattle[] status_battle;
		public StatusField[] status_field;
		public StatusUnk_8E[] status_unk_8E;
		public StatusUnk_9A[][] status_unk_9A;
		public StatusUnk_9B[] status_unk_9B;

		public byte[] unk_9E;
		public ushort[] unk_8C;
		public Description[] npc_names;

		public Unk_17[] unk_17;
		public Description npc_enemy_descriptions;
		public NPCEnemy[] npc_enemies;
		public NPCEnemyDropTable[] npc_enemy_drop_tables;
		public NPCEnemyModel[] npc_enemy_models;
		public NPCEnemyModel_0[] npc_enemy_models_0;
		public Unk_5B[] unk_5B;
		public Unk_62[] unk_62;
		public Unk_52[] unk_52;
		public Unk_54[] unk_54;
		public NPC[] npcs;
		public NPCModel[] npc_models;
		public NPCModel_0[] npc_models_0;
		public Unk_5D[] unk_5D;
		public Unk_5D[] unk_2A;

		public int unk_2C;
		public Unk_2D[] unk_2D;
		public Unk_9D[] unk_9D;
		public Unk_9D[] unk_9C;
		public Unk_E1[] unk_E1;
		public Description unk_4F;
		public Unk_4E[] unk_4E;
		public Description unk_4D;
		public Description unk_76;
		public Description unk_77;

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

			descriptions = Array.Empty<byte[]>();
			description_ptr_handlers = new();
			job_unk_39 = Array.Empty<ushort[]>();
			job_unk_8F = Array.Empty<ushort[]>();
			npc_names = Array.Empty<Description>();
			npc_enemy_models_0 = Array.Empty<NPCEnemyModel_0>();
			npc_models_0 = Array.Empty<NPCModel_0>();

			dynamic item_id;
			int temp;
			int offset;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x03:
						{
							end = reader.ReadInt32(); // this is technically a long int value but the highest 8 bits are always 0
							reader.ReadUInt32();
							offset = reader.offset;
							byte[] description = reader.ReadBytes(end - reader.offset);
							foreach (Action<byte[], int> action in description_ptr_handlers)
							{
								action.Invoke(description, offset);
							}
							description_ptr_handlers.Clear();
							break;
						}
					#region Weapon Data
					case 0x5A: weapon_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x63: weapon_bonus_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x58: weapons = reader.ReadStructs<Weapon>(table_id); break;
					case 0x59: weapon_models = reader.ReadStructs<WeaponModel>(table_id); break;
					#endregion
					#region Shield Data
					case 0x60: shield_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x5E: shields = reader.ReadStructs<Shield>(table_id); break;
					case 0x5F: shield_models = reader.ReadStructs<ShieldModel>(table_id); break;
					#endregion
					#region Accessory Data
					case 0x65: accessory_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x64: accessories = reader.ReadStructs<Accessory>(table_id); break;
					#endregion
					#region Hair Data
					case 0x95: hair_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x96: hair = reader.ReadStructs<Hair>(table_id); break;
					case 0x97: hair_models = reader.ReadStructs<HairModel>(table_id); break;
					#endregion
					#region Item Data
					case 0x6A: item_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x89: item_types = reader.ReadStructs<ItemType>(table_id); break;
					case 0x69: items = reader.ReadStructs<Item>(table_id); break;
					case 0x6C: item_gifts = reader.ReadStructs<ItemGift>(table_id); break;
					case 0xD7: item_data_d7 = reader.ReadStructs<ItemUnk_D7>(table_id); break;
					case 0xD0: item_data_d0 = reader.ReadStructs<ItemUnk_D0>(table_id); break;
					case 0x6B: item_funcs = reader.ReadStructs<ItemFunc>(table_id); break;
					#endregion
					#region Magic Data
					case 0x71: magic_off_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x8A: magic_base = reader.ReadStructs<MagicType>(table_id); break;
					case 0x8B: magic_elements = reader.ReadStructs<MagicType>(table_id); break;
					case 0x70: magic_off = reader.ReadStructs<Magic>(table_id); break;
					case 0x73: magic_def_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x72: magic_def = reader.ReadStructs<Magic>(table_id); break;
					case 0x75: magic_item_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
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
					case 0xD8: ability_darkling_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0xD9: ability_darkling = reader.ReadStructs<Ability>(table_id); break;
					case 0x7E: ability_field_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x7B: ability_field = reader.ReadStructs<Ability>(table_id); break;
					case 0x7D: ability_battle_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x7A: ability_battle = reader.ReadStructs<Ability>(table_id); break;
					#endregion
					#region Job Data
					case 0x3D: job_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
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
					#endregion
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
					case 0x03:
						{
							end = reader.ReadInt32(); // this is technically a 64bit value but the highest 8 bits are always 0
							reader.ReadUInt32();
							offset = reader.offset;
							Array.Resize(ref descriptions, descriptions.Length + 1);
							descriptions[^1] = reader.ReadBytes(end - reader.offset);
							foreach (Action<byte[], int> action in description_ptr_handlers)
							{
								action.Invoke(descriptions[^1], offset);
							}
							description_ptr_handlers.Clear();
							break;
						}

					#region Status Effects
					case 0x81: status_permanent = reader.ReadStructs<StatusPermanent>(table_id); break;
					case 0x82: status_battle = reader.ReadStructs<StatusBattle>(table_id); break;
					case 0x83: status_field = reader.ReadStructs<StatusField>(table_id); break;
					case 0x8E: status_unk_8E = reader.ReadStructs<StatusUnk_8E>(table_id); break;
					case 0x9A: status_unk_9A = reader.ReadStructs2<StatusUnk_9A>(table_id); break;
					case 0x9B: status_unk_9B = reader.ReadStructs<StatusUnk_9B>(table_id); break;
					#endregion
					case 0x9E:
						{
							offset = reader.ReadInt32();
							end = reader.ReadInt32();
							unk_9E = reader.ReadBytes(offset - reader.offset);
							reader.offset = end;
							break;
						}
					case 0x8C:
						{
							end = reader.ReadInt32();
							unk_8C = Array.Empty<ushort>();
							while (reader.offset < end)
							{
								Array.Resize(ref unk_8C, unk_8C.Length + 1);
								unk_8C[^1] = reader.ReadUInt16();
							}
							break;
						}
					case 0x3A:
						{
							Array.Resize(ref npc_names, npc_names.Length + 1);
							reader.ReadInt32();
							npc_names[^1] = ReadDescription_String(reader.ReadInt32(), alignment: sizeof(ushort));
							break;
						}
					case 0x17: unk_17 = reader.ReadStructs<Unk_17>(table_id); break;

					case 0x7C: npc_enemy_descriptions = ReadDescription_String(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x50: npc_enemies = reader.ReadStructs<NPCEnemy>(table_id); break;
					case 0x53: npc_enemy_drop_tables = reader.ReadStructs<NPCEnemyDropTable>(table_id); break;
					case 0x51: npc_enemy_models = reader.ReadStructs<NPCEnemyModel>(table_id); break;
					case 0x61:
						{
							item_id = reader.ReadInt32();
							if (item_id > npc_enemy_models_0.Length - 1)
							{
								Array.Resize(ref npc_enemy_models_0, item_id + 1);
							}
							npc_enemy_models_0[item_id].item_id = item_id;
							npc_enemy_models_0[item_id].f0 = reader.ReadString();
							npc_enemy_models_0[item_id].fg0 = reader.ReadString();
							npc_enemy_models_0[item_id].description = ReadDescription_PointerArray(reader, 0xFFFF, sizeof(ushort));
							break;
						}
					case 0x5B: unk_5B = reader.ReadStructs<Unk_5B>(table_id); break;
					case 0x62: unk_62 = reader.ReadStructs<Unk_62>(table_id); break;
					case 0x52: unk_52 = reader.ReadStructs<Unk_52>(table_id); break;
					case 0x54: unk_54 = reader.ReadStructs<Unk_54>(table_id); break;
					// case 0x03

					case 0x55: npcs = reader.ReadStructs<NPC>(table_id); break;
					case 0x56: npc_models = reader.ReadStructs<NPCModel>(table_id); break;
					case 0x57:
						{
							item_id = reader.ReadInt32();
							if (item_id > npc_models_0.Length - 1)
							{
								Array.Resize(ref npc_models_0, item_id + 1);
							}
							npc_models_0[item_id].item_id = item_id;
							npc_models_0[item_id].f0 = reader.ReadString();
							npc_models_0[item_id].k0 = reader.ReadString();
							npc_models_0[item_id].fg0 = reader.ReadString();
							npc_models_0[item_id].descriptions_0 = ReadDescription_PointerArray(reader, 0xFFFF, sizeof(ushort));
							npc_models_0[item_id].descriptions_1 = ReadDescription_PointerArray(reader, 0xFFFF, sizeof(ushort));
							break;
						}
					case 0x5D: unk_5D = reader.ReadStructs<Unk_5D>(table_id); break;
					case 0x2A: unk_2A = reader.ReadStructs<Unk_5D>(table_id); break;

					case 0x2C: unk_2C = reader.ReadInt32(); break;
					case 0x2D: unk_2D = reader.ReadStructs<Unk_2D>(table_id); break;
					case 0x9D: unk_9D = reader.ReadStructs<Unk_9D>(table_id); break;
					case 0x9C: unk_9C = reader.ReadStructs<Unk_9D>(table_id); break;
					case 0xE1: unk_E1 = reader.ReadStructs<Unk_E1>(table_id); break;
					case 0x4F:
						{
							temp = reader.ReadInt32();
							unk_4F = ReadDescriptions3(temp);
							break;
						}
					case 0x4E: unk_4E = reader.ReadStructs<Unk_4E>(table_id); break;
					case 0x4D:
						{
							temp = reader.ReadInt32(); // start_offset
							unk_4D = ReadDescriptions3(temp);
							break;
						}
					case 0x76:
						{
							temp = reader.ReadInt32();
							unk_76 = ReadDescriptions3(temp);
							break;
						}
					case 0x77: finished = true; break;
					default: throw new Exception($"Unhandled table {(reader.offset - sizeof(int)).ToHexString()}->{((byte)table_id).ToHexString()}");
				}
			}
		}

		// Reads a pointer declaring a starting and ending offset
		private Description ReadDescription_String(int start_offset, int? end_offset = null, int alignment = sizeof(int))
		{
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			description_ptr_handlers.Add((a, b) => Utils.ReadDescription_String(a, b, ref ret, start_offset, end_offset, alignment));
			return ret;
		}

		// Reads an array of ordered pointers separated by a 32bit boolean value
		private Description ReadDescription_PointerArray(DataReader reader, dynamic separator, int alignment = sizeof(int))
		{
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			int origin = reader.offset;
			// while there is another ordered pointer
			while (reader.ReadInt32() != 0)
			{
				// skip the unnecessary id
				reader.ReadInt32();
				// read the starting offset
				int start_offset = reader.ReadInt32();
				// create a new action for the 0x03 object to invoke
				description_ptr_handlers.Add(new(delegate (byte[] a, int b)
				{
					if (start_offset - b == 0)
					{
						Console.Out.WriteLine($"Repeated instance at {origin.ToHexString()}");
					}
					Utils.ReadDescription_Array(a, b, ref ret, start_offset, null, separator, alignment);
				}));
			}
			return ret;
		}

		private Description ReadDescriptions3(int start_offset)
		{
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			description_ptr_handlers.Add((a, b) => Utils.ReadDescription_PointerArray(a, b, ref ret, start_offset));
			return ret;
		}

		public void Write(DataWriter writer)
		{
			writer.ReservePointer(0x52484340, "chr_file_len_ptr");
			writer.Write(0x30);
			writer.offset = 0x30;
			#region Class Data
			writer.ReservePointer(0x01, "class_name_ptr");
			writer.Write(class_data_start);
			writer.WritePointer("class_name_ptr");

			writer.ReservePointer(0x5A, "weapon_descriptions", 2);
			writer.ReservePointer(0x63, "weapon_bonus", 2);
			writer.WriteStructs(0x58, weapons);
			writer.WriteStructs(0x59, weapon_models);
			writer.ReservePointer(0x03, "des_len_pos");
			writer.Write(0);
			writer.WritePointer("weapon_bonus");
			writer.WriteDescriptions(weapon_bonus_descriptions);
			writer.WritePointer("weapon_bonus");
			writer.WritePointer("weapon_descriptions");
			writer.WriteDescriptions(weapon_descriptions);
			writer.WritePointer("weapon_descriptions");
			writer.WritePointer("des_len_pos");

			writer.ReservePointer(0x60, "shield_descriptions", 2);
			writer.WriteStructs(0x5E, shields);
			writer.WriteStructs(0x5F, shield_models);
			writer.ReservePointer(0x03, "des_end_pos");
			writer.Write(0);
			writer.WritePointer("shield_descriptions");
			writer.WriteDescriptions(shield_descriptions);
			writer.WritePointer("shield_descriptions");
			writer.WritePointer("des_end_pos");

			writer.ReservePointer(0x65, "accessory_descriptions", 2);
			writer.WriteStructs(0x64, accessories);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("accessory_descriptions");
			writer.WriteDescriptions(accessory_descriptions);
			writer.WritePointer("accessory_descriptions");
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x95, "hair_descriptions", 2);
			writer.WriteStructs(0x96, hair);
			writer.WriteStructs(0x97, hair_models);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("hair_descriptions");
			writer.WriteDescriptions(hair_descriptions);
			writer.WritePointer("hair_descriptions");
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x6A, "item_descriptions", 2);
			writer.WriteStructs(0x89, item_types);
			writer.WriteStructs(0x69, items);
			writer.WriteStructs(0x6C, item_gifts);
			writer.WriteStructs(0xD7, item_data_d7);
			writer.WriteStructs(0xD0, item_data_d0);
			writer.WriteStructs(0x6B, item_funcs);
			writer.ReservePointer(0x03, "des_end_pos");
			writer.Write(0);
			writer.WritePointer("item_descriptions");
			writer.WriteDescriptions(item_descriptions);
			writer.WritePointer("item_descriptions");
			writer.WritePointer("des_end_pos");

			writer.ReservePointer(0x71, "magic_offensive", 2);
			writer.WriteStructs(0x8A, magic_base);
			writer.WriteStructs(0x8B, magic_elements);
			writer.WriteStructs(0x70, magic_off);
			writer.ReservePointer(0x73, "magic_defensive", 2);
			writer.WriteStructs(0x72, magic_def);
			writer.ReservePointer(0x75, "magic_item", 2);
			writer.WriteStructs(0x74, magic_items);

			writer.Write(0x9F); // ????
			writer.Write(magic_unk_9f);

			writer.WriteStructs(0xD1, magic_unk_d1);

			writer.Write(0xD4);
			writer.Write(magic_unk_d4.Length);
			int d4_end_ptr = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(magic_unk_d4);
			if (writer.offset % sizeof(int) != 0)
			{
				writer.offset += writer.offset - (writer.offset % sizeof(int));
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
			if (writer.offset % sizeof(int) != 0)
			{
				writer.offset += sizeof(int) - (writer.offset % sizeof(int));
			}
			temp_pos = writer.offset;
			writer.offset = d5_end_ptr;
			writer.Write(temp_pos);
			writer.offset = temp_pos;

			writer.ReservePointer(0xD8, "ability_darkling", 2);
			writer.WriteStructs(0xD9, ability_darkling);
			writer.ReservePointer(0x7E, "ability_field", 2);
			writer.WriteStructs(0x7B, ability_field);
			writer.ReservePointer(0x7D, "ability_battle", 2);
			writer.WriteStructs(0x7A, ability_battle);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("ability_darkling");
			writer.WriteDescriptions(ability_darkling_descriptions);
			writer.WritePointer("ability_darkling");
			writer.WritePointer("magic_offensive");
			writer.WriteDescriptions(magic_off_descriptions);
			writer.WritePointer("magic_offensive");
			writer.WritePointer("magic_defensive");
			writer.WriteDescriptions(magic_def_descriptions);
			writer.WritePointer("magic_defensive");
			writer.WritePointer("magic_item");
			writer.WriteDescriptions(magic_item_descriptions);
			writer.WritePointer("magic_item");
			writer.WritePointer("ability_field");
			writer.WriteDescriptions(ability_field_descriptions);
			writer.WritePointer("ability_field");
			writer.WritePointer("ability_battle");
			writer.WriteDescriptions(ability_battle_descriptions);
			writer.WritePointer("ability_battle");
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x3D, "job_des_ptr", 2);
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
			#endregion
			#region Completed Data
			writer.WriteStructs(0x81, status_permanent);
			writer.WriteStructs(0x82, status_battle);
			writer.WriteStructs(0x83, status_field);
			writer.WriteStructs(0x8E, status_unk_8E);
			writer.WriteStructs(0x9A, status_unk_9A);
			writer.WriteStructs(0x9B, status_unk_9B);

			writer.ReservePointer(0x9E, "9E_ptr", 2);
			writer.Write(unk_9E);
			writer.WritePointer("9E_ptr");
			writer.offset += 4 - (writer.offset % 4);
			writer.WritePointer("9E_ptr");

			writer.ReservePointer(0x8C, "8C_ptr");
			for (int i = 0; i < unk_8C.Length; i++)
			{
				writer.Write(unk_8C[i]);
			}
			writer.WritePointer("8C_ptr");

			writer.ReservePointer(0x3A, "male_name_ptr", 2);
			writer.ReservePointer(0x3A, "female_name_ptr", 2);

			writer.ReservePointer(0x03, "des_ptr");
			writer.Write(0);
			writer.WritePointerID("male_name_ptr", 0);
			writer.WriteDescriptions(npc_names[0]);
			writer.Write(0);
			writer.WritePointerID("female_name_ptr", 1);
			writer.WriteDescriptions(npc_names[1]);
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));
			writer.WritePointer("des_ptr");

			writer.WriteStructs(0x17, unk_17);

			writer.ReservePointer(0x7C, "npc_enemy_descriptions", 2);
			writer.WriteStructs(0x50, npc_enemies);
			writer.WriteStructs(0x53, npc_enemy_drop_tables);
			writer.WriteStructs(0x51, npc_enemy_models);
			
			for (int i = 0; i < npc_enemy_models_0.Length; i++)
			{
				if (npc_enemy_models_0[i].description == null)
				{
					continue;
				}
				writer.Write(0x61);
				writer.Write(npc_enemy_models_0[i].item_id);
				writer.Write(npc_enemy_models_0[i].f0);
				writer.Write(npc_enemy_models_0[i].fg0);
				writer.ReservePointerArray($"npc_enemy_models_0", npc_enemy_models_0[i].description.ptrs);
			}
			
			writer.WriteStructs(0x5B, unk_5B);
			writer.WriteStructs(0x62, unk_62);
			writer.WriteStructs(0x52, unk_52);
			writer.WriteStructs(0x54, unk_54);

			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WriteDescriptions("npc_enemy_models_0", npc_enemy_models_0.Select(a => a.description).ToArray(), 0xFFFF);
			writer.WritePointer("npc_enemy_descriptions");
			writer.WriteDescriptions(npc_enemy_descriptions);
			writer.WritePointer("npc_enemy_descriptions");
			writer.WritePointer("des_end_ptr");

			writer.WriteStructs(0x55, npcs);
			writer.WriteStructs(0x56, npc_models);
			for (int i = 0; i < npc_models_0.Length; i++)
			{
				if (npc_models_0[i].f0 == null)
				{
					continue;
				}
				writer.Write(0x57);
				writer.Write(npc_models_0[i].item_id);
				writer.Write(npc_models_0[i].f0);
				writer.Write(npc_models_0[i].k0);
				writer.Write(npc_models_0[i].fg0);
				writer.ReservePointerArray($"npc_models_0_0_{i}", npc_models_0[i].descriptions_0.ptrs);
				writer.ReservePointerArray($"npc_models_0_1_{i}", npc_models_0[i].descriptions_1.ptrs);
			}
			
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WriteDescriptions("npc_models_0", npc_models_0.Select(a => a.descriptions_0).Union(npc_models_0.Select(a => a.descriptions_1)).ToArray(), 0xFFFF);
			writer.WritePointer("des_end_ptr");
			#endregion

			writer.WriteStructs(0x5D, unk_5D);
			writer.WriteStructs(0x2A, unk_2A);
			writer.Write(0x2C);
			writer.Write(unk_2C);
			writer.WriteStructs(0x2D, unk_2D);
			writer.WriteStructs(0x9D, unk_9D);
			writer.WriteStructs(0x9C, unk_9C);
			writer.WriteStructs(0xE1, unk_E1);
			writer.ReservePointer(0x4F, "4F_ptrs");
			writer.WriteStructs(0x4E, unk_4E);
			writer.ReservePointer(0x4D, "4D_ptrs");
			
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			int origin_offset = writer.offset;
			writer.WritePointer("4D_ptrs");
			for (int i = 0; i < unk_4D.ptrs.Length; i++)
			{
				// include the index of the pointer to avoid errors
				// all duplicates will be properly handled
				writer.ReservePointerNoID($"4D_ptrs_{i}_{unk_4D.ptrs[i]}");
			}
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));
			for (int i = 0; i < unk_4D.description.Length; i++)
			{
				writer.WriteAllPointers("4D_ptrs", writer.offset - origin_offset);
				writer.Write(unk_4D.description[i]);
			}
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));

			writer.WritePointer("4F_ptrs");
			for (int i = 0; i < unk_4F.ptrs.Length; i++)
			{
				writer.ReservePointerNoID($"4F_ptrs_{i}_{unk_4F.ptrs[i]}");
			}
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));
			for (int i = 0; i < unk_4F.description.Length; i++)
			{
				writer.WriteAllPointers("4F_ptrs", writer.offset - origin_offset);
				writer.Write(unk_4F.description[i]);
			}
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));

			writer.WritePointer("des_end_ptr");
			
			// more goes here

			writer.WritePointer("chr_file_len_ptr");
		}
	}
}
