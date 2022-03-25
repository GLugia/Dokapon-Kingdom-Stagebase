using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CharaReader
{
	class Program
	{
		static BinaryReader reader;
		static Encoding shift_jis;
		static string position => $"0x{((int)reader.BaseStream.Position):x}";
		const long offset_stagebase = 0x0;
		const long offset_chr = 0x83F0;
		const long offset_bas = 0x60;

		static void Main(string[] args)
		{
			shift_jis = CodePagesEncodingProvider.Instance.GetEncoding("iso-2022-jp");

			if (!SplitStageBase())
			{
				Console.Out.WriteLine($"Failed to split GAME{Path.DirectorySeparatorChar}STAGEBASE.DAT");
				return;
			}

			/*if (!CountTypes(0xe820, 0xf254, offset_stagebase, 0x6C))
			{
				Console.Out.WriteLine($"Failed to count structs.");
				return;
			}*/
			
			if (!ReadCHR())
			{
				Console.Out.WriteLine($"Failed to read CHR.DAT");
				return;
			}
		}

		static bool CountTypes(long starting_position, long ending_position, long offset, uint target_struct_id)
		{
			string file = $"GAME{Path.DirectorySeparatorChar}STAGEBASE.DAT";
			if (!File.Exists(file))
			{
				Console.Out.WriteLine(new FileNotFoundException(file));
				return false;
			}
			try
			{
				Console.Out.WriteLine();
				Console.Out.WriteLine($"Opening {file}");
				reader = new(File.OpenRead(file), shift_jis);
				reader.BaseStream.Position = starting_position;
				int count = 0;
				uint next;
				while (reader.BaseStream.Position < ending_position)
				{
					next = reader.ReadUInt32();
					if (next == target_struct_id)
					{
						count++;
					}
				}
				Console.Out.WriteLine($"Number of structs with ID '{target_struct_id.ToHexString()}': {count.ToHexString()}");
				reader.Close();
			}
			catch (Exception e)
			{
				reader?.Close();
				reader = null;
				Console.Out.WriteLine(e.ToString());
				return false;
			}
			reader?.Close();
			return true;
		}

		static bool ReadCHR()
		{
			if (!File.Exists("@CHR.DAT"))
			{
				Console.Out.WriteLine(new FileNotFoundException("CHR.DAT"));
				return false;
			}
			if (!Directory.Exists("chr_dump"))
			{
				Directory.CreateDirectory("chr_dump");
			}
			try
			{
				Console.Out.WriteLine();
				Console.Out.WriteLine("Opening @CHR.DAT");
				Console.Out.WriteLine("{ POSITION } - {  ID  / DIFF / NEXT } - { COUNT / SIZE / TOTAL } - { TYPE }");
				reader = new(File.OpenRead("@CHR.DAT"), shift_jis);
				reader.BaseStream.Position = 0x60;
				List<object>[] table = new List<object>[0xFF];
				object val;
				byte id;
				byte last_id = 0;
				int position;
				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					position = (int)reader.BaseStream.Position;
					val = reader.ReadStruct();
					if (val == null)
					{
						break;
					}
					id = (byte)(int)val.GetType().GetField("table_id").GetValue(val);
					if (id != last_id)
					{
						table[id] = new();
						if (table[last_id] != null && table[last_id].Count > 0)
						{
							Console.Out.WriteLine($"{{ {position.ToHexString(false)} }} - {{ {((byte)last_id).ToHexString()} / {((sbyte)(id - last_id)).ToHexString()} / {((byte)id).ToHexString()} }} - {{ {table[last_id].Count,+5} / {Activator.CreateInstance(table[last_id][0].GetType()).Size(),+4} / {table[last_id].Sum(a => a.Size()),+5} }} - {table[last_id][0].GetType()}");
							// Console.ReadKey(true);
							;
						}
						last_id = id;
					}
					table[id].Add(val);
				}
			}
			catch (Exception e)
			{
				reader?.Close();
				reader = null;
				Console.Out.WriteLine(e.ToString());
				return false;
			}
			return true;
		}

		static bool SplitStageBase()
		{
			if (!Directory.Exists("GAME"))
			{
				Console.Out.WriteLine(new DirectoryNotFoundException("GAME"));
				return false;
			}

			string path = $"GAME{Path.DirectorySeparatorChar}STAGEBASE.DAT";

			if (!File.Exists(path))
			{
				Console.Out.WriteLine(new FileNotFoundException(path));
				return false;
			}

			string current_file_name = null;
			try
			{
				Console.Out.Write("Opening STAGEBASE.DAT...");
				reader = new(File.OpenRead(path));
				Console.Out.Write("Done\n");
				BinaryWriter writer_temp = null;
				Console.Out.Write("Reading STAGEBASE.DAT...\n");
				int file_start_position;
				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					file_start_position = (int)reader.BaseStream.Position;
					byte[] name = reader.Read(4, "name", 2);
					byte[] length = reader.Read(4, "length");
					byte[] data_start_pos = reader.Read(4, "header size");
					byte[] filler = reader.Read(BitConverter.ToInt32(data_start_pos) - ((int)reader.BaseStream.Position - file_start_position), "filler");
					byte[] data = reader.Read((BitConverter.ToInt32(length) - BitConverter.ToInt32(data_start_pos)) + 8, "data", 0);
					current_file_name = $"{Encoding.UTF8.GetString(name)}.DAT.temp";
					Console.Out.Write($"Opening {current_file_name}...");
					writer_temp = new BinaryWriter(File.OpenWrite(current_file_name));
					Console.Out.Write("Done\n");
					writer_temp.Write(name, "name");
					writer_temp.Write(length, "length");
					writer_temp.Write(data_start_pos, "header size");
					writer_temp.Write(filler, "filler");
					writer_temp.Write(data, "data");
					Console.Out.Write("Flushing...");
					writer_temp.Flush();
					writer_temp.Close();
					Console.Out.Write("Done\n");
					File.Move(current_file_name, current_file_name[0..(current_file_name.LastIndexOf('.'))], true);
				}
				reader.Close();
				Console.Out.Write("Finished\n");
			}
			catch (Exception e)
			{
				if (File.Exists(current_file_name))
				{
					File.Delete(current_file_name);
				}
				Console.Out.WriteLine(e.ToString());
				reader?.Close();
				return false;
			}
			reader?.Close();
			return true;
		}
	}
}
