using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CharaReader.data.ptrs
{
	public class Ptr_List : Ptr_Custom<IntPtr[]>
	{
		public Ptr_List(IntPtr origin, ref long offset) : base(origin, ref offset)
		{
			data = new IntPtr[1]
			{
				// original pointer that leads to the following data
				new IntPtr((long)origin + Marshal.ReadInt32(origin, (int)offset))
			};
			offset += sizeof(int);

			int value;
			int id;
			long temp_offset = 0;
			do
			{
				// read the value
				value = Marshal.ReadInt32(data[0], (int)temp_offset);
				temp_offset += sizeof(int);
				if (value == 1)
				{
					id = Marshal.ReadInt32(data[0], (int)temp_offset);
					temp_offset += sizeof(int);
					value = Marshal.ReadInt32(data[0], (int)temp_offset);
					temp_offset += sizeof(int);
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
			while (true);
		}

		public override Dictionary<IntPtr, byte[]> Handle()
		{
			Dictionary<IntPtr, byte[]> ret = new();
			for (int i = 1; i < data.Length - 1; i++)
			{
				byte[] set = Array.Empty<byte>();
				do
				{
					Array.Resize(ref set, set.Length + 1);
					set[^1] = Marshal.ReadByte(data[i], set.Length);
				}
				while ((long)data[i] + set.Length < (long)data[i + 1]);
				if (!ret.ContainsKey(data[i]))
				{
					ret.Add(data[i], set);
				}
			}
			byte[] last = Array.Empty<byte>();
			for (long end = (long)data[^1]; Marshal.ReadInt32((IntPtr)end) != 0; end++)
			{
				Array.Resize(ref last, last.Length + 1);
				last[^1] = Marshal.ReadByte((IntPtr)end);
			}
			if (!ret.ContainsKey(data[^1]))
			{
				ret.Add(data[^1], last);
			}
			return ret;
		}

		public override bool Equals(object obj)
		{
			if (obj is Ptr_List ptr && ptr != null && ptr.data != null && ptr.data.Length == data.Length)
			{
				for (int i = 0; i < data.Length; i++)
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
			return HashCode.Combine(data);
		}
	}
}
