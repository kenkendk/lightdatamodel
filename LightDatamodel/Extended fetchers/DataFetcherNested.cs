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
	/// Nested data fetcher, use as an in memory transaction
	/// </summary>
	public class DataFetcherNested : DataFetcherCached
	{
        private IDataFetcherCached m_baseFetcher;
		public DataFetcherNested(IDataFetcherCached basefetcher) : base(basefetcher.Provider)
		{
            m_transformer = basefetcher.ObjectTransformer;
            m_relationManager = new RelationManager(this);
            m_baseFetcher = basefetcher;

		}

        /// <summary>
        /// Gets an object by key, this is faster than an ordinary load.
        /// If possible, this call will not touch the database.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override object GetObjectById(Type type, object id)
        {
            object tmp = base.GetObjectById(type, id);
            if (tmp == null)
                return null;
            return ProcessLoad(tmp);
        }

        /// <summary>
        /// Returns an object by its Guid
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object GetObjectByGuid(Guid key)
        {
            object item = base.GetObjectByGuid(key);
            if (item == null)
            {
                item = m_baseFetcher.GetObjectByGuid(key);
                if (item == null) return null;

                InsertObjectsInCache(new object[] { ProcessLoad(item) });
            }

            return item;
        }

        #region Provider interactions
        /* The methods in this regions does all interaction with the provider.
         * This approach enables easy overrides of provider interaction,
         * when using the Nested provider.
         * 
         * It also removes clutter and varying ways to interact with the provider
         */

        protected override object[] LoadObjects(Type type, QueryModel.Operation op)
        {
            object[] tmp = m_baseFetcher.GetObjects(type, op);
            object[] res = new object[tmp.Length];
            for (int i = 0; i < tmp.Length; i++)
                res[i] = ProcessLoad(tmp[i]);

            return res;
        }

        /// <summary>
        /// Does all bookeeping when inserting an object into the fetcher
        /// </summary>
        /// <param name="tmp"></param>
        /// <returns></returns>
        private object ProcessLoad(object tmp)
        {
            object res = null;
            Guid g = (tmp as IDataClass).RelationManager.GetGuidForObject(tmp as IDataClass);
			if (m_relationManager != null && m_relationManager.HasGuid(g))
                res = m_relationManager.GetObjectByGuid(g);
            else
            {
                res = Activator.CreateInstance(tmp.GetType());
                (res as DataClassBase).m_dataparent = this;
            }
            ObjectTransformer.CopyObject(tmp, res);


            return res;
        }

        protected override void InsertObject(object obj)
        {
            object item = m_baseFetcher.Add(obj.GetType());
            ObjectTransformer.CopyObject(obj, item);
            ((DataClassBase)obj).m_isdirty = false;
            ((DataClassBase)obj).m_state = ObjectStates.Default;
        }

        protected override void UpdateObject(object obj)
        {
            ObjectTransformer.CopyObject(obj, m_baseFetcher.GetObjectByGuid(this.RelationManager.GetGuidForObject((IDataClass)obj)));
            if (((IDataClass)obj).IsDirty)
                ((IDataClass)m_baseFetcher.GetObjectByGuid(this.RelationManager.GetGuidForObject((IDataClass)obj))).SetDirty();
        }

        protected override void RemoveObject(object obj)
        {
            object tmp = m_baseFetcher.GetObjectByGuid(m_relationManager.GetGuidForObject((IDataClass)obj));
            m_baseFetcher.DeleteObject(tmp);
            m_relationManager.UnregisterObject((IDataClass)obj);
        }

        #endregion


    }
}
