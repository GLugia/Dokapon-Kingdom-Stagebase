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
			descriptions = Array.Empty<string[]>();
			int table_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x5A:
					case 0x63:
						{
							Array.Resize(ref descriptions, descriptions.Length + 1);
							descriptions[^1] = Array.Empty<string>();
							descriptions[^1] = reader.ReadDescriptions();
							break;
						}
					case 0x58: weapons = reader.ReadStructs<Weapon>(table_id); break;
					case 0x59: models = reader.ReadStructs<WeaponModel>(table_id); break;
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

			Console.Write($"{((byte)weapons.Length).ToHexString()} {((byte)models.Length).ToHexString()}");
			foreach (string[] vals in descriptions)
			{
				Console.Write($" {((byte)vals.Length).ToHexString()}");
			}
			Console.Write('\n');
		}

		public override void Write(DataWriter data)
		{
			data.ReservePointer(0x5A, "des_1_pos", 2);
			data.ReservePointer(0x63, "des_0_pos", 2);
			data.WriteStructs(0x58, weapons);
			data.WriteStructs(0x59, models);
			data.ReservePointer(0x03, "des_len_pos");
			data.Write(0);
			data.WritePointer("des_0_pos");
			data.WriteDescriptions(descriptions[1]);
			data.WritePointer("des_0_pos");
			data.WritePointer("des_1_pos");
			data.WriteDescriptions(descriptions[0]);
			data.WritePointer("des_1_pos");
			data.WritePointer("des_len_pos");
		}
	}
}
