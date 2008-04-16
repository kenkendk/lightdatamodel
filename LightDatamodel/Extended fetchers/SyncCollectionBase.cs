using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel
{
    public class SyncCollectionBase<DATACLASS> : IList<DATACLASS> where DATACLASS : IDataClass
    {
        protected List<DATACLASS> m_baseList = new List<DATACLASS>();
        protected System.Reflection.PropertyInfo m_reverseProperty = null;
        protected object m_owner = null;

        public SyncCollectionBase() { }

        public SyncCollectionBase(object owner, TypeConfiguration.ReferenceField reference)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            m_owner = owner;
            m_reverseProperty = reference.LocalProperty;

            if (m_reverseProperty == null)
                throw new System.Exception("Class " + typeof(DATACLASS).FullName + " does not contain the property " + reference.PropertyName);

            IDataClass db = owner as IDataClass;
            if (db != null && db.RelationManager != null)
            {
                if (db.RelationManager.ExistsInDb(db))
                    db.DataParent.GetObjects<DATACLASS>(reference.LocalField.PropertyName + "=?", db.UniqueValue);

                if (reference.LocalProperty == null)
                    throw new Exception(string.Format("Type {0} does not contain property {1}, which is required for a relation from type {2}", typeof(DATACLASS).FullName, reference.PropertyName, owner.GetType().FullName));
                foreach (DATACLASS o in (db.DataParent as DataFetcherCached).GetObjectsFromCache<DATACLASS>(""))
                    if (reference.LocalProperty.GetValue(o, null) == owner)
                        m_baseList.Add(o);

                //TODO: Perhaps it would be nice if it was possible to query the object for the Guid?
                //m_baseList.AddRange((db.DataParent as DataFetcherCached).GetObjectsFromCache<DATACLASS>(reversePropertyname + ".Guid=?", db.RelationManager.GetGuidForObject(owner)));
            }
        }

        protected void UpdateReverse(object item, bool remove)
        {
            if (m_reverseProperty != null && m_owner != null)
                if (m_reverseProperty.PropertyType.IsAssignableFrom(m_owner.GetType()))
                {
                    object nval = remove ? null : m_owner;
                    if (m_reverseProperty.GetValue(item, null) != nval)
                        m_reverseProperty.SetValue(item, remove ? null : m_owner, null);
                }
                else
                {
                    object col = m_reverseProperty.GetValue(item, null);
                    if (col == null)
                        return;
                    else if (col as System.Collections.ICollection == null)
                        throw new System.Exception("Reverse property must be of type " + m_owner.GetType().FullName + " or ICollection");

                    System.Reflection.MethodInfo mi = col.GetType().GetMethod("Contains");
                    if (mi == null)
                        throw new System.Exception("Reverse property type " + col.GetType().FullName + " does not contain a method called Contains");
                    bool contains = System.Convert.ToBoolean(mi.Invoke(col, new object[] { m_owner }));
                    if (!contains)
                    {
                        mi = col.GetType().GetMethod(remove ? "Remove" : "Add");
                        if (mi == null)
                            throw new System.Exception("Reverse property type " + col.GetType().FullName + " does not contain a method called " + (remove ? "Remove" : "Add"));
                        mi.Invoke(col, new object[] { m_owner });
                    }

                }

        }

        protected virtual void HookItem(object item)
        {
            UpdateReverse(item, false);
        }

        protected virtual void UnhookItem(object item)
        {
            UpdateReverse(item, true);
        }

        public virtual void Add(DATACLASS item)
        {
            m_baseList.Add(item);
            HookItem(item);
        }

        public virtual void AddRange(IEnumerable<DATACLASS> items)
        {
            foreach (object o in items)
                HookItem(o);

            m_baseList.AddRange(items);
        }

        public virtual bool Contains(DATACLASS item)
        {
            return m_baseList.Contains(item);
        }

        public virtual void CopyTo(DATACLASS[] destination)
        {
            m_baseList.CopyTo(destination);
        }

        public virtual void CopyTo(DATACLASS[] destination, int index)
        {
            m_baseList.CopyTo(destination, index);
        }

        public virtual void CopyTo(int sourceindex, DATACLASS[] destination, int destinationindex, int count)
        {
            m_baseList.CopyTo(sourceindex, destination, destinationindex, count);
        }

        public virtual int Count { get { return m_baseList.Count; } }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_baseList.GetEnumerator();
        }

        public virtual IEnumerator<DATACLASS> GetEnumerator()
        {
            return m_baseList.GetEnumerator();
        }

        public virtual List<DATACLASS> GetRange(int index, int count)
        {
            List<DATACLASS> c = new List<DATACLASS>();
            c.AddRange(m_baseList.GetRange(index, count));
            return c;
        }


        public virtual int IndexOf(DATACLASS item)
        {
            return m_baseList.IndexOf(item);
        }

        public virtual int IndexOf(DATACLASS item, int startIndex)
        {
            return m_baseList.IndexOf(item, startIndex);
        }

        public virtual int IndexOf(DATACLASS item, int startIndex, int count)
        {
            return m_baseList.IndexOf(item, startIndex, count);
        }

        public virtual void Insert(int index, DATACLASS item)
        {
            m_baseList.Insert(index, item);
            HookItem(item);
        }

        public virtual void InsertRange(int index, IEnumerable<DATACLASS> items)
        {
            foreach (object o in items)
                HookItem((DATACLASS)o);

            m_baseList.InsertRange(index, items);
        }

        public virtual bool TrueForAll(Predicate<DATACLASS> match)
        {
            return m_baseList.TrueForAll(match);
        }

        public virtual bool IsReadOnly { get { return false; } }

        public virtual int LastIndexOf(DATACLASS item)
        {
            return m_baseList.LastIndexOf(item);
        }

        public virtual int LastIndexOf(DATACLASS item, int startIndex)
        {
            return m_baseList.LastIndexOf(item, startIndex);
        }

        public virtual int LastIndexOf(DATACLASS item, int startIndex, int count)
        {
            return m_baseList.LastIndexOf(item, startIndex, count);
        }

        public virtual bool Remove(DATACLASS item)
        {
            if (m_baseList.Contains(item))
            {
                UnhookItem(item);
                return m_baseList.Remove(item);
            }

            return false;
        }

        public virtual void RemoveAt(int index)
        {
            UnhookItem((DATACLASS)m_baseList[index]);
            m_baseList.RemoveAt(index);
        }

        public virtual void Clear()
        {
            RemoveRange(0, this.Count);
        }

        public virtual void RemoveRange(int index, int count)
        {
            for (int i = 0; i < count; i++)
                RemoveAt(index);
        }

        public virtual void Reverse(int index, int count)
        {
            m_baseList.Reverse(index, count);
        }

        public virtual void Reverse()
        {
            m_baseList.Reverse();
        }


        public virtual void Sort(int index, int count, IComparer<DATACLASS> comparer)
        {
            m_baseList.Sort(index, count, comparer);
        }

        public virtual void Sort(IComparer<DATACLASS> comparer)
        {
            m_baseList.Sort(comparer);
        }

        public virtual void Sort()
        {
            m_baseList.Sort();
        }

        public virtual DATACLASS[] ToArray()
        {
            return m_baseList.ToArray();
        }

        public virtual void TrimExcess()
        {
            m_baseList.TrimExcess();
        }

        public virtual DATACLASS this[int index]
        {
            get { return (DATACLASS)m_baseList[index]; }
            set
            {
                UnhookItem((DATACLASS)m_baseList[index]);
                HookItem(value);
                m_baseList[index] = value;
            }
        }



    }
}
