using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data
{
	public class Ptr<T> where T : notnull
	{
		public T data;

		public Ptr()
		{
			data = default;
		}

		public static explicit operator T(Ptr<T> ptr)
		{
			return ptr.data;
		}

		public static implicit operator Ptr<T>(T val)
		{
			return new Ptr<T>() { data = val };
		}

		public override string ToString()
		{
			return data.ToString();
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(data);
		}

		public override bool Equals(object obj)
		{
			return data.Equals(obj);
		}
	}
}
