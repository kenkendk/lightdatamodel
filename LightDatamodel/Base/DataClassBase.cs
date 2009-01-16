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
using System.Collections.Generic;

[assembly: CLSCompliant(true)]
namespace System.Data.LightDatamodel
{

	/// <summary>
	/// Base class for all 'database classes'
	/// </summary>
	public abstract class DataClassBase : IDataClass
	{
		internal protected bool m_isdirty = true;
		internal protected IDataFetcher m_dataparent;
		internal protected ObjectStates m_state = ObjectStates.New;
		internal protected Dictionary<string, object> m_originalvalues;

		public event DataChangeEventHandler BeforeDataChange;
		public event DataChangeEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataCommit;
		public event DataConnectionEventHandler AfterDataCommit;

		public IDataFetcher DataParent { get { return m_dataparent; } set { m_dataparent = value; } }
		public virtual bool IsDirty{get{return m_isdirty;}}
		public virtual ObjectStates ObjectState{get{return m_state;}set{m_state=value;}}
		public Dictionary<string, object> OriginalValues{get { return m_originalvalues; }}

		protected virtual internal void OnBeforeDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(object.Equals(oldvalue, newvalue)) return;
			if(BeforeDataChange != null) BeforeDataChange(sender, propertyname, oldvalue, newvalue);
		}

		protected virtual internal void OnAfterDataChange(object sender, string propertyname, object oldvalue, object newvalue)
		{
			if(object.Equals(oldvalue, newvalue)) return;
			if (m_originalvalues == null) m_originalvalues = new Dictionary<string, object>();
			if (!m_originalvalues.ContainsKey(propertyname)) m_originalvalues.Add(propertyname, oldvalue);		//preserve original values
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

		/// <summary>
		/// This will rollback a given property change
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public bool Rollback(string property)
		{
			try
			{
				GetType().GetProperty(property).SetValue(this, m_originalvalues[property], null);
				bool success = m_originalvalues.Remove(property);
				if (m_originalvalues.Count == 0) m_isdirty = false;
				return success;
			}
			catch
			{
				throw new Exception("Couldn't remove change");
			}
		}
	}

	/// <summary>
	/// Base class for non updateable data classes (views)
	/// </summary>
	public abstract class DataClassView : DataClassBase
	{
		public override bool IsDirty { get { return false; } }

		public override ObjectStates ObjectState
		{
			get { return ObjectStates.Default; }
			set{ /*meh*/}
		}
	}
}