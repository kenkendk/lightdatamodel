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

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Will fetch and commit objects directly to source
	/// </summary>
	public class DataFetcher : IDataFetcher
	{
		protected IDataProvider m_provider;

		public event DataWriteEventHandler BeforeDataChange;
		public event DataWriteEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataConnection;
		public event DataConnectionEventHandler AfterDataConnection;

		public IDataProvider Provider { get { return m_provider; } }

		public DataFetcher(IDataProvider provider)
		{
			m_provider = provider;
		}

		protected virtual void HookObject(IDataClass obj)
		{
			(obj as DataClassBase).BeforeDataWrite += new DataWriteEventHandler(obj_BeforeDataWrite);
			(obj as DataClassBase).AfterDataWrite += new DataWriteEventHandler(obj_AfterDataWrite);
		}

		protected virtual DataClassLevels GetDataClassLevel(Type type)
		{
			do
			{
				if (type == typeof(DataClassExtended)) return DataClassLevels.Extended;
				else if (type == typeof(DataClassBase)) return DataClassLevels.Base;
				else if (type == typeof(DataClassView)) return DataClassLevels.View;
				type = type.BaseType;
			} while (type != null);
			return DataClassLevels.NoInheritance;
		}

		public virtual DATACLASS[] GetObjects<DATACLASS>() where DATACLASS : IDataClass
		{
			return GetObjects<DATACLASS>(null, null);
		}

		/// <summary>
		/// This will load a list of arbitary objects
		/// If the given object is a DataClassBase it will be hooked into the DataFetcher
		/// DataCustomClassBase will also have it's values filled
		/// All others will just be filled with the data
		/// </summary>
		/// <param name="filter">The filter used to select objects</param>
		/// <returns>All matching objects</returns>
		public virtual DATACLASS[] GetObjects<DATACLASS>(string filter, params object[] parameters) where DATACLASS : IDataClass
		{
			Type type = typeof(DATACLASS);
			string tablename = type.Name;

			OnBeforeDataConnection(null, DataActions.Fetch);
			Data[][] data = m_provider.SelectRows(tablename, filter, parameters);
			if (data == null) return null;
			DATACLASS[] obj = ObjectTransformer.TransformToObjects<DATACLASS>(data, m_provider);

			for (int i = 0; i < obj.Length; i++)
			{
				HookObject((IDataClass)obj[i]);
				(obj[i] as DataClassBase).m_dataparent = this;
				(obj[i] as DataClassBase).m_state = ObjectStates.Default;
				OnAfterDataConnection(obj[i], DataActions.Fetch);
			}

			return obj;
		}

		/// <summary>
		/// Marks an object for deletion
		/// </summary>
		/// <param name="id">ID of the item</param>
		/// <param name="type">Type of the item to delete</param>
		public virtual void DeleteObject<DATACLASS>(object id) where DATACLASS : IDataClass
		{
			DATACLASS tobedeleted = GetObjectById<DATACLASS>(id);
			(tobedeleted as DataClassBase).m_state = ObjectStates.Deleted;
			Commit((IDataClass)tobedeleted);
		}

		/// <summary>
		/// This will load the given DataClassBase object
		/// </summary>
		/// <param name="type"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual DATACLASS GetObjectById<DATACLASS>(object id) where DATACLASS : IDataClass
		{
			Type type = typeof(DATACLASS);
			string tablename = type.Name;

			if(GetDataClassLevel(type) < DataClassLevels.Base) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			//Fetch From Data source
			IDataClass newobj = (IDataClass)Activator.CreateInstance(type);
			OnBeforeDataConnection(newobj, DataActions.Fetch);
			Data[] data = m_provider.SelectRow(tablename, newobj.UniqueColumn, id);
			if (data == null) return default(DATACLASS);
			DATACLASS[] obj = ObjectTransformer.TransformToObjects<DATACLASS>(new Data[][] { data }, m_provider);
			HookObject((IDataClass)obj[0]);
			(obj[0] as DataClassBase).m_dataparent = this;
			(obj[0] as DataClassBase).m_state = ObjectStates.Default;
			OnAfterDataConnection(obj[0], DataActions.Fetch);

			return obj[0];
		}

		public virtual object GetObjectById(Type type, object id)
		{
			string tablename = type.Name;

			if (GetDataClassLevel(type) < DataClassLevels.Base) throw new Exception("This object cannot be fetched by primary key. Use GetObjects instead");

			//Fetch From Data source
			IDataClass newobj = (IDataClass)Activator.CreateInstance(type);
			OnBeforeDataConnection(newobj, DataActions.Fetch);
			Data[] data = m_provider.SelectRow(tablename, newobj.UniqueColumn, id);
			if (data == null) return null;
			object[] obj = ObjectTransformer.TransformToObjects(type, new Data[][] { data }, m_provider);
			HookObject((IDataClass)obj[0]);
			(obj[0] as DataClassBase).m_dataparent = this;
			(obj[0] as DataClassBase).m_state = ObjectStates.Default;
			OnAfterDataConnection(obj[0], DataActions.Fetch);

			return obj[0];
		}

		public virtual DATACLASS CreateObject<DATACLASS>() where DATACLASS : IDataClass
		{
			DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
			HookObject((IDataClass)newobj);
			return newobj;
		}

		public virtual object CreateObject(Type type)
		{
			object newobj = Activator.CreateInstance(type);
			HookObject((IDataClass)newobj);
			return newobj;
		}

		public virtual RETURNVALUE Compute<RETURNVALUE, DATACLASS>(string expression, string filter)
		{
			OnBeforeDataConnection(null, DataActions.Fetch);
			object ret = m_provider.Compute(typeof(DATACLASS).Name, expression, filter);
			OnAfterDataConnection(ret, DataActions.Fetch);
			if (ret == null || ret == DBNull.Value) return default(RETURNVALUE);
			return (RETURNVALUE)ret;
		}

		protected internal virtual void RefreshObject(IDataClass obj)
		{
			if(obj.ObjectState == ObjectStates.Deleted) return;

			string tablename = obj.GetType().Name;
			OnBeforeDataConnection(obj, DataActions.Fetch);
			Data[] data = m_provider.SelectRow(tablename, obj.UniqueColumn, obj.UniqueValue);
			if(data == null) throw new Exception("Row (" + obj.UniqueValue + ") from table \"" + tablename + "\" can't be fetched");
			ObjectTransformer.PopulateDataClass(obj, data, m_provider);
			(obj as DataClassBase).m_state = ObjectStates.Default;
			(obj as DataClassBase).m_isdirty = false;
			OnAfterDataConnection(obj, DataActions.Fetch);
		}

		public virtual void Commit(IDataClass obj)
		{
			//new object?
			if ((obj as DataClassBase).m_dataparent == null)
			{
				(obj as DataClassBase).m_dataparent = this;
				(obj as DataClassBase).m_state = ObjectStates.New;
			}

			//save
			if (obj.ObjectState == ObjectStates.Default)
			{
				OnBeforeDataConnection(obj, DataActions.Update);
				ObjectTransformer.PopulateDatabase(obj, m_provider);
				OnAfterDataConnection(obj, DataActions.Update);
			}
			else if (obj.ObjectState == ObjectStates.New)
			{
				OnBeforeDataConnection(obj, DataActions.Insert);
				ObjectTransformer.PopulateDatabase(obj, m_provider);
				OnAfterDataConnection(obj, DataActions.Insert);
			}
			else if (obj.ObjectState == ObjectStates.Deleted)
			{
				OnBeforeDataConnection(obj, DataActions.Delete);
				ObjectTransformer.PopulateDatabase(obj, m_provider);
				OnAfterDataConnection(obj, DataActions.Delete);
			}

			//Try to read data back from database
			RefreshObject(obj);
		}

		protected virtual void obj_BeforeDataWrite(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(BeforeDataChange != null) BeforeDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual void obj_AfterDataWrite(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(AfterDataChange != null) AfterDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual void OnAfterDataConnection(object obj, DataActions action)
		{
			if (AfterDataConnection != null) AfterDataConnection(obj, action);
		}

		protected virtual void OnBeforeDataConnection(object obj, DataActions action)
		{
			if (BeforeDataConnection != null) BeforeDataConnection(obj, action);
		}
	}
}
