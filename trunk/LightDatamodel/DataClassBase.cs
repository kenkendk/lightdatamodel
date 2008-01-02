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
using System.Data;
using System.Collections;
using System.Reflection;

namespace System.Data.LightDatamodel
{

	#region " DataClassBase "

	/// <summary>
	/// Base class for non updateable data classes (views)
	/// </summary>
	public abstract class DataCustomClassBase
	{
		internal protected DataFetcher m_dataparent;
	}

	/// <summary>
	/// Base class for all 'database classes'
	/// </summary>
	public abstract class DataClassBase
	{
		internal protected bool m_isdirty = true;
		internal protected DataFetcher m_dataparent;
		internal protected ObjectStates m_state = ObjectStates.Default;
		internal protected Guid m_guid = new Guid(0,0,0,0,0,0,0,0,0,0,0);
		public delegate void DataWriteEventHandler(object sender, string propertyname, object oldvalue, object newvalue);
		public event DataWriteEventHandler BeforeDataWrite;
		public event DataWriteEventHandler AfterDataWrite;

		public bool IsDirty{get{return m_isdirty;}}
		public ObjectStates ObjectState{get{return m_state;}}

		public abstract string UniqueColumn	{get;}
		public abstract object UniqueValue{get;}
		public void Commit()
		{
			m_dataparent.PopulateDatabase(this);
		}
		public void Update()
		{
			m_dataparent.UpdateDataClass(this);
		}
		public void Delete()
		{
			if(m_dataparent != null)
			{
				m_dataparent.Remove(this);
				Commit();
			}
		}

		protected void OnBeforeDataWrite(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(oldvalue == newvalue) return;
			if(BeforeDataWrite != null) BeforeDataWrite(sender, propertyname, oldvalue, newvalue);
		}
	
		protected void OnAfterDataWrite(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(oldvalue == newvalue) return;
			m_isdirty=true;
			if(AfterDataWrite != null) AfterDataWrite(sender, propertyname, oldvalue, newvalue);
		}
	}

	#endregion

	#region " DataFetcher "

	public delegate void DataConnectionEventHandler(object sender, DataActions action);

	public interface IDataFetcher
	{
		event DataClassBase.DataWriteEventHandler BeforeDataChange;
		event DataClassBase.DataWriteEventHandler AfterDataChange;
		event DataConnectionEventHandler BeforeDataCommit;
		event DataConnectionEventHandler AfterDataCommit;
		event DataConnectionEventHandler AfterDataFetch;

		void CommitAll();

		object[] GetObjects(Type type, string filter);
		object[] GetObjects(Type type, QueryModel.Operation operation);
		object GetObjectById(Type type, object id);
		void Remove(DataClassBase obj);
		void Add(DataClassBase newobj);
		object CreateObject(Type type);
		IDataProvider Provider { get; }
		Hashtable KnownTypes { get; }
		bool IsDirty { get; }
	}

	public class DataFetcher : IDataFetcher
	{
		private SortedList m_tables = new SortedList();
		private ArrayList m_newobjects = new ArrayList();
		private ArrayList m_deletedobjects = new ArrayList();
		private IDataProvider m_provider;
		public event DataClassBase.DataWriteEventHandler BeforeDataChange;
		public event DataClassBase.DataWriteEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataCommit;
		public event DataConnectionEventHandler AfterDataCommit;
		public event DataConnectionEventHandler AfterDataFetch;
		protected Hashtable m_knownTypes = new Hashtable();

		public bool IsDirty 
		{
			get 
			{
				if ((m_newobjects.Count > 0) || (m_deletedobjects.Count > 0))
					return true;

				foreach(SortedList table in m_tables.Values)
					foreach(DataClassBase obj in table.Values)
						if(obj.IsDirty) 
							return true;
				
				return false;
			}
		}

		public DataFetcher(IDataProvider provider)
		{
			m_provider = provider;
		}

		public Hashtable KnownTypes { get { return m_knownTypes; } }

		public virtual void CommitAll()
		{
			foreach(SortedList table in m_tables.Values)
				foreach(DataClassBase obj in table.Values)
					if(obj.IsDirty) 
						obj.Commit();

			ArrayList work = new ArrayList(m_newobjects);
			foreach(DataClassBase obj in work)
				obj.Commit();
			work = new ArrayList(m_deletedobjects);
			foreach(DataClassBase obj in work)
				obj.Commit();
		}

		protected virtual void HookObject(DataClassBase obj)
		{
			string tablename = obj.GetType().Name;
			obj.BeforeDataWrite += new System.Data.LightDatamodel.DataClassBase.DataWriteEventHandler(obj_BeforeDataWrite);
			obj.AfterDataWrite += new System.Data.LightDatamodel.DataClassBase.DataWriteEventHandler(obj_AfterDataWrite);
			if(!m_tables.ContainsKey(tablename)) m_tables.Add(tablename, new SortedList());
			if(((SortedList)m_tables[tablename]).ContainsKey(obj.UniqueValue)) ((SortedList)m_tables[tablename]).Remove(obj.UniqueValue);
			((SortedList)m_tables[tablename]).Add(obj.UniqueValue, obj);
		}

		protected virtual bool IsDataClassBase(Type type)
		{
			do
			{
				if(type == typeof(DataClassBase)) return true;
				type = type.BaseType;
			}while(type != null);
			return false;
		}

		protected virtual bool IsDataCustomClassBase(Type type)
		{
			do
			{
				if(type == typeof(DataCustomClassBase)) return true;
				type = type.BaseType;
			}while(type != null);
			return false;
		}

		public IDataProvider Provider { get { return m_provider; } }

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hook into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public virtual object[] GetObjects(Type type, string filter)
		{
			string tablename = type.Name;
			if (!m_knownTypes.ContainsKey(tablename))
				m_knownTypes.Add(tablename, type);

			Data[][] data = m_provider.SelectRows(tablename, filter);

			return InsertObjectsInCache(type, data);
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hook into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public virtual object[] GetObjects(Type type, QueryModel.Operation operation)
		{
			string tablename = type.Name;
			if (!m_knownTypes.ContainsKey(tablename))
				m_knownTypes.Add(tablename, type);

#if !SMARTASS_REDUCE_DB_LOAD
			QueryModel.Parameter[] p = new QueryModel.Parameter[((SortedList)m_tables[tablename]).Count + 1];
			int i = 1;
			foreach(object o in ((SortedList)m_tables[tablename]).Keys)
				p[i++] = new QueryModel.Parameter(o, false);

			p[0] = new QueryModel.Parameter(((DataClassBase)Activator.CreateInstance(type)).UniqueColumn, true);
			QueryModel.Operation op = new QueryModel.Operation(QueryModel.Operators.In, p);
			op = new QueryModel.Operation(QueryModel.Operators.Not, op);
			op = new QueryModel.Operation(QueryModel.Operators.And, new QueryModel.OperationOrParameter[] {op, operation});

			Data[][] data = m_provider.SelectRows(tablename, op);
#else
			Data[][] data = m_provider.SelectRows(tablename, operation);
#endif
			object[] items = InsertObjectsInCache(type, data);
			return operation.EvaluateList(((SortedList)m_tables[tablename]).Values);
		}


		private object[] InsertObjectsInCache(Type type, Data[][] data)
		{
			object[] ret = new object[data.Length];

			if(IsDataClassBase(type))
			{
				string tablename = type.Name;
				if (!m_knownTypes.ContainsKey(tablename))
					m_knownTypes.Add(tablename, type);
				if(!m_tables.ContainsKey(tablename)) m_tables.Add(tablename, new SortedList());

				for(int i = 0; i < ret.Length; i++)
				{
					DataClassBase newobj = (DataClassBase)Activator.CreateInstance(type);
					PopulateDataClass(newobj, data[i]);
					if(!((SortedList)m_tables[tablename]).ContainsKey(newobj.UniqueValue))
					{

						newobj.m_dataparent = this;
						newobj.m_state = ObjectStates.Default;
						newobj.m_isdirty = false;
						ret[i] = newobj;

						//add to cache
						HookObject(newobj);
						if(AfterDataFetch != null) AfterDataFetch(newobj, DataActions.Fetch);
					}
					else
						ret[i] = ((SortedList)m_tables[tablename])[newobj.UniqueValue];
				}
			}
			else if(IsDataCustomClassBase(type))
			{
				for(int i = 0; i < ret.Length; i++)
				{
					DataCustomClassBase newobj = (DataCustomClassBase)Activator.CreateInstance(type);
					PopulateDataClass(newobj, data[i]);
					newobj.m_dataparent = this;
					ret[i] = newobj;
				}
			}
			else
			{
				for(int i = 0; i < ret.Length; i++)
				{
					object newobj = Activator.CreateInstance(type);
					PopulateDataClass(newobj, data[i]);
					ret[i] = newobj;
				}
			}

			return ret;
		}

		/// <summary>
		/// Taken from NPersist
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		public void DeleteObject(object id, Type type)
		{
			DataClassBase tobedeleted = (DataClassBase)GetObjectById(type, id);
			if(tobedeleted != null)Remove(tobedeleted);
		}

		/// <summary>
		/// This will load the given DataClassBase object
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual object GetObjectById(Type type, object id)
		{
			string tablename = type.Name;
			if (!m_knownTypes.ContainsKey(tablename))
				m_knownTypes.Add(tablename, type);

			if(!IsDataClassBase(type)) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			if(!m_tables.ContainsKey(tablename)) m_tables.Add(tablename, new SortedList());
			if(!((SortedList)m_tables[tablename]).ContainsKey(id))
			{
				//Fetch From Data source
				DataClassBase newobj = (DataClassBase)Activator.CreateInstance(type);
				Data[] data = m_provider.SelectRow(tablename, newobj.UniqueColumn, id);
				if(data == null) return null;
				PopulateDataClass(newobj, data);
				newobj.m_dataparent = this;
				newobj.m_state = ObjectStates.Default;
				newobj.m_isdirty = false;
				HookObject(newobj);
				if(AfterDataFetch != null) AfterDataFetch(newobj, DataActions.Fetch);
			}
					
			return ((SortedList)m_tables[tablename])[id];
		}

		public virtual void Remove(DataClassBase obj)
		{
			string tablename = obj.GetType().Name;
			if (!m_knownTypes.ContainsKey(tablename))
				m_knownTypes.Add(tablename, obj.GetType().Name);

			if(obj.ObjectState == ObjectStates.New)
			{
				m_newobjects.Remove(obj);
			}
			else
			{
				if(!m_tables.ContainsKey(tablename)) m_tables.Add(tablename, new SortedList());
				((SortedList)m_tables[tablename]).Remove(obj.UniqueValue);
				m_deletedobjects.Add(obj);
				obj.m_state = ObjectStates.Deleted;
			}
		}

		public virtual void Add(DataClassBase newobj)
		{
			newobj.m_guid = Guid.NewGuid();
			newobj.m_dataparent = this;
			newobj.m_state = ObjectStates.New;
			m_newobjects.Add(newobj);
		}

		public virtual object CreateObject(Type type)
		{
			DataClassBase newobj = (DataClassBase)Activator.CreateInstance(type);
			Add(newobj);
			return newobj;
		}

		protected internal virtual void UpdateDataClass(DataClassBase obj)
		{
			if(obj.ObjectState == ObjectStates.Deleted) return;
			string tablename = obj.GetType().Name;
			if (!m_knownTypes.ContainsKey(tablename))
				m_knownTypes.Add(tablename, obj.GetType().Name);
			Data[] data = m_provider.SelectRow(tablename, obj.UniqueColumn, obj.UniqueValue);
			if(data == null) throw new Exception("Row (" + obj.UniqueValue + ") from table \"" + tablename + "\" can't be fetched");
			PopulateDataClass(obj, data);
			obj.m_dataparent = this;
			obj.m_state = ObjectStates.Default;
			obj.m_isdirty = false;
			if(AfterDataFetch != null) AfterDataFetch(obj, DataActions.Fetch);
		}

		protected internal virtual void PopulateDatabase(DataClassBase obj)
		{
			string tablename = obj.GetType().Name;
			if (!m_knownTypes.ContainsKey(tablename))
				m_knownTypes.Add(tablename, obj.GetType().Name);

			//get private fields
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			Data[] data = new Data[fields.Length];
			for(int i = 0; i < data.Length; i++)
				data[i] = new Data(fields[i].Name, fields[i].GetValue(obj), fields[i].FieldType);

			//sql
			if(obj.ObjectState == ObjectStates.Default)
			{
				if(BeforeDataCommit != null) BeforeDataCommit(obj, DataActions.Update);
				m_provider.UpdateRow(tablename, obj.UniqueColumn, obj.UniqueValue, data);
				if(AfterDataCommit != null) AfterDataCommit(obj, DataActions.Update);
			}
			else if(obj.ObjectState == ObjectStates.New)
			{
				if(BeforeDataCommit != null) BeforeDataCommit(obj, DataActions.Insert);
				m_provider.InsertRow(tablename, data);
				m_newobjects.Remove(obj);
				if(!m_tables.ContainsKey(tablename)) m_tables.Add(tablename, new SortedList());
				((SortedList)m_tables[tablename]).Add(obj.UniqueValue, obj);
				if(AfterDataCommit != null) AfterDataCommit(obj, DataActions.Insert);
			}
			else if(obj.ObjectState == ObjectStates.Deleted)
			{
				if(BeforeDataCommit != null) BeforeDataCommit(obj, DataActions.Delete);
				m_provider.DeleteRow(tablename, obj.UniqueColumn, obj.UniqueValue);
				m_deletedobjects.Remove(obj);
				if(AfterDataCommit != null) AfterDataCommit(obj, DataActions.Delete);
			}

			//read back
			obj.Update();
		}

		/// <summary>
		/// This will insert the given data into an arbitary object (the private variables)
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="data"></param>
		protected internal virtual void PopulateDataClass(object obj, Data[] data)
		{
			for(int i = 0; i < data.Length; i++)
			{
				FieldInfo field = obj.GetType().GetField("m_" + data[i].Name, BindingFlags.Instance | BindingFlags.NonPublic);
				try
				{
					if(field != null) 
					{
						if(data[i].Value != DBNull.Value)
							field.SetValue(obj, data[i].Value);
						else
							field.SetValue(obj, m_provider.GetNullValue(data[i].Type));
					}
				}
				catch(Exception ex)
				{
					throw new Exception("Couldn't set field\nError: " + ex.Message);
				}
			}
		}

		protected virtual void obj_BeforeDataWrite(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(BeforeDataChange != null) BeforeDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual void obj_AfterDataWrite(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(AfterDataChange != null) AfterDataChange(sender, propertyname, oldvalue, newvalue);
		}
	}

	/// <summary>
	/// Nested data fetcher, shorthand class for:
	/// = new DataFetcher(new NestedDataProvider(basefetcher))
	/// </summary>
	public class NestedDataFetcher : DataFetcher
	{
		public NestedDataFetcher(IDataFetcher basefetcher)
			: base(new NestedDataProvider(basefetcher))
		{
		}
	}

	public class NestedDataProvider : IDataProvider
	{
		public NestedDataProvider(IDataFetcher parent)
		{
			m_baseFetcher = parent;
			m_provider = m_baseFetcher.Provider;
			m_types = m_baseFetcher.KnownTypes;
		}


		private IDataFetcher m_baseFetcher;
		private IDataProvider m_provider;
		private Hashtable m_types;

		#region IDataProvider Members

		public void DeleteRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			m_baseFetcher.Remove((DataClassBase) m_baseFetcher.GetObjectById((System.Type)m_types[tablename], primaryvalue));
		}

		public Data[] SelectRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			return FromObjectToData((DataClassBase)m_baseFetcher.GetObjectById((System.Type)m_types[tablename], primaryvalue));
		}

		private Data[] FromObjectToData(object o)
		{
			FieldInfo[] fi = o.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			Data[] d = new Data[fi.Length];
			for(int i = 0; i < d.Length; i++)
				if (fi[i].Name.StartsWith("m_"))
					d[i] = new Data(fi[i].Name.Substring(2), fi[i].GetValue(o), fi[i].ReflectedType);
				else
					d[i] = new Data(fi[i].Name, fi[i].GetValue(o), fi[i].ReflectedType);
			return d;
		}

		private void UpdateObject(object o, Data[] d)
		{
			for(int i = 0; i < d.Length; i++)
			{
				string propname = d[i].Name;
				if (propname.StartsWith("m_"))
					propname = propname.Substring(2);
				PropertyInfo pi = o.GetType().GetProperty(propname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				if (pi != null && pi.CanWrite)
					if (d[i].Value == null)
						pi.SetValue(o, null ,null);
					else
						pi.SetValue(o, Convert.ChangeType(d[i].Value, pi.PropertyType), null);
			}
		}

		public Data[][] SelectRows(string tablename, QueryModel.Operation operation)
		{
			object[] items = m_baseFetcher.GetObjects((Type)m_types[tablename], operation);
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
			if (filter == null || filter.Trim().Length == 0)
				return SelectRows(tablename, new QueryModel.Operation(QueryModel.Operators.Not, new QueryModel.Parameter(false, false)));
			//TODO: Implement basic SQL parser
			throw new MissingMethodException();
		}

		public void UpdateRow(string tablename, string primarycolumnname, object primaryvalue, params Data[] values)
		{
			UpdateObject(m_baseFetcher.GetObjectById((Type)m_types[tablename], primaryvalue), values);
		}

		public void InsertRow(string tablename, params Data[] values)
		{
			UpdateObject(m_baseFetcher.CreateObject((Type)m_types[tablename]), values);
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

		public string FormatValue(object value)
		{
			return m_provider.FormatValue(value);
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

		#endregion

	}



	#endregion

	public interface IDataProvider
	{
		void DeleteRow(string tablename, string primarycolumnname, object primaryvalue);
		Data[] SelectRow(string tablename, string primarycolumnname, object primaryvalue);
		Data[][] SelectRows(string tablename, string filter);
		Data[][] SelectRows(string tablename, string filter, object[] values);
		Data[][] SelectRows(string tablename, QueryModel.Operation operation);
		void UpdateRow(string tablename, string primarycolumnname, object primaryvalue, params Data[] values);
		void InsertRow(string tablename, params Data[] values);
		string GetPrimaryKey(string tablename);
		string[] GetTablenames();
		Data[] GetStructure(string sql);
		Data[] GetTableStructure(string tablename);
		string FormatValue(object value);
		void Close();
		string ConnectionString{get;set;}
		object GetNullValue(Type type);
	}

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

	public enum ObjectStates
	{
		Default,
		New,
		Deleted,
	}

	public enum DataActions
	{
		Update,
		Insert,
		Delete,
		Fetch,
	}
}