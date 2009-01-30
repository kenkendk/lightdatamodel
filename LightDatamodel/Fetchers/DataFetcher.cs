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
	public class DataFetcher : IDataFetcher
	{
		protected IDataProvider m_provider;
		protected TypeConfiguration m_mappings = new TypeConfiguration();
		private object m_insertlock = new object();

		public event DataChangeEventHandler BeforeDataChange;
		public event DataChangeEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataConnection;
		public event DataConnectionEventHandler AfterDataConnection;

		public IDataProvider Provider { get { return m_provider; } }
		public TypeConfiguration Mappings { get { return m_mappings; } set { m_mappings = value; } }

        /// <summary>
        /// A delegate for creating unique temporary ID's
        /// </summary>
        /// <param name="item">The object that needs a temporary ID</param>
        /// <param name="type">The type of the primary key field</param>
        /// <returns>A generated ID</returns>
        public delegate object TemporaryUniqueIDGenerator(object item, Type type);

        public static TemporaryUniqueIDGenerator TemporaryIDGenerator
        {
            get { return m_temporaryIdGenerator; }
            set { m_temporaryIdGenerator = value; }
        }

        protected static long m_next_unique_id = -1;
        protected static object m_next_unique_id_lock = new object();
        protected static TemporaryUniqueIDGenerator m_temporaryIdGenerator;

		public static T GetNextTemporaryUniqueID<T>(object item)
		{
			return (T)GetNextTemporaryUniqueID(item, typeof(T));
		}

        public static object GetNextTemporaryUniqueID(object item, Type type)
        {
            lock (m_next_unique_id_lock)
            {
                if (m_temporaryIdGenerator == null)
                {
                    if (type == typeof(int))
                    {
                        if (m_next_unique_id <= int.MinValue)
                            throw new Exception("All out of temporary ID's");
                        return (int)m_next_unique_id--;
                    }
                    else if (type == typeof(long))
                    {
                        if (m_next_unique_id == long.MinValue)
                            throw new Exception("All out of temporary ID's");
                        return m_next_unique_id--;
                    }
                    else if (type == typeof(string))
                        return Guid.NewGuid().ToString();
                    else if (type == typeof(Guid))
                        return Guid.NewGuid();
                    else
                        throw new Exception("Unsupported primary key type, please register a unique ID generator");
                }
                else
                    return m_temporaryIdGenerator(item, type);
            }
        }


		public DataFetcher(IDataProvider provider)
		{
			m_provider = provider;
			m_provider.Parent = this;
        }

        #region Provider interactions
        /* The methods in this regions does all interaction with the provider.
         * This approach enables easy overrides of provider interaction,
         * when using the Nested provider.
         * 
         * It also removes clutter and varying ways to interact with the provider
         */


        /// <summary>
        /// Removes the item from the underlying data source
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void RemoveObject(object obj)
        {
            m_provider.DeleteRow(obj);
        }

        /// <summary>
        /// Inserts an item into the underlying data source
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void InsertObject(object obj)
        {
            //SQLite does not support nested transactions yet
            //bool transaction = true;
            //Guid transactionId = Guid.NewGuid();
            try
            {
                //m_provider.BeginTransaction(transactionId);
                TypeConfiguration.MappedClass typeinfo = m_mappings[obj.GetType()];

				if (typeinfo.PrimaryKey.IsAutoGenerated)
				{
					lock (m_insertlock)		//sad really. But GetLastAutogeneratedValue must be syncronised
					{
						m_provider.InsertRow(obj);

						//fetch new primary key and fire events
						object newprimarykey = m_provider.GetLastAutogeneratedValue(typeinfo.Tablename);
						typeinfo.PrimaryKey.SetValueWithEvents(obj as DataClassBase, newprimarykey);
					}
				}
				else
				{
					m_provider.InsertRow(obj);
				}
                //transaction = false;
            }
            finally
            {
                /*if (transaction)
                    try { m_provider.RollbackTransaction(transactionId); }
                    catch { }*/
            }
        }

        /// <summary>
        /// Updates an object in the underlying data source
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void UpdateObject(object obj)
        {
			m_provider.UpdateRow(obj);
        }

        /// <summary>
        /// Loads objects from data source into memory
        /// </summary>
        /// <param name="type"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        protected virtual object[] LoadObjects(Type type, QueryModel.Operation op)
        {
            return m_provider.SelectRows(type, op);
        }

        #endregion

 		protected static DataClassLevels GetDataClassLevel(Type type)
		{
			do
			{
				if (type == typeof(DataClassBase)) return DataClassLevels.Base;
				else if (type == typeof(DataClassView)) return DataClassLevels.View;
				type = type.BaseType;
			} while (type != null);
			return DataClassLevels.NoInheritance;
		}

		/// <summary>
		/// Returns 1 object given by filter
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public DATACLASS GetObject<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			DATACLASS[] ret = GetObjects<DATACLASS>(filter, parameters);
			if (ret != null && ret.Length > 0) return ret[0];
			return default(DATACLASS);
		}

		/// <summary>
		/// Returns 1 object given by filter
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public object GetObject(Type type, string filter, params object[] parameters)
		{
			object[] ret = GetObjects(type, filter, parameters);
			if (ret != null && ret.Length > 0) return ret[0];
			return null;
		}

        /// <summary>
        /// Returns all objects of a given type in the data source
        /// </summary>
        /// <typeparam name="DATACLASS"></typeparam>
        /// <returns></returns>
		public DATACLASS[] GetObjects<DATACLASS>() where DATACLASS : IDataClass
		{
			return GetObjects<DATACLASS>(null, null);
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hooked into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="filter">The filter used to select objects</param>
		/// <returns>All matching objects</returns>
		public DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			return GetObjects<DATACLASS>(Query.Parse(filter, parameters));
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hooked into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="operation"></param>
		/// <returns></returns>
		public DATACLASS[] GetObjects<DATACLASS>(QueryModel.Operation operation) where DATACLASS : IDataClass
		{
			return (DATACLASS[])(Array)GetObjects(typeof(DATACLASS), operation);
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object[] GetObjects(Type type)
		{
			return GetObjects(type, "", null);
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hooked into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter">The filter used to select objects</param>
		/// <param name="parameters"></param>
		/// <returns>All matching objects</returns>
		public object[] GetObjects(Type type, string filter, params object[] parameters)
		{
			return GetObjects(type, Query.Parse(filter, parameters));
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hooked into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="type"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public virtual object[] GetObjects(Type type, QueryModel.Operation operation)
		{
			OnBeforeDataConnection(null, DataActions.Fetch);

			object[] items = LoadObjects(type, operation);
			for (int i = 0; i < items.Length; i++)
			{
                Add((IDataClass)items[i]);
				(items[i] as DataClassBase).m_state = ObjectStates.Default;
				OnAfterDataConnection(items[i], DataActions.Fetch);
			}

			return items;
		}

        /// <summary>
        /// Deletes an object from the data source
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeleteObject(object item)
        {
            (item as DataClassBase).m_state = ObjectStates.Deleted;
            Commit((IDataClass)item);
        }

		/// <summary>
		/// Deletes an object
		/// </summary>
		/// <param name="id">ID of the item</param>
		/// <param name="type">Type of the item to delete</param>
		public void DeleteObject<DATACLASS>(object id) where DATACLASS : IDataClass
		{
			DeleteObject(GetObjectById<DATACLASS>(id));
		}

		/// <summary>
		/// This will load the given DataClassBase object
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public DATACLASS GetObjectById<DATACLASS>(object id) where DATACLASS : IDataClass
		{
            return (DATACLASS)GetObjectById(typeof(DATACLASS), id);
		}

        /// <summary>
        /// Gets an object by its ID. This is usually faster than an arbitrary load
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
		public virtual object GetObjectById(Type type, object id)
		{

			if (GetDataClassLevel(type) < DataClassLevels.Base) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			//Fetch From Data source
			OnBeforeDataConnection(type, DataActions.Fetch);
            object[] items = LoadObjects(type, Query.Equal(Query.Property(m_mappings[type].PrimaryKey.Databasefield), Query.Value(id)));
            if (items == null || items.Length == 0) return null;

            Add((IDataClass)items[0]);
            OnAfterDataConnection(items[0], DataActions.Fetch);

            return items[0];
		}

        /// <summary>
        /// Creates a new object, but does not commit it?
        /// </summary>
        /// <typeparam name="DATACLASS"></typeparam>
        /// <returns></returns>
		public DATACLASS Add<DATACLASS>() where DATACLASS : IDataClass
		{
			return (DATACLASS)Add(typeof(DATACLASS));
		}

        /// <summary>
		/// Creates a new object, but does not commit it?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Add(Type type)
		{
			object newobj = Activator.CreateInstance(type);
			Add((IDataClass)newobj);
			return newobj;
		}

		/// <summary>
		/// Registers an object as beloning to this data fetcher
		/// </summary>
		/// <param name="obj"></param>
		public virtual IDataClass Add(IDataClass obj)
		{
            //(obj as DataClassBase).m_state = ObjectStates.New;
            HookObject(obj);
			return obj;
		}

		/// <summary>
		/// Register an object as belonging to this fetcher
		/// </summary>
		/// <param name="obj">The object to insert</param>
		protected virtual void HookObject(IDataClass obj)		//TODO: This should be merged with Add
		{
            if (obj.ObjectState == ObjectStates.New)
            {
                TypeConfiguration.MappedClass typeinfo = m_mappings[obj.GetType()];
                if (typeinfo.PrimaryKey.IsAutoGenerated)
                    typeinfo.PrimaryKey.SetValueWithEvents((DataClassBase)obj, GetNextTemporaryUniqueID(obj, typeinfo.PrimaryKey.Field.FieldType));
            }

            ((DataClassBase)obj).BeforeDataChange += new DataChangeEventHandler(OnBeforeDataChange);
			((DataClassBase)obj).AfterDataChange += new DataChangeEventHandler(OnAfterDataChange);
			((DataClassBase)obj).m_dataparent = this;
		}

		/// <summary>
		/// This will evaluate a scalar ... not sure that this is a good function
		/// </summary>
		/// <typeparam name="RETURNVALUE"></typeparam>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="expression"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public virtual RETURNVALUE Compute<RETURNVALUE, DATACLASS>(string expression, string filter)
		{
			OnBeforeDataConnection(null, DataActions.Fetch);
			object ret = m_provider.Compute(typeof(DATACLASS).Name, expression, filter);
			OnAfterDataConnection(ret, DataActions.Fetch);
			if (ret == null || ret == DBNull.Value) return default(RETURNVALUE);
			try
			{
				return (RETURNVALUE)ret;
			}
			catch 
			{
				throw new Exception("Compute couldn't convert \"" + ret.ToString() + "\" (" + ret.GetType().Name + ") to " + typeof(RETURNVALUE).Name);
			}
		}

        /// <summary>
        /// Refreshes all values with those read from the datasource
        /// </summary>
        /// <param name="obj"></param>
		public virtual void RefreshObject(IDataClass obj)
		{
			if(obj.ObjectState == ObjectStates.Deleted) return;

			OnBeforeDataConnection(obj, DataActions.Fetch);
			object[] items = LoadObjects(obj.GetType(), Query.Equal(Query.Property(m_mappings[obj.GetType()].PrimaryKey.Databasefield), Query.Value(m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj))));
			if (items == null || items.Length == 0) throw new NoSuchObjectException("Row (" + m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj) + ") from table \"" + obj.GetType().Name + "\" can't be fetched", obj);
			if (items.Length != 1) throw new Exception("Row (" + m_mappings[obj.GetType()].PrimaryKey.Field.GetValue(obj) + ") from table \"" + obj.GetType().Name + "\" gave " + items.Length.ToString() + " rows");
            ObjectTransformer.CopyObject(items[0], obj);
			((DataClassBase)obj).m_state = ObjectStates.Default;
			((DataClassBase)obj).m_isdirty = false;
			((DataClassBase)obj).m_originalvalues = null;
			OnAfterDataConnection(obj, DataActions.Fetch);
		}

        /// <summary>
        /// Commits an object into the data source
        /// </summary>
        /// <param name="obj"></param>
		public virtual void Commit(params IDataClass[] items)
		{
            if (items == null || items.Length == 0)
                return;

            Guid transactionId = Guid.NewGuid();
            bool inTransaction = false;
            LinkedList<IDataClass> copyobjects = null;

            try
            {
                if (items.Length != 1)
                {
                    inTransaction = true;
                    m_provider.BeginTransaction(transactionId);
                    copyobjects = (LinkedList<IDataClass>)ObjectTransformer.CreateArrayCopy<IDataClass>(items);
                }

                foreach (IDataClass obj in items)
                {
                    //new object?
                    if (((DataClassBase)obj).m_dataparent == null) Add(obj);

                    //save
                    if (obj.ObjectState == ObjectStates.Default)
                    {
                        if (!obj.IsDirty) continue;
                        OnBeforeDataConnection(obj, DataActions.Update);
                        ((DataClassBase)obj).OnBeforeDataCommit(obj, DataActions.Update);
                        UpdateObject(obj);
                        OnAfterDataConnection(obj, DataActions.Update);
                        ((DataClassBase)obj).OnAfterDataCommit(obj, DataActions.Update);
                        RefreshObject(obj);
                    }
                    else if (obj.ObjectState == ObjectStates.New)
                    {
                        OnBeforeDataConnection(obj, DataActions.Insert);
                        ((DataClassBase)obj).OnBeforeDataCommit(obj, DataActions.Insert);
                        InsertObject(obj);
                        OnAfterDataConnection(obj, DataActions.Insert);
                        ((DataClassBase)obj).OnAfterDataCommit(obj, DataActions.Insert);
                        RefreshObject(obj);
                    }
                    else if (obj.ObjectState == ObjectStates.Deleted)
                    {
                        OnBeforeDataConnection(obj, DataActions.Delete);
                        ((DataClassBase)obj).OnBeforeDataCommit(obj, DataActions.Delete);
                        RemoveObject(obj);
                        OnAfterDataConnection(obj, DataActions.Delete);
                        ((DataClassBase)obj).OnAfterDataCommit(obj, DataActions.Delete);
                    }
                }

                if (inTransaction)
                {
                    m_provider.CommitTransaction(transactionId);
                    inTransaction = false;
                }
            }
            finally
            {
                if (inTransaction)
                {
                    m_provider.RollbackTransaction(transactionId);
                    ObjectTransformer.CopyArray<IDataClass>(copyobjects, items);
                }
            }

		}

        public virtual void Dispose()
        {
			m_mappings = null;
			m_insertlock = null;
            m_provider = null;
			GC.SuppressFinalize(this);
        }

		protected virtual void OnBeforeDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(BeforeDataChange != null) BeforeDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual void OnAfterDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(AfterDataChange != null) AfterDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual void OnAfterDataConnection(object obj, DataActions action)
		{
			if (AfterDataConnection != null) AfterDataConnection(obj, action);
		}

		protected virtual void OnBeforeDataConnection(object obj, DataActions action)
		{
			if (BeforeDataConnection != null) BeforeDataConnection(obj, action);
		}
	}
}
