using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data.types.subtypes
{
	public struct Unk_AC_Data
	{
		public int unk_00;
		public int unk_04;
		public int unk_08;
		public int unk_0C;
		public int unk_10;
		public int unk_14;
		public int unk_18;
		public int unk_1C;
		public Dictionary<int, string> dictionary;
		public Dictionary<int, Unk_AC_Sub_Data> sub_dictionary;
	}
}
