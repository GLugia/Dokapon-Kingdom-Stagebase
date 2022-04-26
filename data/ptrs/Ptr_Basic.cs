using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CharaReader.data.ptrs
{
	public class Ptr_Basic : Ptr_Custom<IntPtr>
	{
		public Ptr_Basic(IntPtr origin, ref long offset) : base(origin, ref offset)
		{
			data = new IntPtr((long)origin + Marshal.ReadInt32(origin, (int)offset));
			offset += sizeof(int);
		}

		public override Dictionary<IntPtr, byte[]> Handle()
		{
			byte[] ret = new byte[sizeof(int)];
			int index = 0;
			do
			{
				if (index >= ret.Length - 1)
				{
					Array.Resize(ref ret, ret.Length + sizeof(int));
				}
				ret[index] = Marshal.ReadByte(data, index);
				index++;
			}
			while (ret[index - 1] != 0);
			return new Dictionary<IntPtr, byte[]>() { { data, ret } };
		}

		public override bool Equals(object obj)
		{
			if (obj is Ptr_Basic ptr && ptr != null)
			{
				return (long)ptr.data == (long)data;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return data.GetHashCode();
		}
	}
}
