using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data.chr_data.structs
{
	public struct NPCEnemy
	{
		public byte unk_00;
		public short level;
		public string name;
		public short hp;
		public short att;
		public short def;
		public short spd;
		public short mag;
		public short padding_00;
		public byte offensive_magic;
		public byte defensive_magic;
		public byte battle_skill;
		public byte padding_01;
		public short exp;
		public short gold;
	}
}
