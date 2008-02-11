#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>Table</type>
/// <namespace>UnitTest</namespace>
/// <name>Project</name>
/// <sql></sql>
/// </metadata>

namespace UnitTest
{

	public class Project : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.Int64 m_CurrentTaskNoteID;
		private System.Int64 m_ProjectNoteID;
		private System.String m_Title;
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

		public System.Int64 CurrentTaskNoteID
		{
			get{return m_CurrentTaskNoteID;}
			set{object oldvalue = m_CurrentTaskNoteID;OnBeforeDataWrite(this, "CurrentTaskNoteID", oldvalue, value);m_CurrentTaskNoteID = value;OnAfterDataWrite(this, "CurrentTaskNoteID", oldvalue, value);}
		}

		public System.Int64 ProjectNoteID
		{
			get{return m_ProjectNoteID;}
			set{object oldvalue = m_ProjectNoteID;OnBeforeDataWrite(this, "ProjectNoteID", oldvalue, value);m_ProjectNoteID = value;OnAfterDataWrite(this, "ProjectNoteID", oldvalue, value);}
		}

		public System.String Title
		{
			get{return m_Title;}
			set{object oldvalue = m_Title;OnBeforeDataWrite(this, "Title", oldvalue, value);m_Title = value;OnAfterDataWrite(this, "Title", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here
		public Note ProjectNote
		{
			get { return (Note)base.GetReferenceObject("ProjectNote", "m_ProjectNoteID"); }
			set { base.SetReferenceObject("ProjectNote", "m_ProjectNoteID", "ProjectNotes", value); }
		}

		public Note CurrentTaskNote
		{
			get { return (Note)base.GetReferenceObject("CurrentTaskNote", "m_CurrentTaskNoteID"); }
			set { base.SetReferenceObject("CurrentTaskNote", "m_CurrentTaskNoteID", "TaskNotes", value); }
		}

#endregion

	}
#region " typed collection "

		public class ProjectCollection : System.Data.LightDatamodel.SyncCollectionBase, System.Collections.ICollection, System.Collections.IEnumerable		{

			public ProjectCollection() { }
			public ProjectCollection(object owner, string reversePropertyname, string reversePropertyID)
				: base(typeof(Project), owner, reversePropertyname, reversePropertyID)
			{
			}

			public virtual int Add(Project item)
			{
				int i = m_baseList.Add(item);
				HookItem(item);
				return i;
			}

			public virtual void AddRange(System.Collections.ICollection items)
			{
				foreach(object o in items)
					if (o != null && o as Project == null)
						throw new System.Exception("Only objects of type '" + typeof(Project).FullName + "' may be inserted");

				foreach(object o in items)
					HookItem((Project)o);

				m_baseList.AddRange(items);
			}

			public virtual bool Contains(Project item)
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

			public virtual ProjectCollection GetRange(int index, int count)
			{
				ProjectCollection c = new ProjectCollection();
				c.AddRange(m_baseList.GetRange(index, count));
				return c;
			}


			public virtual int IndexOf(Project item)
			{
				return m_baseList.IndexOf(item);
			}

			public virtual int IndexOf(Project item, int startIndex)
			{
				return m_baseList.IndexOf(item, startIndex);
			}
			
			public virtual int IndexOf(Project item, int startIndex, int count)
			{
				return m_baseList.IndexOf(item, startIndex, count);
			}
			
			public virtual void Insert(int index, Project item)
			{
				m_baseList.Insert(index, item);
				HookItem(item);
			}

			public virtual void InsertRange(int index, System.Collections.ICollection items)
			{
				foreach(object o in items)
					if (o != null && o as Project == null)
						throw new System.Exception("Only objects of type '" + typeof(Project).FullName + "' may be inserted");
				foreach(object o in items)
					HookItem((Project)o);

				m_baseList.InsertRange(index, items);
			}

			public virtual bool IsFixedSize { get { return m_baseList.IsFixedSize; } }
			public virtual bool IsReadOnly { get { return m_baseList.IsReadOnly; } }
			public virtual bool IsSynchronized { get { return m_baseList.IsSynchronized; } }

			public virtual int LastIndexOf(Project item)
			{
				return m_baseList.LastIndexOf(item);
			}

			public virtual int LastIndexOf(Project item, int startIndex)
			{
				return m_baseList.LastIndexOf(item, startIndex);
			}
			
			public virtual int LastIndexOf(Project item, int startIndex, int count)
			{
				return m_baseList.LastIndexOf(item, startIndex, count);
			}

			public virtual void Remove(Project item)
			{
				if (m_baseList.Contains(item))
				{
					UnhookItem(item);
					m_baseList.Remove(item);
				}
			}

			public virtual void RemoveAt(int index)
			{
				UnhookItem((Project)m_baseList[index]);
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
					if (o != null && o as Project == null)
						throw new System.Exception("Only objects of type '" + typeof(Project).FullName + "' may be inserted");

				for(int i = 0; i < System.Math.Min(items.Count, m_baseList.Count - index); i++)
					UnhookItem((Project)m_baseList[i + index]);

				foreach(object o in items)
					HookItem((Project)o);
				
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

			public virtual Project[] ToArray()
			{
				return (Project[])m_baseList.ToArray(typeof(Project));
			}

			public virtual void TrimToSize()
			{
				m_baseList.TrimToSize();
			}

			public virtual Project this[int index]
			{
				get { return (Project)m_baseList[index]; }
				set 
				{ 
					UnhookItem((Project)m_baseList[index]);
					HookItem(value);
					m_baseList[index] = value; 
				}
			}
		}
#endregion


}