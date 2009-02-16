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

	public class DataFetcherNested : DataFetcherWithRelations
	{
		private IDataFetcher m_baseFetcher;
		private Dictionary<IDataClass, IDataClass> m_originalobjects = new Dictionary<IDataClass, IDataClass>();
		private Dictionary<IDataClass, IDataClass> m_tempobjects = new Dictionary<IDataClass, IDataClass>();

		public IDataFetcher BaseFetcher
		{
			get { return m_baseFetcher; }
		}

		public DataFetcherNested(IDataFetcher basefetcher)
			: base(basefetcher.Provider)
		{
            m_baseFetcher = basefetcher;
		}

		/// <summary>
		/// This will commit all the way to the database
		/// </summary>
		public void CommitAllComplete()
		{
			CommitAll();
			if (m_baseFetcher != typeof(DataFetcher))
			{
				if (m_baseFetcher is DataFetcherNested) ((DataFetcherNested)m_baseFetcher).CommitAllComplete();
				else (m_baseFetcher as DataFetcherCached).CommitAll();
			}
		}

		protected override object[] LoadObjects(Type type, QueryModel.Operation op)
		{
			object[] tmp = m_baseFetcher.GetObjects(type, op);
			for (int i = 0; i < tmp.Length; i++)
			    tmp[i] = ConvertBaseObject(tmp[i] as IDataClass);
			return tmp;
		}

		private IDataClass ConvertBaseObject(IDataClass obj)
		{
			if (m_originalobjects.ContainsKey(obj)) return m_originalobjects[obj];
			else
			{
				IDataClass localcopy = (IDataClass)Activator.CreateInstance(obj.GetType());
				m_originalobjects.Add(obj, localcopy);
				m_tempobjects.Add(localcopy, obj);
				ObjectTransformer.CopyObject(obj, localcopy);
				CopyRelationsFromSourceFetcher(obj, localcopy);
				return localcopy;
			}
		}

		public override void LoadAndCacheObjects(params Type[] types)
		{
			DataFetcherCached conn = m_baseFetcher as DataFetcherCached;
			if (conn == null) throw new Exception("The current base-fecther doesn't support the LoadAndCacheObjects");

			//load from base fetcher
			conn.LoadAndCacheObjects(types);

			foreach (Type t in types)
			{
				object[] objs = m_baseFetcher.GetObjects(t);
				if (objs != null)
				{
					//create local copies
					object[] converted = new object[objs.Length];
					int i = 0;
					foreach (object o in objs) converted[i++] = ConvertBaseObject((IDataClass)o);

					//insert manually in cache
					InsertObjectsInCache(converted);
				}

				//register loaded objects
				HasLoaded(t, new QueryModel.Operation(QueryModel.Operators.NOP));
			}
		}

		protected override void InsertObject(object obj)
		{
			IDataClass item = (IDataClass)Activator.CreateInstance(obj.GetType());
			m_originalobjects.Add(item, (IDataClass)obj);
			m_tempobjects.Add((IDataClass)obj, item);
			ObjectTransformer.CopyObject((IDataClass)obj, item);
            ((DataClassBase)item).ObjectState = ObjectStates.Default;
			m_baseFetcher.Add(item);
            ((DataClassBase)item).ObjectState = ((IDataClass)obj).ObjectState;
			CopyRelationsToSourceFetcher((IDataClass)obj, item);
            
            ((DataClassBase)obj).m_originalvalues = null;
			((DataClassBase)obj).m_isdirty = false;
			((DataClassBase)obj).m_state = ObjectStates.Default;
		}

        public override void RefreshObject(IDataClass obj)
        {
            //We do nothing, because the copy is purely in-memory, so there is no chance the properties have changed
            UpdateObjectKeys(obj);
            ((DataClassBase)obj).m_isdirty = false;
        }

		protected override void UpdateObject(object obj)
		{
			IDataClass localcopy = (IDataClass)obj;
			IDataClass originalobject = m_tempobjects[localcopy];
			ObjectTransformer.CopyObject(localcopy, originalobject);
			CopyRelationsToSourceFetcher(localcopy, originalobject);		//should we trigger events here?
			((DataClassBase)(IDataClass)obj).m_isdirty = false;
		}

		protected override void RemoveObject(object obj)
		{
			m_baseFetcher.DeleteObject(m_tempobjects[obj as IDataClass]);
		}

		/// <summary>
		/// Will copy loose relations from base fetcher
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		private void CopyRelationsFromSourceFetcher(IDataClass source, IDataClass target)
		{
			//take care of relations (we have to copy loosely attatched objects along)
			DataFetcherWithRelations sourceManager = source.DataParent as DataFetcherWithRelations;
			DataFetcherWithRelations targetManager = target.DataParent as DataFetcherWithRelations;
			if (sourceManager != null && targetManager != null)
			{

				foreach (ObjectConnection rel in sourceManager.ObjectRelationCache[source].Values)
					foreach (IDataClass obj in sourceManager.ObjectRelationCache[source][rel.Relation.Name].SubObjects.Values)
					{
						IDataClass localcopy = m_originalobjects.ContainsKey(obj) ? m_originalobjects[obj] : null;

						//create copy
						if (localcopy == null)
						{
							localcopy = ConvertBaseObject(obj);
							HookObject(localcopy);					//this one differs from LoadObjects
							InsertObjectsInCache(localcopy);		//this one differs from LoadObjects
							targetManager.AddRelatedObject(rel.Relation.Name, target, localcopy);
						}
					}
			}
		}

		/// <summary>
		/// Will merge relations back to base fetcher
		/// </summary>
		/// <param name="source">This is actualy the nested object</param>
		/// <param name="target">This is the base fetcher object</param>
		private void CopyRelationsToSourceFetcher(IDataClass source, IDataClass target)
		{
			DataFetcherWithRelations sourceManager = source.DataParent as DataFetcherWithRelations;
			DataFetcherWithRelations targetManager = target.DataParent as DataFetcherWithRelations;
			if (sourceManager != null && targetManager != null)
			{
				foreach (ObjectConnection rel in sourceManager.ObjectRelationCache[source].Values)
				{
					SortedList<int, IDataClass> sourcerelations = sourceManager.ObjectRelationCache[source][rel.Relation.Name].SubObjects;
					SortedList<int, IDataClass> targetrelations = targetManager.ObjectRelationCache[target][rel.Relation.Name].SubObjects;

					//Now, MERGE!!! ..... wrrrrrnnnnn cruncy cruncy ... actually, let's just overwrite it
					targetrelations.Clear();
					foreach (IDataClass obj in sourcerelations.Values)
					{
						if (!m_tempobjects.ContainsKey(obj)) Commit(obj);	//relations will be copied here
						else targetManager.AddRelatedObject(rel.Relation.Name, target, m_tempobjects[obj]);
					}
				}
			}
		}
	}
}
