#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>Table</type>
/// <namespace>UnitTest</namespace>
/// <name>ManyToMany</name>
/// <sql></sql>
/// </metadata>

namespace UnitTest
{

	public class ManyToMany : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.Int64 m_LeftID;
		private System.Int64 m_RightID;
#endregion

#region " unique value "

		public override object UniqueValue {get{return ID;}}
		public override string UniqueColumn {get{return "ID";}}
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataWrite(this, "ID", oldvalue, value);m_ID = value;OnAfterDataWrite(this, "ID", oldvalue, value);}
		}

		public System.Int64 LeftID
		{
			get{return m_LeftID;}
			set{object oldvalue = m_LeftID;OnBeforeDataWrite(this, "LeftID", oldvalue, value);m_LeftID = value;OnAfterDataWrite(this, "LeftID", oldvalue, value);}
		}

		public System.Int64 RightID
		{
			get{return m_RightID;}
			set{object oldvalue = m_RightID;OnBeforeDataWrite(this, "RightID", oldvalue, value);m_RightID = value;OnAfterDataWrite(this, "RightID", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

#endregion

	}
#region " typed collection "

		public class ManyToManyCollection : System.Data.LightDatamodel.SyncCollectionBase, System.Collections.ICollection, System.Collections.IEnumerable		{

			public ManyToManyCollection() { }
			public ManyToManyCollection(object owner, string reversePropertyname, string reversePropertyID)
				: base(typeof(ManyToMany), owner, reversePropertyname, reversePropertyID)
			{
			}

			public virtual int Add(ManyToMany item)
			{
				int i = m_baseList.Add(item);
				HookItem(item);
				return i;
			}

			public virtual void AddRange(System.Collections.ICollection items)
			{
				foreach(object o in items)
					if (o != null && o as ManyToMany == null)
						throw new System.Exception("Only objects of type '" + typeof(ManyToMany).FullName + "' may be inserted");

				foreach(object o in items)
					HookItem((ManyToMany)o);

				m_baseList.AddRange(items);
			}

			public virtual bool Contains(ManyToMany item)
			{
				return m_baseList.Contains(item);
			}

			public virtual void CopyTo(System.Array destination)
			{
				m_baseList.CopyTo(destination);
			}

			public virtual void CopyTo(System.Array destination, int index)
			{
				m_baseList.CopyTo(destination, index);
			}

			public virtual void CopyTo(int sourceindex, System.Array destination, int destinationindex, int count)
			{
				m_baseList.CopyTo(sourceindex, destination, destinationindex, count);
			}

			public virtual int Count { get { return m_baseList.Count; } }

			public virtual System.Collections.IEnumerator GetEnumerator()
			{
				return m_baseList.GetEnumerator();
			}

			public virtual System.Collections.IEnumerator GetEnumerator(int index, int count)
			{
				return m_baseList.GetEnumerator(index, count);
			}

			public virtual ManyToManyCollection GetRange(int index, int count)
			{
				ManyToManyCollection c = new ManyToManyCollection();
				c.AddRange(m_baseList.GetRange(index, count));
				return c;
			}


			public virtual int IndexOf(ManyToMany item)
			{
				return m_baseList.IndexOf(item);
			}

			public virtual int IndexOf(ManyToMany item, int startIndex)
			{
				return m_baseList.IndexOf(item, startIndex);
			}
			
			public virtual int IndexOf(ManyToMany item, int startIndex, int count)
			{
				return m_baseList.IndexOf(item, startIndex, count);
			}
			
			public virtual void Insert(int index, ManyToMany item)
			{
				m_baseList.Insert(index, item);
				HookItem(item);
			}

			public virtual void InsertRange(int index, System.Collections.ICollection items)
			{
				foreach(object o in items)
					if (o != null && o as ManyToMany == null)
						throw new System.Exception("Only objects of type '" + typeof(ManyToMany).FullName + "' may be inserted");
				foreach(object o in items)
					HookItem((ManyToMany)o);

				m_baseList.InsertRange(index, items);
			}

			public virtual bool IsFixedSize { get { return m_baseList.IsFixedSize; } }
			public virtual bool IsReadOnly { get { return m_baseList.IsReadOnly; } }
			public virtual bool IsSynchronized { get { return m_baseList.IsSynchronized; } }

			public virtual int LastIndexOf(ManyToMany item)
			{
				return m_baseList.LastIndexOf(item);
			}

			public virtual int LastIndexOf(ManyToMany item, int startIndex)
			{
				return m_baseList.LastIndexOf(item, startIndex);
			}
			
			public virtual int LastIndexOf(ManyToMany item, int startIndex, int count)
			{
				return m_baseList.LastIndexOf(item, startIndex, count);
			}

			public virtual void Remove(ManyToMany item)
			{
				if (m_baseList.Contains(item))
				{
					UnhookItem(item);
					m_baseList.Remove(item);
				}
			}

			public virtual void RemoveAt(int index)
			{
				UnhookItem((ManyToMany)m_baseList[index]);
				m_baseList.RemoveAt(index);
			}

			public virtual void RemoveRange(int index, int count)
			{
				for(int i = 0; i < count; i++)
					RemoveAt(index);					
			}

			public virtual void Reverse(int index, int count)
			{
				m_baseList.Reverse(index, count);
			}

			public virtual void Reverse()
			{
				m_baseList.Reverse();
			}

			public virtual void SetRange(int index, System.Collections.ICollection items)
			{
				foreach(object o in items)
					if (o != null && o as ManyToMany == null)
						throw new System.Exception("Only objects of type '" + typeof(ManyToMany).FullName + "' may be inserted");

				for(int i = 0; i < System.Math.Min(items.Count, m_baseList.Count - index); i++)
					UnhookItem((ManyToMany)m_baseList[i + index]);

				foreach(object o in items)
					HookItem((ManyToMany)o);
				
				m_baseList.SetRange(index, items);
			}

			public virtual void Sort(int index, int count, System.Collections.IComparer comparer)
			{
				m_baseList.Sort(index, count, comparer);
			}

			public virtual void Sort(System.Collections.IComparer comparer)
			{
				m_baseList.Sort(comparer);
			}

			public virtual void Sort()
			{
				m_baseList.Sort();
			}

			public virtual object SyncRoot { get { return m_baseList.SyncRoot; } }

			public virtual ManyToMany[] ToArray()
			{
				return (ManyToMany[])m_baseList.ToArray(typeof(ManyToMany));
			}

			public virtual void TrimToSize()
			{
				m_baseList.TrimToSize();
			}

			public virtual ManyToMany this[int index]
			{
				get { return (ManyToMany)m_baseList[index]; }
				set 
				{ 
					UnhookItem((ManyToMany)m_baseList[index]);
					HookItem(value);
					m_baseList[index] = value; 
				}
			}
		}
#endregion


}