using System;
using System.Collections.Generic;

namespace CharaReader.data.ptrs
{
	public abstract class Ptr_Custom<T>
	{
		protected T data;

		public Ptr_Custom(IntPtr origin, ref long offset) { }

		public abstract Dictionary<IntPtr, byte[]> Handle();

		public abstract override bool Equals(object obj);

		public abstract override int GetHashCode();

		public static explicit operator T(Ptr_Custom<T> ptr)
		{
			return ptr.data;
		}
	}
}
