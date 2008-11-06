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

namespace System.Data.LightDatamodel
{

	//public interface IDataFetcherCached : IDataFetcher
	//{
	//    event ObjectStateChangeHandler ObjectAddRemove;
		
	//    object[] GetObjectsFromCache(Type type, QueryModel.Operation query);
	//    object[] GetObjectsFromCache(Type type, string filter, params object[] parameters);
	//    DATACLASS[] GetObjectsFromCache<DATACLASS>(QueryModel.Operation query) where DATACLASS : IDataClass;
	//    DATACLASS[] GetObjectsFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
	//    object GetObjectFromCache(Type type, QueryModel.Operation query);
	//    object GetObjectFromCache(Type type, string filter, params object[] parameters);
	//    DATACLASS GetObjectFromCache<DATACLASS>(QueryModel.Operation query) where DATACLASS : IDataClass;
	//    DATACLASS GetObjectFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;

	//    object GetObjectByIndex(Type type, string indexname, object indexvalue);
	//    DATACLASS GetObjectByIndex<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass;
	//    object[] GetObjectsByIndex(Type type, string indexname, object indexvalue);
	//    DATACLASS[] GetObjectsByIndex<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass;

	//    void AddIndex(Type type, string indexname);
	//    void RemoveIndex(Type type, string indexname);

	//    bool IsDirty { get; }
	//    void ClearCache();

	//    /// <summary>
	//    /// Will commit all cached objects to the DB
	//    /// </summary>
	//    /// <exception cref="">NoSuchObjectException</exception>
	//    void CommitAll();
	//    void DiscardObject(IDataClass obj);
	//}

	/// <summary>
	/// Will fire when creating or removing an object
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="obj"></param>
	/// <param name="oldstate"></param>
	/// <param name="newstate"></param>
	public delegate void ObjectStateChangeHandler(object sender, IDataClass obj, ObjectStates oldstate, ObjectStates newstate);

}
