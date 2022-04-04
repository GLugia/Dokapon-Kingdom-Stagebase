using CharaReader.data.chr_data.structs;
using System;
using System.Linq;

namespace CharaReader.data.chr_data
{
	public class JobData : BaseData
	{
		public Job[] jobs;
		public JobSkills[][] skills;
		public JobStats[][] stats;
		public JobMastery[][] mastery;
		public JobBag[][] bag_sizes;
		public JobUnk_3E[][] unk_3E;
		public JobMoney[][] money;
		public JobUnk_D6[] unk_d6;
		public JobModel[][][] models;
		public JobModel_0_3[][] model_0_3s;
		public JobModel_4_7[][] model_4_7s;
		public JobUnk_43[][] unk_43;
		public JobUnk_38[][] unk_38;
		public ushort[][] unk_39;
		public ushort[][] unk_8F;
		public string[] descriptions;

		public JobData(DataReader reader) : base(reader)
		{
			unk_39 = Array.Empty<ushort[]>();
			unk_8F = Array.Empty<ushort[]>();
			descriptions = Array.Empty<string>();
			int start;
			int temp_offset;
			int table_id;
			dynamic job_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x3D: descriptions = reader.ReadDescriptions(); break;
					case 0x42: jobs = reader.ReadStructs<Job>(table_id); break;
					case 0x3C: skills = reader.ReadStructs2<JobSkills>(table_id); break;
					case 0x40: stats = reader.ReadStructs2<JobStats>(table_id); break;
					case 0x3B: mastery = reader.ReadStructs2<JobMastery>(table_id); break;
					case 0x44: bag_sizes = reader.ReadStructs2<JobBag>(table_id); break;
					case 0x3E: unk_3E = reader.ReadStructs2<JobUnk_3E>(table_id); break;
					case 0x2E: money = reader.ReadStructs2<JobMoney>(table_id); break;
					case 0xD6: unk_d6 = reader.ReadStructs<JobUnk_D6>(table_id); break;
					case 0x41: models = reader.ReadStructs3<JobModel>(table_id); break;
					case 0x45: model_0_3s = reader.ReadStructs2<JobModel_0_3>(table_id); break;
					case 0x29: model_4_7s = reader.ReadStructs2<JobModel_4_7>(table_id); break;
					case 0x43: unk_43 = reader.ReadStructs2<JobUnk_43>(table_id); break;
					case 0x38: unk_38 = reader.ReadStructs2<JobUnk_38>(table_id); break;
					case 0x39:
						{
							// reads to 0xFFFF
							job_id = reader.ReadInt32();
							if (job_id > unk_39.Length - 1)
							{
								Array.Resize(ref unk_39, job_id + 1);
								unk_39[job_id] = Array.Empty<ushort>();
							}
							start = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < reader.length)
							{
								Array.Resize(ref unk_39[job_id], unk_39[job_id].Length + 1);
								unk_39[job_id][^1] = reader.ReadUInt16();
								if (unk_39[job_id][^1] == ushort.MaxValue)
								{
									reader.ReadInt16();
									break;
								}
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x8F:
						{
							// reads to 0xFFFF
							job_id = reader.ReadInt32();
							if (job_id > unk_8F.Length - 1)
							{
								Array.Resize(ref unk_8F, job_id + 1);
								unk_8F[job_id] = Array.Empty<ushort>();
							}
							start = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < reader.length)
							{
								Array.Resize(ref unk_8F[job_id], unk_8F[job_id].Length + 1);
								unk_8F[job_id][^1] = reader.ReadUInt16();
								if (unk_8F[job_id][^1] == ushort.MaxValue)
								{
									reader.ReadInt16();
									break;
								}
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x03:
						{
							reader.offset = reader.ReadInt32();
							finished = true;
							break;
						}
					default:
						{
							throw new Exception($"Unknown table {(reader.offset - sizeof(int)).ToHexString()}->{((byte)table_id).ToHexString()}");
						}
				}
			}
		}

		public override void Write(DataWriter writer)
		{
			writer.ReservePointer(0x30, "job_des_ptr", 2);
			writer.WriteStructs(0x42, jobs);
			writer.WriteStructs(0x3C, skills);
			writer.WriteStructs(0x40, stats);
			writer.WriteStructs(0x3B, mastery);
			writer.WriteStructs(0x44, bag_sizes);
			writer.WriteStructs(0x3E, unk_3E);
			writer.WriteStructs(0x2E, money);
			writer.WriteStructs(0xD6, unk_d6);
			writer.WriteStructs(0x41, models);
			writer.WriteStructs(0x45, model_0_3s);
			writer.WriteStructs(0x29, model_4_7s);
			writer.WriteStructs(0x43, unk_43);
			writer.WriteStructs(0x38, unk_38.Where(a => a != null).ToArray());
			for (int i = 0; i < unk_39.Length; i++)
			{
				writer.ReservePointer(0x39, $"39_ptr_{i}", 2);
			}
			for (int i = 0; i < unk_8F.Length; i++)
			{
				writer.ReservePointer(0x8F, $"8F_ptr_{i}", 2);
			}
			writer.ReservePointer(0x03, "des_end_ptr");
			writer.Write(0);
			for (int i = 0; i < unk_39.Length; i++)
			{
				writer.WritePointerID($"39_ptr_{i}", i);
				for (int j = 0; j < unk_39[i].Length; j++)
				{
					writer.Write(unk_39[i][j]);
				}
				writer.Write((short)0);
			}
			for (int i = 0; i < unk_8F.Length; i++)
			{
				writer.WritePointerID($"8F_ptr_{i}", i);
				for (int j = 0; j < unk_8F[i].Length; j++)
				{
					writer.Write(unk_8F[i][j]);
				}
				writer.Write((short)0);
			}
			writer.WritePointer("job_des_ptr");
			writer.WriteDescriptions(descriptions);
			writer.WritePointer("job_des_ptr");
			writer.WritePointer("des_end_ptr");
		}
	}
}
