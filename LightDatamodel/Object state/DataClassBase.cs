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

	/// <summary>
	/// Base class for non updateable data classes (views)
	/// </summary>
	public abstract class DataClassView : IDataClass
	{
		internal protected IDataFetcher m_dataparent;
		public event DataWriteEventHandler BeforeDataChange;
		public event DataWriteEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataCommit;
		public event DataConnectionEventHandler AfterDataCommit;

		#region IDataClass Members

		protected void OnAfterDataConnection(object obj, DataActions action)
		{
			if (AfterDataCommit != null) AfterDataCommit(obj, action);
		}

		protected void OnBeforeDataCommit(object obj, DataActions action)
		{
			if (BeforeDataCommit != null) BeforeDataCommit(obj, action);
		}

		protected void OnBeforeDataCommit(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if (oldvalue == newvalue) return;
			if (BeforeDataChange != null) BeforeDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected void OnAfterDataWrite(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if (oldvalue == newvalue) return;
			if (AfterDataChange != null) AfterDataChange(sender, propertyname, oldvalue, newvalue);
		}

		public IDataFetcher DataParent { get { return m_dataparent;	} }
        public IRelationManager RelationManager { get { return (m_dataparent as IDataFetcherCached == null) ? null : (m_dataparent as IDataFetcherCached).RelationManager; } }
		public bool IsDirty { get {	return false; } }

        public void SetDirty()
        {
        }

		public System.Data.LightDatamodel.ObjectStates ObjectState
		{
			get	{ return ObjectStates.Default; }
			set	
            {
				//meh
			}
		}
		public string UniqueColumn { get { return null; } }
		public object UniqueValue { get	{ return null; } }

		#endregion
	}

	/// <summary>
	/// Base class for all 'database classes'
	/// </summary>
	public abstract class DataClassBase : IDataClass
	{
		internal protected bool m_isdirty = true;
		internal protected IDataFetcher m_dataparent;
		internal protected ObjectStates m_state = ObjectStates.Default;
		public event DataWriteEventHandler BeforeDataChange;
		public event DataWriteEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataCommit;
		public event DataConnectionEventHandler AfterDataCommit;

        public IDataFetcher DataParent { get { return m_dataparent; } }
        public IRelationManager RelationManager { get { return (m_dataparent as IDataFetcherCached == null) ? null : (m_dataparent as IDataFetcherCached).RelationManager; } }
		public bool IsDirty{get{return m_isdirty;}}
		public ObjectStates ObjectState{get{return m_state;}set{m_state=value;}}
		public abstract string UniqueColumn	{get;}
		public abstract object UniqueValue{get;}
        public void SetDirty() { m_isdirty = true; }

		protected virtual void OnBeforeDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(oldvalue == newvalue) return;
			if(BeforeDataChange != null) BeforeDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual void OnAfterDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(oldvalue == newvalue) return;
			m_isdirty=true;
			if(AfterDataChange != null) AfterDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual internal void OnAfterDataCommit(object obj, DataActions action)
		{
			if (AfterDataCommit != null) AfterDataCommit(obj, action);
		}

		protected virtual internal void OnBeforeDataCommit(object obj, DataActions action)
		{
			if (BeforeDataCommit != null) BeforeDataCommit(obj, action);
		}
	}
}