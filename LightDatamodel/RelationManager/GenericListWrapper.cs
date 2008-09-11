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
    /// The type system for genrics does not allow one to assign List&gt;A&lt; = List&gt;B&lt; even though A is assignable from B.
    /// This class solves that by wrapping the inner list in a typesafe outer list
    /// </summary>
    /// <typeparam name="Tx">The outer type</typeparam>
    /// <typeparam name="Ty">The inner type</typeparam>
    public class GenericListWrapper<Tx, Ty> : IList<Tx> 
        where Tx : Ty 
        //where Ty : class
    {
        private IList<Ty> m_list;
        private Enumerator<Tx> m_enum;

        public GenericListWrapper(IList<Ty> inner)
        {
            m_list = inner;
            m_enum = new Enumerator<Tx>(m_list.GetEnumerator());
        }

        #region IList<Tx> Members

        public int IndexOf(Tx item)
        {
            return m_list.IndexOf((Ty)item);
        }

        public void Insert(int index, Tx item)
        {
            m_list.Insert(index, (Ty)item);
        }

        public void RemoveAt(int index)
        {
            m_list.RemoveAt(index);
        }

        public Tx this[int index]
        {
            get
            {
                return (Tx)m_list[index];
            }
            set
            {
                m_list[index] = (Ty)value;
            }
        }

        #endregion

        #region ICollection<Tx> Members

        public void Add(Tx item)
        {
            m_list.Add((Ty)item);
        }

        public void Clear()
        {
            m_list.Clear();
        }

        public bool Contains(Tx item)
        {
            return m_list.Contains((Ty)item);
        }

        public void CopyTo(Tx[] array, int arrayIndex)
        {
            for (int i = 0; i < m_list.Count; i++)
                array[i + arrayIndex] = (Tx)m_list[i];
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public bool IsReadOnly
        {
            get { return m_list.IsReadOnly; }
        }

        public bool Remove(Tx item)
        {
            return m_list.Remove((Ty)item);
        }

        #endregion

        #region IEnumerable<Tx> Members

        public IEnumerator<Tx> GetEnumerator()
        {
            return m_enum;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)m_list).GetEnumerator();
        }

        #endregion

        private class Enumerator<Tx> : IEnumerator<Tx> where Tx : Ty
        {

            IEnumerator<Ty> m_enm;
            public Enumerator(IEnumerator<Ty> enm)
            {
                m_enm = enm;
            }

            #region IEnumerator<Tx> Members

            public Tx Current
            {
                get { return (Tx)m_enm.Current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                m_enm.Dispose();
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return m_enm.Current; }
            }

            public bool MoveNext()
            {
                return m_enm.MoveNext();
            }

            public void Reset()
            {
                m_enm.Reset();
            }

            #endregion
        }
    }
}
