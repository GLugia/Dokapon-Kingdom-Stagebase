using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data.chr_data.structs
{
	public struct MagicItem
	{
		public short icon;
		public string name;
		public int price;
		public short power;
		public byte unk_00;
		public byte unk_01;
		public int unk_02;
	}
}
