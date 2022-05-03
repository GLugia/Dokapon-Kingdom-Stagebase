using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data
{
	public class PtrData
	{
		public dynamic ref_obj;
		public long size;

		public PtrData(dynamic ref_obj, long size)
		{
			this.ref_obj = ref_obj;
			this.size = size;
		}
	}
}
