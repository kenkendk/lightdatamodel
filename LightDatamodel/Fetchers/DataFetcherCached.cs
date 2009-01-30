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
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Will fetch and commit objects directly to source
	/// </summary>
	public class DataFetcherCached : DataFetcher, System.Data.LightDatamodel.IDataFetcherCached
	{
		protected Cache m_cache = new Cache();
		protected Dictionary<Type, Dictionary<string, string>> m_loadreducer = new Dictionary<Type, Dictionary<string, string>>();
		private System.Threading.ReaderWriterLock m_transactionlock = new System.Threading.ReaderWriterLock();
#if DEBUG
		private static bool m_isintransaction = false;
#endif

		public event ObjectStateChangeHandler ObjectAddRemove;

		#region " Cache "

		/// <summary>
		/// This is not thread safe. The fetcher is though
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("Count = {CountWithLock}")]
		public class Cache : IEnumerable<IDataClass>, IDisposable
		{
			//this is the actual cache
			//Type, Indexname, (Indexvalue & IDataClass)
			private Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> m_list = new Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>>();
			protected Dictionary<string, IDataClass> m_deletedobjects = new Dictionary<string, IDataClass>();
			public System.Threading.ReaderWriterLock Lock = new System.Threading.ReaderWriterLock();

			public Dictionary<string, IDataClass> DeletedObjects { get { return m_deletedobjects; } }

			public IDataClass this[Type type, string indexname, object indexvalue]
			{
				get { return m_list[type][indexname][indexvalue]; }
				set
				{
					try
					{
						//we assume that the keys already exists 
						m_list[type][indexname][indexvalue] = value;
					}
					catch (ArgumentNullException)
					{
						if(indexvalue == null)
							throw new ArgumentNullException("indexvalue");
						else if(indexname == null)
							throw new ArgumentNullException("indexname");
						else if (type == null)
							throw new ArgumentNullException("type");
					}
					catch
					{
#if DEBUG
						if (!Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
						//the keys doesn't exists ... this is very expensive (the try-catch that is)
						if (!m_list.ContainsKey(type)) m_list.Add(type, new Dictionary<string, MultiDictionary<object, IDataClass>>());
						if (!m_list[type].ContainsKey(indexname)) m_list[type].Add(indexname, new MultiDictionary<object, IDataClass>());
						m_list[type][indexname][indexvalue] = value;
					}
				}
			}

			public bool Contains(object obj)
			{
				if (obj == null) return false;
				Type t = obj.GetType();
				if (!m_list.ContainsKey(t)) return false;
				IEnumerator e = m_list[t].Keys.GetEnumerator();
				if (e == null || !e.MoveNext()) return false; ;
				return m_list[t][(string)e.Current].ContainsKey(obj);
			}

			/// <summary>
			/// This will rebalance an object
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="indexname"></param>
			/// <param name="oldvalue"></param>
			/// <param name="newvalue"></param>
			public void ReindexObject(object obj, string indexname, object oldvalue, object newvalue)
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				RemoveObjectFromIndex(obj.GetType(), indexname, oldvalue, (IDataClass)obj);		//only removes from 1 index
				Add(obj.GetType(), indexname, newvalue, (IDataClass)obj);
			}

			public bool HasIndex(Type type, string indexname)
			{
				try
				{
					return m_list[type].ContainsKey(indexname);
				}
				catch
				{
					return false;
				}
			}

			public void Add(Type type, string indexname, object indexvalue, IDataClass value)
			{
				this[type, indexname, indexvalue] = value;
			}

			public int Count
			{
				get
				{
#if DEBUG
					if (!Lock.IsReaderLockHeld && !Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
					int count = 0;
					foreach (KeyValuePair<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> itm in m_list)
					{
						foreach (KeyValuePair<string, MultiDictionary<object, IDataClass>> kp in itm.Value)
						{
							count += kp.Value.Count;
							break;
						}
					}
					return count;
				}
			}

			public int CountWithLock
			{
				get
				{
					try
					{
						Lock.AcquireReaderLock(-1);
						return Count;
					}
					finally
					{
						Lock.ReleaseReaderLock();
					}
				}
			}

			/// <summary>
			/// This will return a list of objects, if a such is needed (unique indexes doesn't)
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			/// <param name="indexvalue"></param>
			/// <returns></returns>
			public IDataClass[] GetObjects(Type type, string indexname, object indexvalue)
			{
#if DEBUG
				if (!Lock.IsReaderLockHeld && !Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				ICollection<IDataClass> list = m_list[type][indexname].Items[indexvalue];
				IDataClass[] ret = (IDataClass[])Array.CreateInstance(type, list == null ? 0 : list.Count);
				int c = 0;
				if(list != null)
				foreach (IDataClass itm in list)
					ret[c++] = itm;
				return ret;
			}

			public DATACLASS[] GetObjects<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass
			{
#if DEBUG
				if (!Lock.IsReaderLockHeld && !Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				ICollection<IDataClass> list = m_list[typeof(DATACLASS)][indexname].Items[indexvalue];
				DATACLASS[] ret = new DATACLASS[list == null ? 0 : list.Count];
				int c = 0;
				foreach (DATACLASS itm in list)
					ret[c++] = itm;
				return ret;
			}

			#region " TypeObjectCollection (Dummy class really) "

			public class TypeObjectCollection : IEnumerable<IDataClass>
			{
				private Cache m_parent;
				private Type m_type;

				public TypeObjectCollection(Cache parent, Type type)
				{
					m_parent = parent;
					m_type = type;
				}

				public IEnumerator<IDataClass> GetEnumerator()
				{
					if (!m_parent.m_list.ContainsKey(m_type)) return new MultiDictionary<object, IDataClass>.ValueCollection.Enumerator(null);
					Dictionary<string, MultiDictionary<object, IDataClass>>.Enumerator e = m_parent.m_list[m_type].GetEnumerator();
					e.MoveNext();
					if (e.Current.Value != null) return e.Current.Value.Values.GetEnumerator();
					return new MultiDictionary<object, IDataClass>.ValueCollection.Enumerator(null);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}
			}

			#endregion

			/// <summary>
			/// This will give access to all cached objects for the given type. Including new objects
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			public TypeObjectCollection GetObjects(Type type)
			{
#if DEBUG
				if (!Lock.IsReaderLockHeld && !Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				return new TypeObjectCollection(this, type);
			}

			/// <summary>
			/// This will return all unchanged objects (for use in own clear objects function perhaps)
			/// </summary>
			/// <returns></returns>
			public IDataClass[] GetAllUnchanged()
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld && !Lock.IsReaderLockHeld) throw new Exception("This will need lock");
#endif
				List<IDataClass> ret = new List<IDataClass>();
				foreach (KeyValuePair<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> type in m_list)
					foreach (KeyValuePair<string, MultiDictionary<object, IDataClass>> index in type.Value)
					{
						foreach (KeyValuePair<object, IDataClass> obj in index.Value)
							if (!obj.Value.IsDirty) ret.Add(obj.Value);
					}
				return ret.ToArray();
			}

			/// <summary>
			/// This will return all unchanged objects (for use in own clear objects function perhaps)
			/// </summary>
			/// <returns></returns>
			public IDataClass[] GetAllChanged()
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld && !Lock.IsReaderLockHeld) throw new Exception("This will need lock");
#endif
				List<IDataClass> ret = new List<IDataClass>();
				foreach (KeyValuePair<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> type in m_list)
					foreach (KeyValuePair<string, MultiDictionary<object, IDataClass>> index in type.Value)
					{
						foreach (KeyValuePair<object, IDataClass> obj in index.Value)
							if (obj.Value.IsDirty) ret.Add(obj.Value);
						break;	//only iterate first index
					}
				return ret.ToArray();
			}

			/// <summary>
			/// This will remove all non deity objects
			/// </summary>
			public void ClearAllUnchanged()
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				foreach (KeyValuePair<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> type in m_list)
					foreach (KeyValuePair<string, MultiDictionary<object, IDataClass>> index in type.Value)
					{
						LinkedList<KeyValuePair<object, IDataClass>> tobedeleted = new LinkedList<KeyValuePair<object, IDataClass>>();
						foreach (KeyValuePair<object, IDataClass> obj in index.Value)
							if (!obj.Value.IsDirty) tobedeleted.AddLast(obj);
						foreach (KeyValuePair<object, IDataClass> obj in tobedeleted)
							index.Value.Remove(obj.Key, obj.Value);
					}
			}

			/// <summary>
			/// This is not nessesary to call. But it will improve performance
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			public void AddIndex(Type type, string indexname)
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				//add index
				if (!m_list.ContainsKey(type)) m_list.Add(type, new Dictionary<string, MultiDictionary<object, IDataClass>>());
				if (!m_list[type].ContainsKey(indexname)) m_list[type].Add(indexname, new MultiDictionary<object, IDataClass>());
			}

			/// <summary>
			/// If you ever want to remove an index ... this is it
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			/// <returns></returns>
			public bool RemoveIndex(Type type, string indexname)
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				if (!m_list.ContainsKey(type)) return false;
				return m_list[type].Remove(indexname);
			}

			/// <summary>
			/// If you ever want to remove an index ... this is it
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			public bool RemoveIndex(Type type)
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				return m_list.Remove(type);
			}

			/// <summary>
			/// This is used for removing the exact object. Eg. if the index is out of sync
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			//public bool RemoveObject(IDataClass item)
			//{
			//    throw new Exception("This is prolly too costly");
			//}

			/// <summary>
			/// This is used for removing an object. Will not remove from Deleted. Will only remove from 1 index
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			/// <param name="indexvalue"></param>
			/// <returns></returns>
			public bool RemoveObjectFromIndex(Type type, string indexname, object indexvalue, IDataClass item)
			{
#if DEBUG
				if (!Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				return m_list[type][indexname].Remove(indexvalue, item);
			}

			public IEnumerator<IDataClass> GetEnumerator()
			{
#if DEBUG
				if (!Lock.IsReaderLockHeld && !Lock.IsWriterLockHeld) throw new Exception("This will need lock");
#endif
				return new Enumerator(this.m_list);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public class Enumerator : IEnumerator<IDataClass>
			{
				private Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> m_parent;
				private Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>>.Enumerator m_typeenumerator;
				private IEnumerator<KeyValuePair<object, IDataClass>> m_objectenumerator;

				public Enumerator(Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> parent)
				{
					m_parent = parent;
					Reset();
				}

				public IDataClass Current
				{
					get { return m_objectenumerator.Current.Value; }
				}

				public void Dispose()
				{
					m_parent = null;
					m_typeenumerator.Dispose();
					if (m_objectenumerator != null) m_objectenumerator.Dispose();
					m_objectenumerator = null;
					GC.SuppressFinalize(this);
				}

				object IEnumerator.Current
				{
					get { return Current; }
				}

				public bool MoveNext()
				{
					if (m_objectenumerator == null || !m_objectenumerator.MoveNext())
					{
						do
						{
							if (!m_typeenumerator.MoveNext()) return false;
							Dictionary<string, MultiDictionary<object, IDataClass>>.Enumerator e = m_typeenumerator.Current.Value.GetEnumerator();
							if (!e.MoveNext()) return false;
							m_objectenumerator = e.Current.Value.GetEnumerator();
							m_objectenumerator.MoveNext();
						} while (m_objectenumerator == null || m_objectenumerator.Current.Equals(default(KeyValuePair<object, IDataClass>)));
					}
					return true;
				}

				public void Reset()
				{
					m_typeenumerator = m_parent.GetEnumerator();
					if (!m_typeenumerator.MoveNext()) return;
					Dictionary<string, MultiDictionary<object, IDataClass>>.Enumerator e = m_typeenumerator.Current.Value.GetEnumerator();
					if (!e.MoveNext()) return;
					m_objectenumerator = e.Current.Value.GetEnumerator();
				}
			}

			public void Dispose()
			{
				m_deletedobjects = null;
				m_list = null;
				Lock = null;
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		/// <summary>
		/// Gets a value describing if the fetcher contains uncommited changes
		/// </summary>
		public bool IsDirty
		{
			get
			{
				try
				{
					m_cache.Lock.AcquireReaderLock(-1);
					if (m_cache.DeletedObjects.Count > 0) return true;

					foreach (IDataClass obj in m_cache)
						if (obj.IsDirty) return true;

					return false;
				}
				finally
				{
					m_cache.Lock.ReleaseReaderLock();
				}
			}
		}

		public Cache LocalCache { get { return m_cache; } }

		/// <summary>
		/// Constructs a new cached data fetcher
		/// </summary>
		/// <param name="provider">The provider to use</param>
		public DataFetcherCached(IDataProvider provider)
			: base(provider)
		{
			m_mappings.TypesInitialized += new EventHandler(m_mappings_TypesInitialized);
		}

		private void m_mappings_TypesInitialized(object sender, EventArgs e)
		{
			try
			{
			m_cache.Lock.AcquireWriterLock(-1);

				//pre-add indexes (gives a slight startup performance boost)
				foreach (TypeConfiguration.MappedClass type in m_mappings)
					foreach (TypeConfiguration.MappedField index in type.IndexFields)
						m_cache.AddIndex(type.Type, index.Databasefield);
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
		}

        /// <summary>
        /// Traverses an objects references to find related objects
        /// </summary>
        /// <param name="item">The item to list relations for</param>
        /// <returns>A list of related objects</returns>
        public virtual List<IDataClass> FindObjectRelations(IDataClass item)
        {
            Dictionary<IDataClass, object> visited = new Dictionary<IDataClass,object>();
            Queue<IDataClass> unexplored = new Queue<IDataClass>();
            List<IDataClass> results = new List<IDataClass>();
            unexplored.Enqueue(item);

            while (unexplored.Count > 0)
            {
                IDataClass current = unexplored.Dequeue();
                if (visited.ContainsKey(current))
                    continue;
                else
                    visited.Add(current, null);

                results.Add(current);

                GetReferencedItems(current, unexplored);
            }

            return results;
        }

        /// <summary>
        /// Puts all items that are referenced into the supplied queue
        /// </summary>
        /// <param name="item">The item to examine</param>
        /// <param name="queue">The queue to place related items into</param>
        protected virtual void GetReferencedItems(IDataClass item, Queue<IDataClass> queue)
        {
            TypeConfiguration.MappedClass itemtype = m_mappings[item.GetType()];
            foreach (TypeConfiguration.Reference r in itemtype.References.Values)
            {
                object rel;
                if (r.Child == itemtype)
                    rel = r.ChildField.Field.GetValue(item);
                else
                    rel = r.ParentField.Field.GetValue(item);

                if (rel == null)
                    continue;

                if (rel is IEnumerable)
                {
                    foreach (object o in (IEnumerable)rel)
                        if (o is IDataClass)
                            queue.Enqueue((IDataClass)o);
                }
                else if (rel is IDataClass)
                    queue.Enqueue((IDataClass)rel);
            }
        }

        /// <summary>
        /// Commits the objects and any relations they have that are modified
        /// </summary>
        /// <param name="items">The objects to add</param>
        /// <returns>The items that were committed</returns>
        public virtual List<IDataClass> CommitWithRelations(params IDataClass[] items)
        {
            List<IDataClass> dirty = new List<IDataClass>();
            if (items != null)
                foreach(IDataClass item in items)
                    foreach (IDataClass i in FindObjectRelations(item))
                        if (i.IsDirty || i.ObjectState != ObjectStates.Default)
                            dirty.Add(i);

            Commit(dirty.ToArray());

            return dirty;
        }

        public virtual void CommitRecursive(params IDataClass[] items)
        {
            Commit(items);
            if (this is DataFetcherNested)
            {
                DataFetcherNested n = this as DataFetcherNested;
                List<IDataClass> newitems = new List<IDataClass>();
                foreach (IDataClass obj in items)
                    newitems.Add((IDataClass)n.BaseFetcher.GetObjectById(obj.GetType(), m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj)));

                if (((DataFetcherNested)this).BaseFetcher is IDataFetcherCached)
                    ((IDataFetcherCached)n.BaseFetcher).CommitRecursive(newitems.ToArray());
                else
                    n.BaseFetcher.Commit(newitems.ToArray());

                for (int i = 0; i < items.Length; i++)
                {
                    TypeConfiguration.MappedField fi = m_mappings[items[i].GetType()].PrimaryKey;
                    fi.Field.SetValue((DataClassBase)items[i], fi.Field.GetValue(newitems[i]));
                }

                foreach (IDataClass obj in items)
                    RefreshObject(obj);
            }
        }

        public virtual List<IDataClass> CommitRecursiveWithRelations(params IDataClass[] items)
        {
            List<IDataClass> modified = CommitWithRelations(items);
            if (this is DataFetcherNested)
            {
                DataFetcherNested n = this as DataFetcherNested;
                List<IDataClass> newitems = new List<IDataClass>();
                foreach (IDataClass obj in modified)
                    newitems.Add((IDataClass)n.BaseFetcher.GetObjectById(obj.GetType(), m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj)));

                if (n.BaseFetcher is IDataFetcherCached)
                    ((IDataFetcherCached)n.BaseFetcher).CommitRecursiveWithRelations(newitems.ToArray());
                else
                    n.BaseFetcher.Commit(newitems.ToArray());

                for (int i = 0; i < modified.Count; i++)
                {
                    TypeConfiguration.MappedField fi = m_mappings[modified[i].GetType()].PrimaryKey;
                    fi.Field.SetValue((DataClassBase)modified[i], fi.Field.GetValue(newitems[i]));
                }

                foreach (IDataClass obj in modified)
                    RefreshObject(obj);
            }

            return modified;

        }

		/// <summary>
		/// Commits all cached objects to data source
		/// </summary>
		public virtual List<IDataClass> CommitAll()
		{
			return CommitAll(null);
		}

		/// <summary>
		/// Commits all cached objects to data source
        /// <param name="items">The items to commit, or null to commit all dirty items</param>
		/// </summary>
		public virtual List<IDataClass> CommitAll(UpdateProgressHandler updatefunction)
		{
			Guid transactionID = Guid.NewGuid();
			bool inTransaction = false;

			LinkedList<IDataClass> deletedobjects = null;
			LinkedList<IDataClass> newobjects = null;
			LinkedList<IDataClass> updatedobjects = null;

            //get objects
			try
			{
				m_cache.Lock.AcquireReaderLock(-1);			//different lock

                newobjects = new LinkedList<IDataClass>();
                updatedobjects = new LinkedList<IDataClass>();
                deletedobjects = new LinkedList<IDataClass>(m_cache.DeletedObjects.Values);

                foreach (IDataClass obj in m_cache)
                    if (obj.IsDirty)
                    {
                        if (obj.ObjectState == ObjectStates.New)
                            newobjects.AddLast(obj);
                        else
                            updatedobjects.AddLast(obj);
                    }
			}
			finally
			{
				m_cache.Lock.ReleaseReaderLock();
			}

			if (deletedobjects.Count == 0 && newobjects.Count == 0 && updatedobjects.Count == 0) return new List<IDataClass>();
			int maxposts = deletedobjects.Count + newobjects.Count + updatedobjects.Count;
			if (updatefunction != null) updatefunction(0, maxposts);

			//create copies for rollback
			LinkedList<IDataClass> copydeletedobjects = (LinkedList<IDataClass>)ObjectTransformer.CreateArrayCopy<IDataClass>(deletedobjects) ;
			LinkedList<IDataClass> copynewobjects = (LinkedList<IDataClass>)ObjectTransformer.CreateArrayCopy<IDataClass>(newobjects);
			LinkedList<IDataClass> copyupdatedobjects = (LinkedList<IDataClass>)ObjectTransformer.CreateArrayCopy<IDataClass>(updatedobjects);

			try
			{
				m_transactionlock.AcquireWriterLock(-1);		//only 1 of this function (CommitAll) may run
#if DEBUG
				if (m_isintransaction) throw new Exception("Already in transaction!");
				m_isintransaction = true;
#endif

				//start commiting
				try
				{
					m_provider.BeginTransaction(transactionID); //SQLite can only handle 1 transaction ... and not writing from multiple threads isn't that bad, is it?
					inTransaction = true;
					int i = 0;

					//delete (First we delete. In case we've delete a primary key, that also will be inserted)
					foreach (IDataClass c in deletedobjects)
					{
						Commit(c); //no lock
						if (updatefunction != null) updatefunction(++i, maxposts);
					}

					//update (second we update. In case we changed some primary keys, that also will be inserted)
					foreach (IDataClass c in updatedobjects)
					{
						Commit(c); //no lock
						if (updatefunction != null) updatefunction(++i, maxposts);
					}

					//create
					foreach (IDataClass c in newobjects)
					{
						Commit(c); //no lock ... //will also rebalance objects in cache (means lock)
						if (updatefunction != null) updatefunction(++i, maxposts);
					}

					m_provider.CommitTransaction(transactionID);
					inTransaction = false;
				}
				finally
				{
					if (inTransaction)
					{
						m_provider.RollbackTransaction(transactionID);

						//copy back objects
						ObjectTransformer.CopyArray<IDataClass>(copydeletedobjects, deletedobjects);
						ObjectTransformer.CopyArray<IDataClass>(copynewobjects, newobjects);
						ObjectTransformer.CopyArray<IDataClass>(copyupdatedobjects, updatedobjects);
					}
				}

			}
			finally
			{
				#if DEBUG
				m_isintransaction = false;
				#endif
				m_transactionlock.ReleaseWriterLock();
			}

            List<IDataClass> affected = new List<IDataClass>();
            affected.AddRange(newobjects);
            affected.AddRange(updatedobjects);
            affected.AddRange(deletedobjects);

            //recursive commit
            affected.AddRange(CommitAll(updatefunction));

            //Remove duplicates
            Dictionary<IDataClass, object> tmp = new Dictionary<IDataClass, object>();
            for (int i = 0; i < affected.Count; i++)
                if (tmp.ContainsKey(affected[i]))
                {
                    affected.RemoveAt(i);
                    i--;
                }
                else
                    tmp.Add(affected[i], null);
            
            return affected;
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hook into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="type"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public override object[] GetObjects(Type type, QueryModel.Operation operation)
		{
			if (!HasLoaded(type, operation))
				InsertObjectsInCache(LoadObjects(type, operation));

			try
			{
				m_cache.Lock.AcquireReaderLock(-1);
				return (object[])Query.SearchLinear(operation, m_cache.GetObjects(type)).ToArray(type);
			}
			finally
			{
				m_cache.Lock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// This will check wheter the given query has already been loaded
		/// </summary>
		/// <param name="type"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		protected bool HasLoaded(Type type, QueryModel.Operation operation)
		{
			string eq = operation.ToString(false);
			if (eq != null)
			{
				if (!m_loadreducer.ContainsKey(type))
				{
					lock (m_loadreducer)		//double lock
					{
						if (!m_loadreducer.ContainsKey(type)) m_loadreducer.Add(type, new Dictionary<string, string>());
					}
				}

				if (m_loadreducer[type].ContainsKey(eq) || m_loadreducer[type].ContainsKey(""))
					return true;
				else
				{
					//will this need lock? Nah
					m_loadreducer[type][eq] = eq;
					return false;
				}
			}
			else
				return false;
		}

		/// <summary>
		/// Discards all changes from the object, and removes it from the internal cache
		/// </summary>
		/// <param name="obj">The object to discard</param>
		public virtual void DiscardObject(IDataClass obj)
		{
			lock (m_loadreducer)
			{
				m_loadreducer.Clear();
			}

			try
			{
				m_cache.Lock.AcquireWriterLock(-1);
				foreach (TypeConfiguration.MappedField index in m_mappings[obj.GetType()].IndexFields)
				    m_cache.RemoveObjectFromIndex(obj.GetType(), index.Databasefield, index.Field.GetValue(obj), obj);
				string deletekey = obj.GetType().FullName + (char)1 + m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj).ToString();
				m_cache.DeletedObjects.Remove(deletekey);		//it could be a deleted object
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// This will load an object from cache
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public object GetObjectFromCacheById(Type type, object id)
		{
			if (m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id] != null)		//will these need locks?
				return m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id];	//will these need locks?
			else
				return null;
		}

		/// <summary>
		/// This will load an object from cache
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public DATACLASS GetObjectFromCacheById<DATACLASS>(object id)
		{
			return (DATACLASS)GetObjectFromCacheById(typeof(DATACLASS), id);
		}

		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public DATACLASS GetObjectFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			DATACLASS[] ret = GetObjectsFromCache<DATACLASS>(filter, parameters);		//this has lock
			if (ret != null && ret.Length > 0) return ret[0];
			return default(DATACLASS);
		}

		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public DATACLASS[] GetObjectsFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			return GetObjectsFromCache<DATACLASS>(Query.Parse(filter, parameters)); //this has lock
		}


		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		public DATACLASS GetObjectFromCache<DATACLASS>(QueryModel.Operation query) where DATACLASS : IDataClass
		{
			DATACLASS[] ret = GetObjectsFromCache<DATACLASS>(query); //this has lock
			if (ret != null && ret.Length > 0) return ret[0];
			return default(DATACLASS);
		}

		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <param name="type"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public DATACLASS[] GetObjectsFromCache<DATACLASS>(QueryModel.Operation query) where DATACLASS : IDataClass
		{
			return (DATACLASS[])(Array)GetObjectsFromCache(typeof(DATACLASS), query);
		}

		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <param name="type"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public object GetObjectFromCache(Type type, QueryModel.Operation query)
		{
			object[] ret = GetObjectsFromCache(type, query); //this has lock
			if (ret != null && ret.Length > 0) return ret[0];
			return null;
		}

		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <param name="type"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public virtual object[] GetObjectsFromCache(Type type, QueryModel.Operation query)
		{
			try
			{
				m_cache.Lock.AcquireReaderLock(-1);
				return (object[])Query.SearchLinear(query, m_cache.GetObjects(type)).ToArray(type);
			}
			finally
			{
				m_cache.Lock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object GetObjectFromCache(Type type, string filter, params object[] parameters)
		{
			object[] ret = GetObjectsFromCache(type, filter, parameters); //this has lock
			if (ret != null && ret.Length > 0) return ret[0];
			return null;
		}

		/// <summary>
		/// Reads objects from the cache, will not communicate with the database
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object[] GetObjectsFromCache(Type type, string filter, params object[] parameters)
		{
			return GetObjectsFromCache(type, Query.Parse(filter, parameters)); //this has lock
		}

		/// <summary>
		/// This will insert a given list of objects into the cache
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected virtual IDataClass[] InsertObjectsInCache(params object[] data)
		{
			List<IDataClass> res = new List<IDataClass>();

			try
			{
				m_cache.Lock.AcquireWriterLock(-1);

				foreach (IDataClass item in data)
				{
					IDataClass toAdd = item;
					if (item is DataClassView)
					{
						(item as DataClassView).m_dataparent = this;
						m_cache[item.GetType(), "GetHashCode", item.GetHashCode()] = (IDataClass)item;
						toAdd = (IDataClass)item;
					}
					else if (item is DataClassBase)
					{
						DataClassBase dbitem = (DataClassBase)item;
						if (m_mappings[dbitem.GetType()].PrimaryKey == null) throw new Exception("Object " + dbitem.GetType().Name + " doesn't have a primary key");
						if (m_cache[item.GetType(), m_mappings[dbitem.GetType()].PrimaryKey.Databasefield, m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem)] == null)
						{

                            //Manual build of the query:
                            //Query.Parse("(GetType() IS ?) AND (" + m_mappings[dbitem.GetType()].PrimaryKey.Property.Name + " = ?)", item.GetType(), m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem));
                            QueryModel.Operation opdeleted =
                                Query.And(
                                    Query.Is(
                                        Query.FunctionCall("GetType"),
                                        Query.Value(item.GetType())
                                    ),
                                    Query.Equal(
                                        Query.Property(m_mappings[dbitem.GetType()].PrimaryKey.Property.Name),
                                        Query.Value(m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem))
                                    )
                                );
                                
							if (Query.FindFirst(opdeleted, m_cache.DeletedObjects) != null) continue;

							dbitem.m_state = ObjectStates.Default;
							dbitem.m_isdirty = false;

							HookObject(dbitem);
							foreach (TypeConfiguration.MappedField index in m_mappings[dbitem.GetType()].IndexFields)
								m_cache[item.GetType(), index.Databasefield, index.Field.GetValue(dbitem)] = dbitem;
						}
						else
							toAdd = item;
					}

					if (toAdd != null) res.Add(toAdd);
				}
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}

			//TODO: Should not fire events, when the result is cached?
			foreach (object obj in res)
				OnAfterDataConnection(obj, DataActions.Fetch);

			return res.ToArray();
		}

		/// <summary>
		/// This will delete the object ... same as RemoveObject
		/// </summary>
		/// <param name="item"></param>
		public override void DeleteObject(object item)
		{
			IDataClass obj = (IDataClass)item;
			ObjectStates oldstate = obj.ObjectState;
			try
			{
				m_cache.Lock.AcquireWriterLock(-1);
				bool removed = true;
				foreach (TypeConfiguration.MappedField index in m_mappings[obj.GetType()].IndexFields)
					removed = removed & m_cache.RemoveObjectFromIndex(obj.GetType(), index.Databasefield, index.Field.GetValue(obj), obj);
				if (obj.ObjectState != ObjectStates.New)	//only add object 1 time
				{
					//check if it's already in delete queue

                    //Manual build of
                    //Query.Parse("(GetType() IS ?) AND (" + m_mappings[item.GetType()].PrimaryKey.Property.Name + " = ?)", item.GetType(), m_mappings[item.GetType()].PrimaryKey.Field.GetValue(item));
					//QueryModel.Operation op =
					//    Query.And(
					//        Query.Is(
					//            Query.FunctionCall("GetType"),
					//            Query.Value(item.GetType())
					//        ),
					//        Query.Equal(
					//            Query.Property(m_mappings[item.GetType()].PrimaryKey.Property.Name),
					//            Query.Value(m_mappings[item.GetType()].PrimaryKey.Field.GetValue(item))
					//        )
					//    );
					//if (Query.FindFirst(op, m_cache.DeletedObjects) == null) m_cache.DeletedObjects.Add(obj);

					//we assume pks to be not null and unique
					string deletekey = obj.GetType().FullName + (char)1 + m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj).ToString();
					if (!m_cache.DeletedObjects.ContainsKey(deletekey)) m_cache.DeletedObjects.Add(deletekey, obj);
				}
				(obj as DataClassBase).m_state = ObjectStates.Deleted;
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
			if (ObjectAddRemove != null) ObjectAddRemove(this, obj, oldstate, ObjectStates.Deleted);
		}

		/// <summary>
		/// This will load the given DataClassBase object
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public override object GetObjectById(Type type, object id)
		{
			if (GetDataClassLevel(type) < DataClassLevels.Base) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			//load if needed
			if (m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id] == null)		//will these need locks?
			{
				QueryModel.Operation op = Query.Equal(Query.Property(m_mappings[type].PrimaryKey.Databasefield), Query.Value(id));
				if (!HasLoaded(type, op))
				{
					object[] items = LoadObjects(type, op);
					if (items != null && items.Length != 0) InsertObjectsInCache(items);
				}
			}

			if (m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id] != null)		//will these need locks?
				return m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id];	//will these need locks?
			else
				return null;
		}

		/// <summary>
		/// This will assyncronly load and cache the given types, in a safe way
		/// </summary>
		/// <param name="types"></param>
		public virtual void LoadAndCacheObjects(params Type[] types)
		{
			if (types == null || types.Length == 0) return;

			//investigate types
			List<Type> typestoload = new List<Type>();
			foreach (Type type in types)
				if (!HasLoaded(type, new QueryModel.Operation(QueryModel.Operators.NOP))) typestoload.Add(type);
			if (typestoload.Count == 0) return;

			//start your threadings!
			AssyncronObjectLoader[] assyncronloaders = new AssyncronObjectLoader[typestoload.Count];
			for (int i = 0; i < typestoload.Count; i++)
				assyncronloaders[i] = new AssyncronObjectLoader(this, typestoload[i]);
			for (int i = 0; i < assyncronloaders.Length; i++)
				assyncronloaders[i].BeginThread();

			//now we shall wait for the threads to finish
			for (int i = 0; i < assyncronloaders.Length; i++)
				assyncronloaders[i].Event.WaitOne();

			//And now! We shall filter the loaded objects into the cache
			foreach (AssyncronObjectLoader ol in assyncronloaders)
				if (ol.Exception != null) throw new Exception("Couldn't load type " + ol.TypeToLoad.Name + "\nError: " + ol.Exception.Message);
				else InsertObjectsInCache(ol.LoadedObjects);

			//and done
		}

		/// <summary>
		/// This will assyncronly load some given objects (used by LoadAndCacheObjects)
		/// </summary>
		private class AssyncronObjectLoader
		{
			private DataFetcher m_fetcher;
			private Type m_typetoload;
			private object[] m_loadedobjects;
			private Exception m_exception;
			private System.Threading.ManualResetEvent m_event;

			public object[] LoadedObjects { get { return m_loadedobjects; } }
			public Exception Exception { get { return m_exception; } }
			public Type TypeToLoad { get { return m_typetoload; } }
			public System.Threading.ManualResetEvent Event { get { return m_event; } }

			public AssyncronObjectLoader(IDataFetcher templatefetcher, Type typetoload)
			{
				m_typetoload = typetoload;
				m_event = new System.Threading.ManualResetEvent(false);

				//make copy of connection, so that we won't interfere with our parent ... MUAHAHAHAHAHAHAH!!!!! HELT STILLE SOM EN NINJA!
				IDataProvider provider;
				provider = (IDataProvider)Activator.CreateInstance(templatefetcher.Provider.GetType());
				provider.Connection = (IDbConnection)Activator.CreateInstance(templatefetcher.Provider.Connection.GetType());
				provider.ConnectionString = templatefetcher.Provider.OriginalConnectionString;
				m_fetcher = new DataFetcher(provider);

				//also copy the type mappings ... CHEATER!!!
				m_fetcher.Mappings = templatefetcher.Mappings;		//this will be a shared resource then. Beware.
			}

			public void BeginThread()
			{
				System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(this.StartLoad));
			}

			private void StartLoad(object dummy)
			{
				try
				{
					m_loadedobjects = m_fetcher.GetObjects(m_typetoload);
				}
				catch (Exception ex)
				{
					m_exception = ex;
				}
				m_event.Set();
			}
		}

		/// <summary>
		/// This will retrive an object, starting with a search in the indexed cache
		/// </summary>
		/// <param name="type"></param>
		/// <param name="indexname"></param>
		/// <param name="indexvalue"></param>
		/// <returns></returns>
		public virtual object GetObjectByIndex(Type type, string indexname, object indexvalue)
		{
			//search cache
			IDataClass obj = m_cache[type, indexname, indexvalue];		//will this need lock?
			if (obj != null) return obj;

			//check if we have loaded all
			if (m_loadreducer.ContainsKey(type) && m_loadreducer[type].ContainsKey("")) 
				return null;
			else //search DB
				return GetObject(type, m_mappings[type][indexname].Databasefield + "=?", indexvalue);
		}

		/// <summary>
		/// This will retrive an object, starting with a search in the indexed cache
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="indexname"></param>
		/// <param name="indexvalue"></param>
		/// <returns></returns>
		public DATACLASS GetObjectByIndex<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass
		{
			return (DATACLASS)GetObjectByIndex(typeof(DATACLASS), indexname, indexvalue);
		}

		/// <summary>
		/// This will retrive a list of objects, starting with a search in the indexed cache
		/// BEWARE! The DB *will* be searched the first time
		/// </summary>
		/// <param name="type"></param>
		/// <param name="indexname"></param>
		/// <param name="indexvalue"></param>
		/// <returns></returns>
		public virtual object[] GetObjectsByIndex(Type type, string indexname, object indexvalue)
		{
			//First search DB ... the loadreducer will prevent multiple searches
			//GetObjects(type, m_mappings[type][indexname].Databasefield + "=?", indexvalue);	//this will evaluate whole cache
			QueryModel.Operation op = Query.Equal(Query.Property(m_mappings[type][indexname].Databasefield), Query.Value(indexvalue));
			if (!HasLoaded(type, op)) InsertObjectsInCache(LoadObjects(type, op));

			//search cache
			Array objs = null;
			try
			{
				m_cache.Lock.AcquireReaderLock(-1);
				objs = (Array)m_cache.GetObjects(type, indexname, indexvalue);
			}
			finally
			{
				m_cache.Lock.ReleaseReaderLock();
			}
			return (object[])objs;
		}

		/// <summary>
		/// This will retrive a list of objects, starting with a search in the indexed cache
		/// BEWARE! The DB *will* be searched the first time
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="indexname"></param>
		/// <param name="indexvalue"></param>
		/// <returns></returns>
		public DATACLASS[] GetObjectsByIndex<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass
		{
			return (DATACLASS[])(Array)GetObjectsByIndex(typeof(DATACLASS), indexname, indexvalue);
		}

		/// <summary>
		/// This will add an index to the cache and to the model
		/// </summary>
		/// <param name="type"></param>
		/// <param name="indexname"></param>
		public void AddIndex(Type type, string indexname)
		{
			if (m_cache.HasIndex(type, indexname)) return;		//meh

			//add to model
			TypeConfiguration.MappedField field = null;
			try
			{
				field = m_mappings[type][indexname];
				field.Index = true;
				m_mappings[type].IndexFields.AddLast(field);
			}
			catch
			{
				throw new Exception("Couldn't find column \"" + indexname + "\" in " + type.Name);
			}

			//add to cache
			try
			{
				m_cache.Lock.AcquireWriterLock(-1);
				m_cache.AddIndex(type, indexname);

				//index all existing objects
				foreach (IDataClass obj in this.GetObjects(type))
					m_cache[type, indexname, field.Field.GetValue(obj)] = obj;
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// This will remove an index from the cache and the model
		/// </summary>
		/// <param name="type"></param>
		/// <param name="indexname"></param>
		public void RemoveIndex(Type type, string indexname)
		{
			if (!m_cache.HasIndex(type, indexname)) return;		//meh

			//remove from model
			try
			{
				TypeConfiguration.MappedField field = m_mappings[type][indexname];
				field.Index = false;
				m_mappings[type].IndexFields.Remove(field);
			}
			catch
			{
				throw new Exception("Couldn't find column \"" + indexname + "\" in " + type.Name);
			}

			//remove from cache
			try
			{
				m_cache.Lock.AcquireWriterLock(-1);
				m_cache.RemoveIndex(type, indexname);
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// Will hook an object to the internal system
		/// </summary>
		/// <param name="newobj"></param>
		public override IDataClass Add(IDataClass newobj)
		{
            HookObject(newobj);

			try
			{
				m_cache.Lock.AcquireWriterLock(-1);
				foreach (TypeConfiguration.MappedField index in m_mappings[newobj.GetType()].IndexFields)
					m_cache[newobj.GetType(), index.Databasefield, index.Field.GetValue(newobj)] = newobj;
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}

            if (ObjectAddRemove != null) ObjectAddRemove(this, newobj, ObjectStates.New, ObjectStates.New);
			return newobj;
		}

		/// <summary>
		/// This will commit the objects to the database
		/// </summary>
		/// <param name="items"></param>
		public override void Commit(params IDataClass[] items)
		{
            try
            {
                //TODO: Should be in DataFetcher (base class)?
                m_transactionlock.AcquireWriterLock(-1); //we can run this function (Commit) in multiple threads. Hence the readerlock

                List<string> delkeys = new List<string>();
                if (items != null && items.Length > 0)
                    foreach (IDataClass obj in items)
                        if (obj.ObjectState == ObjectStates.Deleted)
                            delkeys.Add(obj.GetType().FullName + (char)1 + m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj).ToString());

                base.Commit(items);

                foreach (string k in delkeys)
                    m_cache.DeletedObjects.Remove(k);

            }
            finally 
            {
                m_transactionlock.ReleaseWriterLock();
            }
		}

		/// <summary>
		/// Clears the cache, and removes all non-modified items 
		/// </summary>
		public virtual void ClearCache()
		{
			lock (m_loadreducer)
			{
				m_loadreducer.Clear();
			}
			try
			{
				m_cache.Lock.AcquireWriterLock(-1);
				m_cache.ClearAllUnchanged();
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// This will keep an eye out for rougue indexes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="propertyname"></param>
		/// <param name="oldvalue"></param>
		/// <param name="newvalue"></param>
		protected override void OnAfterDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			//check if it's an index changing	(Will this be a performance killer I wonder?)
			if (m_mappings[sender.GetType()][propertyname].Index)
			{
				try
				{
					m_cache.Lock.AcquireWriterLock(-1);
					m_cache.ReindexObject(sender, propertyname, oldvalue, newvalue);
				}
				finally
				{
					m_cache.Lock.ReleaseWriterLock();
				}
			}

			//send event
			base.OnAfterDataChange(sender, propertyname, oldvalue, newvalue);
		}

		/// <summary>
		/// Refreshes all values with those read from the datasource and makes sure that the object is still in cache
		/// </summary>
		/// <param name="obj"></param>
		public override void RefreshObject(IDataClass obj)
		{
			base.RefreshObject(obj);
			if (!m_cache.Contains(obj)) InsertObjectsInCache(obj);	//this can accour after a ClearCache
		}

		public override void Dispose()
		{
			base.Dispose();
			m_cache.Dispose();
			m_cache = null;
			m_loadreducer = null;
			GC.SuppressFinalize(this);
		}

	}

}
