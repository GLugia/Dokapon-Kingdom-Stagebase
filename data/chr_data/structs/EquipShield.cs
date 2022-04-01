using System.IO;

namespace CharaReader.data.chr_data.structs
{
	public struct EquipShield
	{
		public short icon_id;
		public string name;
		public byte unk_00;
		public short job_id;
		public byte rating;
		public int price;
		public short def;
		public short att;
		public short mag;
		public short spd;
		public short hp;
		public byte description_table_id;
		public byte description_id;
	}
}
