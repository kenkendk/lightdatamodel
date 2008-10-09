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
		//protected SortedList<string, SortedList<object, IDataClass>> m_cache = new SortedList<string, SortedList<object, IDataClass>>();		//object = PrimaryKeyValue
		protected Cache m_cache = new Cache();
        protected Dictionary<Type, Dictionary<string, string>> m_loadreducer;

		public event ObjectStateChangeHandler ObjectAddRemove;

		#region " Cache "

		[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
		public class Cache : IEnumerable<IDataClass>, IDisposable
		{
			//this is the actual cache
			//Type, Indexname, (Indexvalue & IDataClass)
			private Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> m_list = new Dictionary<Type, Dictionary<string, MultiDictionary<object, IDataClass>>>();

			protected MultiDictionary<Type, IDataClass> m_newobjects = new MultiDictionary<Type, IDataClass>();
			protected List<IDataClass> m_deletedobjects = new List<IDataClass>();

			public MultiDictionary<Type, IDataClass> NewObjects { get { return m_newobjects; } }
			public List<IDataClass> DeletedObjects { get { return m_deletedobjects; } }

			public IDataClass this[Type type, string indexname, object indexvalue]
			{
				get { return m_list[type][indexname][indexvalue]; }
				set
				{
					try
					{
						//we assume that the keys already exists ... see ReaderWriterLock
						//lock (m_list[type][indexname])
						{
							m_list[type][indexname][indexvalue] = value;
						}
					}
					catch
					{
						//the keys doesn't exists ... this is very expensive (the try-catch that is)
						//lock (type)
						{
							if (!m_list.ContainsKey(type)) m_list.Add(type, new Dictionary<string, MultiDictionary<object, IDataClass>>());
							if (!m_list[type].ContainsKey(indexname)) m_list[type].Add(indexname, new MultiDictionary<object, IDataClass>());
							m_list[type][indexname][indexvalue] = value;
						}
					}
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
					foreach (KeyValuePair<Type, Dictionary<string, MultiDictionary<object, IDataClass>>> itm in m_list)
					{
						foreach (KeyValuePair<string, MultiDictionary<object, IDataClass>> itm2 in itm.Value)
						{
							return itm2.Value.Count + m_newobjects.Count;
						}
					}
					return m_newobjects.Count;
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
					return new Enumerator(m_parent, m_type);
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return new Enumerator(m_parent, m_type);
				}

				public class Enumerator : IEnumerator<IDataClass>
				{
					private Cache m_parent;
					private Type m_type;
					private IEnumerator<KeyValuePair<object, IDataClass>> m_mainenumerator;
					private IEnumerator<IDataClass> m_newobjectenumerator;

					public Enumerator(Cache parent, Type type)
					{
						m_parent = parent;
						m_type = type;
						Reset();
					}

					public IDataClass Current
					{
						get 
						{
							if (m_mainenumerator != null) return m_mainenumerator.Current.Value;
							else return m_newobjectenumerator.Current;
						}
					}

					public void Dispose()
					{
						m_parent = null;
						m_type = null;
						if (m_mainenumerator != null) m_mainenumerator.Dispose();
						if (m_newobjectenumerator != null) m_newobjectenumerator.Dispose();
						m_mainenumerator = null;
						m_newobjectenumerator = null;
					}

					object IEnumerator.Current
					{
						get { return Current; }
					}

					public bool MoveNext()
					{
						if (m_mainenumerator == null)
						{
							if (m_newobjectenumerator == null) return false;
							return m_newobjectenumerator.MoveNext();
						}
						else
						{
							if (!m_mainenumerator.MoveNext())
							{
								m_mainenumerator = null;
								if (m_newobjectenumerator == null) return false;
								return m_newobjectenumerator.MoveNext();
							}
							else return true;
						}
					}

					public void Reset()
					{
						if (m_parent.m_list.ContainsKey(m_type))
						{
							Dictionary<string, MultiDictionary<object, IDataClass>>.Enumerator e = m_parent.m_list[m_type].GetEnumerator();
							e.MoveNext();
							if (e.Current.Value != null) m_mainenumerator = e.Current.Value.GetEnumerator();
						}
						ICollection<IDataClass> values = m_parent.m_newobjects.Items[m_type];
						if (values != null) m_newobjectenumerator = values.GetEnumerator();
					}
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
						//lock (index.Value)
						{
							foreach (KeyValuePair<object, IDataClass> obj in index.Value)
								if (!obj.Value.IsDirty) index.Value.Remove(obj.Key, obj.Value);
						}
					}
			}

			/// <summary>
			/// This is not nessesary to call. But it will improve performance
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			public void AddIndex(Type type, string indexname)
			{
				//lock (type)
				{
					if (!m_list.ContainsKey(type)) m_list.Add(type, new Dictionary<string, MultiDictionary<object, IDataClass>>());
					if (!m_list[type].ContainsKey(indexname)) m_list[type].Add(indexname, new MultiDictionary<object, IDataClass>());
				}
			}

			/// <summary>
			/// If you ever want to remove an index ... this is it
			/// </summary>
			/// <param name="type"></param>
			/// <param name="indexname"></param>
			/// <returns></returns>
			public bool RemoveIndex(Type type, string indexname)
			{
				//lock (type)
				{
					if (!m_list.ContainsKey(type)) return false;
					return m_list[type].Remove(indexname);
				}
			}

			/// <summary>
			/// If you ever want to remove an index ... this is it
			/// </summary>
			/// <param name="type"></param>
			/// <returns></returns>
			public bool RemoveIndex(Type type)
			{
				//lock (type)
				{
					return m_list.Remove(type);
				}
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
				//lock (m_list[type][indexname])
				{
					return m_list[type][indexname].Remove(indexvalue, item);
				}
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
					if (m_objectenumerator!=null) m_objectenumerator.Dispose();
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
						} while (m_objectenumerator == null || m_objectenumerator.Current.Equals(default(KeyValuePair<object, IDataClass>)));
					}
					return true;
				}

				public void Reset()
				{
					m_typeenumerator = m_parent.GetEnumerator();
					if(!m_typeenumerator.MoveNext()) return;
					Dictionary<string, MultiDictionary<object, IDataClass>>.Enumerator e = m_typeenumerator.Current.Value.GetEnumerator();
					if(!e.MoveNext()) return;
					m_objectenumerator = e.Current.Value.GetEnumerator();
				}
			}

			public void Dispose()
			{
				m_deletedobjects = null;
				m_newobjects = null;
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
				if (m_cache.NewObjects.Count > 0 || m_cache.DeletedObjects.Count > 0) return true;

				foreach(IDataClass obj in m_cache)
					if (obj.IsDirty) return true;

				return false;
			}
		}

		public Cache LocalCache { get { return m_cache; } }

        /// <summary>
        /// Constructs a new cached data fetcher
        /// </summary>
        /// <param name="provider">The provider to use</param>
		public DataFetcherCached(IDataProvider provider) : base(provider)
		{
		//    //if (m_mappings.RelationConfig.GetAvaliblePropKeys().Length != 0)
		//    //    m_relationManager = new RelationManager(this);
		//    //else
		//    //{
		//    //    //If the user creates the relations later, we need to create it
		//    //    m_mappings.RelationConfig.AddedRelation += new EventHandler(RelationConfig_AddedRelation);
		//    //    m_relationManager = null;
		//    //}

			m_mappings.TypesInitialized += new EventHandler(m_mappings_TypesInitialized);
		}

		private void m_mappings_TypesInitialized(object sender, EventArgs e)
		{
			//pre-add indexes (gives a slight startup performance boost)
			foreach (TypeConfiguration.MappedClass type in m_mappings)
				foreach (TypeConfiguration.MappedField index in type.IndexFields)
					m_cache.AddIndex(type.Type, index.Databasefield);
		}

		//void RelationConfig_AddedRelation(object sender, EventArgs e)
		//{
		//    if (m_relationManager == null)
		//        m_relationManager = new RelationManager(this);
		//}

        //#region Provider interactions
        /* The methods in this regions does all interaction with the provider.
         * This approach enables easy overrides of provider interaction,
         * when using the Nested provider.
         * 
         * It also removes clutter and varying ways to interact with the provider
         */

        /// <summary>
        /// This inserts an object into the datasource
        /// </summary>
        /// <param name="obj">The object to insert</param>
		//protected override void InsertObject(object obj)
		//{
		//    base.InsertObject(obj);
		//    //if(m_relationManager != null)m_relationManager.SetExistsInDb((IDataClass)obj, true);		//Is this still needed?
		//}
        //#endregion

        /// <summary>
        /// Commits all cached objects to data source
        /// </summary>
		public virtual void CommitAll()
		{
			Guid transactionID = Guid.NewGuid();
			bool inTransaction = false;

			lock (m_provider)		//SQLite can only handle 1 transaction ... and not writing from multiple threads isn't that bad, is it?
			{
				m_provider.BeginTransaction(transactionID);
				inTransaction = true;

				try
				{
					bool hasupdated;
					do
					{
						hasupdated = false;

						//delete (First we delete. In case we've delete a primary key, that also will be inserted)
						LinkedList<IDataClass> deletedobjects = new LinkedList<IDataClass>(m_cache.DeletedObjects);
						foreach (IDataClass c in deletedobjects)
							this.Commit(c);

						//create
						LinkedList<IDataClass> newobjects = new LinkedList<IDataClass>(m_cache.NewObjects.Values);
						foreach (IDataClass c in newobjects)
							this.Commit(c);

						//update
						foreach (IDataClass obj in m_cache)
							if (obj.IsDirty)
							{
								this.Commit(obj);
								hasupdated = true;
							}
					}
					while (m_cache.NewObjects.Count > 0 || m_cache.DeletedObjects.Count > 0 || hasupdated);		//in case the save procedures alters something
					m_provider.CommitTransaction(transactionID);
					inTransaction = false;
				}
				finally
				{
					if (inTransaction) m_provider.RollbackTransaction(transactionID);
				}
			}
		}

		///// <summary>
		///// This will load a list of arbitary objects
		///// If the given object is a DataClassBase it will be hooked into the DataFetcher
		///// DataCustomClassBase will also have it's values filled
		///// All others will just be filled with the data
		///// </summary>
		///// <param name="type">The type of objects to load</param>
		///// <param name="filter">The filter used to select objects</param>
		///// <returns>All matching objects</returns>
		//public override DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters)
		//{
		//    //This ensures that we always query the memory
		//    return GetObjects<DATACLASS>(QueryModel.Parser.ParseQuery(filter, parameters));
		//}

		///// <summary>
		///// Geturns all objects of the given type
		///// </summary>
		///// <typeparam name="DATACLASS">The type to retrieve</typeparam>
		///// <returns></returns>
		//public override DATACLASS[] GetObjects<DATACLASS>()
		//{
		//    return GetObjects<DATACLASS>("");
		//}

		//public virtual DATACLASS[] GetObjects<DATACLASS>(string query) where DATACLASS : IDataClass
		//{
		//    return GetObjects<DATACLASS>(QueryModel.Parser.ParseQuery(query));
		//}

        /*/// <summary>
        /// This function checks if the query is for a primary key, and if so finds the results
        /// by looking in the hashtable, rather than itterating the list.
        /// </summary>
        /// <param name="operation">The operation to evaluate</param>
        /// <returns>Either null if the query was not for a primary key, or the resulting items</returns>
        public virtual DATACLASS[] IsQueryForID<DATACLASS>(QueryModel.Operation operation)
        {
            if (operation == null)
                return null;

            if (operation.Operator == System.Data.LightDatamodel.QueryModel.Operators.Equal
                || operation.Operator == System.Data.LightDatamodel.QueryModel.Operators.In)
            {
                if (operation.Parameters != null && operation.Parameters.Length == 2)
                {
                    if (!operation.Parameters[0].IsOperation
                        && !operation.Parameters[1].IsOperation)
                    {
                        if ((operation.Parameters[0] as QueryModel.Parameter).IsColumn
                            && !(operation.Parameters[1] as QueryModel.Parameter).IsColumn
                            && (operation.Parameters[0] as QueryModel.Parameter).Value == m_transformer.TypeConfiguration.UniqueColumn(typeof(DATACLASS)))
                        {
                            if (operation.Operator == System.Data.LightDatamodel.QueryModel.Operators.In)
                            {
                                //List<DATACLASS> lst = new List<DATACLASS>();
                                //foreach (object o in (IEnumerable)(operation.Parameters[1] as QueryModel.Parameter))
                                //{
                                //    object o = this.GetObjectById<DATACLASS>(o);
                                //    if (o != null)
                                //        lst.Add(o);
                                //}

                                return lst.ToArray();
                            }
                            else
                            {
                                DATACLASS c = this.GetObjectById<DATACLASS>((operation.Parameters[0] as QueryModel.Parameter).Value);
                                if (c == null)
                                    return new DATACLASS[0];
                                else
                                    return new DATACLASS[] { c };
                            }
                        }
                        else if ((operation.Parameters[0] as QueryModel.Parameter).IsColumn
                            && !(operation.Parameters[1] as QueryModel.Parameter).IsColumn
                            && (operation.Parameters[0] as QueryModel.Parameter).Value == m_transformer.TypeConfiguration.UniqueColumn(typeof(DATACLASS)))
                        {
                            if (operation.Operator == System.Data.LightDatamodel.QueryModel.Operators.In)
                            {
                                //List<DATACLASS> lst = new List<DATACLASS>();
                                //foreach (object o in (IEnumerable)(operation.Parameters[1] as QueryModel.Parameter).Value)
                                //{
                                //    object o = this.GetObjectById<DATACLASS>(o);
                                //    if (o != null)
                                //        lst.Add(o);
                                //}

                                //return lst.ToArray();
                            }
                            else
                            {
                                DATACLASS c = this.GetObjectById<DATACLASS>((operation.Parameters[1] as QueryModel.Parameter).Value);
                                if (c == null)
                                    return new DATACLASS[0];
                                else
                                    return new DATACLASS[] { c };
                            }
                        }
                    }
                }
            }

            return null;
        }*/

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
            //if (operation == null) return GetObjects<DATACLASS>();

			//string tablename = m_mappings[typeof(DATACLASS)].Tablename;

			if (!HasLoaded(typeof(DATACLASS), operation))
				InsertObjectsInCache(LoadObjects(typeof(DATACLASS), operation));

            /*DATACLASS[] tmp = IsQueryForID<DATACLASS>(operation);
            if (tmp != null)
                return tmp;*/

            //TODO: An enumerable collection that transparently itterates over a number of collections would make this more efficient
			//List<IDataClass> items = new List<IDataClass>(m_cache.GetObjects(typeof(DATACLASS)));
			//foreach (DATACLASS b in m_cache[tablename].Values)
			//    items.Add(b);

            //QueryModel.Operation filter = QueryModel.Parser.ParseQuery("GetType.FullName = ?", typeof(DATACLASS).FullName);
			//items.AddRange(filter.EvaluateList<IDataClass>(m_cache.NewObjects));

			return operation.EvaluateList<DATACLASS>(m_cache.GetObjects(typeof(DATACLASS)));
        }

		//public override object[] GetObjects(Type type)
		//{
		//    return GetObjects(type, "");
		//}

		//public override object[] GetObjects(Type type, string filter, params object[] parameters)
		//{
		//    return GetObjects(type, QueryModel.Parser.ParseQuery(filter, parameters));
		//}

		//public virtual object[] GetObjects(Type type, string query)
		//{
		//    return GetObjects(type, QueryModel.Parser.ParseQuery(query));
		//}

		public override object[] GetObjects(Type type, QueryModel.Operation operation)
		{
			//string tablename = m_mappings[type].Tablename;
			//if (!m_cache.ContainsKey(tablename))
			//    m_cache[tablename] = new SortedList<object, IDataClass>();

            if (!HasLoaded(type, operation))
			    InsertObjectsInCache(LoadObjects(type, operation));

			//ArrayList items = new ArrayList((ICollection)m_cache.GetObjects(type));

			//QueryModel.Operation filter = QueryModel.Parser.ParseQuery("GetType.FullName = ?", type.FullName);
			//items.AddRange(filter.EvaluateList(m_cache.NewObjects));

			return operation.EvaluateList(m_cache.GetObjects(type));
        }

        protected bool HasLoaded(Type type, QueryModel.Operation operation)
        {
            string eq = operation.ToString(false);
            if (eq != null)
            {
                if (m_loadreducer == null) m_loadreducer = new Dictionary<Type, Dictionary<string, string>>();
                if (!m_loadreducer.ContainsKey(type)) m_loadreducer.Add(type, new Dictionary<string, string>());

                if (m_loadreducer[type].ContainsKey(eq) || m_loadreducer[type].ContainsKey(""))
                    return true;
                else
                {
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
		public override void DiscardObject(IDataClass obj)
		{
            //string tablename = m_mappings[obj.GetType()].Tablename;
            
			//if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
			//if ((m_relationManager != null && m_relationManager.ExistsInDb(obj)) || m_relationManager == null)
			//if (m_cache[obj.GetType()].ContainsKey(m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj)))
			//    {
			//        m_cache[tablename].Remove(m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj));
                    m_loadreducer = null;
				//}

					foreach (TypeConfiguration.MappedField index in m_mappings[obj.GetType()].IndexFields)
					{
						m_cache.RemoveObject(obj.GetType(), index.Databasefield, index.Field.GetValue(obj), obj);
					}
        
            //if (m_relationManager != null) m_relationManager.UnregisterObject(obj);
        }

		public DATACLASS GetObjectFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			DATACLASS[] ret = GetObjectsFromCache<DATACLASS>(filter, parameters);
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
			return GetObjectsFromCache <DATACLASS>(QueryModel.Parser.ParseQuery(filter, parameters));
		}

		public DATACLASS GetObjectFromCache<DATACLASS>(QueryModel.Operation query) where DATACLASS : IDataClass
		{
			DATACLASS[] ret = GetObjectsFromCache < DATACLASS>(query);
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
            object[] tmp = GetObjectsFromCache(typeof(DATACLASS), query);
            DATACLASS[] res = new DATACLASS[tmp.Length];
            System.Array.Copy(tmp, res, res.Length);
            return res;
		}

		public object GetObjectFromCache(Type type, QueryModel.Operation query)
		{
			object[] ret = GetObjectsFromCache(type, query);
			if (ret != null && ret.Length > 0) return ret[0];
			return null;
		}

		public object[] GetObjectsFromCache(Type type, QueryModel.Operation query)
		{
			//string tablename = m_mappings[type].Tablename;
			//if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());

			//List<IDataClass> allItems = new List<IDataClass>();
			if (this as DataFetcherNested != null)
                if (!HasLoaded(type, query))
				    InsertObjectsInCache(LoadObjects(type, query));

			//allItems.AddRange(m_cache.GetObjects(type));
			//foreach (DataClassBase o in m_cache.NewObjects)
			//    if (o.GetType().IsAssignableFrom(type)) allItems.Add(o);

			return query.EvaluateList(m_cache.GetObjects(type));
		}

		public object GetObjectFromCache(Type type, string filter, params object[] parameters)
		{
			object[] ret = GetObjectsFromCache(type, filter, parameters);
			if (ret != null && ret.Length > 0) return ret[0];
			return null;
		}

		public object[] GetObjectsFromCache(Type type, string filter, params object[] parameters)
		{
			return GetObjectsFromCache(type, QueryModel.Parser.ParseQuery(filter, parameters));
		}

		protected IDataClass[] InsertObjectsInCache(object[] data)
        {
			List<IDataClass> res = new List<IDataClass>();

			foreach (IDataClass item in data)
            {
                IDataClass toAdd = item;
                if (item as DataClassBase != null)
                {
					//string tablename = m_mappings[item.GetType()].Tablename;
					//if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
                    DataClassBase dbitem = item as DataClassBase;
					if (m_cache[item.GetType(), m_mappings[dbitem.GetType()].PrimaryKey.Databasefield, m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem)] == null)
                    {
						QueryModel.Operation opdeleted = QueryModel.Parser.ParseQuery("GetType.FullName = ? AND UniqueValue = ?", item.GetType().FullName, m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem));
                        if (opdeleted.EvaluateList(m_cache.DeletedObjects).Length > 0) continue;

                        dbitem.m_state = ObjectStates.Default;
                        dbitem.m_isdirty = false;

                        HookObject(dbitem);

						//if (this as DataFetcherNested == null && m_relationManager != null) m_relationManager.SetExistsInDb(dbitem, true);
						//m_cache[tablename][m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem)] = dbitem;
						m_cache[item.GetType(), m_mappings[dbitem.GetType()].PrimaryKey.Databasefield, m_mappings[dbitem.GetType()].PrimaryKey.Field.GetValue(dbitem)] = dbitem;
                    }
                    else
						toAdd = item;
                }
                else if (item as DataClassView != null)
                {
					string viewname = m_mappings[item.GetType()].Tablename;
                    (item as DataClassView).m_dataparent = this;
					m_cache[item.GetType(), "GetHashCode", item.GetHashCode()] = (IDataClass)item;
					toAdd = (IDataClass)item;
                }

                //TODO: Should not fire events, when the result is cached?
                if (toAdd != null)
                {
                    res.Add(toAdd);
                    OnAfterDataConnection(toAdd, DataActions.Fetch);
                }

            }
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
			//string tablename = m_mappings[item.GetType()].Tablename;
			if (obj.ObjectState == ObjectStates.New)
			{
				m_cache.NewObjects.Remove(obj.GetType(), obj);
				//if(m_relationManager != null)m_relationManager.UnregisterObject(obj);
			}
			else
			{
				//if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
				m_cache.RemoveObject(item.GetType(), m_mappings[item.GetType()].PrimaryKey.Databasefield, m_mappings[item.GetType()].PrimaryKey.Field.GetValue(obj), obj);
				m_cache.DeletedObjects.Add(obj);
				(obj as DataClassBase).m_state = ObjectStates.Deleted;
			}
			if (ObjectAddRemove != null) ObjectAddRemove(this, obj, oldstate, ObjectStates.Deleted);
		}

		/// <summary>
		/// Marks an object for deletion ... same as RemoveObject
		/// </summary>
		/// <param name="id">ID of the item</param>
		/// <param name="type">Type of the item to delete</param>
		public override void DeleteObject<DATACLASS>(object id) 
		{
            //In case someone tries to pass in the object rather than the key
            if (id as IDataClass != null)
                DeleteObject((IDataClass)id);
            else
            {
                DATACLASS tobedeleted = GetObjectById<DATACLASS>(id);
                if (tobedeleted != null) DeleteObject((IDataClass)tobedeleted);
            }
		}

		/// <summary>
		/// Gets an object by its Guid
		/// </summary>
		/// <param name="key">The key to look for, must be of type Guid</param>
		/// <returns>The item matching the Guid, or null</returns>
		//public virtual DATACLASS GetObjectByGuid<DATACLASS>(Guid key) where DATACLASS : IDataClass
		//{
		//    return (DATACLASS)GetObjectByGuid(key);
		//}

		//public virtual object GetObjectByGuid(Guid key)
		//{
		//    if (key == Guid.Empty) return null;
		//    //if (m_relationManager != null && m_relationManager.HasGuid(key))
		//    //    return m_relationManager.GetObjectByGuid(key);
		//    //else
		//    //    return null;
		//}

		///// <summary>
		///// This will load the given DataClassBase object
		///// </summary>
		///// <param name="id"></param>
		///// <returns></returns>
		//public override DATACLASS GetObjectById<DATACLASS>(object id)
		//{
		//    return (DATACLASS)GetObjectById(typeof(DATACLASS), id);
		//}

		/// <summary>
		/// This will load the given DataClassBase object
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public override object GetObjectById(Type type, object id)
		{
			//string tablename = m_mappings[type].Tablename;

			if (GetDataClassLevel(type) < DataClassLevels.Base) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			//if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
			if (m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id] == null)
			{
				QueryModel.Operation op = QueryModel.Parser.ParseQuery(m_mappings[type].PrimaryKey.Databasefield + "=?", id);
				if (!HasLoaded(type, op))
				{
					object[] items = LoadObjects(type, op);
					if (items != null && items.Length != 0)	InsertObjectsInCache(items);
				}
			}

			if(m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id] != null)
				return m_cache[type, m_mappings[type].PrimaryKey.Databasefield, id];
			else
			{
                //There is no point in returning this if the ID is incorrect ... why is the ID incorrect if it's auto generated? if it's a negative random int, it has 99.99999995% chance to be unique
				//if (m_relationManager != null && m_transformer.TypeConfiguration.GetTypeInfo(type).PrimaryKey.IsAutoGenerated)
				//    return null;

				QueryModel.Operation filter = QueryModel.Parser.ParseQuery(m_mappings[type].PrimaryKey.Databasefield + "=?", id);
				IList lst = filter.EvaluateList(m_cache.NewObjects.Items[type]);
				if (lst.Count == 1)
					return lst[0];
				else
					return null;
			}
		}

		///// <summary>
		///// This will mark an object for deletion
		///// </summary>
		///// <param name="obj"></param>
		//protected virtual void Remove(IDataClass obj)
		//{
		//    ObjectStates oldstate = obj.ObjectState;
		//    string tablename = m_transformer.TypeConfiguration.GetTableName(obj);
		//    if(obj.ObjectState == ObjectStates.New)
		//    {
		//        m_newobjects.Remove(obj);
		//        m_relationManager.UnregisterObject(obj);
		//    }
		//    else
		//    {
		//        if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
		//        m_cache[tablename].Remove(obj.UniqueValue);
		//        m_deletedobjects.Add(obj);
		//        (obj as DataClassBase).m_state = ObjectStates.Deleted;
		//    }
		//    if (ObjectAddRemove != null) ObjectAddRemove(this, obj, oldstate, ObjectStates.Deleted);
		//}

		/// <summary>
		/// Will hook an object to the internal system
		/// </summary>
		/// <param name="newobj"></param>
		public override IDataClass Add(IDataClass newobj)
		{
            (newobj as DataClassBase).m_state = ObjectStates.New;
			m_cache.NewObjects.Add(newobj.GetType(), newobj);
			//if (m_relationManager != null && !m_relationManager.IsRegistered(newobj))
			//{
			//    m_relationManager.RegisterObject(newobj);
			//    m_relationManager.SetExistsInDb(newobj, false);
			//}
            HookObject(newobj);
			if (ObjectAddRemove != null) ObjectAddRemove(this, newobj, ObjectStates.New, ObjectStates.New);
            return newobj;
		}

		/// <summary>
		/// Register an object as belonging to this fetcher
		/// </summary>
		/// <param name="obj">The object to insert</param>
		//protected override void HookObject(IDataClass obj)		//TODO: This should be merged with Add
		//{
		//    base.HookObject(obj);
		//    //string tablename = m_mappings[obj.GetType()].Tablename;
		//    //if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
		//    //if (m_relationManager != null && !m_relationManager.IsRegistered(obj)) m_relationManager.RegisterObject(obj);
		//    //if (!m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj) || (m_relationManager != null && m_relationManager.ExistsInDb(obj)))
		//    //if (obj.ObjectState == ObjectStates.Default )//|| (m_relationManager != null && m_relationManager.ExistsInDb(obj) && obj.ObjectState == ObjectStates.Default))
		//    //{
		//    //    m_cache[tablename][m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj)] = obj;
		//    //}
		//}

		///// <summary>
		///// Will create a new instance
		///// Instead of using this you can create a new object yourself and then use the Add function
		///// </summary>
		///// <typeparam name="DATACLASS"></typeparam>
		///// <returns></returns>
		//public override DATACLASS Add<DATACLASS>()
		//{
		//    DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
		//    Add(newobj);
		//    return newobj;
		//}

		///// <summary>
		///// Will create a new instance
		///// Instead of using this you can create a new object yourself and then use the Add function
		///// </summary>
		///// <param name="dataclass"></param>
		///// <returns></returns>
		//public override object Add(Type dataclass)
		//{
		//    object newobj = Activator.CreateInstance(dataclass);
		//    Add((IDataClass)newobj);
		//    return newobj;
		//}

		/// <summary>
		/// This will reload the object values from the DB
		/// </summary>
		/// <param name="obj"></param>
		//public override void RefreshObject(IDataClass obj)
		//{
		//    //if(obj.ObjectState == ObjectStates.Deleted) return;
		//    //if (m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj) && (m_relationManager != null && !m_relationManager.ExistsInDb(obj)))
		//    //{
		//    //    (obj as DataClassBase).m_dataparent = this;
		//    //    (obj as DataClassBase).m_state = ObjectStates.Default;
		//    //    (obj as DataClassBase).m_isdirty = false;
		//    //    return;
		//    //}

		//    base.RefreshObject(obj);

		//    //insert into cache
		//    //string tablename = m_mappings[obj.GetType()].Tablename;
		//    //if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
		//    //m_cache[tablename][m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj)] = obj;
		//}

		public override void Commit(IDataClass obj)
		{
			//string tablename = m_mappings[obj.GetType()].Tablename;

			//new object?
			if ((obj as DataClassBase).m_dataparent == null) Add(obj);

			//sql
			if (obj.ObjectState == ObjectStates.Default)
			{
				//TODO: WHY?
				//if ((m_relationManager != null && m_relationManager.ExistsInDb(obj)) || !m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj))
					base.Commit(obj);
			}
			else if (obj.ObjectState == ObjectStates.New)
			{
				base.Commit(obj);		//TODO: Beware of the events
				m_cache.NewObjects.Remove(obj.GetType(), obj);

				//if ((m_relationManager != null && m_relationManager.ExistsInDb(obj)) || !m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj))
				//if ((m_relationManager != null && m_relationManager.ExistsInDb(obj)) || m_relationManager == null)
				{
					//if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
					m_cache[obj.GetType(), m_mappings[obj.GetType()].PrimaryKey.Databasefield, m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj)] = obj;
					//m_cache[tablename][m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj)] = obj;
				}
			}
			else if (obj.ObjectState == ObjectStates.Deleted)
			{
				base.Commit(obj); //TODO: Beware of the events
				m_cache.DeletedObjects.Remove(obj);
                //if (m_relationManager != null && m_relationManager.IsRegistered(obj)) m_relationManager.DeleteObject(obj);
				//if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
				//m_cache[tablename].Remove(m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj));
				//m_cache.RemoveObject(obj.GetType(), m_mappings[obj.GetType()].PrimaryKey.Databasefield, m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj), obj);
			}
		}

        /// <summary>
        /// Clears the cache, and removes all non-modified items 
        /// </summary>
        public void ClearCache()
        {
            m_loadreducer = null;
			m_cache.ClearAllUnchanged();
        }

        /*public virtual void CommitAllRecursive()
        {
            List<IDataClass> added, deleted, modified;
            GetDirty(added, updated, deleted);
        }*/

        public override void Dispose()
        {
            base.Dispose();
			m_cache.Dispose();
		    m_cache = null;
            //m_relationManager = null;
            m_loadreducer = null;
        }

	}

}
