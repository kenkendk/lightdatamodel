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
	public class MultiDictionary<KEY, VALUE> : IDictionary<KEY, VALUE>
	{
		private Dictionary<KEY, LinkedList<VALUE>> m_list = new Dictionary<KEY, LinkedList<VALUE>>();
		private int m_count = 0;

		public void Add(KEY key, VALUE value)
		{
			if (!m_list.ContainsKey(key)) m_list.Add(key, new LinkedList<VALUE>());
			m_list[key].AddLast(new LinkedListNode<VALUE>(value));
			m_count++;
		}

		public bool ContainsKey(KEY key)
		{
			return m_list.ContainsKey(key);
		}

		public bool Remove(KEY key)
		{
			if (m_list.ContainsKey(key))
			{
				m_count -= m_list[key].Count;
				return m_list.Remove(key);
			}
			return false;
		}

		public bool TryGetValue(KEY key, out VALUE value)
		{
			try
			{
				LinkedList<VALUE> tmp = new LinkedList<VALUE>();
				bool succed = m_list.TryGetValue(key, out tmp);
				if (succed) value = tmp.First.Value;
				else value = default(VALUE);
				return succed;
			}
			catch
			{
				value = default(VALUE);
				return false;
			}
		}

		public ICollection<KEY> Keys
		{
			get { return m_list.Keys; }
		}

		ICollection<VALUE> IDictionary<KEY, VALUE>.Values
		{
			get
			{
				return Values;
			}
		}

		public ValueCollection<KEY, VALUE> Values
		{
			get
			{
				return new ValueCollection<KEY, VALUE>(this);
			}
		}

#pragma warning disable 693
		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		public sealed class ValueCollection<KEY, VALUE> : ICollection<VALUE>, IEnumerable<VALUE>
		{
			private MultiDictionary<KEY, VALUE> m_parent;

			public ValueCollection(MultiDictionary<KEY, VALUE> parent)
			{
				m_parent = parent;
			}

			public ICollection<VALUE> this[KEY key]
			{
				get
				{
					if (!m_parent.m_list.ContainsKey(key)) return null;
					return (ICollection<VALUE>)m_parent.m_list[key];
				}
			}

			public IEnumerator<VALUE> GetEnumerator()
			{
				return new Enumerator<VALUE>(m_parent.m_list);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator<VALUE> IEnumerable<VALUE>.GetEnumerator()
			{
				return GetEnumerator();
			}

			public int Count
			{
				get { return m_parent.Count; }
			}

			void ICollection<VALUE>.Add(VALUE item)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			void ICollection<VALUE>.Clear()
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			bool ICollection<VALUE>.Contains(VALUE item)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			void ICollection<VALUE>.CopyTo(VALUE[] array, int arrayIndex)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			bool ICollection<VALUE>.IsReadOnly
			{
				get { return true; }
			}

			bool ICollection<VALUE>.Remove(VALUE item)
			{
				throw new NotSupportedException("This Dictionary is read-only");
			}

			public class Enumerator<VALUE> : IEnumerator<VALUE>
			{
				private Dictionary<KEY, LinkedList<VALUE>> m_parent;
				private Dictionary<KEY, LinkedList<VALUE>>.Enumerator m_keyenumerator;
				private LinkedList<VALUE>.Enumerator m_valueenumerator;

				public Enumerator(Dictionary<KEY, LinkedList<VALUE>> parent)
				{
					m_parent = parent;
					Reset();
				}

				public VALUE Current
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
					if (m_valueenumerator.Equals(default(LinkedList<VALUE>.Enumerator)) || !m_valueenumerator.MoveNext())
					{
						do
						{
							if (m_keyenumerator.Equals(default(Dictionary<KEY, LinkedList<VALUE>>.Enumerator)) || !m_keyenumerator.MoveNext()) return false;
							LinkedList<VALUE> tmp = m_keyenumerator.Current.Value;
							m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<VALUE>.Enumerator);
							if (!m_valueenumerator.Equals(default(LinkedList<VALUE>.Enumerator))) m_valueenumerator.MoveNext();
						} while (m_valueenumerator.Equals(default(LinkedList<VALUE>.Enumerator)) || m_valueenumerator.Current == null);
						return true;
					}
					else return true;
				}

				public void Reset()
				{
					if (m_parent != null)
					{
						m_keyenumerator = m_parent.GetEnumerator();
						LinkedList<VALUE> tmp = m_keyenumerator.Current.Value;
						m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<VALUE>.Enumerator);
					}
				}
			}

		}
#pragma warning restore 693

		public VALUE this[KEY key]
		{
			get
			{
				if (!m_list.ContainsKey(key)) return default(VALUE);
				LinkedListNode<VALUE> n = m_list[key].First;
				if (n == null) return default(VALUE);
				return n.Value;
			}
			set
			{
				if (!m_list.ContainsKey(key)) m_list.Add(key, new LinkedList<VALUE>());
				LinkedListNode<VALUE> node = m_list[key].Find(value);
				if (node == null) Add(key, value);
				else node.Value = value;
			}
		}

		public ValueCollection<KEY, VALUE> Items
		{
			get
			{
				return new ValueCollection<KEY, VALUE>(this);
			}
		}

		public void Add(KeyValuePair<KEY, VALUE> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			m_count = 0;
			m_list.Clear();
		}

		public bool Contains(KeyValuePair<KEY, VALUE> item)
		{
			if (m_list.ContainsKey(item.Key))
			{
				LinkedListNode<VALUE> node = m_list[item.Key].Find(item.Value);
				return node != null;
			}
			return false;
		}

		public void CopyTo(KeyValuePair<KEY, VALUE>[] array, int arrayIndex)
		{
			foreach (KeyValuePair<KEY, VALUE> item in this)
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

		public bool Remove(KeyValuePair<KEY, VALUE> item)
		{
			return Remove(item.Key, item.Value);
		}

		public bool Remove(KEY key, VALUE value)
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

		public IEnumerator<KeyValuePair<KEY, VALUE>> GetEnumerator()
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

		public VALUE[] ToArray()
		{
			VALUE[] arr = new VALUE[m_count];
			int c = 0;
			foreach (KeyValuePair<KEY, VALUE> itm in this)
				arr[c++] = itm.Value;
			return arr;
		}

		public class Enumerator : IEnumerator<KeyValuePair<KEY, VALUE>>
		{
			private Dictionary<KEY, LinkedList<VALUE>> m_parent;
			private Dictionary<KEY, LinkedList<VALUE>>.Enumerator m_keyenumerator;
			private LinkedList<VALUE>.Enumerator m_valueenumerator;

			public Enumerator(Dictionary<KEY, LinkedList<VALUE>> parent)
			{
				m_parent = parent;
				Reset();
			}

			public KeyValuePair<KEY, VALUE> Current
			{
				get
				{
					return new KeyValuePair<KEY, VALUE>(m_keyenumerator.Current.Key, m_valueenumerator.Current);
				}
			}

			public void Dispose()
			{
				m_parent = null;
				m_keyenumerator.Dispose();
				m_valueenumerator.Dispose();
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
				if (m_valueenumerator.Equals(default(LinkedList<VALUE>.Enumerator)) || !m_valueenumerator.MoveNext())
				{
					do
					{
						if (!m_keyenumerator.MoveNext()) return false;
						LinkedList<VALUE> tmp = m_keyenumerator.Current.Value;
						m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<VALUE>.Enumerator);
						m_valueenumerator.MoveNext();
					} while (m_valueenumerator.Current == null);
					return true;
				}
				else return true;
			}

			public void Reset()
			{
				m_keyenumerator = m_parent.GetEnumerator();
				LinkedList<VALUE> tmp = m_keyenumerator.Current.Value;
				m_valueenumerator = tmp != null ? tmp.GetEnumerator() : default(LinkedList<VALUE>.Enumerator);
			}
		}
	}
}
