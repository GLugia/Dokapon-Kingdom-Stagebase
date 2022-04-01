using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data.chr_data.structs
{
	public struct JobMoney
	{
		public byte bonus_salary;
		public byte job_id;
		public int starting_money;
		public short level_multiplier;
		public short bonus_multiplier;
	}
}
