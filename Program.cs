using CharaReader.data;
using System.Text;

namespace CharaReader
{
	internal unsafe class Program
	{
		// stagebase string encoding
		public static Encoding shift_jis { get; private set; }
		public static Stagebase testing { get; private set; }
		private static void Main(string[] _)
		{
			// find the shift-jis encoding
			shift_jis = CodePagesEncodingProvider.Instance.GetEncoding("shift-jis");
			testing = new();
			// handle data from testing here
		}
	}
}
