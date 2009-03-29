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
using System.Collections.Generic;

namespace System.Data.LightDatamodel
{

	/// <summary>
	/// Interface for all data fetchers
	/// </summary>
	public interface IDataFetcher : IDisposable 
	{
		event DataChangeEventHandler BeforeDataChange;
		event DataChangeEventHandler AfterDataChange;
		event DataConnectionEventHandler BeforeDataConnection;
		event DataConnectionEventHandler AfterDataConnection;

		DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
		DATACLASS[] GetObjects<DATACLASS>() where DATACLASS : IDataClass;
		DATACLASS[] GetObjects<DATACLASS>(QueryModel.OperationOrParameter operation) where DATACLASS : IDataClass;
		object[] GetObjects(Type type);
		object[] GetObjects(Type type, string filter, params object[] parameters);
		object[] GetObjects(Type type, QueryModel.OperationOrParameter operation);
		DATACLASS GetObject<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
		object GetObject(Type type, string filter, params object[] parameters);
		DATACLASS GetObjectById<DATACLASS>(object id) where DATACLASS : IDataClass;
		object GetObjectById(Type type, object id);
		void Commit(params IDataClass[] obj);
		DATACLASS Add<DATACLASS>() where DATACLASS : IDataClass;
		object Add(Type type);
		IDataClass Add(IDataClass newobj);
		IDataProvider Provider { get; }
        RETURNVALUE Compute<RETURNVALUE, DATACLASS>(string expression, string filter, params object[] parameters) where DATACLASS : IDataClass;
		void DeleteObject<DATACLASS>(object id) where DATACLASS : IDataClass;
        void DeleteObject(object item);
		void RefreshObject(IDataClass obj);
		TypeConfiguration Mappings { get; }
	}

    public interface IDataFetcherCached : IDataFetcher
    {
        List<IDataClass> CommitAll();
        List<IDataClass> CommitAll(UpdateProgressHandler updatefunction);

        void ClearCache();
        void DiscardObject(IDataClass obj);
        void CommitRecursive(params IDataClass[] items);
        void CommitAllRecursive();
        List<IDataClass> CommitRecursiveWithRelations(params IDataClass[] items);
        List<IDataClass> CommitWithRelations(params IDataClass[] items);
        List<IDataClass> FindObjectRelations(IDataClass item);

        DATACLASS GetObjectByIndex<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass;
        object GetObjectByIndex(Type type, string indexname, object indexvalue);
        object GetObjectFromCache(Type type, string filter, params object[] parameters);
		object GetObjectFromCache(Type type, System.Data.LightDatamodel.QueryModel.OperationOrParameter query);
        DATACLASS GetObjectFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
		DATACLASS GetObjectFromCache<DATACLASS>(System.Data.LightDatamodel.QueryModel.OperationOrParameter query) where DATACLASS : IDataClass;
        DATACLASS GetObjectFromCacheById<DATACLASS>(object id);
        object GetObjectFromCacheById(Type type, object id);
        object[] GetObjectsByIndex(Type type, string indexname, object indexvalue);
        DATACLASS[] GetObjectsByIndex<DATACLASS>(string indexname, object indexvalue) where DATACLASS : IDataClass;
		object[] GetObjectsFromCache(Type type, System.Data.LightDatamodel.QueryModel.OperationOrParameter query);
        object[] GetObjectsFromCache(Type type, string filter, params object[] parameters);
        DATACLASS[] GetObjectsFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
		DATACLASS[] GetObjectsFromCache<DATACLASS>(System.Data.LightDatamodel.QueryModel.OperationOrParameter query) where DATACLASS : IDataClass;
        bool IsDirty { get; }
        void LoadAndCacheObjects(params Type[] types);
        DataFetcherCached.Cache LocalCache { get; }
    }

	public interface IDataClass
	{
		IDataFetcher DataParent { get; set;}
		bool IsDirty{get;}
		ObjectStates ObjectState{get;}
		event DataChangeEventHandler BeforeDataChange;
		event DataChangeEventHandler AfterDataChange;
		event DataConnectionEventHandler BeforeDataCommit;
		event DataConnectionEventHandler AfterDataCommit;

	}


	public delegate void DataConnectionEventHandler(object sender, DataActions action);
	public delegate void DataChangeEventHandler(object sender, string propertyname, object oldvalue, object newvalue);
	public delegate void UpdateProgressHandler(int currentvalue, int maximumvalue);

	/// <summary>
	/// This enum represents the different states an object may have
	/// </summary>
	public enum ObjectStates
	{
		/// <summary>
		/// The object is loaded from the database, and may be modified
		/// </summary>
		Default,

		/// <summary>
		/// The object is not yet created in the database
		/// </summary>
		New,

		/// <summary>
		/// The object has been marked for deletion in the database
		/// </summary>
		Deleted,
	}

	/// <summary>
	/// This enum represents the different command types a provider can perform
	/// </summary>
	public enum DataActions
	{
		Update,
		Insert,
		Delete,
		Fetch,
	}

	/// <summary>
	/// DataClass functionality levels (rising)
	/// </summary>
	public enum DataClassLevels
	{
		NoInheritance = 0,
		View = 1,
		Base = 2,
	}

    public delegate void ObjectStateChangeHandler(object sender, IDataClass obj, ObjectStates oldstate, ObjectStates newstate);
}
