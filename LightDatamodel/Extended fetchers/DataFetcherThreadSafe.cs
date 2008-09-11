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
    /// <summary>
    /// This class is a wrapper class for a datafetcher.
    /// It is protected by a lock, so that all operations can be called from threads.
    /// </summary>
    public class DataFetcherThreadSafe : IDataFetcherCached
    {
        private object m_lock;
        private IDataFetcherCached m_basefetcher;
        private IRelationManager m_manager;

        public DataFetcherThreadSafe(IDataFetcherCached basefetcher)
            : this(new object(), basefetcher)
        {
        }
        public DataFetcherThreadSafe(object @lock, IDataFetcherCached basefetcher)
        {
            m_lock = @lock;
            m_basefetcher = basefetcher;
            m_manager = new RelationManagerThreadSafe(m_lock, m_basefetcher.RelationManager);

            m_basefetcher.AfterDataChange += new DataChangeEventHandler(m_basefetcher_AfterDataChange);
            m_basefetcher.AfterDataConnection += new DataConnectionEventHandler(m_basefetcher_AfterDataConnection);
            m_basefetcher.BeforeDataChange += new DataChangeEventHandler(m_basefetcher_BeforeDataChange);
            m_basefetcher.BeforeDataConnection += new DataConnectionEventHandler(m_basefetcher_BeforeDataConnection);
            m_basefetcher.ObjectAddRemove += new ObjectStateChangeHandler(m_basefetcher_ObjectAddRemove);
        }

        public object Lock { get { return m_lock; } }

        void m_basefetcher_ObjectAddRemove(object sender, IDataClass obj, ObjectStates oldstate, ObjectStates newstate)
        {
            lock (m_lock)
                if (ObjectAddRemove != null)
                    ObjectAddRemove(sender, obj, oldstate, newstate);
        }

        void m_basefetcher_BeforeDataConnection(object sender, DataActions action)
        {
            lock (m_lock)
                if (BeforeDataConnection != null)
                    BeforeDataConnection(sender, action);
        }

        void m_basefetcher_BeforeDataChange(object sender, string propertyname, object oldvalue, object newvalue)
        {
            lock (m_lock)
                if (BeforeDataChange != null)
                    BeforeDataChange(sender, propertyname, oldvalue, newvalue);
        }

        void m_basefetcher_AfterDataConnection(object sender, DataActions action)
        {
            lock (m_lock)
                if (AfterDataConnection != null)
                    AfterDataConnection(sender, action);
        }

        void m_basefetcher_AfterDataChange(object sender, string propertyname, object oldvalue, object newvalue)
        {
            lock (m_lock)
                if (AfterDataChange != null)
                    AfterDataChange(sender, propertyname, oldvalue, newvalue);
        }

        #region IDataFetcherCached Members

        public event ObjectStateChangeHandler ObjectAddRemove;

        public DATACLASS GetObjectByGuid<DATACLASS>(Guid guid) where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.GetObjectByGuid<DATACLASS>(guid);
        }

        public object GetObjectByGuid(Guid guid)
        {
            lock (m_lock)
                return m_basefetcher.GetObjectByGuid(guid);
        }

        public object[] GetObjectsFromCache(Type type, System.Data.LightDatamodel.QueryModel.Operation query)
        {
            lock (m_lock)
                return m_basefetcher.GetObjectsFromCache(type, query);
        }

        public object[] GetObjectsFromCache(Type type, string filter, params object[] parameters)
        {
            lock (m_lock)
                return m_basefetcher.GetObjectsFromCache(type, filter, parameters);
        }

        public DATACLASS[] GetObjectsFromCache<DATACLASS>(System.Data.LightDatamodel.QueryModel.Operation query) where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.GetObjectsFromCache<DATACLASS>(query);
        }

        public DATACLASS[] GetObjectsFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.GetObjectsFromCache<DATACLASS>(filter, parameters);
        }

        public IRelationManager RelationManager
        {
            get { return m_manager; }
        }

        public bool IsDirty
        {
            get 
            {
                lock (m_lock)
                    return m_basefetcher.IsDirty;
            }
        }

        public void DiscardObject(IDataClass obj)
        {
            lock (m_lock)
                m_basefetcher.DiscardObject(obj);
        }

        #endregion

        #region IDataFetcher Members

        public event DataChangeEventHandler BeforeDataChange;

        public event DataChangeEventHandler AfterDataChange;

        public event DataConnectionEventHandler BeforeDataConnection;

        public event DataConnectionEventHandler AfterDataConnection;

        public DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
        {
            lock (m_lock) 
                return m_basefetcher.GetObjects<DATACLASS>(filter, parameters);
        }

        public DATACLASS[] GetObjects<DATACLASS>() where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.GetObjects<DATACLASS>();
        }

        public DATACLASS[] GetObjects<DATACLASS>(System.Data.LightDatamodel.QueryModel.Operation operation) where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.GetObjects<DATACLASS>(operation);
        }

        public object[] GetObjects(Type type)
        {
            lock (m_lock)
                return m_basefetcher.GetObjects(type);
        }

        public object[] GetObjects(Type type, string filter, params object[] parameters)
        {
            lock (m_lock)
                return m_basefetcher.GetObjects(type, filter, parameters);
        }

        public object[] GetObjects(Type type, System.Data.LightDatamodel.QueryModel.Operation operation)
        {
            lock (m_lock)
                return m_basefetcher.GetObjects(type, operation);
        }

        public DATACLASS GetObject<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.GetObject<DATACLASS>(filter, parameters);
        }

        public DATACLASS GetObjectById<DATACLASS>(object id) where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.GetObjectById<DATACLASS>(id);
        }

        public object GetObjectById(Type type, object id)
        {
            lock (m_lock)
                return m_basefetcher.GetObjectById(type, id);
        }

        public void Commit(IDataClass obj)
        {
            lock (m_lock)
                m_basefetcher.Commit(obj);
        }

        public void CommitAll()
        {
            lock (m_lock)
                m_basefetcher.CommitAll();
        }

        public DATACLASS Add<DATACLASS>() where DATACLASS : IDataClass
        {
            lock (m_lock)
                return m_basefetcher.Add<DATACLASS>();
        }

        public object Add(Type type)
        {
            lock (m_lock)
                return m_basefetcher.Add(type);
        }

        public IDataClass Add(IDataClass newobj)
        {
            lock (m_lock)
                return m_basefetcher.Add(newobj);
        }

        public IDataProvider Provider
        {
            get { return m_basefetcher.Provider; }
        }

        public IObjectTransformer ObjectTransformer
        {
            get { return m_basefetcher.ObjectTransformer; }
        }

        public RETURNVALUE Compute<RETURNVALUE, DATACLASS>(string expression, string filter)
        {
            lock (m_lock)
                return m_basefetcher.Compute<RETURNVALUE, DATACLASS>(expression, filter);
        }

        public void DeleteObject<DATACLASS>(object id) where DATACLASS : IDataClass
        {
            lock (m_lock)
                m_basefetcher.DeleteObject<DATACLASS>(id);
        }

        public void DeleteObject(object item)
        {
            lock (m_lock)
                m_basefetcher.DeleteObject(item);
        }

        public void RefreshObject(IDataClass obj)
        {
            lock (m_lock)
                m_basefetcher.RefreshObject(obj);
        }

        public void ClearCache()
        {
            lock (m_lock)
                m_basefetcher.ClearCache();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            lock (m_lock)
                if (m_basefetcher != null)
                {
                    m_basefetcher.AfterDataChange -= new DataChangeEventHandler(m_basefetcher_AfterDataChange);
                    m_basefetcher.AfterDataConnection -= new DataConnectionEventHandler(m_basefetcher_AfterDataConnection);
                    m_basefetcher.BeforeDataChange -= new DataChangeEventHandler(m_basefetcher_BeforeDataChange);
                    m_basefetcher.BeforeDataConnection -= new DataConnectionEventHandler(m_basefetcher_BeforeDataConnection);
                    m_basefetcher.ObjectAddRemove -= new ObjectStateChangeHandler(m_basefetcher_ObjectAddRemove);

                    m_basefetcher.Dispose();
                    m_basefetcher = null;
                }
        }

        #endregion
    }
}
