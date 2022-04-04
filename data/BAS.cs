using System;
using System.IO;

namespace CharaReader.data
{
	public class BAS
	{


		public BAS(BinaryReader reader)
		{
			string name = string.Join("", reader.ReadChars(4));
			if (name != "@BAS")
			{
				throw new Exception($"Expected '@BAS' got '{name}'");
			}

			byte[] data = reader.ReadBytes(reader.ReadInt32());
		}

		public void Write(ref byte[] file)
		{

		}
	}
}
