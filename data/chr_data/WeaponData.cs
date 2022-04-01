using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class WeaponData : BaseData
	{
		public Weapon[] weapons;
		public WeaponModel[] models;
		public string[][] descriptions;

		public WeaponData(DataReader reader) : base(reader)
		{
			weapons = Array.Empty<Weapon>();
			models = Array.Empty<WeaponModel>();
			descriptions = Array.Empty<string[]>();
			int start;
			int end;
			int temp_offset;
			int table_id;
			dynamic weapon_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x5A:
					case 0x63:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							Array.Resize(ref descriptions, descriptions.Length + 1);
							descriptions[^1] = Array.Empty<string>();
							while (reader.offset < end)
							{
								Array.Resize(ref descriptions[^1], descriptions[^1].Length + 1);
								descriptions[^1][^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x58:
						{
							weapon_id = reader.ReadInt16();
							if (weapon_id > weapons.Length - 1)
							{
								Array.Resize(ref weapons, weapon_id + 1);
							}
							weapons[weapon_id] = reader.ReadStruct<Weapon>();
							break;
						}
					case 0x59:
						{
							weapon_id = reader.ReadInt32();
							if (weapon_id > models.Length - 1)
							{
								Array.Resize(ref models, weapon_id + 1);
							}
							models[weapon_id] = reader.ReadStruct<WeaponModel>();
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

		public override void Write(DataWriter data)
		{
			data.Write(0x5A);
			int des_1_ptr_pos = data.offset;
			data.offset += sizeof(int) * 2;
			data.Write(0x63);
			int des_0_ptr_pos = data.offset;
			data.offset += sizeof(int) * 2;
			for (short i = 1; i < weapons.Length; i++)
			{
				data.Write(0x58);
				data.Write(i);
				data.WriteStruct(weapons[i]);
			}
			for (int i = 1; i < models.Length; i++)
			{
				data.Write(0x59);
				data.Write(i);
				data.WriteStruct(models[i]);
			}
			data.Write(0x03);
			int old_pos = data.offset;
			data.offset += sizeof(int);
			data.Write(0);
			for (int i = 0; i < descriptions[1].Length; i++)
			{
				data.Write(descriptions[1][i]);
			}
			int half_pos = data.offset;
			for (int i = 0; i < descriptions[0].Length; i++)
			{
				data.Write(descriptions[0][i]);
			}
			int new_pos = data.offset;
			data.offset = des_1_ptr_pos;
			data.Write(half_pos);
			data.Write(new_pos);
			data.offset = des_0_ptr_pos;
			data.Write(old_pos);
			data.Write(half_pos);
			data.offset = old_pos;
			data.Write(new_pos);
			data.offset = new_pos;
		}
	}
}
