using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharaReader.data
{
	public abstract class List_Base<T>
	{
		private List<T> list;
		public int Count => list.Count;

		public T this[int index]
		{
			get => list[index];
			set => list[index] = value;
		}

		public List_Base()
		{
			list = new();
		}

		public void Add(T obj)
		{
			list.Add(obj);
		}

		public bool TryAdd(T obj)
		{
			if (list.Contains(obj))
			{
				return false;
			}
			list.Add(obj);
			return true;
		}

		public void AddRange(IEnumerable<T> enumerable)
		{
			foreach (T obj in enumerable)
			{
				Add(obj);
			}
		}

		public bool Insert(T obj, int index)
		{
			if (index >= 0 && index < list.Count)
			{
				list.Insert(index, obj);
				return true;
			}
			return false;
		}

		public bool Remove(T obj)
		{
			return list.Remove(obj);
		}

		public bool RemoveAt(int index)
		{
			if (index >= 0 && index < list.Count)
			{
				list.RemoveAt(index);
				return true;
			}
			return false;
		}

		public T ElementAt(int index)
		{
			if (index >= 0 && index < list.Count)
			{
				return list.ElementAt(index);
			}
			return default;
		}

		public T[] ToArray() => list.ToArray();

		public void Clear()
		{
			list.Clear();
		}

		public void Destroy()
		{
			list.Clear();
			list = null;
		}

		public sealed override string ToString()
		{
			return $"{list.ToArray().ConvertToString()}";
		}

		public sealed override int GetHashCode()
		{
			return HashCode.Combine(list, list.GetHashCode());
		}
	}
}
