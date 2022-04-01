namespace CharaReader.data
{
	public abstract class BaseData
	{
		public BaseData(DataReader reader) { }
		public abstract void Write(DataWriter writer);
	}
}
