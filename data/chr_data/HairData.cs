using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class HairData : BaseData
	{
		public Hair[] hair;
		public HairModel[] models;
		public string[] descriptions;

		public HairData(DataReader reader) : base(reader)
		{
			int table_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x95: descriptions = reader.ReadDescriptions(); break;
					case 0x96: hair = reader.ReadStructs<Hair>(table_id); break;
					case 0x97: models = reader.ReadStructs<HairModel>(table_id); break;
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
			writer.ReservePointer(0x95, "hair_des_ptr", 2);
			writer.WriteStructs(0x96, hair);
			writer.WriteStructs(0x97, models);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("hair_des_ptr");
			writer.WriteDescriptions(descriptions);
			writer.WritePointer("hair_des_ptr");
			writer.WritePointer("des_end_ptr");
		}
	}
}
