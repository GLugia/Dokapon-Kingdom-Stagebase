using CharaReader.data.chr_data.structs;
using System;

namespace CharaReader.data.chr_data
{
	public class BuffData : BaseData
	{
		public StatusPermanent[] status_permanent;
		public StatusBattle[] status_battle;
		public StatusField[] status_field;
		public StatusUnk_8E[] unk_8E;
		public StatusUnk_9A[][] unk_9A;
		public StatusUnk_9B[] unk_9B;

		public BuffData(DataReader reader) : base(reader)
		{
			int table_id;
			bool finished = false;
			while (!finished && reader.offset < reader.length)
			{
				table_id = reader.ReadInt32();
				switch (table_id)
				{
					case 0x81: status_permanent = reader.ReadStructs<StatusPermanent>(table_id); break;
					case 0x82: status_battle = reader.ReadStructs<StatusBattle>(table_id); break;
					case 0x83: status_field = reader.ReadStructs<StatusField>(table_id); break;
					case 0x8E: unk_8E = reader.ReadStructs<StatusUnk_8E>(table_id); break;
					case 0x9A: unk_9A = reader.ReadStructs2<StatusUnk_9A>(table_id); break;
					case 0x9B: unk_9B = reader.ReadStructs<StatusUnk_9B>(table_id); break;
					case 0x9E:
						{
							finished = true;
							reader.offset -= sizeof(int);
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
			writer.WriteStructs(0x81, status_permanent);
			writer.WriteStructs(0x82, status_battle);
			writer.WriteStructs(0x83, status_field);
			writer.WriteStructs(0x8E, unk_8E);
			writer.WriteStructs(0x9A, unk_9A);
			writer.WriteStructs(0x9B, unk_9B);
		}
	}
}
