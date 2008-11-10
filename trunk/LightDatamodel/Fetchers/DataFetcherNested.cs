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
			object[] res = (object[])Array.CreateInstance(type, tmp.Length);
			for (int i = 0; i < tmp.Length; i++)
			{
				if (m_originalobjects.ContainsKey(tmp[i] as IDataClass)) res[i] = m_originalobjects[tmp[i] as IDataClass];
				else
				{
					//create local copy
					IDataClass localcopy = (IDataClass)Activator.CreateInstance(tmp[i].GetType());
					m_originalobjects.Add(tmp[i] as IDataClass, localcopy as IDataClass);
					m_tempobjects.Add(localcopy as IDataClass, tmp[i] as IDataClass);
					ObjectTransformer.CopyObject(tmp[i] as IDataClass, localcopy as IDataClass);
					HookObject(localcopy as IDataClass);
					res[i] = localcopy;
					CopyRelationsFromSourceFetcher(tmp[i] as IDataClass, localcopy);
				}
			}

			return res;
		}

		protected override void InsertObject(object obj)
		{
			IDataClass item = (IDataClass)Activator.CreateInstance(obj.GetType());
			m_originalobjects.Add(item, (IDataClass)obj);
			m_tempobjects.Add((IDataClass)obj, item);
			ObjectTransformer.CopyObject((IDataClass)obj, item);
			m_baseFetcher.Add(item);
			CopyRelationsToSourceFetcher((IDataClass)obj, item);
			((DataClassBase)obj).m_isdirty = false;
			((DataClassBase)obj).m_state = ObjectStates.Default;
		}

		protected override void UpdateObject(object obj)
		{
			ObjectTransformer.CopyObject((IDataClass)obj, m_tempobjects[(IDataClass)obj]);
			CopyRelationsToSourceFetcher((IDataClass)obj, m_tempobjects[(IDataClass)obj]);		//should we trigger events here?
			((DataClassBase)m_tempobjects[(IDataClass)obj]).m_isdirty = false;
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
							localcopy = (IDataClass)Activator.CreateInstance(obj.GetType());
							m_originalobjects.Add(obj, localcopy);
							m_tempobjects.Add(localcopy, obj);
							ObjectTransformer.CopyObject(obj, localcopy);
							HookObject(localcopy);
							InsertObjectsInCache(localcopy);		//this one differs from LoadObjects
							CopyRelationsFromSourceFetcher(obj, localcopy);
							targetManager.AddRelatedObject(rel.Relation.Name, target, localcopy);//???
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
					SortedList<IDataClass, IDataClass> sourcerelations = sourceManager.ObjectRelationCache[source][rel.Relation.Name].SubObjects;
					SortedList<IDataClass, IDataClass> targetrelations = targetManager.ObjectRelationCache[target][rel.Relation.Name].SubObjects;

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
