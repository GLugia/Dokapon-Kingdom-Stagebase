using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CharaReader.data.ptrs
{
	public class Ptr_Texture : Ptr_Custom<IntPtr>
	{
		public Ptr_Texture(IntPtr origin, ref long offset) : base(origin, ref offset)
		{
			data = new IntPtr((long)origin + Marshal.ReadInt32(origin, (int)offset));
			offset += sizeof(int);
		}

		public override Dictionary<IntPtr, byte[]> Handle()
		{
			byte[] set = Array.Empty<byte>();
			int offset;
			// read asm data
			for (offset = 0; offset < 7 * sizeof(int); offset += sizeof(int))
			{
				Array.Resize(ref set, set.Length + sizeof(int));
				BitConverter.GetBytes(Marshal.ReadInt32(data, offset)).CopyTo(set, offset);
			}
			// read random int
			Array.Resize(ref set, set.Length + sizeof(int));
			BitConverter.GetBytes(Marshal.ReadInt32(data, offset)).CopyTo(set, offset);
			offset += sizeof(int);
			// read string data
			int b;
			int count = 0;
			do
			{
				b = Marshal.ReadInt32(data, offset);
				// copy the value
				Array.Resize(ref set, set.Length + sizeof(int));
				BitConverter.GetBytes(b).CopyTo(set, offset);
				offset += sizeof(int);
				// copy the id
				Array.Resize(ref set, set.Length + sizeof(int));
				BitConverter.GetBytes(Marshal.ReadInt32(data, offset)).CopyTo(set, offset);
				offset += sizeof(int);
				// copy the string
				do
				{
					Array.Resize(ref set, set.Length + 1);
					set[^1] = Marshal.ReadByte(data, offset);
					offset++;
				}
				while (set[^1] != 0);
				if (set.Length % sizeof(int) != 0)
				{
					Array.Resize(ref set, set.Length + (sizeof(int) - (set.Length % sizeof(int))));
					offset = set.Length;
				}
				count++;
			}
			while (b != 0);
			// read int data
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					Array.Resize(ref set, set.Length + sizeof(int));
					BitConverter.GetBytes(Marshal.ReadInt32(data, offset)).CopyTo(set, offset);
					offset += sizeof(int);
				}
			}
			return new Dictionary<IntPtr, byte[]>() { { data, set } };
		}

		public override bool Equals(object obj)
		{
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}
	}
}
