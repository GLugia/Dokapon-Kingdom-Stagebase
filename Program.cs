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
		static int real_size;
		static int decompiled_size;

		private static void Main(string[] _)
		{
			var encodings = CodePagesEncodingProvider.Instance.GetEncodings().Select(a => a.DisplayName).ToArray();
			shift_jis = CodePagesEncodingProvider.Instance.GetEncoding("shift-jis");

			
			if (!SplitStagebase())
			{
				Console.Out.WriteLine($"Failed to split STAGEBASE.DAT");
				return;
			}

			if (!ReadBAS())
			{
				Console.Out.WriteLine($"Failed to read @BAS.DAT");
				return;
			}
			
			if (!ReadCHR())
			{
				Console.Out.WriteLine($"Failed to read @CHR.DAT");
				return;
			}

			
			if (!CombineStagebase())
			{
				Console.Out.WriteLine($"Failed to combine STAGEBASE_NEW.DAT");
				return;
			}

			Console.Out.WriteLine($"Completion rate: {Math.Round(((double)decompiled_size / (double)real_size) * 100, 2)}%");
		}

		private static bool ReadBAS()
		{
			string bas = $"@BAS.DAT";
			if (!File.Exists(bas))
			{
				Console.Out.WriteLine(new FileNotFoundException(bas));
				return false;
			}
			DataReader reader = null;
			DataWriter writer = null;
			try
			{
				Console.Out.WriteLine();
				Console.Out.WriteLine($"Opening {bas}");

				reader = new("@BAS.DAT");
				BAS bas_file = new(reader);

				writer = new("@BAS_NEW.DAT");
				bas_file.Write(writer);

				writer?.Close();
				writer = null;
				reader?.Close();
				reader = null;
				Console.Out.WriteLine($"Finished reading @BAS.DAT");
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

		private static bool ReadCHR()
		{
			string chr = $"@CHR.DAT";
			if (!File.Exists(chr))
			{
				Console.Out.WriteLine(new FileNotFoundException(chr));
				return false;
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

		private static bool SplitStagebase()
		{
			string sb = "STAGEBASE.DAT";
			if (!File.Exists(sb))
			{
				Console.Out.WriteLine(new FileNotFoundException(sb));
				return false;
			}
			try
			{
				Stream stream = File.OpenRead(sb);
				real_size = (int)stream.Length;
				long origin;
				string name;
				int length;
				byte[] temp;
				byte[] data;
				while (stream.Position < stream.Length)
				{
					origin = stream.Position;
					temp = new byte[sizeof(int)];
					stream.Read(temp);
					name = "";
					foreach (byte b in temp)
					{
						name += (char)b;
					}
					name += ".DAT";
					temp = new byte[sizeof(int)];
					stream.Read(temp);
					length = BitConverter.ToInt32(temp);
					length += sizeof(int);
					length += 16 - (length % 16);
					data = new byte[length];
					stream.Seek(origin, SeekOrigin.Begin);
					stream.Read(data);
					File.WriteAllBytes(name, data);
				}
				stream.Close();
				return true;
			}
			catch (Exception e)
			{
				Console.Out.WriteLine(e);
			}
			return false;
		}

		private static bool CombineStagebase()
		{
			string bas = "@BAS_NEW.DAT";
			string chr = "@CHR_NEW.DAT";
			if (!File.Exists(bas))
			{
				Console.Out.WriteLine(new FileNotFoundException(bas));
				return false;
			}
			if (!File.Exists(chr))
			{
				Console.Out.WriteLine(new FileNotFoundException(chr));
				return false;
			}
			try
			{
				byte[] bas_data = File.ReadAllBytes(bas);
				byte[] chr_data = File.ReadAllBytes(chr);
				decompiled_size = bas_data.Length + chr_data.Length;
				Stream stream = File.Create("STAGEBASE_NEW.DAT");
				stream.Write(bas_data);
				stream.Write(chr_data);
				stream.Flush();
				stream.Close();
				//File.Delete(bas);
				//File.Delete(chr);
				return true;
			}
			catch (Exception e)
			{
				Console.Out.WriteLine(e);
			}
			return false;
		}
	}
}
