using System.IO;

namespace CharaReader.data.chr_data.structs
{
	public struct Item
	{
		public byte type;
		public short icon_id;
		public string name;
		public int price;
		public byte unk_00;
		public byte unk_01;
		public short padding;
	}
}
