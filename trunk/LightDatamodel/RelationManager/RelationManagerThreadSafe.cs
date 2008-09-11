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
    public class RelationManagerThreadSafe : IRelationManager
    {
        private object m_lock;
        private IRelationManager m_basemanager;

        public RelationManagerThreadSafe(IRelationManager basemanager)
            : this(new object(), basemanager)
        {
        }

        public RelationManagerThreadSafe(object @lock, IRelationManager basemanager)
        {
            m_lock = @lock;
            m_basemanager = basemanager;
        }

        public object Lock { get { return m_lock; } }

        #region IRelationManager Members

        public bool ExistsInDb(IDataClass item)
        {
            lock (m_lock)
                return m_basemanager.ExistsInDb(item);
        }

        public Guid GetGuidForObject(IDataClass item)
        {
            lock (m_lock)
                return m_basemanager.GetGuidForObject(item);
        }

        public IDataClass GetObjectByGuid(Guid g)
        {
            lock (m_lock)
                return m_basemanager.GetObjectByGuid(g);
        }

        public T GetReferenceObject<T>(string propertyname, IDataClass owner)
        {
            lock (m_lock)
                return m_basemanager.GetReferenceObject<T>(propertyname, owner);

        }

        public IDataClass GetReferenceObject(string propertyname, IDataClass owner)
        {
            lock (m_lock)
                return m_basemanager.GetReferenceObject(propertyname, owner);
        }

        public GenericListWrapper<T, IDataClass> GetReferenceCollection<T>(string propertyname, IDataClass owner) where T : IDataClass
        {
            lock (m_lock)
                return m_basemanager.GetReferenceCollection<T>(propertyname, owner);
        }

        public void SetReferenceObject(string propertyname, IDataClass owner, IDataClass value)
        {
            lock (m_lock)
                m_basemanager.SetReferenceObject(propertyname, owner, value);
        }

        public void SetReferenceObject<T>(string propertyname, IDataClass owner, T value) where T : IDataClass
        {
            lock (m_lock)
                m_basemanager.SetReferenceObject<T>(propertyname, owner, value);
        }

        public bool IsRegistered(IDataClass item)
        {
            lock (m_lock)
                return m_basemanager.IsRegistered(item);
        }

        public void ReassignGuid(Guid oldGuid, Guid newGuid)
        {
            lock (m_lock)
                m_basemanager.ReassignGuid(oldGuid, newGuid);
        }

        public Guid RegisterObject(Guid g, IDataClass item)
        {
            lock (m_lock)
                return m_basemanager.RegisterObject(g, item);
        }

        public Guid RegisterObject(IDataClass item)
        {
            lock (m_lock)
                return m_basemanager.RegisterObject(item);
        }

        public void UnregisterObject(IDataClass item)
        {
            lock (m_lock)
                m_basemanager.UnregisterObject(item);
        }

        public void UnregisterObject(Guid g)
        {
            lock (m_lock)
                m_basemanager.UnregisterObject(g);
        }

        public void SetExistsInDb(IDataClass item, bool state)
        {
            lock (m_lock)
                m_basemanager.SetExistsInDb(item, state);
        }

        public void DeleteObject(IDataClass itm)
        {
            lock (m_lock)
                m_basemanager.DeleteObject(itm);
        }

        public Dictionary<string, List<Guid>> GetReferenceObjects(Type type, Guid item)
        {
            lock (m_lock)
                return m_basemanager.GetReferenceObjects(type, item);
        }

        public void SetReferenceObjects(Type type, Guid item, Dictionary<string, List<Guid>> references)
        {
            lock (m_lock)
                m_basemanager.SetReferenceObjects(type, item, references);
        }

        public bool HasGuid(Guid g)
        {
            lock (m_lock)
                return m_basemanager.HasGuid(g);
        }

        #endregion
    }
}
