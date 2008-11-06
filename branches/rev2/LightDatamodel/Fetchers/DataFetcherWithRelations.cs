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
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel
{
	public class DataFetcherWithRelations : DataFetcherCached
	{
		/// <summary>
		/// This is a lookuptable for the relations
		/// </summary>
		private Dictionary<string, TypeConfiguration.ReferenceField> m_relations = new Dictionary<string, TypeConfiguration.ReferenceField>();

		/// <summary>
		/// This is a cache for 'loosely' related objects
		/// </summary>
		private Dictionary<IDataClass, Dictionary<string, ObjectConnection>> m_objectrelationcache = new Dictionary<IDataClass, Dictionary<string, ObjectConnection>>();

		public Dictionary<IDataClass, Dictionary<string, ObjectConnection>> ObjectRelationCache { get { return m_objectrelationcache; } }

		public class ObjectConnection
		{
			public TypeConfiguration.ReferenceField Relation;
			public List<IDataClass> SubObjects = new List<IDataClass>();
			public ObjectConnection(TypeConfiguration.ReferenceField relation)
			{
				Relation = relation;
			}
		}

		public DataFetcherWithRelations(IDataProvider provider)
			: base(provider)
		{
			m_mappings.TypesInitialized += new EventHandler(m_mappings_TypesInitialized);
		}

		private void m_mappings_TypesInitialized(object sender, EventArgs e)
		{
			foreach (TypeConfiguration.MappedClass mc in m_mappings)
				foreach (TypeConfiguration.ReferenceField rf in mc.ReferenceFields.Values)
					if (!m_relations.ContainsKey(rf.Name)) m_relations.Add(rf.Name, rf);
		}

		/// <summary>
		/// This will search through the database for the given related objects. Indexes are used if possible
		/// </summary>
		/// <param name="relationkey"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		protected virtual IDataClass GetRelatedObjectFromDatabase(string relationkey, IDataClass owner)
		{
			IDataClass cacheobject = null;
			if (owner.GetType() == m_relations[relationkey].Parent.Type)
			{
				if (m_relations[relationkey].ChildField.Index)
					cacheobject = (IDataClass)GetObjectByIndex(m_relations[relationkey].Child.Type, m_relations[relationkey].ChildField.Databasefield, m_relations[relationkey].ParentField.Field.GetValue(owner));
				else
					cacheobject = (IDataClass)GetObject(m_relations[relationkey].Child.Type, m_relations[relationkey].ChildField.Databasefield + " = ?", m_relations[relationkey].ParentField.Field.GetValue(owner));
			}
			else
			{
				if (m_relations[relationkey].ParentField.Index)
					cacheobject = (IDataClass)GetObjectByIndex(m_relations[relationkey].Parent.Type, m_relations[relationkey].ParentField.Databasefield, m_relations[relationkey].ChildField.Field.GetValue(owner));
				else
					cacheobject = (IDataClass)GetObject(m_relations[relationkey].Parent.Type, m_relations[relationkey].ParentField.Databasefield + " = ?", m_relations[relationkey].ChildField.Field.GetValue(owner));
			}
			return cacheobject;
		}

		/// <summary>
		/// This will search through the database for the given related objects. Indexes are used if possible
		/// </summary>
		/// <param name="relationkey"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		protected virtual IDataClass[] GetRelatedObjectsFromDatabase(string relationkey, IDataClass owner)
		{
			IDataClass[] cacheobjects = null;
			if (owner.GetType() == m_relations[relationkey].Parent.Type)
			{
				if (m_relations[relationkey].ChildField.Index)
					cacheobjects = (IDataClass[])GetObjectsByIndex(m_relations[relationkey].Child.Type, m_relations[relationkey].ChildField.Databasefield, m_relations[relationkey].ParentField.Field.GetValue(owner));
				else
					cacheobjects = (IDataClass[])GetObjects(m_relations[relationkey].Child.Type, m_relations[relationkey].ChildField.Databasefield + " = ?", m_relations[relationkey].ParentField.Field.GetValue(owner));
			}
			else
			{
				cacheobjects = (IDataClass[])Array.CreateInstance(m_relations[relationkey].Parent.Type, 1);
				cacheobjects[0] = GetRelatedObjectFromDatabase(relationkey, owner);
				if (cacheobjects[0] == null) cacheobjects = null;
			}
			return cacheobjects;
		}

		/// <summary>
		/// This will return the objects with explicit relations (DB not searched)
		/// </summary>
		/// <param name="relationkey"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		protected virtual IDataClass GetRelatedObjectFromCesspool(string relationkey, IDataClass owner)
		{
			if (m_objectrelationcache[owner][relationkey].SubObjects.Count > 0) return m_objectrelationcache[owner][relationkey].SubObjects[0];
			return null;
		}

		/// <summary>
		/// This will return the objects with explicit relations (DB not searched)
		/// </summary>
		/// <param name="relationkey"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		protected virtual IDataClass[] GetRelatedObjectsFromCesspool(string relationkey, IDataClass owner)
		{
			IDataClass[] ret = null;
			if (owner.GetType() == m_relations[relationkey].Parent.Type)
				ret = (IDataClass[])Array.CreateInstance(m_relations[relationkey].Child.Type, m_objectrelationcache[owner][relationkey].SubObjects.Count);
			else
				ret = (IDataClass[])Array.CreateInstance(m_relations[relationkey].Parent.Type, m_objectrelationcache[owner][relationkey].SubObjects.Count);
			m_objectrelationcache[owner][relationkey].SubObjects.CopyTo(ret, 0);
			return ret;
		}

		public virtual IList<DATACLASS> GetRelatedObjects<DATACLASS>(string relationkey, IDataClass owner) where DATACLASS : IDataClass
		{
			//fetch from cache
			DATACLASS[] cacheobjects = (DATACLASS[])(Array)GetRelatedObjectsFromDatabase(relationkey, owner);
			DATACLASS[] cesspitobjects = (DATACLASS[])(Array)GetRelatedObjectsFromCesspool(relationkey, owner);
			return (IList<DATACLASS>)new RelatedObjectCollection<DATACLASS>(owner, relationkey, cacheobjects, cesspitobjects, this);
		}

		public IList<IDataClass> GetRelatedObjects(string relationkey, IDataClass owner)
		{
			return GetRelatedObjects<IDataClass>(relationkey, owner);
		}

		public virtual IDataClass GetRelatedObject(string relationkey, IDataClass owner)
		{
			IDataClass ret = GetRelatedObjectFromCesspool(relationkey, owner);
			if (ret != null) return ret;
			return GetRelatedObjectFromDatabase(relationkey, owner);
		}

		public DATACLASS GetRelatedObject<DATACLASS>(string relationkey, IDataClass owner)
		{
			return (DATACLASS)GetRelatedObject(relationkey, owner);
		}

		public void SetRelatedObject(string relationkey, IDataClass owner, IDataClass target)
		{
			IDataClass oldtarget = GetRelatedObject(relationkey, owner);
			if (object.Equals(target, oldtarget)) return;

			if (target == null)
			{
				//clear related objects
				if (oldtarget != null) m_objectrelationcache[oldtarget][relationkey].SubObjects.Clear();
				m_objectrelationcache[owner][relationkey].SubObjects.Clear();

				//change value from child
				if (owner.GetType() == m_relations[relationkey].Child.Type)
					m_relations[relationkey].ChildField.SetValueWithEvents(owner as DataClassBase, m_relations[relationkey].ChildField.GetDefaultValue(m_provider));
			}
			else
			{
				//only 1
				List<IDataClass> c = new List<IDataClass>();
				c.Add(target);
				m_objectrelationcache[owner][relationkey].SubObjects = c;

				//also set the counterpart
				List<IDataClass> p = new List<IDataClass>();
				p.Add(owner);
				m_objectrelationcache[target][relationkey].SubObjects = p;

			}

			//change value from child
			if (oldtarget != null && oldtarget.GetType() == m_relations[relationkey].Child.Type)
				m_relations[relationkey].ChildField.SetValueWithEvents(oldtarget as DataClassBase, m_relations[relationkey].ChildField.GetDefaultValue(m_provider));
		}

		public void AddRelatedObject(string relationkey, IDataClass owner, IDataClass target)
		{
			m_objectrelationcache[owner][relationkey].SubObjects.Add(target);
			m_objectrelationcache[target][relationkey].SubObjects.Add(owner);
		}


		#region " ProxyCollection "

		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		public class RelatedObjectCollection<DATACLASS> : IList<DATACLASS> where DATACLASS : IDataClass
		{
			private DataFetcherWithRelations m_fetcher;
			private IDataClass m_owner;
			private string m_relationkey;
			private IList<DATACLASS> m_combinedlist;

			public RelatedObjectCollection(IDataClass obj, string relationkey, DATACLASS[] objectsfromcache, DATACLASS[] objectsfromcesspit, DataFetcherWithRelations parent)
			{
				m_relationkey = relationkey;
				m_owner = obj;
				m_fetcher = parent;
				m_combinedlist = new List<DATACLASS>(objectsfromcache == null ? new DATACLASS[] { } : objectsfromcache);
				foreach (DATACLASS i in objectsfromcesspit)
					if (!m_combinedlist.Contains(i)) m_combinedlist.Add(i);
			}

			public int IndexOf(DATACLASS item)
			{
				return m_combinedlist.IndexOf(item);
			}

			public void Insert(int index, DATACLASS item)
			{
				if (index < m_combinedlist.Count) RemoveAt(index);

				if (item != null)
				{
					//add object
					m_fetcher.m_objectrelationcache[m_owner][m_relationkey].SubObjects.Add(item);
					m_combinedlist.Insert(index, item);

					//add reverse
					m_fetcher.m_objectrelationcache[item][m_relationkey].SubObjects.Add(m_owner);
				}
			}

			public void RemoveAt(int index)
			{
				Remove(m_combinedlist[index]);
			}

			public DATACLASS this[int index]
			{
				get
				{
					return m_combinedlist[index];
				}
				set
				{
					Insert(index, value);
				}
			}

			public void Add(DATACLASS item)
			{
				//add object
				m_fetcher.m_objectrelationcache[m_owner as IDataClass][m_relationkey].SubObjects.Add(item);
				m_combinedlist.Add(item);
				//add reverse
				m_fetcher.m_objectrelationcache[item as IDataClass][m_relationkey].SubObjects.Add(m_owner);
			}

			public void Clear()
			{
				//remove from all reverse objects
				foreach (DATACLASS obj in m_combinedlist)
				{
					//reset column values
					if (obj.GetType() == m_fetcher.m_relations[m_relationkey].Child.Type)
						m_fetcher.m_relations[m_relationkey].ChildField.SetValueWithEvents(obj as DataClassBase, m_fetcher.m_relations[m_relationkey].ChildField.GetDefaultValue(m_fetcher.m_provider));

					//remove
					m_fetcher.m_objectrelationcache[obj][m_relationkey].SubObjects.Remove(m_owner);
				}

				//reset column values
				foreach(IDataClass obj in m_fetcher.m_objectrelationcache[m_owner][m_relationkey].SubObjects)
					if (obj.GetType() == m_fetcher.m_relations[m_relationkey].Child.Type)
						m_fetcher.m_relations[m_relationkey].ChildField.SetValueWithEvents(obj as DataClassBase, m_fetcher.m_relations[m_relationkey].ChildField.GetDefaultValue(m_fetcher.m_provider));

				//remove current
				m_fetcher.m_objectrelationcache[m_owner][m_relationkey].SubObjects.Clear();
				m_combinedlist.Clear();
			}

			public bool Contains(DATACLASS item)
			{
				return m_combinedlist.Contains(item);
			}

			public void CopyTo(DATACLASS[] array, int arrayIndex)
			{
				m_combinedlist.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return m_combinedlist.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(DATACLASS item)
			{
				if (m_combinedlist.Remove(item))
				{
					//remove from current
					m_fetcher.m_objectrelationcache[m_owner][m_relationkey].SubObjects.Remove(item);

					//reset column values
					if (item.GetType() == m_fetcher.m_relations[m_relationkey].Child.Type)
						m_fetcher.m_relations[m_relationkey].ChildField.SetValueWithEvents(item as DataClassBase, m_fetcher.m_relations[m_relationkey].ChildField.GetDefaultValue(m_fetcher.m_provider));

					//remove from reverse
					m_fetcher.m_objectrelationcache[item][m_relationkey].SubObjects.Remove(m_owner);

					//reset column values
					if (m_owner.GetType() == m_fetcher.m_relations[m_relationkey].Child.Type)
						m_fetcher.m_relations[m_relationkey].ChildField.SetValueWithEvents(m_owner as DataClassBase, m_fetcher.m_relations[m_relationkey].ChildField.GetDefaultValue(m_fetcher.m_provider));

					return true;
				}
				else return false;
			}

			public IEnumerator<DATACLASS> GetEnumerator()
			{
				if (m_combinedlist == null) return default(IEnumerator<DATACLASS>);
				return (IEnumerator<DATACLASS>)m_combinedlist.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				if (m_combinedlist == null) return default(System.Collections.IEnumerator);
				return (System.Collections.IEnumerator)m_combinedlist.GetEnumerator();
			}

			public override string ToString()
			{
				return "RelatedObjects<" + typeof(DATACLASS).Name + ">";
			}
		}

		#endregion

		private void UnregisterObject(IDataClass obj)
		{
			foreach (ObjectConnection rel in m_objectrelationcache[obj].Values)
			{
				foreach (IDataClass child in rel.SubObjects)
					m_objectrelationcache[child][rel.Relation.Name].SubObjects.Remove(obj);
			}
			m_objectrelationcache.Remove(obj);
		}

		private void RegisterObject(IDataClass obj)
		{
			if (!m_objectrelationcache.ContainsKey(obj))
			{
				m_objectrelationcache.Add(obj, new Dictionary<string, ObjectConnection>());
				foreach (TypeConfiguration.ReferenceField r in m_mappings[obj.GetType()].ReferenceFields.Values)
					m_objectrelationcache[obj].Add(r.Name, new ObjectConnection(r));
			}
		}

		/// <summary>
		/// This will update the column values in the relations. (Will only update registered 'loose' objects)
		/// </summary>
		/// <param name="obj"></param>
		private void UpdateObjectKeys(IDataClass obj)
		{
			foreach (ObjectConnection rel in m_objectrelationcache[obj].Values)
			{
				//give or take value?
				if (obj.GetType() == rel.Relation.Child.Type)
				{
					//take from parent
					IDataClass parent = GetRelatedObjectFromCesspool(rel.Relation.Name, obj);
					if (parent != null)
					{
						object value = rel.Relation.ParentField.Field.GetValue(parent);
						rel.Relation.ChildField.SetValueWithEvents(obj as DataClassBase, value);
					}
				}
				else
				{
					//give to childs
					object value = rel.Relation.ParentField.Field.GetValue(obj);
					IList<IDataClass> childs = GetRelatedObjectsFromCesspool(rel.Relation.Name, obj);
					foreach (IDataClass child in childs)
					{
						rel.Relation.ChildField.SetValueWithEvents(child as DataClassBase, value);
					}
				}
			}
		}

		#region " overrides "

		protected override void HookObject(IDataClass obj)
		{
			base.HookObject(obj);
			RegisterObject(obj);
		}

		public override void DiscardObject(IDataClass obj)
		{
			base.DiscardObject(obj);
			UnregisterObject(obj);
		}

		public override void DeleteObject(object item)
		{
			base.DeleteObject(item);
			UnregisterObject(item as IDataClass);
		}

		public override void Commit(IDataClass obj)
		{
			//update IDs
			UpdateObjectKeys(obj);

#if DEBUG
			//validate
			List<IList<IDataClass>> tmp = new List<IList<IDataClass>>();
			foreach(TypeConfiguration.ReferenceField rel in m_mappings[obj.GetType()].ReferenceFields.Values)
				tmp.Add(GetRelatedObjects(rel.Name, obj));
#endif

			base.Commit(obj);

#if DEBUG
			//validate
			List<IList<IDataClass>> tmp2 = new List<IList<IDataClass>>();
			foreach (TypeConfiguration.ReferenceField rel in m_mappings[obj.GetType()].ReferenceFields.Values)
				tmp2.Add(GetRelatedObjects(rel.Name, obj));
#endif

			//update IDs again, in case of auto increment
			if (obj.ObjectState != ObjectStates.Deleted) UpdateObjectKeys(obj);

#if DEBUG
			//validate
			List<IList<IDataClass>> tmp3 = new List<IList<IDataClass>>();
			foreach (TypeConfiguration.ReferenceField rel in m_mappings[obj.GetType()].ReferenceFields.Values)
				tmp3.Add(GetRelatedObjects(rel.Name, obj));
#endif

			//remove if needed
			if (obj.ObjectState == ObjectStates.Deleted) UnregisterObject(obj);
		}

		public override void ClearCache()
		{
			lock (m_loadreducer)
			{
				m_loadreducer.Clear();
			}
			try
			{
				m_cache.Lock.AcquireWriterLock(-1);

				//first update all IDs, so we won't clear 'updated' objects
				foreach (IDataClass obj in m_cache.GetAllUnchanged())
					UpdateObjectKeys(obj);

				//clear
				foreach (IDataClass obj in m_cache.GetAllUnchanged())
					UnregisterObject(obj);
				m_cache.ClearAllUnchanged();
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
		}

		#endregion
	}
}
