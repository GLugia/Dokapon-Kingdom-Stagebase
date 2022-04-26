using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CharaReader.data.ptrs
{
	public class Ptr_Unhandled : Ptr_Custom<IntPtr>
	{
		public Ptr_Unhandled(IntPtr origin, ref long offset) : base(origin, ref offset)
		{
			data = new IntPtr((long)origin + Marshal.ReadInt32(origin, (int)offset));
			offset += sizeof(int);
		}

		public override Dictionary<IntPtr, byte[]> Handle()
		{
			return new Dictionary<IntPtr, byte[]>();
		}

		public override bool Equals(object obj)
		{
			if (obj is Ptr_Unhandled ptr && ptr != null)
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
