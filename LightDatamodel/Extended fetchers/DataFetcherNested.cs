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
	/// Nested data fetcher, shorthand class for:
	/// = new DataFetcher(new NestedDataProvider(basefetcher))
	/// </summary>
	public class DataFetcherNested : DataFetcherCached
	{
		public DataFetcherNested(IDataFetcherCached basefetcher) : base(new DataProviderNested(basefetcher))
		{
			((DataProviderNested)m_provider).KnownTypes = this.KnownTypes;
		}
	}

	/// <summary>
	/// Nested data provider, reads data from an existing DataFetcher.
	/// Use this class to perform in-memory transactions that can be easily undone.
	/// </summary>
	public class DataProviderNested : IDataProvider
	{
		protected IDataFetcherCached m_baseFetcher;
		protected IDataProvider m_provider;
		protected SortedList<string, Type> m_types = null;

		public SortedList<string, Type> KnownTypes { get { return m_types; } set { m_types = value; } }
		public IDataFetcherCached BaseFetcher { get { return m_baseFetcher; } }

		public DataProviderNested(IDataFetcherCached parent)
		{
			m_baseFetcher = parent;
			m_provider = m_baseFetcher.Provider;
		}

		#region IDataProvider Members

		public void DeleteRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			m_baseFetcher.Remove((DataClassExtended)m_baseFetcher.GetObjectById((System.Type)KnownTypes[tablename], primaryvalue));
		}

		public Data[] SelectRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			DataClassBase obj = (DataClassBase)m_baseFetcher.GetObjectById((System.Type)KnownTypes[tablename], primaryvalue);
			if (obj == null)
				return null;
			return FromObjectToData(obj);
		}

		public object Compute(string tablename, string expression, string filter)
		{
			return m_provider.Compute(tablename, expression, filter);
		}

		internal Data[] FromObjectToData(object o)
		{
			FieldInfo[] fi = o.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			ArrayList fields = new ArrayList();

			for(int i = 0; i < fi.Length; i++)
				if (fi[i].Name.StartsWith("m_"))
					fields.Add(new Data(fi[i].Name.Substring(2), fi[i].GetValue(o), fi[i].ReflectedType));
				else
					fields.Add(new Data(fi[i].Name, fi[i].GetValue(o), fi[i].ReflectedType));

			if (o as DataClassBase != null)
				foreach (string s in new string[] { "m_guid", "m_existsInDB", "m_referenceObjects" } )
				{
					FieldInfo bfi = typeof(DataClassExtended).GetField(s, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					if (bfi != null)
						fields.Add(new Data(bfi.Name, bfi.GetValue(o), null));
					//All 'special' values are tagged with type == null
				}
            
			return (Data[])fields.ToArray(typeof(Data));
		}

		internal void UpdateObject(object o, Data[] d)
		{
			bool isDataClassBase = o as DataClassBase != null;

			for(int i = 0; i < d.Length; i++)
			{
				FieldInfo fi = o.GetType().GetField("m_" + d[i].Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				if (fi != null && d[i].Type != null)
				{
					if (d[i].Value == null)
						fi.SetValue(o, null);
					else
						fi.SetValue(o, Convert.ChangeType(d[i].Value, fi.FieldType));
				}
					//We tag the Guid property with an invalid type, to avoid potential conflicts with similarly named properties
				else if (d[i].Type == null && isDataClassBase)
				{
					FieldInfo bfi = typeof(DataClassExtended).GetField(d[i].Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					if (bfi != null)
						bfi.SetValue(o, d[i].Value);
				}
			}
		}

		public Data[][] SelectRowsFromCache(string tablename, QueryModel.Operation operation)
		{
			object[] items = m_baseFetcher.GetObjectsFromCache(KnownTypes[tablename], operation);
			Data[][] v = new Data[items.Length][];
			for (int i = 0; i < items.Length; i++)
				v[i] = FromObjectToData(items[i]);
			return v;
		}

		public Data[][] SelectRows(string tablename, QueryModel.Operation operation)
		{
			object[] items = m_baseFetcher.GetObjects(KnownTypes[tablename], operation);
			Data[][] v = new Data[items.Length][];
			for(int i = 0; i < items.Length; i++)
				v[i] = FromObjectToData(items[i]);
			return v;
		}

		public Data[][] SelectRows(string tablename, string filter)
		{
			return SelectRows(tablename, filter, null);
		}

		public Data[][] SelectRows(string tablename, string filter, object[] values)
		{
			return SelectRows(tablename, QueryModel.Parser.ParseQuery(filter, values));
		}

		public void UpdateRow(string tablename, string primarycolumnname, object primaryvalue, params Data[] values)
		{
			DataClassBase obj = (DataClassBase)m_baseFetcher.GetObjectById(KnownTypes[tablename], primaryvalue);
			obj.m_isdirty = true;
			UpdateObject(obj, values);
		}

		public void InsertRow(string tablename, params Data[] values)
		{
			DataClassExtended o = (DataClassExtended)m_baseFetcher.CreateObject(KnownTypes[tablename]);
			Guid oldkey = o.m_guid;
			UpdateObject(o, values);
			if (m_baseFetcher as IDataFetcher == null)
				throw new Exception("Nested provider can only commit to fetcher derived from DataFetcher");
			((DataFetcherCached)m_baseFetcher).ReRegisterObjectGuid(oldkey, o);
		}

		public string GetPrimaryKey(string tablename)
		{
			return m_provider.GetPrimaryKey(tablename);
		}

		public string[] GetTablenames()
		{
			return m_provider.GetTablenames();
		}

		public Data[] GetStructure(string sql)
		{
			return m_provider.GetStructure(sql);
		}

		public Data[] GetTableStructure(string tablename)
		{
			return m_provider.GetTableStructure(tablename);
		}

		public void Close()
		{
		}

		public string ConnectionString
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		public object GetNullValue(Type type)
		{
			return m_provider.GetNullValue(type);
		}

		public void BeginTransaction(Guid id)
		{
			//TODO: Implement these...	
		}

		public void CommitTransaction(Guid id)
		{
			//TODO: Implement these...	
		}

		public void RollbackTransaction(Guid id)
		{
			//TODO: Implement these...	
		}

		public object GetLastAutogeneratedValue(string tablename)
		{
			return null;
		}


		#endregion

	}
}
