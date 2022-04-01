using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class ItemData : BaseData
	{
		public string[] item_types;
		public Item[] items;
		public ItemGift[] gift_items;
		public ItemUnk_D7[] item_data_d7;
		public ItemUnk_D0[] item_data_d0;
		public short[] item_funcs;
		public string[] descriptions;

		public ItemData(DataReader reader) : base(reader)
		{
			item_types = Array.Empty<string>();
			items = Array.Empty<Item>();
			gift_items = Array.Empty<ItemGift>();
			item_data_d7 = Array.Empty<ItemUnk_D7>();
			item_data_d0 = Array.Empty<ItemUnk_D0>();
			item_funcs = Array.Empty<short>();
			descriptions = Array.Empty<string>();
			int start;
			int end;
			int temp_offset;
			int table_id;
			dynamic item_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x6A:
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
					case 0x89:
						{
							item_id = reader.ReadInt32();
							if (item_id > item_types.Length - 1)
							{
								Array.Resize(ref item_types, item_id + 1);
							}
							item_types[item_id] = reader.ReadString();
							break;
						}
					case 0x69:
						{
							item_id = reader.ReadByte();
							if (item_id > items.Length - 1)
							{
								Array.Resize(ref items, item_id + 1);
							}
							items[item_id] = reader.ReadStruct<Item>();
							break;
						}
					case 0x6C:
						{
							item_id = reader.ReadInt16();
							if (item_id > gift_items.Length - 1)
							{
								Array.Resize(ref gift_items, item_id + 1);
							}
							gift_items[item_id] = reader.ReadStruct<ItemGift>();
							break;
						}
					case 0xD7:
						{
							item_id = reader.ReadByte();
							if (item_id > item_data_d7.Length - 1)
							{
								Array.Resize(ref item_data_d7, item_id + 1);
							}
							item_data_d7[item_id] = reader.ReadStruct<ItemUnk_D7>();
							break;
						}
					case 0xD0:
						{
							item_id = reader.ReadByte();
							if (item_id > item_data_d0.Length - 1)
							{
								Array.Resize(ref item_data_d0, item_id + 1);
							}
							item_data_d0[item_id] = reader.ReadStruct<ItemUnk_D0>();
							break;
						}
					case 0x6B:
						{
							item_id = reader.ReadInt16();
							if (item_id > item_funcs.Length - 1)
							{
								Array.Resize(ref item_funcs, item_id + 1);
							}
							item_funcs[item_id] = reader.ReadInt16();
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
			writer.Write(0x6A);
			int des_ptr_pos = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (int i = 1; i < item_types.Length; i++)
			{
				writer.Write(0x89);
				writer.Write(i);
				writer.Write(item_types[i]);
			}
			for (byte i = 1; i < items.Length; i++)
			{
				writer.Write(0x69);
				writer.Write(i);
				writer.WriteStruct(items[i]);
			}
			for (short i = 1; i < gift_items.Length; i++)
			{
				if (gift_items[i].name == null)
				{
					continue;
				}
				writer.Write(0x6C);
				writer.Write(i);
				writer.WriteStruct(gift_items[i]);
			}
			for (byte i = 1; i < item_data_d7.Length; i++)
			{
				writer.Write(0xD7);
				writer.Write(i);
				writer.WriteStruct(item_data_d7[i]);
			}
			for (byte i = 1; i < item_data_d0.Length; i++)
			{
				writer.Write(0xD0);
				writer.Write(i);
				writer.WriteStruct(item_data_d0[i]);
			}
			for (short i = 1; i < item_funcs.Length; i++)
			{
				writer.Write(0x6B);
				writer.Write(i);
				writer.Write(item_funcs[i]);
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
