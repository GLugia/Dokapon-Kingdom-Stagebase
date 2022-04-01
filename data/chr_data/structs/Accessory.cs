using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data.chr_data.structs
{
	public struct Accessory
	{
		public short icon_id;
		public string name;
		public int unk_00;
		public int price;
		public short att;
		public short def;
		public short mag;
		public short spd;
		public short hp;
		public byte did_class;
		public byte did_item;
		public int unk_01;
	}
}
