using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class AccessoryData : BaseData
	{
		public Accessory[] accessories;
		public string[] descriptions;

		public AccessoryData(DataReader reader) : base(reader)
		{
			int table_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x65: descriptions = reader.ReadDescriptions(); break;
					case 0x64: accessories = reader.ReadStructs<Accessory>(table_id); break;
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
			writer.ReservePointer(0x65, "acc_des_ptr", 2);
			writer.WriteStructs(0x64, accessories);
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			writer.WritePointer("acc_des_ptr");
			writer.WriteDescriptions(descriptions);
			writer.WritePointer("acc_des_ptr");
			writer.WritePointer("des_end_ptr");
		}
	}
}
