using System;
using System.Runtime.InteropServices;

namespace CharaReader.data.types.subtypes
{
	public unsafe struct Unk_07
	{
		public byte id_00;  // 07
		public byte unk_01; // FF
		public byte unk_02; // 00
		public byte unk_03; // 00
		public byte unk_04; // 00
		public byte unk_05; // 00
		public byte unk_06; // 12
		public byte unk_07; // 00
		[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4, SizeConst = 20)]
		public float[] unk_00;
	}
}
