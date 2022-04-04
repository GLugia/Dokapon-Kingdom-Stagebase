using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class ShieldData : BaseData
	{
		public Shield[] shields;
		public ShieldModel[] models;
		public string[] descriptions;

		public ShieldData(DataReader reader) : base(reader)
		{
			int table_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x60: descriptions = reader.ReadDescriptions(); break;
					case 0x5E: shields = reader.ReadStructs<Shield>(table_id); break;
					case 0x5F: models = reader.ReadStructs<ShieldModel>(table_id); break;
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

		public override void Write(DataWriter writer)
		{
			writer.ReservePointer(0x60, "des_ptr_pos", 2);
			writer.WriteStructs(0x5E, shields);
			writer.WriteStructs(0x5F, models);
			writer.ReservePointer(0x03, "des_end_pos");
			writer.Write(0);
			writer.WritePointer("des_ptr_pos");
			writer.WriteDescriptions(descriptions);
			writer.WritePointer("des_ptr_pos");
			writer.WritePointer("des_end_pos");
		}
	}
}
