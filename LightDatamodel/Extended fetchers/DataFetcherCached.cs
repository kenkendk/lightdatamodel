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
		protected SortedList<string, SortedList<object, IDataClass>> m_cache = new SortedList<string, SortedList<object, IDataClass>>();
        protected List<IDataClass> m_newobjects = new List<IDataClass>();
        protected List<IDataClass> m_deletedobjects = new List<IDataClass>();
        protected RelationManager m_relationManager;

        public IRelationManager RelationManager { get { return m_relationManager; } }

        /// <summary>
        /// Gets a value describing if the fetcher contains uncommited changes
        /// </summary>
		public bool IsDirty
		{
			get
			{
				if ((m_newobjects.Count > 0) || (m_deletedobjects.Count > 0))
					return true;

				foreach (SortedList<object, IDataClass> table in m_cache.Values)
					foreach (IDataClass obj in table.Values)
						if (obj.IsDirty)
							return true;

				return false;
			}
		}

        /// <summary>
        /// Constructs a new cached data fetcher
        /// </summary>
        /// <param name="provider">The provider to use</param>
		public DataFetcherCached(IDataProvider provider) : base(provider)
		{
            m_relationManager = new RelationManager(base.m_transformer.TypeConfiguration);
		}

        #region Provider interactions
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
        protected override void InsertObject(object obj)
        {
            base.InsertObject(obj);
            m_relationManager.SetExistsInDb((IDataClass)obj, true);
        }

        #endregion

        /// <summary>
        /// Commits all cached objects to data source
        /// </summary>
		public override void CommitAll()
		{
			Guid transactionID = Guid.NewGuid();
			bool inTransaction = false;

			m_provider.BeginTransaction(transactionID);
			inTransaction = true;

			try
			{
                List<IDataClass> updated = new List<IDataClass>();
				foreach (SortedList<object, IDataClass> table in m_cache.Values)
                    foreach (IDataClass obj in table.Values)
						if(obj.IsDirty)
							updated.Add(obj);


                List<IDataClass> added = new List<IDataClass>(m_newobjects);
                List<IDataClass> removed = new List<IDataClass>(m_deletedobjects);

                m_relationManager.CommitItems(this, added, removed, updated);

				m_provider.CommitTransaction(transactionID);
                m_newobjects = new List<IDataClass>();
                m_deletedobjects = new List<IDataClass>();
				inTransaction = false;
			}
			finally
			{
				if (inTransaction)
					m_provider.RollbackTransaction(transactionID);
			}
		}

        /// <summary>
        /// Register an object as belonging to this fetcher
        /// </summary>
        /// <param name="obj">The object to insert</param>
		protected override void HookObject(IDataClass obj)
		{
			base.HookObject(obj);
            string tablename = m_transformer.TypeConfiguration.GetTableName(obj);
			if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
            if (!m_relationManager.IsRegistered(obj))
                m_relationManager.RegisterObject(obj);
			if (!m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj) || m_relationManager.ExistsInDb(obj))
			{
                m_cache[tablename][obj.UniqueValue] = obj;
			}

            
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hooked into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="type">The type of objects to load</param>
		/// <param name="filter">The filter used to select objects</param>
		/// <returns>All matching objects</returns>
		public override DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters)
		{
			//This ensures that we always query the memory
			return GetObjects<DATACLASS>(QueryModel.Parser.ParseQuery(filter, parameters));
		}

        /// <summary>
        /// Geturns all objects of the given type
        /// </summary>
        /// <typeparam name="DATACLASS">The type to retrieve</typeparam>
        /// <returns></returns>
        public override DATACLASS[] GetObjects<DATACLASS>()
        {
            return GetObjects<DATACLASS>("");
        }

        public virtual DATACLASS[] GetObjects<DATACLASS>(string query) where DATACLASS : IDataClass
        {
            return GetObjects<DATACLASS>(QueryModel.Parser.ParseQuery(query));
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
		public virtual DATACLASS[] GetObjects<DATACLASS>(QueryModel.Operation operation) where DATACLASS : IDataClass
		{
            string tablename = m_transformer.TypeConfiguration.GetTableName(typeof(DATACLASS));
            if (!m_cache.ContainsKey(tablename))
                m_cache[tablename] = new SortedList<object, IDataClass>();

//          QueryModel.Operation filter = QueryModel.Parser.ParseQuery("Not (" + m_transformer.TypeConfiguration.UniqueColumn(typeof(DATACLASS)) + " IN ?)", m_cache[tablename].Keys);
//			QueryModel.Operation op = new QueryModel.Operation(QueryModel.Operators.And, new QueryModel.OperationOrParameter[] {filter, operation});
//			InsertObjectsInCache(LoadObjects(typeof(DATACLASS), op));

			InsertObjectsInCache(LoadObjects(typeof(DATACLASS), operation));
            //TODO: An enumerable collection that transparently itterates over a number of collections would make this more efficient
            List<DATACLASS> items = new List<DATACLASS>();
            foreach (DATACLASS b in m_cache[tablename].Values)
                items.Add(b);

            QueryModel.Operation filter = QueryModel.Parser.ParseQuery("GetType.FullName = ?", typeof(DATACLASS).FullName);
            items.AddRange(filter.EvaluateList<DATACLASS>(m_newobjects));

            return operation.EvaluateList<DATACLASS>(items);
        }

        public virtual object[] GetObjects(Type type)
        {
            return GetObjects(type, "");
        }

        public virtual object[] GetObjects(Type type, string query)
        {
            return GetObjects(type, QueryModel.Parser.ParseQuery(query));
        }

		public virtual object[] GetObjects(Type type, QueryModel.Operation operation)
		{
			string tablename = m_transformer.TypeConfiguration.GetTableName(type);
            if (!m_cache.ContainsKey(tablename))
                m_cache[tablename] = new SortedList<object, IDataClass>();

            QueryModel.Operation filter = QueryModel.Parser.ParseQuery("Not (" + m_transformer.TypeConfiguration.UniqueColumn(type) + " IN ?)", m_cache[tablename].Keys);

            QueryModel.Operation op = new QueryModel.Operation(QueryModel.Operators.And, new QueryModel.OperationOrParameter[] { filter, operation });

			InsertObjectsInCache(LoadObjects(type, operation));
			return operation.EvaluateList(m_cache[tablename].Values);
		}

		/// <summary>
		/// Discards all changes from the object, and removes it from the internal cache
		/// </summary>
		/// <param name="obj">The object to discard</param>
		public void DiscardObject(IDataClass obj)
		{
            string tablename = m_transformer.TypeConfiguration.GetTableName(obj);

			if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
			if (m_relationManager.ExistsInDb(obj))
				if (m_cache[tablename].ContainsKey(obj.UniqueValue))
					m_cache[tablename].Remove(obj.UniqueValue);
        
            m_relationManager.UnregisterObject(obj);
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

		public object[] GetObjectsFromCache(Type type, QueryModel.Operation query)
		{
			string tablename = m_transformer.TypeConfiguration.GetTableName(type);
			if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());

			List<IDataClass> allItems = new List<IDataClass>();
			if (this as DataFetcherNested != null)
				InsertObjectsInCache(LoadObjects(type, query));

			allItems.AddRange(m_cache[tablename].Values);
			foreach (DataClassBase o in m_newobjects)
				if (o.GetType().IsAssignableFrom(type))
					allItems.Add(o);

			return query.EvaluateList(allItems);
		}

        protected object[] InsertObjectsInCache(object[] data)
        {
            ArrayList res = new ArrayList();

            foreach (object item in data)
            {
                object toAdd = item;
                if (item as DataClassBase != null)
                {
                    string tablename = m_transformer.TypeConfiguration.GetTableName(item);
                    if (!m_cache.ContainsKey(tablename))
                        m_cache.Add(tablename, new SortedList<object, IDataClass>());
                    DataClassBase dbitem = item as DataClassBase;
                    if (!m_cache[tablename].ContainsKey(dbitem.UniqueValue))
                    {

                        QueryModel.Operation opdeleted = QueryModel.Parser.ParseQuery("GetType.FullName = ? AND UniqueValue = ?", item.GetType().FullName, dbitem.UniqueValue);

                        if (opdeleted.EvaluateList(m_deletedobjects).Length > 0)
                            continue;

                        dbitem.m_dataparent = this;
                        dbitem.m_state = ObjectStates.Default;
                        dbitem.m_isdirty = false;

                        HookObject(dbitem);

                        if (this as DataFetcherNested == null) m_relationManager.SetExistsInDb(dbitem, true);
                        m_cache[tablename][dbitem.UniqueValue] = dbitem;
                    }
                    else
                        toAdd = m_cache[tablename][dbitem.UniqueValue];
                }
                else if (item as DataClassView != null)
                {
                    (item as DataClassView).m_dataparent = this;
                }

                if (toAdd != null)
                {
                    res.Add(toAdd);
                    OnAfterDataConnection(toAdd, DataActions.Fetch);
                }

            }
            return res.ToArray();
        }

		public override void DeleteObject(object item)
		{
			Remove((IDataClass)item);
		}

		/// <summary>
		/// Marks an object for deletion
		/// </summary>
		/// <param name="id">ID of the item</param>
		/// <param name="type">Type of the item to delete</param>
		public override void DeleteObject<DATACLASS>(object id) 
		{
			DATACLASS tobedeleted = GetObjectById<DATACLASS>(id);
			if(tobedeleted != null) Remove((IDataClass)tobedeleted);
		}

		/// <summary>
		/// Gets an object by its Guid
		/// </summary>
		/// <param name="key">The key to look for, must be of type Guid</param>
		/// <returns>The item matching the Guid, or null</returns>
		public virtual DATACLASS GetObjectByGuid<DATACLASS>(Guid key) where DATACLASS : IDataClass
		{
            return (DATACLASS)GetObjectByGuid(key);
		}

		public virtual object GetObjectByGuid(Guid key)
		{
			if (key == Guid.Empty) return null;

            return m_relationManager.GetObjectByGuid(key);
		}

		/// <summary>
		/// This will load the given DataClassBase object
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public override DATACLASS GetObjectById<DATACLASS>(object id)
		{
			return (DATACLASS)GetObjectById(typeof(DATACLASS), id);
		}

		public override object GetObjectById(Type type, object id)
		{
			string tablename = m_transformer.TypeConfiguration.GetTableName(type);

			if (GetDataClassLevel(type) < DataClassLevels.Base) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
			if (!m_cache[tablename].ContainsKey(id))
			{
                object[] items = LoadObjects(type, QueryModel.Parser.ParseQuery(m_relationManager.GetUniqueColumn(type) + "=?", id));
                if (items == null || items.Length == 0)
                    return null;
                else
                    InsertObjectsInCache(items);
			}

            if (m_cache[tablename].ContainsKey(id))
                return m_cache[tablename][id];
            else
                return null;
		}

		public virtual void Remove(IDataClass obj)
		{
            string tablename = m_transformer.TypeConfiguration.GetTableName(obj);
			if(obj.ObjectState == ObjectStates.New)
			{
				m_newobjects.Remove(obj);
                m_relationManager.UnregisterObject(obj);
			}
			else
			{
				if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
				m_cache[tablename].Remove(obj.UniqueValue);
				m_deletedobjects.Add(obj);
				(obj as DataClassBase).m_state = ObjectStates.Deleted;
			}
		}

		public virtual void Add(IDataClass newobj)
		{
			(newobj as DataClassBase).m_dataparent = this;
            (newobj as DataClassBase).m_state = ObjectStates.New;
			m_newobjects.Add(newobj);
            if (!m_relationManager.IsRegistered(newobj))
            {
                m_relationManager.RegisterObject(newobj);
                m_relationManager.SetExistsInDb(newobj, false);
            }
		}

		public override DATACLASS CreateObject<DATACLASS>()
		{
			DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
			Add(newobj);
			return newobj;
		}

		public override object CreateObject(Type dataclass)
		{
			object newobj = Activator.CreateInstance(dataclass);
			Add((IDataClass)newobj);
			return newobj;
		}

		protected override void RefreshObject(IDataClass obj)
		{
			if(obj.ObjectState == ObjectStates.Deleted) return;
            if (m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj) && !m_relationManager.ExistsInDb(obj))
			{
				(obj as DataClassBase).m_dataparent = this;
                (obj as DataClassBase).m_state = ObjectStates.Default;
                (obj as DataClassBase).m_isdirty = false;
				return;
			}

            base.RefreshObject(obj);
		}

		public override void Commit(IDataClass obj)
		{
			string tablename = m_transformer.TypeConfiguration.GetTableName(obj);

			//sql
			if (obj.ObjectState == ObjectStates.Default)
			{
                if (m_relationManager.ExistsInDb(obj) || !m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj))
					base.Commit(obj);
			}
			else if (obj.ObjectState == ObjectStates.New)
			{
				base.Commit(obj);		//TODO: Beware of the events

                if (m_relationManager.ExistsInDb(obj) || !m_transformer.TypeConfiguration.IsPrimaryKeyAutoGenerated(obj))
				{
					if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
					m_cache[tablename].Add(obj.UniqueValue, obj);
				}
			}
			else if (obj.ObjectState == ObjectStates.Deleted)
			{
				base.Commit(obj); //TODO: Beware of the events
				m_deletedobjects.Remove(obj);
                m_relationManager.UnregisterObject(obj);
				if (!m_cache.ContainsKey(tablename)) m_cache.Add(tablename, new SortedList<object, IDataClass>());
				m_cache[tablename].Remove(obj.UniqueValue);
			}
		}
	}

}
