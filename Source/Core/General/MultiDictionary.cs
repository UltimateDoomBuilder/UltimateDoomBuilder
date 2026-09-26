using System.Collections.Generic;

namespace CodeImp.DoomBuilder
{
	public class MultiDictionary<TKey, TValue>
	{
		protected readonly Dictionary<TKey, List<TValue>> dict = new Dictionary<TKey, List<TValue>>();

		public List<TValue> this[TKey key] => dict[key];
		public bool ContainsKey(TKey key) => dict.ContainsKey(key);

		public bool Add(TKey key, TValue value)
		{
			List<TValue> list;

			if (!dict.TryGetValue(key, out list))
			{
				list = new List<TValue>();
				dict[key] = list;
			}

			list.Add(value);

			return true;
		}

		public bool Remove(TKey key, TValue value)
		{
			List<TValue> list;

			if (!dict.TryGetValue(key, out list)) return false;
			if (!list.Remove(value)) return false;
			if (list.Count == 0)
				dict.Remove(key);
			return true;
		}

		public void Clear()
		{
			dict.Clear();
		}
	}
}
