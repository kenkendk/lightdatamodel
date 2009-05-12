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
		private Dictionary<string, TypeConfiguration.Reference> m_relations = new Dictionary<string, TypeConfiguration.Reference>();

		/// <summary>
		/// This is a cache for 'loosely' related objects
		/// </summary>
		private Dictionary<IDataClass, Dictionary<string, ObjectConnection>> m_objectrelationcache = new Dictionary<IDataClass, Dictionary<string, ObjectConnection>>();

		public Dictionary<IDataClass, Dictionary<string, ObjectConnection>> ObjectRelationCache { get { return m_objectrelationcache; } }

		[System.Diagnostics.DebuggerDisplay("Name = {Name}")]
		public class ObjectConnection
		{
			public string Name { get { return Relation.Name; } }
			public TypeConfiguration.Reference Relation;
			public SortedList<int, IDataClass> SubObjects = new SortedList<int, IDataClass>();		//hashcode, IDataClass
			public ObjectConnection(TypeConfiguration.Reference relation)
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
				foreach (TypeConfiguration.Reference rf in mc.References.Values)
					if (!m_relations.ContainsKey(rf.Name)) m_relations.Add(rf.Name, rf);
		}

		/// <summary>
		/// This will manually add a relation to the model
		/// </summary>
		/// <param name="name"></param>
		/// <param name="childobject"></param>
		/// <param name="childcolumn"></param>
		/// <param name="parentobject"></param>
		/// <param name="parentcolumn"></param>
		public void AddRelation(string name, Type childobject, string childcolumn, Type parentobject, string parentcolumn)
		{
			TypeConfiguration.Reference r = new TypeConfiguration.Reference(name, m_mappings[childobject].MappedFields[childcolumn], m_mappings[parentobject].MappedFields[parentcolumn], m_mappings[childobject], m_mappings[parentobject]);
			m_mappings[childobject].References.Add(r.Name, r);
			m_mappings[parentobject].References.Add(r.Name, r);
			m_relations.Add(r.Name, r);
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
        /// This will search through the cache for the given related objects. Indexes are used if possible
        /// </summary>
        /// <param name="relationkey"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected virtual IDataClass[] GetRelatedObjectsFromCache(string relationkey, IDataClass owner)
        {
            IDataClass[] cacheobjects = null;
            if (owner.GetType() == m_relations[relationkey].Parent.Type)
            {
                    cacheobjects = (IDataClass[])GetObjectsFromCache(m_relations[relationkey].Child.Type, m_relations[relationkey].ChildField.Databasefield + " = ?", m_relations[relationkey].ParentField.Field.GetValue(owner));
            }
            else
            {
                cacheobjects = new IDataClass[] { GetRelatedObjectFromCache(relationkey, owner) };
                if (cacheobjects[0] == null) cacheobjects = null;
            }
            return cacheobjects;
        }

        /// <summary>
        /// This will search through the cache for the given related objects. Indexes are used if possible
        /// </summary>
        /// <param name="relationkey"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected virtual IDataClass GetRelatedObjectFromCache(string relationkey, IDataClass owner)
        {
            IDataClass cacheobject = null;
            if (owner.GetType() == m_relations[relationkey].Parent.Type)
            {
                cacheobject = (IDataClass)GetObjectFromCache(m_relations[relationkey].Child.Type, m_relations[relationkey].ChildField.Databasefield + " = ?", m_relations[relationkey].ParentField.Field.GetValue(owner));
            }
            else
            {
                cacheobject = (IDataClass)GetObjectFromCache(m_relations[relationkey].Parent.Type, m_relations[relationkey].ParentField.Databasefield + " = ?", m_relations[relationkey].ChildField.Field.GetValue(owner));
            }
            return cacheobject;
        }


		/// <summary>
		/// This will return the objects with explicit relations (DB not searched)
		/// </summary>
		/// <param name="relationkey"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		protected virtual IDataClass GetRelatedObjectFromCesspool(string relationkey, IDataClass owner)
		{
			if (m_objectrelationcache[owner][relationkey].SubObjects.Count > 0) return m_objectrelationcache[owner][relationkey].SubObjects.Values[0];
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
            if (owner.DataParent != this)
                throw new Exception("Item was on wrong path");

			IDataClass[] ret = null;
			if (owner.GetType() == m_relations[relationkey].Parent.Type)
				ret = (IDataClass[])Array.CreateInstance(m_relations[relationkey].Child.Type, m_objectrelationcache[owner][relationkey].SubObjects.Count);
			else
				ret = (IDataClass[])Array.CreateInstance(m_relations[relationkey].Parent.Type, m_objectrelationcache[owner][relationkey].SubObjects.Count);
			m_objectrelationcache[owner][relationkey].SubObjects.Values.CopyTo(ret, 0);
			return ret;
		}

        public virtual IList<DATACLASS> GetRelatedObjectsFromMemory<DATACLASS>(string relationkey, IDataClass owner) where DATACLASS : IDataClass
        {
            //fetch from cache
            DATACLASS[] cacheobjects = (DATACLASS[])(Array)GetRelatedObjectsFromCache(relationkey, owner);
            DATACLASS[] cesspitobjects = (DATACLASS[])(Array)GetRelatedObjectsFromCesspool(relationkey, owner);
            return (IList<DATACLASS>)new RelatedObjectCollection<DATACLASS>(owner, relationkey, cacheobjects, cesspitobjects, this);
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
				SortedList<int, IDataClass> c = new SortedList<int, IDataClass>(1);
				c.Add(target.GetHashCode(), target);
				m_objectrelationcache[owner][relationkey].SubObjects = c;

				//also set the counterpart
				SortedList<int, IDataClass> p = new SortedList<int, IDataClass>(1);
				p.Add(owner.GetHashCode(), owner);
				m_objectrelationcache[target][relationkey].SubObjects = p;

			}

			//change value from child
			if (oldtarget != null && oldtarget.GetType() == m_relations[relationkey].Child.Type)
				m_relations[relationkey].ChildField.SetValueWithEvents(oldtarget as DataClassBase, m_relations[relationkey].ChildField.GetDefaultValue(m_provider));
		}

		public void AddRelatedObject(string relationkey, IDataClass owner, IDataClass target)
		{
			if (!m_objectrelationcache[owner][relationkey].SubObjects.ContainsKey(target.GetHashCode())) m_objectrelationcache[owner][relationkey].SubObjects.Add(target.GetHashCode(), target);
			if (!m_objectrelationcache[target][relationkey].SubObjects.ContainsKey(owner.GetHashCode())) m_objectrelationcache[target][relationkey].SubObjects.Add(owner.GetHashCode(), owner);
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
					m_fetcher.AddRelatedObject(m_relationkey, m_owner, item);
					m_combinedlist.Insert(index, item);
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
				m_fetcher.AddRelatedObject(m_relationkey, m_owner, item);
				m_combinedlist.Add(item);
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
					m_fetcher.m_objectrelationcache[obj][m_relationkey].SubObjects.Remove(m_owner.GetHashCode());
				}

				//reset column values
				foreach(IDataClass obj in m_fetcher.m_objectrelationcache[m_owner][m_relationkey].SubObjects.Values)
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
					m_fetcher.m_objectrelationcache[m_owner][m_relationkey].SubObjects.Remove(item.GetHashCode());

					//reset column values
					if (item.GetType() == m_fetcher.m_relations[m_relationkey].Child.Type)
						m_fetcher.m_relations[m_relationkey].ChildField.SetValueWithEvents(item as DataClassBase, m_fetcher.m_relations[m_relationkey].ChildField.GetDefaultValue(m_fetcher.m_provider));

					//remove from reverse
					m_fetcher.m_objectrelationcache[item][m_relationkey].SubObjects.Remove(m_owner.GetHashCode());

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
			if(m_objectrelationcache.ContainsKey(obj))
			{
				foreach (ObjectConnection rel in m_objectrelationcache[obj].Values)
				{
                    foreach (IDataClass child in rel.SubObjects.Values)
                        if (m_objectrelationcache.ContainsKey(child) && m_objectrelationcache[child].ContainsKey(rel.Relation.Name))
                            m_objectrelationcache[child][rel.Relation.Name].SubObjects.Remove(obj.GetHashCode());
                        else
                            throw new Exception("GOTCHA! (hard to find bug :))");
				}
				m_objectrelationcache.Remove(obj);
			}
		}

		private void RegisterObject(IDataClass obj)
		{
			if (!m_objectrelationcache.ContainsKey(obj))
			{
				m_objectrelationcache.Add(obj, new Dictionary<string, ObjectConnection>());
				foreach (TypeConfiguration.Reference r in m_mappings[obj.GetType()].References.Values)
					m_objectrelationcache[obj].Add(r.Name, new ObjectConnection(r));
			}
		}

		/// <summary>
		/// This will update the column values in the relations. (Will only update registered 'loose' objects)
		/// </summary>
		/// <param name="obj"></param>
		protected void UpdateObjectKeys(IDataClass obj)
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

		public override void Commit(params IDataClass[] items)
		{
            if (items == null || items.Length == 0)
                return;

            bool stillDirty;

            do
            {
                List<IDataClass> updated = new List<IDataClass>();
                List<IDataClass> deleted = new List<IDataClass>();

                foreach (IDataClass obj in items)
                {
                    if (obj.DataParent != this)
                        throw new Exception("An item was passed to commit, but belongs to another DataParent");

                    if (obj.ObjectState != ObjectStates.Deleted)
                    {
                        UpdateObjectKeys(obj);
                        updated.Add(obj);
                    }
                    else
                        deleted.Add(obj);
                }


                base.Commit(items);

                //Refresh keys
                foreach (IDataClass obj in updated)
                    UpdateObjectKeys(obj);

                //Unhook
                foreach (IDataClass obj in deleted)
                    UnregisterObject(obj);

                stillDirty = false;
                foreach (IDataClass obj in items)
                    stillDirty |= obj.IsDirty;

            } while (stillDirty);
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

        protected override void GetReferencedItems(IDataClass item, Queue<IDataClass> queue)
        {
            TypeConfiguration.MappedClass ic = m_mappings[item.GetType()];
            foreach (TypeConfiguration.Reference r in ic.References.Values)
                foreach (IDataClass c in GetRelatedObjectsFromMemory<IDataClass>(r.Name, item))
                    queue.Enqueue(c);
        }
	}
}
