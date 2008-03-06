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

namespace System.Data.LightDatamodel
{

	public interface IDataFetcherCached : IDataFetcher
	{
		DATACLASS[] GetObjectsFromCache<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass;
		DATACLASS[] GetObjectsFromCache<DATACLASS>(QueryModel.Operation operation) where DATACLASS : IDataClass;
		object[] GetObjectsFromCache(Type type, QueryModel.Operation operation);
		DATACLASS[] GetReferenceObjects<DATACLASS>(string reverseProperty, string idProperty, IDataClass refObj) where DATACLASS : IDataClass;
		DATACLASS GetObjectByGuid<DATACLASS>(Guid guid) where DATACLASS : IDataClass;
		object GetObjectByGuid(Guid guid);
		DATACLASS[] GetObjects<DATACLASS>(QueryModel.Operation operation) where DATACLASS : IDataClass;
		object[] GetObjects(Type type, QueryModel.Operation operation);
		void Remove(IDataClass obj);
		void Add(IDataClass obj);
	}

	/// <summary>
	/// This class is used to transfer data to and from a configureable data provider
	/// </summary>
	public class ConfigureProperties
	{
		public string Connectionstring;
		public string DestinationDir;
		public string Namespace;
	}

	/// <summary>
	/// If a data provider supports assisted configuration, it must implement this interface.
	/// Must also have a default constructor.
	/// </summary>
	public interface IConfigureableDataProvider
	{
		/// <summary>
		/// This method should present a dialog for the user to configure the data provider.
		/// </summary>
		/// <param name="owner">The owner dialog</param>
		/// <param name="previousConnectionString">Any previously configured properties</param>
		/// <returns>The new connection properties, or null if the setup was cancelled</returns>
		ConfigureProperties Configure(System.Windows.Forms.Form owner, ConfigureProperties previousConnectionProperties);

		/// <summary>
		/// Returns a user identifiable name for the provider.
		/// </summary>
		string FriendlyName { get; }

		/// <summary>
		/// This method should return a configuration for the provider, given the commandline arguments, or return null if the commandline was not meaningfull to the provider.
		/// </summary>
		/// <param name="arguments">The commandline arguments</param>
		/// <returns>A configuration for the provider, or null if the arguments where not meaningfull for the provider</returns>
		ConfigureProperties AutoConfigure(string[] arguments);

		/// <summary>
		/// Returns a dataprovider, given the connectionstring
		/// </summary>
		/// <param name="connectionstring"></param>
		/// <returns></returns>
		IDataProvider GetProvider(string connectionstring);
	}
}
