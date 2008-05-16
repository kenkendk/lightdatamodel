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

	public interface IDataFetcherCached : IDataFetcher
	{
		event ObjectStateChangeHandler ObjectAddRemove;
		
		DATACLASS GetObjectByGuid<DATACLASS>(Guid guid) where DATACLASS : IDataClass;
		object GetObjectByGuid(Guid guid);
		object[] GetObjectsFromCache(Type type, QueryModel.Operation query);
		object[] GetObjectsFromCache(Type type, string filter, params object[] parameters);
		DATACLASS[] GetObjectsFromCache<DATACLASS>(QueryModel.Operation query) where DATACLASS : IDataClass;
		DATACLASS[] GetObjectsFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
		//void Remove(IDataClass obj);
        IRelationManager RelationManager { get; }
		bool IsDirty { get; }
	}

	/// <summary>
	/// Will fire when creating or removing an object
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="obj"></param>
	/// <param name="oldstate"></param>
	/// <param name="newstate"></param>
	public delegate void ObjectStateChangeHandler(object sender, IDataClass obj, ObjectStates oldstate, ObjectStates newstate);

    public interface IRelationManager
    {
        void CommitItems(IDataFetcher fetcher, List<IDataClass> added, List<IDataClass> deleted, List<IDataClass> modified);
        bool ExistsInDb(IDataClass item);
        Guid GetGuidForObject(IDataClass item);
        IDataClass GetObjectByGuid(Guid g);
        object GetReferenceObject(IDataClass owner, TypeConfiguration.ReferenceField reference);
        T GetReferenceObject<T>(IDataClass owner, string propertyname);
        object GetReferenceObject(IDataClass owner, string propertyname);
        SyncCollectionBase<T> GetReferenceCollection<T>(IDataClass owner, string propertyname) where T : IDataClass;

        void SetReferenceObject(IDataClass owner, string propertyname, IDataClass value);
        void SetReferenceObject<T>(IDataClass owner, string propertyname, T value) where T : IDataClass;

        string GetUniqueColumn(Type type);
        bool IsRegistered(IDataClass item);
        void ReassignGuid(Guid oldGuid, Guid newGuid);
        Guid RegisterObject(Guid g, IDataClass item);
        Guid RegisterObject(IDataClass item);
        void UnregisterObject(IDataClass item);
        void UnregisterObject(Guid g);
        void SetExistsInDb(IDataClass item, bool state);
        Dictionary<string, Guid> GetReferenceObjects(IDataClass item);
        void SetReferenceObjects(IDataClass item, Dictionary<string, Guid> references);
        void SetReferenceObject(IDataClass owner, TypeConfiguration.ReferenceField reference, IDataClass value);
        bool HasGuid(Guid g);

		void AddRelation<REVERSEDATACLASS, LOCALDATACLASS>(string reservekeypropertyname, string localkeypropertyname, string localrelationproperty);

    }

}
