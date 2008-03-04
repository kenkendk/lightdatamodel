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

namespace System.Data.LightDatamodel
{

	/// <summary>
	/// Interface for all data fetchers
	/// </summary>
	public interface IDataFetcher
	{
		event DataWriteEventHandler BeforeDataChange;
		event DataWriteEventHandler AfterDataChange;
		event DataConnectionEventHandler BeforeDataConnection;
		event DataConnectionEventHandler AfterDataConnection;

		DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
		DATACLASS[] GetObjects<DATACLASS>() where DATACLASS : IDataClass;
		DATACLASS GetObjectById<DATACLASS>(object id) where DATACLASS : IDataClass;
		void Commit(IDataClass obj);
		DATACLASS CreateObject<DATACLASS>() where DATACLASS : IDataClass;
		IDataProvider Provider { get; }
		RETURNVALUE Compute<RETURNVALUE, DATACLASS>(string expression, string filter);
		void DeleteObject<DATACLASS>(object id) where DATACLASS : IDataClass;

	}

	public interface IDataClass
	{
		IDataFetcher DataParent { get ; }
		bool IsDirty{get;}
		ObjectStates ObjectState{get;}
		string UniqueColumn	{get;}
		object UniqueValue{get;}
	}

	public delegate void DataConnectionEventHandler(object sender, DataActions action);
	public delegate void DataWriteEventHandler(object sender, string propertyname, object oldvalue, object newvalue);

	/// <summary>
	/// This data structure is used to return data from a select performed on the provider into the datafetcher.
	/// </summary>
	public struct Data
	{
		public string Name;
		public object Value;
		public Type Type;
		public Data(string name, object value, Type type)
		{
			Name = name;
			Value = value;
			Type = type;
		}
		public Data(string name, object value)
		{
			Name = name;
			Value = value;
			Type = null;
		}
	}

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
		Extended = 3,
	}
}
