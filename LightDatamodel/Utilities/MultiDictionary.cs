#region Disclaimer / License
// Copyright (C) 2008, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Text;

namespace System.Collections.Generic
{
	[System.Diagnostics.DebuggerDisplay("Count = {Count}"), System.Runtime.InteropServices.ComVisible(false)]
	public sealed class MultiDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private Dictionary<TKey, LinkedList<TValue>> m_list = new Dictionary<TKey, LinkedList<TValue>>();
		private int m_count = 0;

		public void Add(TKey key, TValue value)
		{
			if (!m_list.ContainsKey(key)) m_list.Add(key, new LinkedList<TValue>());
			m_list[key].AddLast(new LinkedListNode<TValue>(value));
			m_count++;
		}

		public bool ContainsKey(TKey key)
		{
			return m_list.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			if (m_list.ContainsKey(key))
			{
				m_count -= m_list[key].Count;
				return m_list.Remove(key);
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			try
			{
				LinkedList<TValue> tmp = new LinkedList<TValue>();
				bool succed = m_list.TryGetValue(key, out tmp);
				if (succed) value = tmp.First.Value;
				else value = default(TValue);
				return succed;
			}
			catch
			{
				value = default(TValue);
				return false;
			}
		}

		public ICollection<TKey> Keys
		{
			get { return m_list.Keys; }
		}

		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get
			{
				return Values;
			}
		}

		public ValueCollection Values
		{
			get
			{
				return new ValueCollection(this);
			}
		}

		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>
		{
			private MultiDictionary<TKey, TValue> m_parent;

			public ValueCollection(MultiDictionary<TKey, TValue> parent)
			{
				m_parent = parent;
			}

			public ICollection<TValue> this[TKey key]
			{
				get
				{
					if (!m_parent.m_list.ContainsKey(key)) return null;
					return (ICollection<TValue>)m_parent.m_list[key];
				}
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return new Enumerator(m_parent.m_list);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
			{
				return GetEnumerator();
			}

			public int Count
			{
				get { return m_parent.Count; }
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			bool ICollection<TValue>.IsReadOnly
			{
				get { return true; }
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			public sealed class Enumerator : IEnumerator<TValue>, IDisposable
			{
				private Dictionary<TKey, LinkedList<TValue>> m_parent;
				private Dictionary<TKey, LinkedList<TValue>>.Enumerator m_keyenumerator;
				private LinkedList<TValue>.Enumerator m_valueenumerator;

				public Enumerator(Dictionary<TKey, LinkedList<TValue>> parent)
				{
					m_parent = parent;
					Reset();
				}

				public TValue Current
				{
					get
					{
						return m_valueenumerator.Current;
					}
				}

				public void Dispose()
				{
					m_parent = null;
					m_keyenumerator.Dispose();
					m_valueenumerator.Dispose();
					GC.SuppressFinalize(this);
				}

				object IEnumerator.Current
				{
					get
					{
						return Current;
					}
				}

				public bool MoveNext()
				{
					if (m_valueenumerator.Equals(default(LinkedList<TValue>.Enumerator)) || !m_valueenumerator.MoveNext())
					{
						do
						{
							if (m_keyenumerator.Equals(default(Dictionary<TKey, LinkedList<TValue>>.Enumerator)) || !m_keyenumerator.MoveNext()) return false;
							LinkedList<TValue> tmp = m_keyenumerator.Current.Value;
							m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<TValue>.Enumerator);
							if (!m_valueenumerator.Equals(default(LinkedList<TValue>.Enumerator))) m_valueenumerator.MoveNext();
						} while (m_valueenumerator.Equals(default(LinkedList<TValue>.Enumerator)) || m_valueenumerator.Current == null);
						return true;
					}
					else return true;
				}

				public void Reset()
				{
					if (m_parent != null)
					{
						m_keyenumerator = m_parent.GetEnumerator();
						LinkedList<TValue> tmp = m_keyenumerator.Current.Value;
						m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<TValue>.Enumerator);
					}
				}
			}

		}

		public TValue this[TKey key]
		{
			get
			{
				if (!m_list.ContainsKey(key)) return default(TValue);
				LinkedListNode<TValue> n = m_list[key].First;
				if (n == null) return default(TValue);
				return n.Value;
			}
			set
			{
				if (!m_list.ContainsKey(key)) m_list.Add(key, new LinkedList<TValue>());
				LinkedListNode<TValue> node = m_list[key].Find(value);
				if (node == null) Add(key, value);
				else node.Value = value;
			}
		}

		public ValueCollection Items
		{
			get
			{
				return new ValueCollection(this);
			}
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			m_count = 0;
			m_list.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if (m_list.ContainsKey(item.Key))
			{
				LinkedListNode<TValue> node = m_list[item.Key].Find(item.Value);
				return node != null;
			}
			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			foreach (KeyValuePair<TKey, TValue> item in this)
			{
				if (arrayIndex >= array.Length) break;
				array[arrayIndex++] = item;
			}
		}

		public int Count
		{
			get { return m_count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key, item.Value);
		}

		public bool Remove(TKey key, TValue value)
		{
			if (m_list.ContainsKey(key))
			{
				bool succed = m_list[key].Remove(value);
				if (succed) m_count--;
				if (m_list[key].Count == 0) m_list.Remove(key);
				return succed;
			}
			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new Enumerator(this.m_list);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this.m_list);
		}

		public override string ToString()
		{
			return "MultiDictionary (" + m_count.ToString() + ")";
		}

		public TValue[] ToArray()
		{
			TValue[] arr = new TValue[m_count];
			int c = 0;
			foreach (KeyValuePair<TKey, TValue> itm in this)
				arr[c++] = itm.Value;
			return arr;
		}

		public class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			private Dictionary<TKey, LinkedList<TValue>> m_parent;
			private Dictionary<TKey, LinkedList<TValue>>.Enumerator m_keyenumerator;
			private LinkedList<TValue>.Enumerator m_valueenumerator;

			public Enumerator(Dictionary<TKey, LinkedList<TValue>> parent)
			{
				m_parent = parent;
				Reset();
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return new KeyValuePair<TKey, TValue>(m_keyenumerator.Current.Key, m_valueenumerator.Current);
				}
			}

			public void Dispose()
			{
				m_parent = null;
				m_keyenumerator.Dispose();
				m_valueenumerator.Dispose();
				GC.SuppressFinalize(this);
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public bool MoveNext()
			{
				if (m_valueenumerator.Equals(default(LinkedList<TValue>.Enumerator)) || !m_valueenumerator.MoveNext())
				{
					do
					{
						if (!m_keyenumerator.MoveNext()) return false;
						LinkedList<TValue> tmp = m_keyenumerator.Current.Value;
						m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<TValue>.Enumerator);
						m_valueenumerator.MoveNext();
					} while (m_valueenumerator.Current == null);
					return true;
				}
				else return true;
			}

			public void Reset()
			{
				m_keyenumerator = m_parent.GetEnumerator();
				LinkedList<TValue> tmp = m_keyenumerator.Current.Value;
				m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<TValue>.Enumerator);
			}
		}
	}
}
