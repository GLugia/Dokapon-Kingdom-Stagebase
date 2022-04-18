using CharaReader.data.chr_data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharaReader.data
{
	public class CHR
	{
		#region Completed Fields
		public string[] file_labels;
		public int next_label_id;

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

		public Unk_E2[] unk_E2;
		public Unk_49[] unk_49;
		public Unk_5C[] unk_5C;
		public Description npc_all_text;
		public Description[] npc_texts;

		public int unk_47;
		public Unk_84[] unk_84;
		public Unk_1C[] unk_1C;
		public Unk_1D[] unk_1D;
		public Unk_1E[] unk_1E;
		public Unk_46[] unk_46;
		public Unk_4C[] unk_4C;
		public Unk_4C[] unk_48;
		public Unk_4C[] unk_3F;
		public Unk_DF[] unk_DF;
		public Unk_D2[] unk_D2;
		public Unk_D2[] unk_D3;
		public Description unk_C0;
		public Unk_BF[] unk_BF;
		#endregion

		public Description battle_system_text;
		public Description battle_system_text_b;
		public Unk_A1 unk_A1;
		public Unk_A0 unk_A0;
		public Unk_A2 unk_A2;
		public Unk_A3 unk_A3;
		public Unk_A4 unk_A4;
		public Unk_A5 unk_A5;
		public Description unk_AD;

		public int length;

		/// <summary>
		/// A class for containing character related data.
		/// </summary>
		/// <param name="reader">The DataReader instance to use for reading.</param>
		/// <exception cref="Exception">If the <paramref name="reader"/> is not specifically meant for this file.</exception>
		public CHR(DataReader reader)
		{
			// read the first 4 bytes as char values
			string name = string.Join("", reader.ReadChars(4));
			// if they do not equal @CHR, throw an exception
			if (name != "@CHR")
			{
				throw new Exception($"Expected '@CHR' got {name}");
			}
			// read the length of this file
			length = reader.ReadInt32();
			// jump to the offset listed after the length (0x30)
			reader.offset = reader.ReadInt32();

			next_label_id = 0;
			file_labels = Array.Empty<string>();
			descriptions = Array.Empty<byte[]>();
			description_ptr_handlers = new();
			job_unk_39 = Array.Empty<ushort[]>();
			job_unk_8F = Array.Empty<ushort[]>();
			npc_names = Array.Empty<Description>();
			npc_enemy_models_0 = Array.Empty<NPCEnemyModel_0>();
			npc_models_0 = Array.Empty<NPCModel_0>();
			npc_texts = Array.Empty<Description>();
			unk_48 = Array.Empty<Unk_4C>();

			int table_id;
			int end;
			dynamic item_id;
			int temp;
			int offset;
			bool finished = false;
			// while finished is false and the offset is lower than the length
			// TODO: remove finished variable once file handling is complete
			while (!finished && reader.offset < reader.length)
			{
				// read the next table_id
				table_id = reader.ReadInt32();
				// according to the table_id's value, jump
				switch (table_id)
				{
					case 0x01:
						{
							// read the pointer to the end of the data
							end = reader.ReadInt32();
							// append the name of this label
							Array.Resize(ref file_labels, file_labels.Length + 1);
							file_labels[^1] = reader.ReadString();
							Console.Out.WriteLine(file_labels[^1]);
							// jump to the end
							reader.offset = end;
							break;
						}
					case 0x03:
						{
							// read the pointer to the end of the data
							end = reader.ReadInt32();
							// skip the 0
							reader.ReadUInt32();
							// mark the current offset
							// it's used as the 'origin' offset of the data
							// essentially 'origin' means the offset of the first byte in an array
							offset = reader.offset;
							// read all bytes from the current offset to the end pointer
							byte[] description = reader.ReadBytes(end - reader.offset);

							/* this is where things get complicated.
							 * this object can be handled 4 different ways so far. none of which are obvious.
							 * due to this, we have to handle things a little different than with other table_ids.
							 * here, we iterate over all actions in the list and invoke each one. typically this
							 * will only invoke one or two actions but there are cases where it may invoke tons.
							 * more will be explained about this as we go on.
							 */
							foreach (Action<byte[], int> action in description_ptr_handlers)
							{
								// invoke the action using the buffered bytes from description and the origin offset
								action.Invoke(description, offset);
							}
							// clear all actions
							description_ptr_handlers.Clear();
							break;
						}
					#region Weapon Data
					// handle the next 0x03 object, from 'a' to 'b', as a string array
					case 0x5A: weapon_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					// handle the next 0x03 object, from 'a' to 'b', as a string array
					// i properly name the parameters according to what they're used for so i won't be using the
					// "from 'a' to 'b'" shit anymore
					case 0x63: weapon_bonus_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					// read the weapons directly as structs. this function explains how it works and what it does.
					// i will not be commenting these objects as they're literally everywhere.
					case 0x58: weapons = reader.ReadStructs<Weapon>(table_id); break;
					case 0x59: weapon_models = reader.ReadStructs<WeaponModel>(table_id); break;
					#endregion
					#region Shield Data
					case 0x60: shield_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x5E: shields = reader.ReadStructs<Shield>(table_id); break;
					case 0x5F: shield_models = reader.ReadStructs<ShieldModel>(table_id); break;
					#endregion
					#region Accessory Data
					case 0x65: accessory_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x64: accessories = reader.ReadStructs<Accessory>(table_id); break;
					#endregion
					#region Hair Data
					case 0x95: hair_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x96: hair = reader.ReadStructs<Hair>(table_id); break;
					case 0x97: hair_models = reader.ReadStructs<HairModel>(table_id); break;
					#endregion
					#region Item Data
					case 0x6A: item_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x89: item_types = reader.ReadStructs<ItemType>(table_id); break;
					case 0x69: items = reader.ReadStructs<Item>(table_id); break;
					case 0x6C: item_gifts = reader.ReadStructs<ItemGift>(table_id); break;
					case 0xD7: item_data_d7 = reader.ReadStructs<ItemUnk_D7>(table_id); break;
					case 0xD0: item_data_d0 = reader.ReadStructs<ItemUnk_D0>(table_id); break;
					case 0x6B: item_funcs = reader.ReadStructs<ItemFunc>(table_id); break;
					#endregion
					#region Magic Data
					case 0x71: magic_off_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x8A: magic_base = reader.ReadStructs<MagicType>(table_id); break;
					case 0x8B: magic_elements = reader.ReadStructs<MagicType>(table_id); break;
					case 0x70: magic_off = reader.ReadStructs<Magic>(table_id); break;
					case 0x73: magic_def_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x72: magic_def = reader.ReadStructs<Magic>(table_id); break;
					case 0x75: magic_item_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x74: magic_items = reader.ReadStructs<MagicItem>(table_id); break;
					// TODO: this value is extremely out of place. no idea why it's here or what purpose it serves.
					case 0x9F: magic_unk_9f = reader.ReadInt32(); break;
					case 0xD1: magic_unk_d1 = reader.ReadStructs<MagicUnk_D1>(table_id); break;
					case 0xD4:
						{
							// ignore the name, it's simply reading a value telling the size of the array to read
							item_id = reader.ReadInt32();
							// the offset after re-aligning
							end = reader.ReadInt32();
							// read the bytes
							magic_unk_d4 = reader.ReadBytes(item_id);
							// jump to a the end of this object
							reader.offset = end;
							break;
						}
					case 0xD5:
						{
							// same as 0xD4
							item_id = reader.ReadInt32();
							end = reader.ReadInt32();
							magic_unk_d5 = reader.ReadBytes(item_id);
							reader.offset = end;
							break;
						}
					case 0xD8: ability_darkling_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0xD9: ability_darkling = reader.ReadStructs<Ability>(table_id); break;
					case 0x7E: ability_field_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x7B: ability_field = reader.ReadStructs<Ability>(table_id); break;
					case 0x7D: ability_battle_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x7A: ability_battle = reader.ReadStructs<Ability>(table_id); break;
					#endregion
					#region Job Data
					case 0x3D: job_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
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
					case 0x39: // reads to 0xFFFF
						{
							// read the item id
							item_id = reader.ReadInt32();
							// if the item id is larger than the array can hold
							if (item_id > job_unk_39.Length - 1)
							{
								// resize the array to allow it
								Array.Resize(ref job_unk_39, item_id + 1);
								// init the array
								job_unk_39[item_id] = Array.Empty<ushort>();
							}
							// read the pointer
							temp = reader.ReadInt32();
							// store the current offset
							offset = reader.offset;
							// jump to the previous pointer
							reader.offset = temp;
							// this is almost always true. left here just in case.
							while (reader.offset < reader.length)
							{
								// read and append the next bytes as u16
								Array.Resize(ref job_unk_39[item_id], job_unk_39[item_id].Length + 1);
								job_unk_39[item_id][^1] = reader.ReadUInt16();
								// if the last u16 is the max value it can contain
								if (job_unk_39[item_id][^1] == ushort.MaxValue)
								{
									// skip the 0
									reader.ReadInt16();
									// and stop reading
									break;
								}
							}
							// return to the previous offset from before we jumped to the pointer offset
							reader.offset = offset;
							break;
						}
					case 0x8F:
						{
							// exactly the same as 0x39
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
					#region Status Effects
					case 0x81: status_permanent = reader.ReadStructs<StatusPermanent>(table_id); break;
					case 0x82: status_battle = reader.ReadStructs<StatusBattle>(table_id); break;
					case 0x83: status_field = reader.ReadStructs<StatusField>(table_id); break;
					case 0x8E: status_unk_8E = reader.ReadStructs<StatusUnk_8E>(table_id); break;
					case 0x9A: status_unk_9A = reader.ReadStructs2<StatusUnk_9A>(table_id); break;
					case 0x9B: status_unk_9B = reader.ReadStructs<StatusUnk_9B>(table_id); break;
					#endregion
					#region Completed Objects
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
							// read the pointer marking the end of this data
							end = reader.ReadInt32();
							// init the array
							unk_8C = Array.Empty<ushort>();
							// read u16 values until we reach the end
							while (reader.offset < end)
							{
								Array.Resize(ref unk_8C, unk_8C.Length + 1);
								unk_8C[^1] = reader.ReadUInt16();
							}
							break;
						}
					case 0x3A:
						{
							// listed as male then female but in separate ids
							Array.Resize(ref npc_names, npc_names.Length + 1);
							// skip the listed item_id. it isn't necessary.
							reader.ReadInt32();
							// read, from the next 0x03 object, a string array starting at the s32 value and aligned to 2
							npc_names[^1] = ReadDescription_StringArray(reader.ReadInt32(), alignment: sizeof(ushort));
							break;
						}
					case 0x17: unk_17 = reader.ReadStructs<Unk_17>(table_id); break;

					case 0x7C: npc_enemy_descriptions = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x50: npc_enemies = reader.ReadStructs<NPCEnemy>(table_id); break;
					case 0x53: npc_enemy_drop_tables = reader.ReadStructs<NPCEnemyDropTable>(table_id); break;
					case 0x51: npc_enemy_models = reader.ReadStructs<NPCEnemyModel>(table_id); break;
					case 0x61:
						{
							// read the item_id
							item_id = reader.ReadInt32();
							// resize to fit the item_id
							if (item_id > npc_enemy_models_0.Length - 1)
							{
								Array.Resize(ref npc_enemy_models_0, item_id + 1);
							}
							// set the struct data
							npc_enemy_models_0[item_id].item_id = item_id;
							npc_enemy_models_0[item_id].f0 = reader.ReadString();
							npc_enemy_models_0[item_id].fg0 = reader.ReadString();
							// read, from the next 0x03 object, an array of values separated by 0xFFFF and aligned to 2
							npc_enemy_models_0[item_id].description = ReadDescription_Array(reader, 0xFFFF, sizeof(ushort));
							break;
						}
					case 0x5B: unk_5B = reader.ReadStructs<Unk_5B>(table_id); break;
					case 0x62: unk_62 = reader.ReadStructs<Unk_62>(table_id); break;
					case 0x52: unk_52 = reader.ReadStructs<Unk_52>(table_id); break;
					case 0x54: unk_54 = reader.ReadStructs<Unk_54>(table_id); break;
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
							npc_models_0[item_id].descriptions_0 = ReadDescription_Array(reader, 0xFFFF, sizeof(ushort));
							npc_models_0[item_id].descriptions_1 = ReadDescription_Array(reader, 0xFFFF, sizeof(ushort));
							break;
						}
					case 0x5D: unk_5D = reader.ReadStructs<Unk_5D>(table_id); break;
					case 0x2A: unk_2A = reader.ReadStructs<Unk_5D>(table_id); break;

					// TODO: another out of place s32 value
					case 0x2C: unk_2C = reader.ReadInt32(); break;
					case 0x2D: unk_2D = reader.ReadStructs<Unk_2D>(table_id); break;
					case 0x9D: unk_9D = reader.ReadStructs<Unk_9D>(table_id); break;
					case 0x9C: unk_9C = reader.ReadStructs<Unk_9D>(table_id); break;
					case 0xE1: unk_E1 = reader.ReadStructs<Unk_E1>(table_id); break;
					// read, from the next 0x03 object, an array of pointers, then an array of data following those pointers
					case 0x4F: unk_4F = ReadDescription_PointerArray(reader.ReadInt32()); break;
					case 0x4E: unk_4E = reader.ReadStructs<Unk_4E>(table_id); break;
					case 0x4D: unk_4D = ReadDescription_PointerArray(reader.ReadInt32()); break;
					// read, from the next 0x03 object, an array of raw s32 values
					case 0x76: unk_76 = ReadDescription_Value(reader.ReadInt32()); break;
					case 0x77: unk_77 = ReadDescription_PointerArray(reader.ReadInt32()); break;
					#endregion
					case 0xE2: unk_E2 = reader.ReadStructs<Unk_E2>(table_id); break;
					case 0x49: unk_49 = reader.ReadStructs<Unk_49>(table_id); break;
					case 0x5C: unk_5C = reader.ReadStructs<Unk_5C>(table_id); break;
					case 0x4A: npc_all_text = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0x4B:
						{
							item_id = reader.ReadInt32();
							if (item_id > npc_texts.Length - 1)
							{
								Array.Resize(ref npc_texts, item_id + 1);
							}
							npc_texts[item_id] = ReadDescription_String(reader);
							break;
						}
					case 0x47: unk_47 = reader.ReadInt32(); break;
					case 0x84: unk_84 = reader.ReadStructs<Unk_84>(table_id); break;
					case 0x1C: unk_1C = reader.ReadStructs<Unk_1C>(table_id); break;
					case 0x1D: unk_1D = reader.ReadStructs<Unk_1D>(table_id); break;
					case 0x1E: unk_1E = reader.ReadStructs<Unk_1E>(table_id); break;
					case 0x46: unk_46 = reader.ReadStructs<Unk_46>(table_id); break;
					case 0x4C: unk_4C = reader.ReadStructs<Unk_4C>(table_id); break;
					case 0x48:
						{
							Unk_4C[] temp_48 = reader.ReadStructs<Unk_4C>(table_id);
							temp = unk_48.Length;
							Array.Resize(ref unk_48, unk_48.Length + temp_48.Length);
							temp_48.CopyTo(unk_48, temp);
							break;
						}
					case 0x3F: unk_3F = reader.ReadStructs<Unk_4C>(table_id); break;
					case 0xDF: unk_DF = reader.ReadStructs<Unk_DF>(table_id); break;
					case 0xD2: unk_D2 = reader.ReadStructs<Unk_D2>(table_id); break;
					case 0xD3: unk_D3 = reader.ReadStructs<Unk_D2>(table_id); break;
					case 0xC0: unk_C0 = ReadDescription_PointerArray(reader.ReadInt32()); reader.ReadInt32(); break;
					case 0xBF: unk_BF = reader.ReadStructs<Unk_BF>(table_id); break;
					case 0xAE: battle_system_text = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0xAF: battle_system_text_b = ReadDescription_StringArray(reader.ReadInt32(), reader.ReadInt32()); break;
					case 0xA1: unk_A1 = reader.ReadStruct<Unk_A1>(); break;
					case 0xA0: unk_A0 = reader.ReadStruct<Unk_A0>(); break;
					case 0xA2: unk_A2 = reader.ReadStruct<Unk_A2>(); break;
					case 0xA3: unk_A3 = reader.ReadStruct<Unk_A3>(); break;
					case 0xA4: unk_A4 = reader.ReadStruct<Unk_A4>(); break;
					case 0xA5: unk_A5 = reader.ReadStruct<Unk_A5>(); break;
					case 0xAD: finished = true; break;
					// if, for some reason, we forget to handle a table_id, this will let us know.
					default: throw new Exception($"Unhandled table {(reader.offset - sizeof(int)).ToHexString()}->{((byte)table_id).ToHexString()}");
				}
			}
		}

		/// <summary>
		/// Allocates a new <see cref="Description"/> class then adds <seealso cref="Utils.ReadDescription_StringArray(byte[], int, ref Description, int, int?, int)"/>
		/// to the 0x03 handler list.
		/// </summary>
		/// <param name="start_offset">The offset to start reading from.</param>
		/// <param name="end_offset">The offset to stop reading at.</param>
		/// <param name="alignment">The alignment of this set of data. Defaults to int.</param>
		/// <returns></returns>
		private Description ReadDescription_StringArray(int start_offset, int? end_offset = null, int alignment = sizeof(int))
		{
			// allocate a new Description class
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			// create a new handler that references the Description class so we can modify its data in post
			description_ptr_handlers.Add((a, b) => Utils.ReadDescription_StringArray(a, b, ref ret, start_offset, end_offset, alignment));
			return ret;
		}

		private Description ReadDescription_String(DataReader reader)
		{
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			// until the next s32 value is a 0
			while (true)
			{
				int ptr = reader.ReadInt32();
				if (ptr == 0)
				{
					break;
				}
				description_ptr_handlers.Add((a, b) => Utils.ReadDescription_String(a, b, ref ret, ptr));
			}
			return ret;
		}

		/// <summary>
		/// Allocates a new <see cref="Description"/> class and adds <seealso cref="Utils.ReadDescription_Value(byte[], int, ref Description, int)"/>
		/// to the 0x03 handler list.
		/// </summary>
		/// <param name="start_offset">The offset to start reading from.</param>
		/// <returns></returns>
		private Description ReadDescription_Value(int start_offset)
		{
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			description_ptr_handlers.Add((a, b) => Utils.ReadDescription_Value(a, b, ref ret, start_offset));
			return ret;
		}

		/// <summary>
		/// Allocates a new <see cref="Description"/> class. Then, while the next s32 value is not 0, adds
		/// <seealso cref="Utils.ReadDescription_Array(byte[], int, ref Description, int, int?, dynamic, int)"/> to the 0x03 handler list.
		/// </summary>
		/// <param name="reader">The current <see cref="DataReader"/> instance.</param>
		/// <param name="separator">A primitive value of any type that would be used to separate arrays of bytes (0xFF or '\0' for instance)</param>
		/// <param name="alignment">The alignment of this set of data. Defaults to int.</param>
		/// <returns></returns>
		private Description ReadDescription_Array(DataReader reader, dynamic separator, int alignment = sizeof(int))
		{
			// allocate a Description class to return later
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			// while there is another ordered pointer
			while (reader.ReadInt32() != 0)
			{
				// skip the unnecessary id
				reader.ReadInt32();
				// read the starting offset
				int start_offset = reader.ReadInt32();
				// create a new action for the 0x03 object to invoke
				description_ptr_handlers.Add((a, b) => Utils.ReadDescription_Array(a, b, ref ret, start_offset, null, separator, alignment));
			}
			return ret;
		}

		/// <summary>
		/// Allocates a new <see cref="Description"/> class and adds
		/// <seealso cref="Utils.ReadDescription_PointerArray(byte[], int, ref Description, int)"/> to the 0x03 handler list.
		/// </summary>
		/// <param name="start_offset">The offset to start reading from.</param>
		/// <returns></returns>
		private Description ReadDescription_PointerArray(int start_offset)
		{
			Description ret = new()
			{
				ptrs = Array.Empty<int>(),
				description = Array.Empty<byte>()
			};
			description_ptr_handlers.Add((a, b) => Utils.ReadDescription_PointerArray(a, b, ref ret, start_offset));
			return ret;
		}

		/// <summary>
		/// Handles writing all data contained in this class to the current <see cref="DataWriter"/> instance.
		/// </summary>
		/// <param name="writer">The <see cref="DataWriter"/> instance to write to.</param>
		public void Write(DataWriter writer)
		{
			writer.ReservePointer(0x52484340, "chr_file_len_ptr");
			writer.Write(0x30);
			writer.offset = 0x30;
			#region Class Data
			writer.ReservePointer(0x01, "label_ptr");
			writer.Write(file_labels[next_label_id++]);
			writer.WritePointer("label_ptr");

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

			writer.ReservePointer(0x01, "label_ptr");
			writer.Write(file_labels[next_label_id++]);
			writer.WritePointer("label_ptr");
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
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));
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
				writer.ReservePointerNoID($"4D_ptrs_{i}_{unk_4D.ptrs[i]}");
			}
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));
			writer.WriteDescriptions("4D_ptrs", unk_4D, origin_offset);
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));

			writer.WritePointer("4F_ptrs");
			for (int i = 0; i < unk_4F.ptrs.Length; i++)
			{
				writer.ReservePointerNoID($"4F_ptrs_{i}_{unk_4F.ptrs[i]}");
			}
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));
			writer.WriteDescriptions("4F_ptrs", unk_4F, origin_offset);
			writer.offset += sizeof(int) - (writer.offset % sizeof(int));

			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x76, "76_ptr");
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("76_ptr");
			writer.Write(unk_76.description);
			writer.WritePointer("des_end_ptr");

			writer.ReservePointer(0x77, "77_ptr_start");
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			origin_offset = writer.offset;
			writer.WritePointer("77_ptr_start");
			for (int i = 0; i < unk_77.ptrs.Length; i++)
			{
				writer.ReservePointerNoID($"77_ptrs_{i}_{unk_77.ptrs[i]}");
			}
			writer.Write(0);
			writer.Write(0);
			writer.WriteDescriptions("77_ptrs", unk_77, origin_offset);
			writer.WritePointer("des_end_ptr");

			writer.WriteStructs(0xE2, unk_E2);
			writer.WriteStructs(0x49, unk_49);
			writer.WriteStructs(0x5C, unk_5C);
			writer.ReservePointer(0x4A, "all_npc_text", 2);
			for (int i = 0; i < npc_texts.Length; i++)
			{
				if (npc_texts[i] == null)
				{
					continue;
				}
				writer.Write(0x4B);
				writer.Write(i);
				for (int j = 0; j < npc_texts[i].ptrs.Length; j++)
				{
					writer.ReservePointerNoID($"npc_text_{i}_{j}_{npc_texts[i].ptrs[j]}");
				}
				writer.Write(0);
			}
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("all_npc_text");
			writer.WriteDescriptions("npc_text", npc_texts, (byte)0);
			writer.WritePointer("all_npc_text");
			writer.WritePointer("des_end_ptr");

			writer.Write(0x47);
			writer.Write(unk_47);
			writer.WriteStructs(0x84, unk_84);
			writer.WriteStructs(0x1C, unk_1C);
			writer.WriteStructs(0x1D, unk_1D);
			writer.WriteStructs(0x1E, unk_1E);
			writer.WriteStructs(0x46, unk_46);
			writer.WriteStructs(0x4C, unk_4C);
			for (int i = 0; i < unk_48.Length; i++)
			{
				switch (unk_48[i].item_id)
				{
					case 0x22:
					case 0x4A:
					case 0x4B:
					case 0x4C:
					case 0x4D:
					case 0x4E:
						{
							writer.Write(0x48);
							writer.Write(unk_48[i].item_id);
							writer.Write(unk_48[i].unk_00);
							writer.Write(unk_48[i].unk_01);
							writer.Write(unk_48[i].unk_02);
							writer.Write(unk_48[i].padding);
							break;
						}
					default: continue;
				}
			}
			writer.WriteStructs(0x3F, unk_3F);
			for (int i = 0; i < unk_48.Length; i++)
			{
				switch (unk_48[i].item_id)
				{
					case 0x22:
					case 0x4A:
					case 0x4B:
					case 0x4C:
					case 0x4D:
					case 0x4E: continue;
					default:
						if (unk_48[i].Equals(default(Unk_4C)))
						{
							continue;
						}
						break;
				}
				writer.Write(0x48);
				writer.Write(unk_48[i].item_id);
				writer.Write(unk_48[i].unk_00);
				writer.Write(unk_48[i].unk_01);
				writer.Write(unk_48[i].unk_02);
				writer.Write(unk_48[i].padding);
			}
			writer.ReservePointer(0x01, "label_ptr");
			writer.Write(file_labels[next_label_id++]);
			writer.WritePointer("label_ptr");
			#endregion

			/* more goes here */

			writer.WritePointer("chr_file_len_ptr");
		}
	}
}
