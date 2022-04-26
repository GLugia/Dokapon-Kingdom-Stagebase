using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CharaReader.data.ptrs
{
	public class Ptr_Data : Ptr_Custom<(IntPtr start, IntPtr end)>
	{
		public Ptr_Data(IntPtr origin, ref long offset) : base(origin, ref offset)
		{
			data.start = new((long)origin + Marshal.ReadInt32(origin, (int)offset));
			offset += sizeof(int);
			data.end = new((long)origin + Marshal.ReadInt32(origin, (int)offset));
			offset += sizeof(int);
		}

		public override Dictionary<IntPtr, byte[]> Handle()
		{
			byte[] ret = new byte[(int)((long)data.end - (long)data.start)];
			for (int index = 0; index < ret.Length; index++)
			{
				ret[index] = Marshal.ReadByte(data.start, index);
			}
			return new Dictionary<IntPtr, byte[]>() { { data.start, ret } };
		}

		public override bool Equals(object obj)
		{
			if (obj is Ptr_Data ptr && ptr != null && (long)ptr.data.start == (long)data.start)
			{
				return (long)ptr.data.end == (long)data.end;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(data.start, data.end);
		}
	}
}
