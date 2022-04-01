using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data.chr_data.structs
{
	public struct NPCEnemyDropTable
	{
		public byte chance_for_a;
		public byte chance_for_b;
		public byte padding;
		public byte item_id_a;
		public byte table_id_a;
		public byte item_id_b;
		public byte table_id_b;
	}
}
