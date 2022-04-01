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
			accessories = Array.Empty<Accessory>();
			descriptions = Array.Empty<string>();
			int start;
			int end;
			int temp_offset;
			int table_id;
			dynamic accessory_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x65:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref descriptions, descriptions.Length + 1);
								descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x64:
						{
							accessory_id = reader.ReadInt16();
							if (accessory_id > accessories.Length - 1)
							{
								Array.Resize(ref accessories, accessory_id + 1);
							}
							accessories[accessory_id] = reader.ReadStruct<Accessory>();
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

		public override void Write(DataWriter writer)
		{
			writer.Write(0x65);
			int des_ptr_pos = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (short i = 1; i < accessories.Length; i++)
			{
				writer.Write(0x64);
				writer.Write(i);
				writer.WriteStruct(accessories[i]);
			}
			writer.Write(0x03);
			int des_end_pos = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(0);
			for (int i = 0; i < descriptions.Length; i++)
			{
				writer.Write(descriptions[i]);
			}
			int ret_pos = writer.offset;
			writer.offset = des_ptr_pos;
			writer.Write(des_end_pos);
			writer.Write(ret_pos);
			writer.offset = des_end_pos;
			writer.Write(ret_pos);
			writer.offset = ret_pos;
		}
	}
}
