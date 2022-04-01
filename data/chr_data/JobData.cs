using CharaReader.data.chr_data.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data.chr_data
{
	public class JobData : BaseData
	{
		public Job[] jobs;
		public JobSkills[][] skills;
		public JobStats[][] stats;
		public JobMastery[][] mastery;
		public JobBag[][] bag_sizes;
		public short[][] unk_3E;
		public JobMoney[][] money;
		public JobUnk_D6[] unk_d6;
		public JobModel[][][] models;
		public JobModel_0_3[][] model_0_3s;
		public JobModel_4_7[][] model_4_7s;
		public float[][] unk_43;
		public string[] unk_38;
		public ushort[][] unk_39;
		public ushort[][] unk_8F;
		public string[] descriptions;

		public JobData(DataReader reader) : base(reader)
		{
			jobs = Array.Empty<Job>();
			skills = Array.Empty<JobSkills[]>();
			stats = Array.Empty<JobStats[]>();
			mastery = Array.Empty<JobMastery[]>();
			bag_sizes = Array.Empty<JobBag[]>();
			unk_3E = Array.Empty<short[]>();
			money = Array.Empty<JobMoney[]>();
			unk_d6 = Array.Empty<JobUnk_D6>();
			models = Array.Empty<JobModel[][]>();
			model_0_3s = Array.Empty<JobModel_0_3[]>();
			model_4_7s = Array.Empty<JobModel_4_7[]>();
			unk_43 = Array.Empty<float[]>();
			unk_38 = Array.Empty<string>();
			unk_39 = Array.Empty<ushort[]>();
			unk_8F = Array.Empty<ushort[]>();
			descriptions = Array.Empty<string>();
			int start;
			int end;
			int temp_offset;
			int table_id;
			dynamic job_id;
			dynamic gender_id;
			dynamic extra;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x3D:
						{
							start = reader.ReadInt32();
							end = reader.ReadInt32();
							temp_offset = reader.offset;
							reader.offset = start;
							while (reader.offset < end)
							{
								Array.Resize(ref descriptions, descriptions.Length + 1);
								descriptions[^1] = reader.ReadString();
							}
							reader.offset = temp_offset;
							break;
						}
					case 0x42:
						{
							job_id = reader.ReadInt32();
							if (job_id > jobs.Length - 1)
							{
								Array.Resize(ref jobs, job_id + 1);
							}
							jobs[job_id] = reader.ReadStruct<Job>();
							break;
						}
					case 0x3C:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							if (job_id > skills.Length - 1)
							{
								Array.Resize(ref skills, job_id + 1);
								skills[job_id] = Array.Empty<JobSkills>();
							}
							if (gender_id > skills[job_id].Length - 1)
							{
								Array.Resize(ref skills[job_id], gender_id + 1);
							}
							skills[job_id][gender_id] = reader.ReadStruct<JobSkills>();
							break;
						}
					case 0x40:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							if (job_id > stats.Length - 1)
							{
								Array.Resize(ref stats, job_id + 1);
								stats[job_id] = Array.Empty<JobStats>();
							}
							if (gender_id > stats[job_id].Length - 1)
							{
								Array.Resize(ref stats[job_id], gender_id + 1);
							}
							stats[job_id][gender_id] = reader.ReadStruct<JobStats>();
							break;
						}
					case 0x3B:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							if (job_id > mastery.Length - 1)
							{
								Array.Resize(ref mastery, job_id + 1);
								mastery[job_id] = Array.Empty<JobMastery>();
							}
							if (gender_id > mastery[job_id].Length - 1)
							{
								Array.Resize(ref mastery[job_id], gender_id + 1);
							}
							mastery[job_id][gender_id] = reader.ReadStruct<JobMastery>();
							break;
						}
					case 0x44:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							if (job_id > bag_sizes.Length - 1)
							{
								Array.Resize(ref bag_sizes, job_id + 1);
								bag_sizes[job_id] = Array.Empty<JobBag>();
							}
							if (gender_id > bag_sizes[job_id].Length - 1)
							{
								Array.Resize(ref bag_sizes[job_id], gender_id + 1);
							}
							bag_sizes[job_id][gender_id] = reader.ReadStruct<JobBag>();
							break;
						}
					case 0x3E:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							if (job_id > unk_3E.Length - 1)
							{
								Array.Resize(ref unk_3E, job_id + 1);
								unk_3E[job_id] = Array.Empty<short>();
							}
							if (gender_id > unk_3E[job_id].Length - 1)
							{
								Array.Resize(ref unk_3E[job_id], gender_id + 1);
							}
							unk_3E[job_id][gender_id] = reader.ReadInt16();
							break;
						}
					case 0x2E:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							if (job_id > money.Length - 1)
							{
								Array.Resize(ref money, job_id + 1);
								money[job_id] = Array.Empty<JobMoney>();
							}
							if (gender_id > money[job_id].Length - 1)
							{
								Array.Resize(ref money[job_id], gender_id + 1);
							}
							money[job_id][gender_id] = reader.ReadStruct<JobMoney>();
							break;
						}
					case 0xD6:
						{
							job_id = reader.ReadInt32();
							if (job_id > unk_d6.Length - 1)
							{
								Array.Resize(ref unk_d6, job_id + 1);
							}
							unk_d6[job_id] = reader.ReadStruct<JobUnk_D6>();
							break;
						}
					case 0x41:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							extra = reader.ReadInt16();
							if (job_id > models.Length - 1)
							{
								Array.Resize(ref models, job_id + 1);
								models[job_id] = Array.Empty<JobModel[]>();
							}
							if (gender_id > models[job_id].Length - 1)
							{
								Array.Resize(ref models[job_id], gender_id + 1);
								models[job_id][gender_id] = Array.Empty<JobModel>();
							}
							if (extra > models[job_id][gender_id].Length - 1)
							{
								Array.Resize(ref models[job_id][gender_id], extra + 1);
							}
							models[job_id][gender_id][extra] = reader.ReadStruct<JobModel>();
							break;
						}
					case 0x45:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							reader.ReadInt16();
							if (job_id > model_0_3s.Length - 1)
							{
								Array.Resize(ref model_0_3s, job_id + 1);
								model_0_3s[job_id] = Array.Empty<JobModel_0_3>();
							}
							if (gender_id > model_0_3s[job_id].Length - 1)
							{
								Array.Resize(ref model_0_3s[job_id], gender_id + 1);
							}
							model_0_3s[job_id][gender_id] = reader.ReadStruct<JobModel_0_3>();
							break;
						}
					case 0x29:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							reader.ReadInt16();
							if (job_id > model_4_7s.Length - 1)
							{
								Array.Resize(ref model_4_7s, job_id + 1);
								model_4_7s[job_id] = Array.Empty<JobModel_4_7>();
							}
							if (gender_id > model_4_7s[job_id].Length - 1)
							{
								Array.Resize(ref model_4_7s[job_id], gender_id + 1);
							}
							model_4_7s[job_id][gender_id] = reader.ReadStruct<JobModel_4_7>();
							break;
						}
					case 0x43:
						{
							job_id = reader.ReadByte();
							gender_id = reader.ReadByte();
							reader.ReadInt16();
							if (job_id > unk_43.Length - 1)
							{
								Array.Resize(ref unk_43, job_id + 1);
								unk_43[job_id] = Array.Empty<float>();
							}
							if (gender_id > unk_43[job_id].Length - 1)
							{
								Array.Resize(ref unk_43[job_id], gender_id + 1);
							}
							unk_43[job_id][gender_id] = reader.ReadSingle();
							break;
						}
					case 0x38:
						{
							job_id = reader.ReadInt32();
							if (job_id > unk_38.Length - 1)
							{
								Array.Resize(ref unk_38, job_id + 1);
							}
							unk_38[job_id] = reader.ReadString();
							break;
						}
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
			writer.Write(0x3D);
			int job_des_ptr = writer.offset;
			writer.offset += sizeof(int) * 2;
			for (int i = 0; i < jobs.Length; i++)
			{
				writer.Write(0x42);
				writer.Write(i);
				writer.WriteStruct(jobs[i]);
			}
			for (byte i = 0; i < skills.Length; i++)
			{
				for (byte j = 0; j < skills[i].Length; j++)
				{
					writer.Write(0x3C);
					writer.Write(i);
					writer.Write(j);
					writer.WriteStruct(skills[i][j]);
				}
			}
			for (byte i = 0; i < stats.Length; i++)
			{
				for (byte j = 0; j < stats[i].Length; j++)
				{
					writer.Write(0x40);
					writer.Write(i);
					writer.Write(j);
					writer.WriteStruct(stats[i][j]);
				}
			}
			for (byte i = 0; i < mastery.Length; i++)
			{
				for (byte j = 0; j < mastery[i].Length; j++)
				{
					writer.Write(0x3B);
					writer.Write(i);
					writer.Write(j);
					writer.WriteStruct(mastery[i][j]);
				}
			}
			for (byte i = 0; i < bag_sizes.Length; i++)
			{
				for (byte j = 0; j < bag_sizes[i].Length; j++)
				{
					writer.Write(0x44);
					writer.Write(i);
					writer.Write(j);
					writer.WriteStruct(bag_sizes[i][j]);
				}
			}
			for (byte i = 0; i < unk_3E.Length; i++)
			{
				for (byte j = 0; j < unk_3E[i].Length; j++)
				{
					writer.Write(0x3E);
					writer.Write(i);
					writer.Write(j);
					writer.Write(unk_3E[i][j]);
				}
			}
			for (byte i = 0; i < money.Length; i++)
			{
				for (byte j = 0; j < money[i].Length; j++)
				{
					writer.Write(0x2E);
					writer.Write(i);
					writer.Write(j);
					writer.WriteStruct(money[i][j]);
				}
			}
			for (int i = 0; i < unk_d6.Length; i++)
			{
				if (unk_d6[i].unk_01 == 0)
				{
					continue;
				}
				writer.Write(0xD6);
				writer.Write(i);
				writer.WriteStruct(unk_d6[i]);
			}
			for (byte i = 0; i < models.Length; i++)
			{
				for (byte j = 0; j < models[i].Length; j++)
				{
					for (short k = 0; k < models[i][j].Length; k++)
					{
						writer.Write(0x41);
						writer.Write(i);
						writer.Write(j);
						writer.Write(k);
						writer.WriteStruct(models[i][j][k]);
					}
				}
			}
			for (byte i = 0; i < model_0_3s.Length; i++)
			{
				for (byte j = 0; j < model_0_3s[i].Length; j++)
				{
					writer.Write(0x45);
					writer.Write(i);
					writer.Write(j);
					writer.Write((short)0);
					writer.WriteStruct(model_0_3s[i][j]);
				}
			}
			for (byte i = 0; i < model_4_7s.Length; i++)
			{
				for (byte j = 0; j < model_4_7s[i].Length; j++)
				{
					writer.Write(0x29);
					writer.Write(i);
					writer.Write(j);
					writer.Write((short)0);
					writer.WriteStruct(model_4_7s[i][j]);
				}
			}
			for (byte i = 0; i < unk_43.Length; i++)
			{
				for (byte j = 0; j < unk_43[i].Length; j++)
				{
					writer.Write(0x43);
					writer.Write(i);
					writer.Write(j);
					writer.Write((short)0);
					writer.Write(unk_43[i][j]);
				}
			}
			for (int i = 0; i < unk_38.Length; i++)
			{
				if (unk_38[i] == null)
				{
					continue;
				}
				writer.Write(0x38);
				writer.Write(i);
				writer.Write(unk_38[i]);
			}
			int[] pos_39 = Array.Empty<int>();
			for (int i = 0; i < unk_39.Length; i++)
			{
				writer.Write(0x39);
				writer.Write(i);
				Array.Resize(ref pos_39, pos_39.Length + 1);
				pos_39[^1] = writer.offset;
				writer.offset += sizeof(int);
			}
			int[] pos_8F = Array.Empty<int>();
			for (int i = 0; i < unk_8F.Length; i++)
			{
				writer.Write(0x8F);
				writer.Write(i);
				Array.Resize(ref pos_8F, pos_8F.Length + 1);
				pos_8F[^1] = writer.offset;
				writer.offset += sizeof(int);
			}
			writer.Write(0x03);
			int des_ptr = writer.offset;
			writer.offset += sizeof(int);
			writer.Write(0);
			int temp_pos;
			for (int i = 0; i < unk_39.Length; i++)
			{
				temp_pos = writer.offset;
				writer.offset = pos_39[i];
				writer.Write(temp_pos);
				writer.offset = temp_pos;
				for (int j = 0; j < unk_39[i].Length; j++)
				{
					writer.Write(unk_39[i][j]);
				}
				writer.Write((short)0);
			}
			for (int i = 0; i < unk_8F.Length; i++)
			{
				temp_pos = writer.offset;
				writer.offset = pos_8F[i];
				writer.Write(temp_pos);
				writer.offset = temp_pos;
				for (int j = 0; j < unk_8F[i].Length; j++)
				{
					writer.Write(unk_8F[i][j]);
				}
				writer.Write((short)0);
			}
			int des_start_ptr = writer.offset;
			for (int i = 0; i < descriptions.Length; i++)
			{
				writer.Write(descriptions[i]);
			}
			int ret_pos = writer.offset;
			writer.offset = job_des_ptr;
			writer.Write(des_start_ptr);
			writer.Write(ret_pos);
			writer.offset = des_ptr;
			writer.Write(ret_pos);
			writer.offset = ret_pos;
		}
	}
}
