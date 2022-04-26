using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CharaReader.data.ptrs
{
	public class Ptr_Array : Ptr_Custom<IntPtr[]>
	{
		public Ptr_Array(IntPtr origin, ref long offset) : base(origin, ref offset)
		{
			data = Array.Empty<IntPtr>();
			int value;
			int id;
			do
			{
				// read the value
				value = Marshal.ReadInt32(origin, (int)offset);
				offset += sizeof(int);
				if (value == 1)
				{
					id = Marshal.ReadInt32(origin, (int)offset);
					offset += sizeof(int);
					value = Marshal.ReadInt32(origin, (int)offset);
					offset += sizeof(int);
				}
				else if (value == 0)
				{
					break;
				}
				else
				{
					id = data.Length;
				}
				Array.Resize(ref data, id + 1);
				data[id] = new IntPtr((long)origin + value);
			}
			while ((long)origin + offset < (long)data[0]);
		}

		public override Dictionary<IntPtr, byte[]> Handle()
		{
			Dictionary<IntPtr, byte[]> ret = new();
			for (int i = 0; i < data.Length - 1; i++)
			{
				byte[] set = Array.Empty<byte>();
				do
				{
					Array.Resize(ref set, set.Length + 1);
					set[^1] = Marshal.ReadByte(data[i], set.Length - 1);
					if (set.Length > sizeof(int) && set[^1] == 0)
					{
						break;
					}
					if (set.Length >= sizeof(ushort) && BitConverter.ToUInt16(set, set.Length - sizeof(ushort)) == ushort.MaxValue)
					{
						Array.Resize(ref set, set.Length + sizeof(ushort));
						BitConverter.GetBytes(Marshal.ReadInt16(data[i], set.Length - sizeof(ushort))).CopyTo(set, set.Length - sizeof(ushort));
						break;
					}
				}
				while (true);
				if (!ret.ContainsKey(data[i]))
				{
					ret.Add(data[i], set);
				}
			}
			return ret;
		}

		public override bool Equals(object obj)
		{
			if (obj is Ptr_Array ptr && ptr != null && (ptr.data?.Length).GetValueOrDefault() == (data?.Length).GetValueOrDefault())
			{
				for (int i = 0; i < (data?.Length).GetValueOrDefault(); i++)
				{
					if ((long)data[i] != (long)ptr.data[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(data, data.Length);
		}
	}
}
