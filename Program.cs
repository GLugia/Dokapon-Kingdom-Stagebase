using CharaReader.data;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CharaReader
{
	internal unsafe class Program
	{
		public static Encoding shift_jis { get; private set; }

		private static void Main(string[] _)
		{
			var encodings = CodePagesEncodingProvider.Instance.GetEncodings().Select(a => a.DisplayName).ToArray();
			shift_jis = CodePagesEncodingProvider.Instance.GetEncoding("shift-jis");

			StreamWriter writer = new(File.Create("temp.txt"));
			for (byte b = 0; b < byte.MaxValue; b++)
			{
				writer.WriteLine($"\t\t\t/* {b.ToHexString()} */ null,");
			}
			writer.Flush();
			writer.Close();

			if (!ReadCHR())
			{
				Console.Out.WriteLine($"Failed to read GAME/STAGEBASE.DAT");
				return;
			}
		}

		private static bool ReadCHR()
		{
			string chr = $"@CHR.DAT";
			if (!File.Exists(chr))
			{
				Console.Out.WriteLine(new FileNotFoundException(chr));
				return false;
			}
			if (!Directory.Exists("chr_dump"))
			{
				Directory.CreateDirectory("chr_dump");
			}
			DataReader reader = null;
			DataWriter writer = null;
			try
			{
				Console.Out.WriteLine();
				Console.Out.WriteLine($"Opening {chr}");

				reader = new("@CHR.DAT");
				CHR chr_file = new(reader);

				writer = new("@CHR_NEW.DAT");
				chr_file.Write(writer);

				writer?.Close();
				writer = null;
				reader?.Close();
				reader = null;
				Console.Out.WriteLine($"Finished reading @CHR.DAT");
				return true;
			}
			catch (ArgumentException ae)
			{
				Console.Out.WriteLine(ae);
			}
			catch (IndexOutOfRangeException iore)
			{
				Console.Out.WriteLine(iore);
			}
			catch (Exception e)
			{
				Console.Out.WriteLine(e);
			}
			writer?.Close();
			writer = null;
			reader?.Close();
			reader = null;
			return false;
		}
	}
}
