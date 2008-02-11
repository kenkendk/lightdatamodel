#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>Table</type>
/// <namespace>UnitTest</namespace>
/// <name>SpecificEvent</name>
/// <sql></sql>
/// </metadata>

namespace UnitTest
{

	public class SpecificEvent : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.String m_Data;
		private System.Int64 m_RegistrationID;
		private System.DateTime m_Time;
		private System.Int64 m_EventType;
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

		public System.String Data
		{
			get{return m_Data;}
			set{object oldvalue = m_Data;OnBeforeDataWrite(this, "Data", oldvalue, value);m_Data = value;OnAfterDataWrite(this, "Data", oldvalue, value);}
		}

		public System.Int64 RegistrationID
		{
			get{return m_RegistrationID;}
			set{object oldvalue = m_RegistrationID;OnBeforeDataWrite(this, "RegistrationID", oldvalue, value);m_RegistrationID = value;OnAfterDataWrite(this, "RegistrationID", oldvalue, value);}
		}

		public System.DateTime Time
		{
			get{return m_Time;}
			set{object oldvalue = m_Time;OnBeforeDataWrite(this, "Time", oldvalue, value);m_Time = value;OnAfterDataWrite(this, "Time", oldvalue, value);}
		}

		public System.Int64 EventType
		{
			get{return m_EventType;}
			set{object oldvalue = m_EventType;OnBeforeDataWrite(this, "EventType", oldvalue, value);m_EventType = value;OnAfterDataWrite(this, "EventType", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

#endregion

	}
#region " typed collection "

		public class SpecificEventCollection : System.Data.LightDatamodel.SyncCollectionBase, System.Collections.ICollection, System.Collections.IEnumerable		{

			public SpecificEventCollection() { }
			public SpecificEventCollection(object owner, string reversePropertyname, string reversePropertyID)
				: base(typeof(SpecificEvent), owner, reversePropertyname, reversePropertyID)
			{
			}

			public virtual int Add(SpecificEvent item)
			{
				int i = m_baseList.Add(item);
				HookItem(item);
				return i;
			}

			public virtual void AddRange(System.Collections.ICollection items)
			{
				foreach(object o in items)
					if (o != null && o as SpecificEvent == null)
						throw new System.Exception("Only objects of type '" + typeof(SpecificEvent).FullName + "' may be inserted");

				foreach(object o in items)
					HookItem((SpecificEvent)o);

				m_baseList.AddRange(items);
			}

			public virtual bool Contains(SpecificEvent item)
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

			public virtual SpecificEventCollection GetRange(int index, int count)
			{
				SpecificEventCollection c = new SpecificEventCollection();
				c.AddRange(m_baseList.GetRange(index, count));
				return c;
			}


			public virtual int IndexOf(SpecificEvent item)
			{
				return m_baseList.IndexOf(item);
			}

			public virtual int IndexOf(SpecificEvent item, int startIndex)
			{
				return m_baseList.IndexOf(item, startIndex);
			}
			
			public virtual int IndexOf(SpecificEvent item, int startIndex, int count)
			{
				return m_baseList.IndexOf(item, startIndex, count);
			}
			
			public virtual void Insert(int index, SpecificEvent item)
			{
				m_baseList.Insert(index, item);
				HookItem(item);
			}

			public virtual void InsertRange(int index, System.Collections.ICollection items)
			{
				foreach(object o in items)
					if (o != null && o as SpecificEvent == null)
						throw new System.Exception("Only objects of type '" + typeof(SpecificEvent).FullName + "' may be inserted");
				foreach(object o in items)
					HookItem((SpecificEvent)o);

				m_baseList.InsertRange(index, items);
			}

			public virtual bool IsFixedSize { get { return m_baseList.IsFixedSize; } }
			public virtual bool IsReadOnly { get { return m_baseList.IsReadOnly; } }
			public virtual bool IsSynchronized { get { return m_baseList.IsSynchronized; } }

			public virtual int LastIndexOf(SpecificEvent item)
			{
				return m_baseList.LastIndexOf(item);
			}

			public virtual int LastIndexOf(SpecificEvent item, int startIndex)
			{
				return m_baseList.LastIndexOf(item, startIndex);
			}
			
			public virtual int LastIndexOf(SpecificEvent item, int startIndex, int count)
			{
				return m_baseList.LastIndexOf(item, startIndex, count);
			}

			public virtual void Remove(SpecificEvent item)
			{
				if (m_baseList.Contains(item))
				{
					UnhookItem(item);
					m_baseList.Remove(item);
				}
			}

			public virtual void RemoveAt(int index)
			{
				UnhookItem((SpecificEvent)m_baseList[index]);
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
					if (o != null && o as SpecificEvent == null)
						throw new System.Exception("Only objects of type '" + typeof(SpecificEvent).FullName + "' may be inserted");

				for(int i = 0; i < System.Math.Min(items.Count, m_baseList.Count - index); i++)
					UnhookItem((SpecificEvent)m_baseList[i + index]);

				foreach(object o in items)
					HookItem((SpecificEvent)o);
				
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

			public virtual SpecificEvent[] ToArray()
			{
				return (SpecificEvent[])m_baseList.ToArray(typeof(SpecificEvent));
			}

			public virtual void TrimToSize()
			{
				m_baseList.TrimToSize();
			}

			public virtual SpecificEvent this[int index]
			{
				get { return (SpecificEvent)m_baseList[index]; }
				set 
				{ 
					UnhookItem((SpecificEvent)m_baseList[index]);
					HookItem(value);
					m_baseList[index] = value; 
				}
			}
		}
#endregion


}