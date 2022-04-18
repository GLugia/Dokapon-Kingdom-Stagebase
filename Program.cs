using CharaReader.data;
using System;
using System.IO;
using System.Text;

namespace CharaReader
{
	internal unsafe class Program
	{
		// stagebase string encoding
		public static Encoding shift_jis { get; private set; }
		private static void Main(string[] _)
		{
			// find the shift-jis encoding
			shift_jis = CodePagesEncodingProvider.Instance.GetEncoding("shift-jis");

			// attempt to split stagebase into bas and chr
			if (!SplitStagebase())
			{
				// if it fails, stop here
				Console.Out.WriteLine($"Failed to split STAGEBASE.DAT");
				return;
			}

			// attempt to read bas
			if (!ReadBAS())
			{
				// if it fails, stop here
				Console.Out.WriteLine($"Failed to read @BAS.DAT");
				return;
			}

			// attempt to read chr
			if (!ReadCHR())
			{
				// if it fails, stop here
				Console.Out.WriteLine($"Failed to read @CHR.DAT");
				return;
			}

			// attempt to combine bas and chr into a new stagebase
			if (!CombineStagebase())
			{
				// if it fails, stop here
				Console.Out.WriteLine($"Failed to combine STAGEBASE_NEW.DAT");
				return;
			}

			// attempt to compare the newly created stagebase to the original
			if (!CompareStagebase())
			{
				// if it fails, stop here
				Console.Out.WriteLine("Failed to compare STAGEBASE.DAT to STAGEBASE_NEW.DAT");
				return;
			}
		}

		/// <summary>
		/// Reads the newly created @BAS.DAT file taken from STAGEBASE.DAT
		/// </summary>
		/// <returns>false if an error is caught. true if no error is caught.</returns>
		private static bool ReadBAS()
		{
			string bas = $"@BAS.DAT";
			// if the file doesn't exist, return early
			if (!File.Exists(bas))
			{
				Console.Out.WriteLine(new FileNotFoundException(bas));
				return false;
			}
			// init reader and writer outside of try/catch so we can close them before returning
			DataReader reader = null;
			DataWriter writer = null;
			try
			{
				Console.Out.WriteLine();
				Console.Out.WriteLine($"Opening {bas}");
				// allocate a new reader to read the @BAS.DAT file
				reader = new(bas);
				// allocate a new BAS class to use the reader
				BAS bas_file = new(reader);
				// allocate a new writer to write a new @BAS.DAT
				// we don't use the same name as the original simply for comparing in HxD
				writer = new("@BAS_NEW.DAT");
				// let BAS handle writing
				bas_file.Write(writer);
				// close the writer if we reached this far
				writer?.Close();
				writer = null;
				// close the reader if we reached this far
				reader?.Close();
				reader = null;
				Console.Out.WriteLine($"Finished reading @BAS.DAT");
				// return true since no errors were found
				return true;
			}
			catch (ArgumentException ae)
			{
				// useful in catching errors thrown when using dynamics
				Console.Out.WriteLine(ae);
			}
			catch (IndexOutOfRangeException iore)
			{
				// sometimes i'm stupid and don't know how arrays work
				Console.Out.WriteLine(iore);
			}
			catch (Exception e)
			{
				// any other exception will print normally
				Console.Out.WriteLine(e);
			}
			// close the writer if it was open
			writer?.Close();
			writer = null;
			// close the reader if it was open
			reader?.Close();
			reader = null;
			// return false since an error was caught
			return false;
		}

		/// <summary>
		/// Reads the newly created @CHR.DAT file taken from STAGEBASE.DAT
		/// </summary>
		/// <returns>false if an error is caught. true if no error is caught.</returns>
		private static bool ReadCHR()
		{
			string chr = $"@CHR.DAT";
			// if the file doesn't exist, return early.
			if (!File.Exists(chr))
			{
				Console.Out.WriteLine(new FileNotFoundException(chr));
				return false;
			}
			// init reader and writer outside of try/catch so we can close them before returning
			DataReader reader = null;
			DataWriter writer = null;
			try
			{
				Console.Out.WriteLine();
				Console.Out.WriteLine($"Opening {chr}");
				// allocate a new reader to read the @BAS.DAT file
				reader = new(chr);
				// allocate a new CHR class to use the reader
				CHR chr_file = new(reader);
				// allocate a new writer to write a new @CHR.DAT
				// we don't use the same name as the original simply for comparing in HxD
				writer = new("@CHR_NEW.DAT");
				// let CHR handle writing
				chr_file.Write(writer);
				// close the writer if we reached this far
				writer?.Close();
				writer = null;
				// close the reader if we reached this far
				reader?.Close();
				reader = null;
				Console.Out.WriteLine($"Finished reading @CHR.DAT");
				// return true since no errors were found
				return true;
			}
			catch (ArgumentException ae)
			{
				// useful in catching errors thrown when using dynamics
				Console.Out.WriteLine(ae);
			}
			catch (IndexOutOfRangeException iore)
			{
				// sometimes i'm stupid and don't know how arrays work
				Console.Out.WriteLine(iore);
			}
			catch (Exception e)
			{
				// any other exception will print normally
				Console.Out.WriteLine(e);
			}
			// close the writer if it was open
			writer?.Close();
			writer = null;
			// close the reader if it was open
			reader?.Close();
			reader = null;
			// return false since an error was caught
			return false;
		}

		private static bool SplitStagebase()
		{
			string sb = "STAGEBASE.DAT";
			// if the STAGEBASE.DAT file is not found, return early.
			if (!File.Exists(sb))
			{
				Console.Out.WriteLine(new FileNotFoundException(sb));
				return false;
			}
			try
			{
				// allocate a new stream targeting STAGEBASE.DAT
				Stream stream = File.OpenRead(sb);
				long origin;
				string name;
				int length;
				byte[] temp;
				byte[] data;
				// while the current offset is lower than the stream length
				while (stream.Position < stream.Length)
				{
					// set the origin offset to the current offset
					// this is used to know the offset of the first byte in the next file
					origin = stream.Position;
					// buffer a new 4-byte array
					temp = new byte[sizeof(int)];
					// read and copy bytes from the stream to the temp array
					stream.Read(temp);
					// read the bytes from the temp array as char values
					// this gets us the name of the file
					name = "";
					foreach (byte b in temp)
					{
						name += (char)b;
					}
					// append the extension
					name += ".DAT";
					// buffer another 4-byte array
					temp = new byte[sizeof(int)];
					// read and copy bytes from the stream to the temp array
					stream.Read(temp);
					// parse those bytes as s32
					length = BitConverter.ToInt32(temp);
					// the first 4 bytes of the file are never included in the total length
					length += sizeof(int);
					// align to 16
					length += 16 - (length % 16);
					// buffer the file data array
					data = new byte[length];
					// from the beginning of the file, move to the origin offset
					stream.Seek(origin, SeekOrigin.Begin);
					// read and copy bytes from the stream to the data array
					stream.Read(data);
					// write the entire data array to a new file using the name we previously took from the first 4 bytes
					File.WriteAllBytes(name, data);
				}
				// close the stream
				stream.Close();
				// return true since no error was found
				return true;
			}
			catch (Exception e)
			{
				// juuuuust in case...
				Console.Out.WriteLine(e);
			}
			// return false since no error was found
			return false;
		}

		/// <summary>
		/// Combines @BAS_NEW.DAT and @CHR_NEW.DAT into STAGEBASE_NEW.DAT<para></para>
		/// TODO: technically this should combine all files in a given folder into a new stagebase but
		/// since we know there are only ever 2 files, it isn't necessary "YET".
		/// </summary>
		/// <returns>false if an error is caught. true if no error is caught.</returns>
		private static bool CombineStagebase()
		{
			string bas = "@BAS_NEW.DAT";
			string chr = "@CHR_NEW.DAT";
			// if the bas file is not found, return early
			if (!File.Exists(bas))
			{
				Console.Out.WriteLine(new FileNotFoundException(bas));
				return false;
			}
			// if the chr file is not found, return early
			if (!File.Exists(chr))
			{
				Console.Out.WriteLine(new FileNotFoundException(chr));
				return false;
			}
			try
			{
				// read the entire bas file
				byte[] bas_data = File.ReadAllBytes(bas);
				// read the entire chr file
				byte[] chr_data = File.ReadAllBytes(chr);
				// create a new stagebase file
				Stream stream = File.Create("STAGEBASE_NEW.DAT");
				// write bas to the new stagebase
				stream.Write(bas_data);
				// write chr to the new stagebase
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

		/// <summary>
		/// Compares the new STAGEBASE_NEW.DAT contents to the original STAGEBASE.DAT contents
		/// </summary>
		/// <returns></returns>
		private static bool CompareStagebase()
		{
			string sb = "STAGEBASE.DAT";
			string sbn = "STAGEBASE_NEW.DAT";
			// if stagebase SOMEHOW doesn't exist, return early
			if (!File.Exists(sb))
			{
				Console.Out.WriteLine(new FileNotFoundException(sb));
				return false;
			}
			// if the newly created STAGEBASE_NEW.DAT file doesn't exist, return early
			if (!File.Exists(sbn))
			{
				Console.Out.WriteLine(new FileNotFoundException(sbn));
				return false;
			}
			try
			{
				// read the contents of the original stagebase
				byte[] sb_data = File.ReadAllBytes(sb);
				// read the contents of the new stagebase
				byte[] sbn_data = File.ReadAllBytes(sbn);
				// find the smallest file
				// this is actually important as i could easily make a mistake somewhere and
				// the new stagebase could suddenly be twice as large as the original.
				int smallest_array_size = sb_data.Length > sbn_data.Length ? sbn_data.Length - 1 : sb_data.Length - 1;
				// intialize two s32 values
				// the total amount of bytes that are equal
				int total_correct_bytes = 0;
				// the total amount of bytes that are not equal
				int total_incorrect_bytes = 0;
				// start scanning
				for (int i = 0; i < smallest_array_size; i++)
				{
					// if, at this offset, the byte in the original stagebase equals the byte in the new stagebase
					if (sb_data[i] == sbn_data[i])
					{
						// correct
						total_correct_bytes++;
					}
					// if those bytes are not equal
					else
					{
						// incorrect
						total_incorrect_bytes++;
					}
				}
				// compare the amount of correct bytes to the total amount of bytes in the original stagebase, rounded to 2 decimal places
				double eq = Math.Round(total_correct_bytes / ((double)(sb_data.Length - 1)) * 100.0d, 2);
				// compare the amount of incorrect bytes to the total amount of bytes in the original stagebase, rounded to 2 decimal places
				double ieq = Math.Round(total_incorrect_bytes / ((double)(sb_data.Length - 1)) * 100.0d, 2);
				// compare the total amount of bytes in the new stagebase to the amount of bytes in the original stagebase, rounded to 2 decimal places
				double size = Math.Round((sbn_data.Length - 1) / ((double)(sb_data.Length - 1)) * 100.0d, 2);
				Console.Out.WriteLine($"Completion rate:");
				Console.Out.WriteLine($"\tEquality: {eq}%");
				Console.Out.WriteLine($"\tInequality: {ieq}%");
				Console.Out.WriteLine($"\tSize: {size}%");
				// return true since no error was found
				return true;
			}
			catch (Exception e)
			{
				Console.Out.WriteLine(e);
			}
			// return false since an error was found
			return false;
		}
	}
}
