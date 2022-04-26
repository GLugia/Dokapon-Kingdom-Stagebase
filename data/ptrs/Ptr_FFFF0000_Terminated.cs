using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CharaReader.data.ptrs
{
	public class Ptr_FFFF0000_Terminated : Ptr_Custom<IntPtr>
	{
		public Ptr_FFFF0000_Terminated(IntPtr origin, ref long offset) : base(origin, ref offset)
		{
			data = new IntPtr((long)origin + Marshal.ReadInt32(origin, (int)offset));
			offset += sizeof(int);
		}

		public override Dictionary<IntPtr, byte[]> Handle()
		{
			byte[] ret = new byte[sizeof(int)];
			int index = 0;
			// read until 0xFFFF and copy the bytes read to 'ret'
			do
			{
				if (index >= ret.Length - 1)
				{
					Array.Resize(ref ret, ret.Length + sizeof(int));
				}
				BitConverter.GetBytes(Marshal.ReadInt16(data, index)).CopyTo(ret, index);
				index += sizeof(ushort);
			}
			while (BitConverter.ToUInt32(ret, ret.Length - sizeof(uint)) != ushort.MaxValue);
			// copy the 0x0000 to the array as well
			BitConverter.GetBytes(Marshal.ReadInt16(data, index)).CopyTo(ret, index);
			return new Dictionary<IntPtr, byte[]>() { { data, ret } };
		}

		public override bool Equals(object obj)
		{
			if (obj is Ptr_FFFF0000_Terminated ptr && ptr != null)
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
