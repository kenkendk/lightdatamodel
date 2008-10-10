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
	public class DataFetcherCached : DataFetcher, IDataFetcherCached
	{
		protected Cache m_cache = new Cache();
		protected Dictionary<Type, Dictionary<string, string>> m_loadreducer = new Dictionary<Type, Dictionary<string, string>>();
		private System.Threading.ReaderWriterLock m_transactionlock = new System.Threading.ReaderWriterLock();

		public event ObjectStateChangeHandler ObjectAddRemove;

		#region " Cache "

		/// <summary>
		/// This is not thread safe. The fetcher is though
		/// </summary>
		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		public class Cache : IEnumerable<IDataClass>, IDisposable
		{
			//this is the actual cache
			//Type, Indexname, (Indexvalue & IDataClass)
			private Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> m_list = new Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>>();
			protected List<IDataClass> m_deletedobjects = new List<IDataClass>();
			public List<IDataClass> DeletedObjects { get { return m_deletedobjects; } }
			public System.Threading.ReaderWriterLock Lock = new System.Threading.ReaderWriterLock();

			public IDataClass this[Type type, string indexname, object indexvalue]
			{
				get { return m_list[type][indexname][indexvalue]; }
				set
				{
					try
					{
						//we assume that the keys already exists ... see ReaderWriterLock
						m_list[type][indexname][indexvalue] = value;
					}
					catch
					{
						//the keys doesn't exists ... this is very expensive (the try-catch that is)
						if (!m_list.ContainsKey(type)) m_list.Add(type, new Dictionary<string, MultiDictionary<object, IDataClass>>());
						if (!m_list[type].ContainsKey(indexname)) m_list[type].Add(indexname, new MultiDictionary<object, IDataClass>());
						m_list[type][indexname][indexvalue] = value;
					}
				}
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
				RemoveObject(obj.GetType(), indexname, oldvalue, (IDataClass)obj);
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

			/// <summary>
			/// This will return a list of objects, if a such is needed (unique indexes doesn't)
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			/// <param name="indexvalue"></param>
			/// <returns></returns>
			public IDataClass[] GetObjects(Type type, string indexname, object indexvalue)
			{
				ICollection<IDataClass> list = m_list[type][indexname].Items[indexvalue];
				IDataClass[] ret = new IDataClass[list == null ? 0 : list.Count];
				int c = 0;
				foreach (IDataClass itm in list)
					ret[c++] = itm;
				return ret;
			}

			public DATACLASS[] GetObjects<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass
			{
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
					if (!m_parent.m_list.ContainsKey(m_type)) return new MultiDictionary<object, IDataClass>.ValueCollection<object, IDataClass>.Enumerator<IDataClass>(null);
					Dictionary<string, MultiDictionary<object, IDataClass>>.Enumerator e = m_parent.m_list[m_type].GetEnumerator();
					e.MoveNext();
					if (e.Current.Value != null) return e.Current.Value.Values.GetEnumerator();
					return new MultiDictionary<object, IDataClass>.ValueCollection<object, IDataClass>.Enumerator<IDataClass>(null);
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
				return new TypeObjectCollection(this, type);
			}

			/// <summary>
			/// This will remove all non deity objects
			/// </summary>
			public void ClearAllUnchanged()
			{
				foreach (KeyValuePair<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> type in m_list)
					foreach (KeyValuePair<string, MultiDictionary<object, IDataClass>> index in type.Value)
					{
						foreach (KeyValuePair<object, IDataClass> obj in index.Value)
							if (!obj.Value.IsDirty) index.Value.Remove(obj.Key, obj.Value);
					}
			}

			/// <summary>
			/// This is not nessesary to call. But it will improve performance
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			public void AddIndex(Type type, string indexname)
			{
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
				return m_list.Remove(type);
			}

			/// <summary>
			/// This is used for removing the exact object. Eg. if the index is out of sync
			/// </summary>
			/// <param name="item"></param>
			/// <returns></returns>
			public bool RemoveObject(IDataClass item)
			{
				throw new Exception("This is prolly too costly");
			}

			/// <summary>
			/// This is used for removing an object
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			/// <param name="indexvalue"></param>
			/// <returns></returns>
			public bool RemoveObject(Type type, string indexname, object indexvalue, IDataClass item)
			{
				return m_list[type][indexname].Remove(indexvalue, item);
			}

			public IEnumerator<IDataClass> GetEnumerator()
			{
				return new Enumerator(this.m_list);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(this.m_list);
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
			//only 1 thread will enter this

			//pre-add indexes (gives a slight startup performance boost)
			foreach (TypeConfiguration.MappedClass type in m_mappings)
				foreach (TypeConfiguration.MappedField index in type.IndexFields)
					m_cache.AddIndex(type.Type, index.Databasefield);
		}

		/// <summary>
		/// Commits all cached objects to data source
		/// </summary>
		public virtual void CommitAll()
		{
			Guid transactionID = Guid.NewGuid();
			bool inTransaction = false;

			LinkedList<IDataClass> deletedobjects = null;
			LinkedList<IDataClass> newobjects = null;
			LinkedList<IDataClass> updatedobjects = null;
			try
			{
				m_transactionlock.AcquireWriterLock(-1);		//only 1 of this function (CommitAll) may run

				//get objects
				try
				{
					m_cache.Lock.AcquireReaderLock(-1);
					deletedobjects = new LinkedList<IDataClass>(m_cache.DeletedObjects);
					newobjects = new LinkedList<IDataClass>();
					updatedobjects = new LinkedList<IDataClass>();
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

				//start commiting
				m_provider.BeginTransaction(transactionID); //SQLite can only handle 1 transaction ... and not writing from multiple threads isn't that bad, is it?
				inTransaction = true;

				try
				{
					//delete (First we delete. In case we've delete a primary key, that also will be inserted)
					foreach (IDataClass c in deletedobjects)
						base.Commit(c); //no lock

					//update (second we update. In case we changed some primary keys, that also will be inserted)
					foreach (IDataClass c in updatedobjects)
						base.Commit(c, false); //no lock

					//create
					foreach (IDataClass c in newobjects)
						base.Commit(c, false); //no lock

					m_provider.CommitTransaction(transactionID);
					inTransaction = false;
				}
				finally
				{
					if (inTransaction) m_provider.RollbackTransaction(transactionID);
				}

				if (inTransaction) throw new Exception("Can this happen????");

			}
			finally
			{
				m_transactionlock.ReleaseWriterLock();
			}

			//refresh objects ... only do this if the transaction is a success ... kind of "commit objects"
			foreach (IDataClass c in newobjects)
				RefreshObject(c);
			foreach (IDataClass c in updatedobjects)
				RefreshObject(c);

			//commit objects to cache
			if (deletedobjects.Count > 0)
			{
				try
				{
					m_cache.Lock.AcquireWriterLock(-1);
					foreach (IDataClass c in deletedobjects)
						m_cache.DeletedObjects.Remove(c);
				}
				finally
				{
					m_cache.Lock.ReleaseWriterLock();
				}
			}

			//recursive commit
			if (deletedobjects.Count > 0 || newobjects.Count > 0 || updatedobjects.Count > 0) CommitAll();

		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hook into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="type">The type of objects to load</param>
		/// <param name="operation">The filter used to select objects</param>
		/// <returns>All matching objects</returns>
		public override DATACLASS[] GetObjects<DATACLASS>(QueryModel.Operation operation)
		{
			if (!HasLoaded(typeof(DATACLASS), operation))
				InsertObjectsInCache(LoadObjects(typeof(DATACLASS), operation));		//this is locked

			try
			{
				m_cache.Lock.AcquireReaderLock(-1);
				return operation.EvaluateList<DATACLASS>(m_cache.GetObjects(typeof(DATACLASS)));
			}
			finally
			{
				m_cache.Lock.ReleaseReaderLock();
			}
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
				return operation.EvaluateList(m_cache.GetObjects(type));
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
		public void DiscardObject(IDataClass obj)
		{
			//lock (m_loadreducer)
			{
				m_loadreducer.Clear();
			}
			//}

			try
			{
				m_cache.Lock.AcquireWriterLock(-1);
				foreach (TypeConfiguration.MappedField index in m_mappings[obj.GetType()].IndexFields)
				{
					m_cache.RemoveObject(obj.GetType(), index.Databasefield, index.Field.GetValue(obj), obj);
				}
			}
			finally
			{
				m_cache.Lock.ReleaseWriterLock();
			}
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
			return GetObjectsFromCache<DATACLASS>(QueryModel.Parser.ParseQuery(filter, parameters)); //this has lock
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
			try
			{
				m_cache.Lock.AcquireReaderLock(-1);
				return query.EvaluateList<DATACLASS>(m_cache.GetObjects(typeof(DATACLASS)));
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
				return query.EvaluateList(m_cache.GetObjects(type));
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
			return GetObjectsFromCache(type, QueryModel.Parser.ParseQuery(filter, parameters)); //this has lock
		}

		/// <summary>
		/// This will insert a given list of objects into the cache
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		protected IDataClass[] InsertObjectsInCache(object[] data)
		{
			List<IDataClass> res = new List<IDataClass>();

			try
			{
				m_cache.Lock.AcquireWriterLock(-1);

				foreach (IDataClass item in data)
				{
					IDataClass toAdd = item;
					if (item as DataClassBase != null)
					{
						DataClassBase dbitem = item as DataClassBase;
						if (m_cache[item.GetType(), m_mappings[dbitem.GetType()].PrimaryKey.Databasefield, m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem)] == null)
						{
							QueryModel.Operation opdeleted = QueryModel.Parser.ParseQuery("GetType.FullName = ? AND " + m_mappings[dbitem.GetType()].PrimaryKey.Property.Name + " = ?", item.GetType().FullName, m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem));
							if (opdeleted.EvaluateList(m_cache.DeletedObjects).Length > 0) continue;

							dbitem.m_state = ObjectStates.Default;
							dbitem.m_isdirty = false;

							HookObject(dbitem);
							foreach (TypeConfiguration.MappedField index in m_mappings[dbitem.GetType()].IndexFields)
								m_cache[item.GetType(), index.Databasefield, index.Field.GetValue(dbitem)] = dbitem;
						}
						else
							toAdd = item;
					}
					else if (item as DataClassView != null)
					{
						(item as DataClassView).m_dataparent = this;
						m_cache[item.GetType(), "GetHashCode", item.GetHashCode()] = (IDataClass)item;
						toAdd = (IDataClass)item;
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
				m_cache.RemoveObject(item.GetType(), m_mappings[item.GetType()].PrimaryKey.Databasefield, m_mappings[item.GetType()].PrimaryKey.Field.GetValue(obj), obj);
				if(obj.ObjectState != ObjectStates.New) m_cache.DeletedObjects.Add(obj);
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
				QueryModel.Operation op = QueryModel.Parser.ParseQuery(m_mappings[type].PrimaryKey.Databasefield + "=?", id);
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

			//search DB
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
			GetObjects(type, m_mappings[type][indexname].Databasefield + "=?", indexvalue);

			//search cache
			IDataClass[] objs = null;
			try
			{
				m_cache.Lock.AcquireReaderLock(-1);
				objs = m_cache.GetObjects(type, indexname, indexvalue);
			}
			finally
			{
				m_cache.Lock.ReleaseReaderLock();
			}
			return objs;
		}

		/// <summary>
		/// This will retrive a list of objects, starting with a search in the indexed cache
		/// BEWARE! The DB *will* be searched the first time
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="indexname"></param>
		/// <param name="indexvalue"></param>
		/// <returns></returns>
		public virtual DATACLASS[] GetObjectsByIndex<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass
		{
			//First search DB ... the loadreducer will prevent multiple searches
			GetObjects<DATACLASS>(m_mappings[typeof(DATACLASS)][indexname].Databasefield + "=?", indexvalue);

			//search cache
			DATACLASS[] objs = null;
			try
			{
				m_cache.Lock.AcquireReaderLock(-1);
				objs = m_cache.GetObjects<DATACLASS>(indexname, indexvalue);
			}
			finally
			{
				m_cache.Lock.ReleaseReaderLock();
			}
			return objs;
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
			(newobj as DataClassBase).m_state = ObjectStates.New;
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

			HookObject(newobj);
			if (ObjectAddRemove != null) ObjectAddRemove(this, newobj, ObjectStates.New, ObjectStates.New);
			return newobj;
		}

		/// <summary>
		/// This will commit the object to the database
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="refreshobject"></param>
		public override void Commit(IDataClass obj, bool refreshobject)
		{
			//new object?
			if ((obj as DataClassBase).m_dataparent == null) Add(obj);

			try
			{
				m_transactionlock.AcquireReaderLock(-1);		//we can run this function (Commit) in multiple threads. Hence the readerlock

				//sql
				if (obj.ObjectState == ObjectStates.Default)
				{
					base.Commit(obj, refreshobject);
				}
				else if (obj.ObjectState == ObjectStates.New)
				{
					base.Commit(obj, refreshobject);		//TODO: Beware of the events
				}
				else if (obj.ObjectState == ObjectStates.Deleted)
				{
					base.Commit(obj, refreshobject); //TODO: Beware of the events
					try
					{
						m_cache.Lock.AcquireWriterLock(-1);
						m_cache.DeletedObjects.Remove(obj);
					}
					finally
					{
						m_cache.Lock.ReleaseWriterLock();
					}
				}
			}
			finally
			{
				m_transactionlock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Clears the cache, and removes all non-modified items 
		/// </summary>
		public void ClearCache()
		{
			//lock (m_loadreducer)
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
			if (m_mappings[sender.GetType()][propertyname].Index) m_cache.ReindexObject(sender, propertyname, oldvalue, newvalue);

			//send event
			base.OnAfterDataChange(sender, propertyname, oldvalue, newvalue);
		}

		public override void Dispose()
		{
			base.Dispose();
			m_cache.Dispose();
			m_cache = null;
			m_loadreducer = null;
		}

	}

}
