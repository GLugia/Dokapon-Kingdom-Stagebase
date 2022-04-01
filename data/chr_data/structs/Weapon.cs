using System.Runtime.InteropServices;

namespace CharaReader.data.chr_data.structs
{
	public struct Weapon
	{
		public short icon_id;
		public string name;
		public byte unk_00;
		public short class_id;
		public byte rating;
		public int price;
		public short att;
		public short def;
		public short mag;
		public short spd;
		public short hp;
		public byte did_class;
		public byte did_item;
	}
}
