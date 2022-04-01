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
			hair = Array.Empty<Hair>();
			models = Array.Empty<HairModel>();
			descriptions = Array.Empty<string>();
			int start;
			int end;
			int temp_offset;
			int table_id;
			dynamic hair_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x95:
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
					case 0x96:
						{
							hair_id = reader.ReadByte();
							if (hair_id > hair.Length - 1)
							{
								Array.Resize(ref hair, hair_id + 1);
							}
							hair[hair_id] = reader.ReadStruct<Hair>();
							break;
						}
					case 0x97:
						{
							hair_id = reader.ReadInt32();
							if (hair_id > models.Length - 1)
							{
								Array.Resize(ref models, hair_id + 1);
							}
							models[hair_id] = reader.ReadStruct<HairModel>();
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
			writer.Write(0x95);
			int des_ptr_pos = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (byte i = 1; i < hair.Length; i++)
			{
				writer.Write(0x96);
				writer.Write(i);
				writer.WriteStruct(hair[i]);
			}
			for (int i = 1; i < models.Length; i++)
			{
				if (models[i].model == null)
				{
					continue;
				}
				writer.Write(0x97);
				writer.Write(i);
				writer.WriteStruct(models[i]);
			}
			writer.Write(0x03);
			int des_end_pos = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(0);
			int des_start_pos = writer.offset;
			for (int i = 0; i < descriptions.Length; i++)
			{
				writer.Write(descriptions[i]);
			}
			int ret_pos = writer.offset;
			writer.offset = des_ptr_pos;
			writer.Write(des_start_pos);
			writer.Write(ret_pos);
			writer.offset = des_end_pos;
			writer.Write(ret_pos);
			writer.offset = ret_pos;
		}
	}
}
