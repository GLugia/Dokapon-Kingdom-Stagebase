using System.IO;

namespace CharaReader.data.chr_data
{
	public abstract class StructBase
	{
		public virtual dynamic GetID(BinaryReader reader)
		{
			return (byte)reader.ReadInt32();
		}

		public virtual void Read(BinaryReader reader)
		{
			//reader.AutoReadFields(this);
		}

		public virtual void Write(BinaryWriter writer)
		{
			throw new System.NotImplementedException();
		}
	}
}
