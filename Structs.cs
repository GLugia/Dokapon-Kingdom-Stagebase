using System;
using System.Runtime.InteropServices;

namespace CharaReader
{
	static class Structs
	{
		public static Type[] structs =
		{
			/* 0x00 */ null,
			/* 0x01 */ typeof(FileStringLabel),
			/* 0x02 */ null,
			/* 0x03 */ typeof(Description),
			/* 0x04 */ null,
			/* 0x05 */ null,
			/* 0x06 */ null,
			/* 0x07 */ null,
			/* 0x08 */ null,
			/* 0x09 */ null,
			/* 0x0A */ null,
			/* 0x0B */ null,
			/* 0x0C */ null,
			/* 0x0D */ null,
			/* 0x0E */ null,
			/* 0x0F */ null,
			/* 0x10 */ null,
			/* 0x11 */ null,
			/* 0x12 */ null,
			/* 0x13 */ null,
			/* 0x14 */ null,
			/* 0x15 */ null,
			/* 0x16 */ null,
			/* 0x17 */ typeof(Unk_17),
			/* 0x18 */ null,
			/* 0x19 */ null,
			/* 0x1A */ null,
			/* 0x1B */ null,
			/* 0x1C */ typeof(Unk_1C),
			/* 0x1D */ typeof(Unk_1D),
			/* 0x1E */ typeof(Unk_4F),
			/* 0x1F */ null,
			/* 0x20 */ null,
			/* 0x21 */ null,
			/* 0x22 */ null,
			/* 0x23 */ null,
			/* 0x24 */ null,
			/* 0x25 */ null,
			/* 0x26 */ null,
			/* 0x27 */ null,
			/* 0x28 */ null,
			/* 0x29 */ typeof(JobModel_4_7),
			/* 0x2A */ typeof(Unk_2A),
			/* 0x2B */ null,
			/* 0x2C */ typeof(Unk_2C),
			/* 0x2D */ typeof(Unk_2D),
			/* 0x2E */ typeof(JobMoney),
			/* 0x2F */ null,
			/* 0x30 */ null,
			/* 0x31 */ null,
			/* 0x32 */ null,
			/* 0x33 */ null,
			/* 0x34 */ null,
			/* 0x35 */ null,
			/* 0x36 */ null,
			/* 0x37 */ null,
			/* 0x38 */ typeof(JobUnk_38),
			/* 0x39 */ typeof(JobUnk_39),
			/* 0x3A */ typeof(Unk_3A),
			/* 0x3B */ typeof(JobMastery),
			/* 0x3C */ typeof(JobSkills),
			/* 0x3D */ typeof(FileDescriptionPointer),
			/* 0x3E */ typeof(JobUnk_3E),
			/* 0x3F */ typeof(Unk_1D),
			/* 0x40 */ typeof(JobStats),
			/* 0x41 */ typeof(JobModel),
			/* 0x42 */ typeof(Job),
			/* 0x43 */ typeof(JobUnk_43),
			/* 0x44 */ typeof(JobBag),
			/* 0x45 */ typeof(JobModel_0_3),
			/* 0x46 */ typeof(Unk_46),
			/* 0x47 */ typeof(Unk_4F),
			/* 0x48 */ typeof(Unk_48),
			/* 0x49 */ typeof(Unk_49),
			/* 0x4A */ typeof(Unk_4A),
			/* 0x4B */ typeof(Unk_4B),
			/* 0x4C */ typeof(Unk_46),
			/* 0x4D */ typeof(Unk_4F),
			/* 0x4E */ typeof(Unk_4E),
			/* 0x4F */ typeof(Unk_4F),
			/* 0x50 */ typeof(Enemy),
			/* 0x51 */ typeof(EnemyModel),
			/* 0x52 */ typeof(Unk_52),
			/* 0x53 */ typeof(EnemyDropTable),
			/* 0x54 */ typeof(Unk_54),
			/* 0x55 */ typeof(Unk_55),
			/* 0x56 */ typeof(Unk_56),
			/* 0x57 */ typeof(Unk_57),
			/* 0x58 */ typeof(Weapon),
			/* 0x59 */ typeof(WeaponModel),
			/* 0x5A */ typeof(FileDescriptionPointer),
			/* 0x5B */ typeof(Unk_5B),
			/* 0x5C */ typeof(Unk_5C),
			/* 0x5D */ typeof(Unk_5D),
			/* 0x5E */ typeof(Shield),
			/* 0x5F */ typeof(ShieldModel),
			/* 0x60 */ typeof(FileDescriptionPointer),
			/* 0x61 */ typeof(EnemyModel_0),
			/* 0x62 */ typeof(Unk_62),
			/* 0x63 */ typeof(FileDescriptionPointer),
			/* 0x64 */ typeof(Accessory),
			/* 0x65 */ typeof(FileDescriptionPointer),
			/* 0x66 */ null,
			/* 0x67 */ null,
			/* 0x68 */ null,
			/* 0x69 */ typeof(Item),
			/* 0x6A */ typeof(FileDescriptionPointer),
			/* 0x6B */ typeof(ItemUnknown),
			/* 0x6C */ typeof(ItemGift),
			/* 0x6D */ null,
			/* 0x6E */ null,
			/* 0x6F */ null,
			/* 0x70 */ typeof(MagicSpell),
			/* 0x71 */ typeof(FileDescriptionPointer),
			/* 0x72 */ typeof(MagicSpell),
			/* 0x73 */ typeof(FileDescriptionPointer),
			/* 0x74 */ typeof(ItemMagic),
			/* 0x75 */ typeof(FileDescriptionPointer),
			/* 0x76 */ typeof(Unk_4F),
			/* 0x77 */ typeof(Unk_4F),
			/* 0x78 */ null,
			/* 0x79 */ typeof(Unk_79),
			/* 0x7A */ typeof(BattleAbility),
			/* 0x7B */ typeof(FieldAbility),
			/* 0x7C */ typeof(FileDescriptionPointer),
			/* 0x7D */ typeof(FileDescriptionPointer),
			/* 0x7E */ typeof(FileDescriptionPointer),
			/* 0x7F */ null,
			/* 0x80 */ null,
			/* 0x81 */ typeof(Status1),
			/* 0x82 */ typeof(Status2),
			/* 0x83 */ typeof(Status3),
			/* 0x84 */ typeof(Unk_84),
			/* 0x85 */ null,
			/* 0x86 */ null,
			/* 0x87 */ null,
			/* 0x88 */ null,
			/* 0x89 */ typeof(ItemType),
			/* 0x8A */ typeof(MagicBase),
			/* 0x8B */ typeof(MagicElement),
			/* 0x8C */ typeof(Unk_8C),
			/* 0x8D */ null,
			/* 0x8E */ typeof(Unk_8E),
			/* 0x8F */ typeof(JobUnk_39),
			/* 0x90 */ null,
			/* 0x91 */ null,
			/* 0x92 */ null,
			/* 0x93 */ null,
			/* 0x94 */ null,
			/* 0x95 */ typeof(FileDescriptionPointer),
			/* 0x96 */ typeof(Hairstyle),
			/* 0x97 */ typeof(HairstyleModel),
			/* 0x98 */ null,
			/* 0x99 */ typeof(Unk_99),
			/* 0x9A */ typeof(Unk_9A),
			/* 0x9B */ typeof(Unk_9B),
			/* 0x9C */ typeof(Unk_9D),
			/* 0x9D */ typeof(Unk_9D),
			/* 0x9E */ typeof(Unk_9E),
			/* 0x9F */ typeof(Unk_9F),
			/* 0xA0 */ typeof(Unk_A0),
			/* 0xA1 */ typeof(Unk_A1),
			/* 0xA2 */ typeof(Unk_A2),
			/* 0xA3 */ typeof(Unk_A3),
			/* 0xA4 */ typeof(Unk_A4),
			/* 0xA5 */ typeof(Unk_A1),
			/* 0xA6 */ null,
			/* 0xA7 */ null,
			/* 0xA8 */ null,
			/* 0xA9 */ null,
			/* 0xAA */ null,
			/* 0xAB */ null,
			/* 0xAC */ null,
			/* 0xAD */ typeof(Unk_AD),
			/* 0xAE */ typeof(Unk_2A),
			/* 0xAF */ typeof(Unk_2A),
			/* 0xB0 */ typeof(Unk_B0),
			/* 0xB1 */ typeof(Unk_B1),
			/* 0xB2 */ typeof(Unk_B2),
			/* 0xB3 */ typeof(Unk_B3),
			/* 0xB4 */ typeof(Unk_B3),
			/* 0xB5 */ typeof(Unk_B5),
			/* 0xB6 */ typeof(Unk_B6),
			/* 0xB7 */ typeof(StringFunction),
			/* 0xB8 */ typeof(StringFunction),
			/* 0xB9 */ typeof(StringFunction),
			/* 0xBA */ typeof(StringFunction),
			/* 0xBB */ typeof(StringFunction),
			/* 0xBC */ typeof(StringFunction),
			/* 0xBD */ typeof(Unk_BD),
			/* 0xBE */ typeof(Unk_BE),
			/* 0xBF */ typeof(Unk_55),
			/* 0xC0 */ typeof(Unk_4A),
			/* 0xC1 */ null,
			/* 0xC2 */ null,
			/* 0xC3 */ null,
			/* 0xC4 */ null,
			/* 0xC5 */ null,
			/* 0xC6 */ null,
			/* 0xC7 */ null,
			/* 0xC8 */ null,
			/* 0xC9 */ null,
			/* 0xCA */ null,
			/* 0xCB */ null,
			/* 0xCC */ null,
			/* 0xCD */ null,
			/* 0xCE */ null,
			/* 0xCF */ null,
			/* 0xD0 */ typeof(ItemFunction),
			/* 0xD1 */ typeof(Unk_D1),
			/* 0xD2 */ typeof(Unk_2C),
			/* 0xD3 */ typeof(Unk_2C),
			/* 0xD4 */ typeof(Unk_D4),
			/* 0xD5 */ typeof(Unk_D5),
			/* 0xD6 */ typeof(Unk_D6),
			/* 0xD7 */ typeof(ItemData),
			/* 0xD8 */ typeof(FileDescriptionPointer),
			/* 0xD9 */ typeof(DarklingSpell),
			/* 0xDA */ null,
			/* 0xDB */ null,
			/* 0xDC */ null,
			/* 0xDD */ null,
			/* 0xDE */ null,
			/* 0xDF */ typeof(Unk_5B),
			/* 0xE0 */ null,
			/* 0xE1 */ typeof(Unk_9D),
			/* 0xE2 */ typeof(Unk_E2),
			/* 0xE3 */ null,
			/* 0xE4 */ null,
			/* 0xE5 */ null,
			/* 0xE6 */ null,
			/* 0xE7 */ null,
			/* 0xE8 */ null,
			/* 0xE9 */ null,
			/* 0xEA */ null,
			/* 0xEB */ null,
			/* 0xEC */ null,
			/* 0xED */ null,
			/* 0xEE */ null,
			/* 0xEF */ null,
			/* 0xF0 */ null,
			/* 0xF1 */ null,
			/* 0xF2 */ null,
			/* 0xF3 */ null,
			/* 0xF4 */ null,
			/* 0xF5 */ null,
			/* 0xF6 */ null,
			/* 0xF7 */ null,
			/* 0xF8 */ null,
			/* 0xF9 */ null,
			/* 0xFA */ null,
			/* 0xFB */ null,
			/* 0xFC */ null,
			/* 0xFD */ null,
			/* 0xFE */ null,
			/* 0xFF */ null,
		};

		public struct FileStringLabel
		{
			public int table_id;
			public int file_pointer;
			public string label;
		}

		public struct Description
		{
			public int table_id;
			public int end_of_data;
			public byte[] data;
		}

		public struct Weapon
		{
			public int table_id;
			public byte id;
			public byte padding_00;
			public byte icon;
			public byte padding_01;
			public string name;
			public byte unk_00;
			public short class_bonus;
			public byte star_rating;
			public int price;
			public short att;
			public short def;
			public short mag;
			public short spd;
			public short hp;
			public byte description_table_id;
			public byte description_id;
		}

		public struct WeaponModel
		{
			public int table_id;
			public int id;
			public int unused_00;
			public int unused_01;
			public int unk_00;
			public int unused_02;
			public int unused_03;
			public int unk_01;
			public string model_name;
		}

		public struct Shield
		{
			public int table_id;
			public byte id;
			public byte padding_00;
			public byte icon;
			public byte padding_01;
			public string name;
			public byte unk_00;
			public short class_bonus;
			public byte star_rating;
			public int price;
			public short def;
			public short att;
			public short mag;
			public short spd;
			public short hp;
			public byte description_table_id;
			public byte description_id;
		}

		public struct ShieldModel
		{
			public int table_id;
			public byte id;
			public byte padding_00;
			public short padding_01;
			public int model_name;
			public int unk_03;
		}

		public struct Accessory
		{
			public int table_id;
			public byte id;
			public byte padding_00;
			public byte icon;
			public byte padding_01;
			public string name;
			public int unk_01;
			public int price;
			public short att;
			public short def;
			public short mag;
			public short spd;
			public short hp;
			public byte description_table_id;
			public byte description_id;
			public int unk_02;
		}

		public struct ItemType
		{
			public int table_id;
			public int id;
			public string name;
		}

		public struct Item
		{
			public int table_id;
			public byte id;
			public byte type;
			public byte icon;
			public byte unk_00;
			public string name;
			public int price;
			public byte unk_01;
			public byte unk_02;
			public short padding;
		}

		public struct ItemGift
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public string name;
			public int price;
			public int gift_value;
			public int unk_03;
		}

		public struct ItemData
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public byte unk_04;
			public byte unk_05;
			public byte unk_06;
		}

		public struct ItemFunction
		{
			public int table_id;
			public byte id;
			public byte function_id;
			public byte unk_00;
			public byte padding_00;
		}

		public struct ItemUnknown
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public struct FileDescriptionPointer
		{
			public int table_id;
			public int start;
			public int end;
		}

		public struct Hairstyle
		{
			public int table_id;
			public byte id;
			public byte non_job_id;
			public byte icon;
			public byte padding_00;
			public string name;
			public byte unk_00;
			public byte non_job_id_2;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public byte unk_04;
			public byte unk_05;
			public byte gallery_stars;
		}

		public struct HairstyleModel
		{
			public int table_id;
			public int id;
			public string model_name;
		}

		public struct Job
		{
			public int table_id;
			public byte id;
			public byte padding_00;
			public short padding_01;
			public string name;
		}

		public struct JobSkills
		{
			public int table_id;
			public byte id;
			public byte gender;
			public byte job_id;
			public byte level_2;
			public byte level_4;
			public byte field_skill_chance;
			public byte field_skill_max;
			public byte padding;
		}

		public struct JobStats
		{
			public int table_id;
			public byte id;
			public byte gender;
			public short att;
			public short def;
			public short mag;
			public short spd;
			public short hp;
		}

		public struct JobMastery
		{
			public int table_id;
			public byte id;
			public byte gender;
			public short level_up_att;
			public short level_up_def;
			public short level_up_mag;
			public short level_up_spd;
			public short level_up_hp;
			public short mastery_att;
			public short mastery_def;
			public short mastery_mag;
			public short mastery_spd;
			public short mastery_hp;
			public short padding;
		}

		public struct JobBag
		{
			public int table_id;
			public short id;
			public byte local_item_size;
			public byte field_magic_size;
		}

		public struct JobUnk_3E
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public struct JobMoney
		{
			public int table_id;
			public byte id;
			public byte gender;
			public byte bonus_salary;
			public byte job_id;
			public int starting_money;
			public short level_multiplier;
			public short bonus_multiplier;
		}

		public struct MagicBase
		{
			public int table_id;
			public int id;
			public string name;
		}

		public struct MagicElement
		{
			public int table_id;
			public int id;
			public string name;
		}

		public struct MagicSpell
		{
			public int table_id;
			public short id;
			public short unk_00;
			public string name;
			public int price;
			public short power;
			public byte magic_base;
			public byte magic_element;
			public int unk_01;
		}

		public struct ItemMagic
		{
			public int table_id;
			public short id;
			public short icon;
			public string name;
			public int price;
			public short power;
			public byte unk_00;
			public byte unk_01;
			public int unk_02;
		}

		public struct Unk_9F
		{
			public int table_id;
			public int unk_00;
		}

		public struct Unk_D1
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte padding;
			public int unk_02;
		}

		public struct Unk_D4
		{
			public int table_id;
			public int unk_00;
			public int file_pointer;
			public int unk_01;
			public int unk_02;
			public int unk_03;
			public int unk_04;
		}

		public struct Unk_D5
		{
			public int table_id;
			public int unk_00;
			public int file_pointer;
			public int unk_01;
			public int unk_02;
		}

		public struct DarklingSpell
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public short func_id;
			public string name;
		}

		public struct FieldAbility
		{
			public int table_id;
			public byte id;
			public byte func_id;
			public short unk_00;
			public string name;
		}

		public struct BattleAbility
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public string name;
		}

		public struct Unk_D6
		{
			public int table_id;
			public int id;
			public int unk_00;
			public int unk_01;
		}

		public struct JobModel
		{
			public int table_id;
			public byte unk_00;
			public byte unk_01;
			public byte gender_id;
			public byte padding;
			public string name;
			public int unk_02;
		}

		public struct JobModel_0_3
		{
			public int table_id;
			public byte id;
			public byte gender;
			public short padding;
			public string f0;
			public string k0;
			public string fg0;
			public string fg1;
			public string fg2;
			public string fg3;
		}

		public struct JobModel_4_7
		{
			public int table_id;
			public byte id;
			public byte gender;
			public short padding;
			public string fg4;
			public string fg5;
			public string fg6;
			public string fg7;
		}

		public struct JobUnk_43
		{
			public int table_id;
			public byte id;
			public byte gender;
			public short padding;
			public float unk_00;
		}

		public struct JobUnk_38
		{
			public int table_id;
			public byte id;
			public byte gender;
			public short padding;
			public byte unk_00;
			public byte unk_01;
			public short padding2;
		}

		public struct JobUnk_39
		{
			public int table_id;
			public int id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte padding;
		}

		public struct Status1
		{
			public int table_id;
			public short id;
			public short unk_00;
			public string name;
		}

		public struct Status2
		{
			public int table_id;
			public short id;
			public short unk_00;
			public byte unk_01;
			public byte unk_02;
			public short padding;
			public string name;
		}

		public struct Status3
		{
			public int table_id;
			public int id;
			public byte unk_00;
			public byte unk_01;
			public short padding;
			public string name;
		}

		public struct Unk_8E
		{
			public int table_id;
			public int id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
		}

		public struct Unk_9A
		{
			public int table_id;
			public short id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public short padding;
		}

		public struct Unk_9B
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte padding;
		}

		public struct Unk_9E
		{
			public int table_id;
			public int unk_00;
			public int unk_01;
			public int unk_02;
			public int unk_04;
			public int unk_05;
			public int unk_06;
			public int unk_07;
		}

		public struct Unk_8C
		{
			public int table_id;
			public int unk_00;
			public int unk_01;
			public int unk_02;
			public int unk_04;
			public int unk_05;
			public int unk_06;
			public int unk_07;
			public int unk_08;
			public int unk_09;
			public int unk_0A;
			public int unk_0B;
			public int unk_0C;
			public int unk_0D;
			public int unk_0E;
			public int unk_0F;
			public int unk_10;
			public int unk_11;
			public int unk_12;
			public int unk_13;
			public int unk_14;
			public int unk_15;
			public int unk_16;
			public int unk_17;
			public int unk_18;
			public int unk_19;
			public int unk_1A;
			public int unk_1B;
			public int unk_1C;
			public int unk_1D;
			public int unk_1E;
			public int unk_1F;
			public int unk_20;
			public int unk_21;
			public int unk_22;
			public int unk_23;
			public int unk_24;
			public int unk_25;
			public int unk_26;
			public int unk_27;
			public int unk_28;
			public int unk_29;
			public int unk_2A;
			public int unk_2B;
			public int unk_2C;
			public int unk_2D;
			public int unk_2E;
			public int unk_2F;
			public int unk_30;
			public int unk_31;
			public int unk_32;
			public int unk_33;
			public int unk_34;
			public int unk_35;
			public int unk_36;
			public int unk_37;
			public int unk_38;
			public int unk_39;
			public int unk_3A;
			public int unk_3B;
			public int unk_3C;
			public int unk_3D;
			public int unk_3E;
			public int unk_3F;
			public int unk_40;
			public int unk_41;
			public int unk_42;
			public int unk_43;
			public int unk_44;
			public int unk_45;
			public int unk_46;
			public int unk_47;
			public int unk_48;
			public int unk_49;
			public int unk_4A;
			public int unk_4B;
			public int unk_4C;
			public int unk_4D;
			public int unk_4E;
			public int unk_4F;
			public int unk_50;
			public int unk_51;
			public int unk_52;
			public int unk_53;
			public int unk_54;
			public int unk_55;
			public int unk_56;
			public int unk_57;
			public int unk_58;
			public int unk_59;
			public int unk_5A;
			public int unk_5B;
			public int unk_5C;
			public int unk_5D;
			public int unk_5E;
			public int unk_5F;
			public int unk_60;
			public int unk_61;
			public int unk_62;
			public int unk_63;
			public int unk_64;
			public int unk_65;
			public int unk_66;
			public int unk_67;
			public int unk_68;
			public int unk_69;
			public int unk_6A;
			public int unk_6B;
			public int unk_6C;
			public int unk_6D;
			public int unk_6E;
			public int unk_6F;
			public int unk_70;
			public int unk_71;
			public int unk_72;
			public int unk_73;
			public int unk_74;
			public int unk_75;
			public int unk_76;
			public int unk_77;
			public int unk_78;
			public int unk_79;
			public int unk_7A;
			public int unk_7B;
			public int unk_7C;
			public int unk_7D;
			public int unk_7E;
			public int unk_7F;
			public int unk_80;
			public int unk_81;
			public int unk_82;
			public int unk_83;
			public int unk_84;
			public int unk_85;
		}

		public struct Unk_3A
		{
			public int table_id;
			public int id;
			public int file_pointer;
		}

		public struct Unk_17
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public struct Enemy
		{
			public int table_id;
			public byte id;
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

		public struct EnemyDropTable
		{
			public int table_id;
			public byte id;
			public byte chance_for_a;
			public byte chance_for_b;
			public byte padding;
			public byte item_id_a;
			public byte table_id_a;
			public byte item_id_b;
			public byte table_id_b;
		}

		public struct EnemyModel
		{
			public int table_id;
			public short id;
			public byte unk_00;
			public byte unk_01;
			public float unk_02;
			public string name;
			public int unused;
		}

		public struct EnemyModel_0
		{
			public int table_id;
			public int id;
			public string f0;
			public string fg0;
			public Pointer[] file_pointers;
		}

		public struct Unk_5B
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public struct Unk_62
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public byte unk_04;
			public byte unk_05;
			public byte unk_06;
		}

		public struct Unk_52
		{
			public int table_id;
			public int id;
			public char[] data;
		}

		public struct Unk_54
		{
			public int table_id;
			public short unk_00;
			public short unk_01;
		}

		public struct Unk_55
		{
			public int table_id;
			public int id;
			public string name;
		}

		public struct Unk_56
		{
			public int table_id;
			public short id;
			public short unk_00;
			public short unk_01;
			public short padding;
			public string name;
		}

		public struct Unk_57
		{
			public int table_id;
			public int id;
			public string f0;
			public string k0;
			public string fg0;
			public Pointer[] data_00;
			public Pointer[] data_01;
		}

		public struct Unk_5D
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte padding;
			public string name;
		}

		public struct Unk_2A
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte padding;
			public string name;
		}

		public struct Unk_2C
		{
			public int table_id;
			public int unk_00;
		}

		public struct Unk_2D
		{
			public int table_id;
			public int id;
			public float unk_00;
			public float unk_01;
			public float unk_02;
			public float unk_03;
		}

		public struct Unk_9D
		{
			public int table_id;
			public int id;
			public float unk_00;
			public float unk_01;
		}

		public struct Unk_4F
		{
			public int table_id;
			public int value;
		}

		public struct Unk_4E
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public struct Unk_E2
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public byte unk_01;
			public byte padding;
		}

		public struct Unk_49
		{
			public int table_id;
			public int id;
			public int value;
		}

		public struct Unk_4A
		{
			public int table_id;
			public int start;
			public int end;
		}

		public struct Unk_4B
		{
			public int table_id;
			public int id;
			public int[] pointers;
		}

		public struct Unk_5C
		{
			public int table_id;
			public byte id;
			public byte unk_00;
			public short padding;
		}

		public struct Unk_84
		{
			public int table_id;
			public int id;
			public short unk_00;
			public short unk_01;
			public short unk_02;
			public short unk_03;
			public short unk_04;
			public short unk_05;
		}

		public struct Unk_1C
		{
			public int table_id;
			public int id;
			public short unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public struct Unk_1D
		{
			public int table_id;
			public int id;
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
		}

		public struct Unk_99
		{
			public int table_id;
			public short unk_00;
			public short unk_01;
		}

		public struct Unk_46
		{
			public int table_id;
			public short id;
			public short unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public byte padding;
		}

		public struct Unk_48
		{
			public int table_id;
			public int id;
			public int value;
		}

		public struct Unk_A0
		{
			public int table_id;
			public float value;
		}

		public struct Unk_A1
		{
			public int table_id;
			public float unk_00;
			public float unk_01;
			public float unk_02;
		}

		public struct Unk_A2
		{
			public int table_id;
			public float unk_00;
			public float unk_01;
		}

		public struct Unk_A3
		{
			public int table_id;
			public int value;
		}

		public struct Unk_A4
		{
			public int table_id;
			public short unk_00;
			public short unk_01;
		}

		public struct Unk_AD
		{
			public int table_id;
			public int[] pointers;
		}

		public struct Unk_B0
		{
			public int table_id;
			public int id;
			public float unk_00;
			public float unk_01;
			public float unk_02;
			public float unk_04;
		}

		public struct Unk_B1
		{
			public int table_id;
			public int id;
			public float value;
		}

		public struct Unk_B2
		{
			public int table_id;
			public int id;
			public float unk_00;
			public float unk_01;
		}

		public struct Unk_B3
		{
			public int table_id;
			public int id;
			public int unk_00;
			public int unk_01;
			public int unk_02;
		}

		public struct Unk_B5
		{
			public int table_id;
			public int id;
			public int unk_00;
			public int unk_01;
			public int unk_02;
			public int unk_03;
			public int unk_04;
			public int unk_05;
			public int unk_06;
		}

		public struct Unk_B6
		{
			public int table_id;
			public int id;
			public int unk_00;
			public int unk_01;
			public int unk_02;
			public int unk_03;
			public int unk_04;
			public int unk_05;
			public int unk_06;
			public int unk_07;
			public int unk_08;
			public int unk_09;
			public int unk_0a;
			public int unk_0b;
			public int unk_0c;
			public int unk_0d;
			public int unk_0e;
			public int unk_0f;
		}

		public struct StringFunction
		{
			public int table_id;
			public int id;
			public string func;
			public int unk_00;
		}

		public struct Unk_BD
		{
			public int table_id;
			public int id;
			public int unk_00;
			public int unk_01;
			public int unk_02;
		}

		public struct Unk_BE
		{
			public int table_id;
			public int id;
			public int value;
		}

		public struct Unk_79
		{
			public int table_id;
			public int unk_00;
			public int unk_01;
		}
	}

	public struct Pointer
	{
		public int id;
		public int value;

		public Pointer(int id, int value)
		{
			this.id = id;
			this.value = value;
		}
	}
}
