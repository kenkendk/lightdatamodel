using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel
{
    public class GuidProxyCollection : IList<IDataClass>
    {
        private RelationManager m_manager;
        private List<Guid> m_list;
        private Guid m_owner;
        private Type m_ownerType;
        private string m_relationKey;

        public GuidProxyCollection(RelationManager manager, string relationKey, List<Guid> list, Type ownerType, Guid owner)
        {
            m_relationKey = relationKey;
            m_list = list;
            m_ownerType = ownerType;
            m_owner = owner;
            m_manager = manager;
        }

        #region IList<IDataClass> Members

        public int IndexOf(IDataClass item)
        {
            return m_list.IndexOf(m_manager.GetGuidForObject(item));
        }

        public void Insert(int index, IDataClass item)
        {
            throw new NotImplementedException("Use the add method instead");
        }

        public void RemoveAt(int index)
        {
            m_manager.RemoveReferenceObjectInternal(m_relationKey, m_ownerType, m_owner, m_list[index], true);
        }

        public IDataClass this[int index]
        {
            get
            {
                if (m_manager.HasGuid(m_list[index]))
                    return m_manager.GetObjectByGuid(m_list[index]);
                else
                {
                    IDataClass ic = m_manager.GetObjectByGuid(m_owner);
                    return (IDataClass)((IDataFetcherCached)ic.DataParent).GetObjectByGuid(m_list[index]);
                }
            }
            set
            {
                throw new NotImplementedException("Use the add method instead");
            }
        }

        #endregion

        #region ICollection<IDataClass> Members

        public void Add(IDataClass item)
        {
            m_manager.AddReferenceObjectInternal(m_relationKey, m_ownerType, m_owner, m_manager.GetGuidForObject(item), true);
        }

        public void Clear()
        {
            while (m_list.Count > 0)
                RemoveAt(0);
        }

        public bool Contains(IDataClass item)
        {
            return m_list.Contains(m_manager.GetGuidForObject(item));
        }

        public void CopyTo(IDataClass[] array, int arrayIndex)
        {
            for (int i = 0; i < m_list.Count; i++)
                array[i + arrayIndex] = m_manager.GetObjectByGuid(m_list[i]);
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IDataClass item)
        {
            if (this.Contains(item))
            {
                m_manager.RemoveReferenceObjectInternal(m_relationKey, m_ownerType, m_owner, m_manager.GetGuidForObject(item), true);
                return true;
            }
            else
                return false;
        }

        #endregion

        #region IEnumerable<IDataClass> Members

        public IEnumerator<IDataClass> GetEnumerator()
        {
            return new GuidProxyEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new GuidProxyEnumerator(this);
        }

        #endregion
    }

    public class GuidProxyEnumerator : IEnumerator<IDataClass>
    {
        private GuidProxyCollection m_owner;
        private int index;

        public GuidProxyEnumerator(GuidProxyCollection owner)
        {
            m_owner = owner;
            this.Reset();
        }

        #region IEnumerator<IDataClass> Members

        public IDataClass Current
        {
            get
            {
                try
                {
                    return m_owner[index];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            index++;
            return index < m_owner.Count;
        }

        public void Reset()
        {
            index = -1;
        }

        #endregion
    }
}
