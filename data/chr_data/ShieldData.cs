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
			shields = Array.Empty<Shield>();
			models = Array.Empty<ShieldModel>();
			descriptions = Array.Empty<string>();
			int start;
			int end;
			int temp_offset;
			int table_id;
			dynamic shield_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x60:
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
					case 0x5E:
						{
							shield_id = reader.ReadInt16();
							if (shield_id > shields.Length - 1)
							{
								Array.Resize(ref shields, shield_id + 1);
							}
							shields[shield_id] = reader.ReadStruct<Shield>();
							break;
						}
					case 0x5F:
						{
							shield_id = reader.ReadInt32();
							if (shield_id > models.Length - 1)
							{
								Array.Resize(ref models, shield_id + 1);
							}
							models[shield_id] = reader.ReadStruct<ShieldModel>();
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
			writer.Write(0x60);
			int des_ptr_pos = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (short i = 1; i < shields.Length; i++)
			{
				writer.Write(0x5E);
				writer.Write(i);
				writer.WriteStruct(shields[i]);
			}
			for (int i = 1; i < models.Length; i++)
			{
				writer.Write(0x5F);
				writer.Write(i);
				writer.WriteStruct(models[i]);
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
