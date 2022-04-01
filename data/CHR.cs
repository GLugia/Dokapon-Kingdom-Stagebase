using CharaReader.data.chr_data;
using System;
using System.Linq;
using System.Reflection;

namespace CharaReader.data
{
	public class CHR
	{
		public string class_data_start;
		public string class_data_end;
		public WeaponData weapon_data;
		public ShieldData shield_data;
		public AccessoryData accessory_data;
		public HairData hair_data;
		public ItemData item_data;
		public MagicData magic_data;
		public JobData job_data;
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
			weapon_data = new(reader);
			shield_data = new(reader);
			accessory_data = new(reader);
			hair_data = new(reader);
			item_data = new(reader);
			magic_data = new(reader);
			job_data = new(reader);

			table_id = reader.ReadInt32();
			if (table_id == 0x01)
			{
				end = reader.ReadInt32();
				class_data_end = reader.ReadString();
				reader.offset = end;
			}
			// add more here
			;
		}

		public void Write(DataWriter writer)
		{
			writer.Write("@CHR");
			int file_len_pos = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(0x30);
			writer.offset = 0x30;

			writer.Write(0x01);
			int pre_name_pos = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(class_data_start);
			int post_name_pos = writer.offset;
			writer.offset = pre_name_pos;
			writer.Write(post_name_pos);
			writer.offset = post_name_pos;

			weapon_data.Write(writer);
			shield_data.Write(writer);
			accessory_data.Write(writer);
			hair_data.Write(writer);
			item_data.Write(writer);
			magic_data.Write(writer);
			job_data.Write(writer);

			writer.Write(0x01);
			pre_name_pos = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(class_data_end);
			post_name_pos = writer.offset;
			writer.offset = pre_name_pos;
			writer.Write(post_name_pos);
			writer.offset = post_name_pos;

			// add more here
			int len = writer.offset;
			writer.offset = file_len_pos;
			writer.Write(len);
			writer.offset = len;
		}
	}
}
