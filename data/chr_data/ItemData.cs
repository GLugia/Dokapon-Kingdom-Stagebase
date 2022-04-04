using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class ItemData : BaseData
	{
		public ItemType[] item_types;
		public Item[] items;
		public ItemGift[] gift_items;
		public ItemUnk_D7[] item_data_d7;
		public ItemUnk_D0[] item_data_d0;
		public ItemFunc[] item_funcs;
		public string[] descriptions;

		public ItemData(DataReader reader) : base(reader)
		{
			int table_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x6A: descriptions = reader.ReadDescriptions(); break;
					case 0x89: item_types = reader.ReadStructs<ItemType>(table_id); break;
					case 0x69: items = reader.ReadStructs<Item>(table_id); break;
					case 0x6C: gift_items = reader.ReadStructs<ItemGift>(table_id); break;
					case 0xD7: item_data_d7 = reader.ReadStructs<ItemUnk_D7>(table_id); break;
					case 0xD0: item_data_d0 = reader.ReadStructs<ItemUnk_D0>(table_id); break;
					case 0x6B: item_funcs = reader.ReadStructs<ItemFunc>(table_id); break;
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
			writer.ReservePointer(0x6A, "des_ptr_pos", 2);
			writer.WriteStructs(0x89, item_types);
			writer.WriteStructs(0x69, items);
			writer.WriteStructs(0x6C, gift_items);
			writer.WriteStructs(0xD7, item_data_d7);
			writer.WriteStructs(0xD0, item_data_d0);
			writer.WriteStructs(0x6B, item_funcs);
			writer.ReservePointer(0x03, "des_end_pos");
			writer.Write(0);
			writer.WritePointer("des_ptr_pos");
			writer.WriteDescriptions(descriptions);
			writer.WritePointer("des_ptr_pos");
			writer.WritePointer("des_end_pos");
		}
	}
}
