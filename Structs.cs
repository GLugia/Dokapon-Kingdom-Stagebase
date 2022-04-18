/* This class is unused but left here for reference
 * 
namespace CharaReader
{
	internal static class Structs
	{
		public class FileStringLabel
		{
			public int table_id;
			public int file_pointer;
			public string label;
		}

		public class Description
		{
			public int table_id;
			public int pointer_to_end_of_data;
			public byte[] data;
		}

		public class Weapon
		{
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

		public class WeaponModel
		{
			public int unused_00;
			public int unused_01;
			public int unk_00;
			public int unused_02;
			public int unused_03;
			public int unk_01;
			public string model_name;
		}

		public class Shield
		{
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

		public class ShieldModel
		{
			public byte padding_00;
			public short padding_01;
			public int model_name;
			public int unk_03;
		}

		public class Accessory
		{
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

		public class ItemType
		{
			public string name;
		}

		public class Item
		{
			public byte type;
			public byte icon;
			public byte unk_00;
			public string name;
			public int price;
			public byte unk_01;
			public byte unk_02;
			public short padding;
		}

		public class ItemGift
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public string name;
			public int price;
			public int gift_value;
			public int unk_03;
		}

		public class ItemData
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public byte unk_04;
			public byte unk_05;
			public byte unk_06;
		}

		public class ItemFunction
		{
			public byte function_id;
			public byte unk_00;
			public byte padding_00;
		}

		public class ItemUnknown
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public class FileDescriptionPointer
		{
			public int table_id;
			public int start;
			public int end;
		}

		public class Hairstyle
		{
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

		public class HairstyleModel
		{
			public string model_name;
		}

		public class Job
		{
			public byte padding_00;
			public short padding_01;
			public string name;
		}

		public class JobSkills
		{
			public byte gender;
			public byte job_id;
			public byte level_2;
			public byte level_4;
			public byte field_skill_chance;
			public byte field_skill_max;
			public byte padding;
		}

		public class JobStats
		{
			public byte gender;
			public short att;
			public short def;
			public short mag;
			public short spd;
			public short hp;
		}

		public class JobMastery
		{
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

		public class JobBag
		{
			public byte local_item_size;
			public byte field_magic_size;
		}

		public class JobUnk_3E
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public class JobMoney
		{
			public byte gender;
			public byte bonus_salary;
			public byte job_id;
			public int starting_money;
			public short level_multiplier;
			public short bonus_multiplier;
		}

		public class MagicBase
		{
			public string name;
		}

		public class MagicElement
		{
			public string name;
		}

		public class MagicSpell
		{
			public short unk_00;
			public string name;
			public int price;
			public short power;
			public byte magic_base;
			public byte magic_element;
			public int unk_01;
		}

		public class ItemMagic
		{
			public short icon;
			public string name;
			public int price;
			public short power;
			public byte unk_00;
			public byte unk_01;
			public int unk_02;
		}

		public class Unk_9F
		{
			public int table_id;
			public int unk_00;
		}

		public class Unk_D1
		{
			public byte unk_00;
			public byte unk_01;
			public byte padding;
			public int unk_02;
		}

		public class Unk_D4
		{
			public int table_id;
			public int unk_00;
			public int file_pointer;
			public int unk_01;
			public int unk_02;
			public int unk_03;
			public int unk_04;
		}

		public class Unk_D5
		{
			public int table_id;
			public int unk_00;
			public int file_pointer;
			public int unk_01;
			public int unk_02;
		}

		public class DarklingSpell
		{
			public byte unk_00;
			public short func_id;
			public string name;
		}

		public class FieldAbility
		{
			public byte func_id;
			public short unk_00;
			public string name;
		}

		public class BattleAbility
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public string name;
		}

		public class Unk_D6
		{
			public int unk_00;
			public int unk_01;
		}

		public class JobModel
		{
			public int table_id;
			public byte unk_00;
			public byte unk_01;
			public byte gender_id;
			public byte padding;
			public string name;
			public int unk_02;
		}

		public class JobModel_0_3
		{
			public byte gender;
			public short padding;
			public string f0;
			public string k0;
			public string fg0;
			public string fg1;
			public string fg2;
			public string fg3;
		}

		public class JobModel_4_7
		{
			public byte gender;
			public short padding;
			public string fg4;
			public string fg5;
			public string fg6;
			public string fg7;
		}

		public class JobUnk_43
		{
			public byte gender;
			public short padding;
			public float unk_00;
		}

		public class JobUnk_38
		{
			public byte gender;
			public short padding;
			public byte unk_00;
			public byte unk_01;
			public short padding2;
		}

		public class JobUnk_39
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte padding;
		}

		public class Status1
		{
			public short unk_00;
			public string name;
		}

		public class Status2
		{
			public short unk_00;
			public byte unk_01;
			public byte unk_02;
			public short padding;
			public string name;
		}

		public class Status3
		{
			public byte unk_00;
			public byte unk_01;
			public short padding;
			public string name;
		}

		public class Unk_8E
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
		}

		public class Unk_9A
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public short padding;
		}

		public class Unk_9B
		{
			public byte unk_00;
			public byte unk_01;
			public byte padding;
		}

		public class Unk_9E
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

		public class Unk_8C
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

		public class Unk_3A
		{
			public int file_pointer;
		}

		public class Unk_17
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public class Enemy
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

		public class EnemyDropTable
		{
			public byte chance_for_a;
			public byte chance_for_b;
			public byte padding;
			public byte item_id_a;
			public byte table_id_a;
			public byte item_id_b;
			public byte table_id_b;
		}

		public class EnemyModel
		{
			public byte unk_00;
			public byte unk_01;
			public float unk_02;
			public string name;
			public int unused;
		}

		public class EnemyModel_0
		{
			public string f0;
			public string fg0;
			public Pointer[] file_pointers;
		}

		public class Unk_5B
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public class Unk_62
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public byte unk_04;
			public byte unk_05;
			public byte unk_06;
		}

		public class Unk_52
		{
			public char[] data;
		}

		public class Unk_54
		{
			public int table_id;
			public short unk_00;
			public short unk_01;
		}

		public class Unk_55
		{
			public string name;
		}

		public class Unk_56
		{
			public short unk_00;
			public short unk_01;
			public short padding;
			public string name;
		}

		public class Unk_57
		{
			public string f0;
			public string k0;
			public string fg0;
			public Pointer[] data_00;
			public Pointer[] data_01;
		}

		public class Unk_5D
		{
			public byte unk_00;
			public byte unk_01;
			public byte padding;
			public string name;
		}

		public class Unk_2A
		{
			public byte unk_00;
			public byte unk_01;
			public byte padding;
			public string name;
		}

		public class Unk_2C
		{
			public int table_id;
			public int unk_00;
		}

		public class Unk_2D
		{
			public float unk_00;
			public float unk_01;
			public float unk_02;
			public float unk_03;
		}

		public class Unk_9D
		{
			public float unk_00;
			public float unk_01;
		}

		public class Unk_4F
		{
			public int table_id;
			public int value;
		}

		public class Unk_4E
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public class Unk_E2
		{
			public byte unk_00;
			public byte unk_01;
			public byte padding;
		}

		public class Unk_49
		{
			public int value;
		}

		public class Unk_4A
		{
			public int table_id;
			public int start;
			public int end;
		}

		public class Unk_4B
		{
			public int[] pointers;
		}

		public class Unk_5C
		{
			public byte unk_00;
			public short padding;
		}

		public class Unk_84
		{
			public short unk_00;
			public short unk_01;
			public short unk_02;
			public short unk_03;
			public short unk_04;
			public short unk_05;
		}

		public class Unk_1C
		{
			public short unk_00;
			public byte unk_01;
			public byte unk_02;
		}

		public class Unk_1D
		{
			public byte unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
		}

		public class Unk_99
		{
			public int table_id;
			public short unk_00;
			public short unk_01;
		}

		public class Unk_46
		{
			public short unk_00;
			public byte unk_01;
			public byte unk_02;
			public byte unk_03;
			public byte padding;
		}

		public class Unk_48
		{
			public int value;
		}

		public class Unk_A0
		{
			public int table_id;
			public float value;
		}

		public class Unk_A1
		{
			public int table_id;
			public float unk_00;
			public float unk_01;
			public float unk_02;
		}

		public class Unk_A2
		{
			public int table_id;
			public float unk_00;
			public float unk_01;
		}

		public class Unk_A3
		{
			public int table_id;
			public int value;
		}

		public class Unk_A4
		{
			public int table_id;
			public short unk_00;
			public short unk_01;
		}

		public class Unk_AD
		{
			public int table_id;
			public int[] pointers;
		}

		public class Unk_B0
		{
			public float unk_00;
			public float unk_01;
			public float unk_02;
			public float unk_04;
		}

		public class Unk_B1
		{
			public float value;
		}

		public class Unk_B2
		{
			public float unk_00;
			public float unk_01;
		}

		public class Unk_B3
		{
			public int unk_00;
			public int unk_01;
			public int unk_02;
		}

		public class Unk_B5
		{
			public int unk_00;
			public int unk_01;
			public int unk_02;
			public int unk_03;
			public int unk_04;
			public int unk_05;
			public int unk_06;
		}

		public class Unk_B6
		{
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

		public class StringFunction
		{
			public string func;
			public int unk_00;
		}

		public class Unk_BD
		{
			public int unk_00;
			public int unk_01;
			public int unk_02;
		}

		public class Unk_BE
		{
			public int value;
		}

		public class Unk_79
		{
			public int table_id;
			public int unk_00;
			public int unk_01;
		}

		public class StageFilePtr
		{
			public StageFile target;
		}

		public class StageFile
		{
			public string name;
		}
	}

	public class Pointer
	{
		public int id;
		public int value;

		public Pointer(int id, int value)
		{
			this.id = id;
			this.value = value;
		}
	}
}*/
