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

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Will fetch and commit objects directly to source
	/// </summary>
	public class DataFetcher : IDataFetcher
	{
		protected IDataProvider m_provider;
        protected IObjectTransformer m_transformer;

		public event DataChangeEventHandler BeforeDataChange;
		public event DataChangeEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataConnection;
		public event DataConnectionEventHandler AfterDataConnection;

		public IDataProvider Provider { get { return m_provider; } }
        public IObjectTransformer ObjectTransformer { get { return m_transformer; } }

		public DataFetcher(IDataProvider provider)
		{
			m_provider = provider;
            m_transformer = new ObjectTransformer();
            m_provider.Transformer = m_transformer;
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
                TypeConfiguration.MappedClass typeinfo = m_transformer.TypeConfiguration.GetTypeInfo(obj);
                m_provider.InsertRow(obj);
                if (typeinfo.PrimaryKey.IsAutoGenerated)
                    typeinfo.PrimaryKey.Field.SetValue(obj, m_provider.GetLastAutogeneratedValue(typeinfo.TableName));
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

 		protected virtual DataClassLevels GetDataClassLevel(Type type)
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
		public virtual DATACLASS GetObject<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			DATACLASS[] ret = GetObjects<DATACLASS>(filter, parameters);
			if (ret != null && ret.Length > 0) return ret[0];
			return default(DATACLASS);
		}

        /// <summary>
        /// Returns all objects of a given type in the data source
        /// </summary>
        /// <typeparam name="DATACLASS"></typeparam>
        /// <returns></returns>
		public virtual DATACLASS[] GetObjects<DATACLASS>() where DATACLASS : IDataClass
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
		public virtual DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			return GetObjects<DATACLASS>(Query.Parse(filter, parameters));
		}

		public virtual DATACLASS[] GetObjects<DATACLASS>(QueryModel.Operation operation) where DATACLASS : IDataClass
		{
			Type type = typeof(DATACLASS);
			string tablename = type.Name;

			OnBeforeDataConnection(null, DataActions.Fetch);

			object[] items = LoadObjects(type, operation);
			DATACLASS[] res = new DATACLASS[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				Add((IDataClass)items[i]);
				(items[i] as DataClassBase).m_state = ObjectStates.Default;
				OnAfterDataConnection(items[i], DataActions.Fetch);
				res[i] = (DATACLASS)items[i];
			}

			return res;
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual object[] GetObjects(Type type)
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
		public virtual object[] GetObjects(Type type, string filter, params object[] parameters)
		{
			return GetObjects(type, Query.Parse(filter, parameters));
		}

		public virtual object[] GetObjects(Type type, QueryModel.Operation operation)
		{
			string tablename = type.Name;

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
		public virtual void DeleteObject<DATACLASS>(object id) where DATACLASS : IDataClass
		{
			DeleteObject(GetObjectById<DATACLASS>(id));
		}

        /// <summary>
        /// Commits all cached objects, actually does nothing since there is no cache on this fetcher
        /// </summary>
        public virtual void CommitAll()
        {
        }

		/// <summary>
		/// This will load the given DataClassBase object
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual DATACLASS GetObjectById<DATACLASS>(object id) where DATACLASS : IDataClass
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
			string tablename = type.Name;

			if (GetDataClassLevel(type) < DataClassLevels.Base) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			//Fetch From Data source
			IDataClass newobj = (IDataClass)Activator.CreateInstance(type);
			OnBeforeDataConnection(newobj, DataActions.Fetch);
            object[] items = LoadObjects(type, Query.Equal(Query.Property(newobj.UniqueColumn), Query.Value(id)));
            if (items == null || items.Length == 0)
                return null;

			Add((IDataClass)items[0]);
            (items[0] as DataClassBase).m_state = ObjectStates.Default;
            OnAfterDataConnection(items[0], DataActions.Fetch);

            return items[0];
		}

        /// <summary>
        /// Creates a new object, but does not commit it?
        /// </summary>
        /// <typeparam name="DATACLASS"></typeparam>
        /// <returns></returns>
		public virtual DATACLASS Add<DATACLASS>() where DATACLASS : IDataClass
		{
			DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
			Add((IDataClass)newobj);
			return newobj;
		}

        /// <summary>
		/// Creates a new object, but does not commit it?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual object Add(Type type)
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
			HookObject(obj);
			(obj as DataClassBase).m_state = ObjectStates.New;
			return obj;
		}

		/// <summary>
		/// Register an object as belonging to this fetcher
		/// </summary>
		/// <param name="obj">The object to insert</param>
		protected virtual void HookObject(IDataClass obj)		//TODO: This should be merged with Add
		{
			(obj as DataClassBase).BeforeDataChange += new DataChangeEventHandler(obj_BeforeDataChange);
			(obj as DataClassBase).AfterDataChange += new DataChangeEventHandler(obj_AfterDataChange);
			(obj as DataClassBase).m_dataparent = this;
		}

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

			string tablename = obj.GetType().Name;
			OnBeforeDataConnection(obj, DataActions.Fetch);
            object[] items = LoadObjects(obj.GetType(), Query.Equal(Query.Property(obj.UniqueColumn), Query.Value(obj.UniqueValue)));
            if (items == null || items.Length == 0) throw new Exception("Row (" + obj.UniqueValue + ") from table \"" + tablename + "\" can't be fetched");
            if (items.Length != 1) throw new Exception("Row (" + obj.UniqueValue + ") from table \"" + tablename + "\" gave " + items.Length.ToString() + " rows");
            ObjectTransformer.CopyObject(items[0], obj);
			(obj as DataClassBase).m_state = ObjectStates.Default;
			(obj as DataClassBase).m_isdirty = false;
			(obj as DataClassBase).m_originalvalues = null;
			OnAfterDataConnection(obj, DataActions.Fetch);
		}

        /// <summary>
        /// Commits an object into the data source
        /// </summary>
        /// <param name="obj"></param>
		public virtual void Commit(IDataClass obj)
		{
			//new object?
			if ((obj as DataClassBase).m_dataparent == null) Add(obj);

			//save
			if (obj.ObjectState == ObjectStates.Default)
			{
				OnBeforeDataConnection(obj, DataActions.Update);
				(obj as DataClassBase).OnBeforeDataCommit(obj, DataActions.Update);
                UpdateObject(obj);
				OnAfterDataConnection(obj, DataActions.Update);
				(obj as DataClassBase).OnAfterDataCommit(obj, DataActions.Update);

                //Try to read data back from database, but not from a nested
                if (this as DataFetcherNested == null) RefreshObject(obj);
            }
			else if (obj.ObjectState == ObjectStates.New)
			{
				OnBeforeDataConnection(obj, DataActions.Insert);
				(obj as DataClassBase).OnBeforeDataCommit(obj, DataActions.Insert);
                InsertObject(obj);
				OnAfterDataConnection(obj, DataActions.Insert);
				(obj as DataClassBase).OnAfterDataCommit(obj, DataActions.Insert);

                //Try to read data back from database, but not from a nested
                if (this as DataFetcherNested == null) RefreshObject(obj);
            }
			else if (obj.ObjectState == ObjectStates.Deleted)
			{
				OnBeforeDataConnection(obj, DataActions.Delete);
				(obj as DataClassBase).OnBeforeDataCommit(obj, DataActions.Delete);
                RemoveObject(obj);
				OnAfterDataConnection(obj, DataActions.Delete);
				(obj as DataClassBase).OnAfterDataCommit(obj, DataActions.Delete);
			}

		}

        /// <summary>
		/// Discards all changes from the object, and removes it from the internal cache.
        /// Since there is no cache on the basic provider, it does nothing.
		/// </summary>
		/// <param name="obj">The object to discard</param>
        public virtual void DiscardObject(IDataClass obj)
        {
        }


        public virtual void Dispose()
        {
            m_provider = null;
            m_transformer = null;
        }

		protected virtual void obj_BeforeDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(BeforeDataChange != null) BeforeDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual void obj_AfterDataChange(object sender, string propertyname, object oldvalue, object newvalue)
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
