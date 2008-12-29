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
		internal protected ObjectStates m_state = ObjectStates.Default;
		internal protected Dictionary<string, object> m_originalvalues;
		internal protected static Random rnd = new Random();		//used to provide unique (almost) to new objects

		public event DataChangeEventHandler BeforeDataChange;
		public event DataChangeEventHandler AfterDataChange;
		public event DataConnectionEventHandler BeforeDataCommit;
		public event DataConnectionEventHandler AfterDataCommit;

		public IDataFetcher DataParent { get { return m_dataparent; } set { m_dataparent = value; } }
		public virtual bool IsDirty{get{return m_isdirty;}}
		public virtual ObjectStates ObjectState{get{return m_state;}set{m_state=value;}}

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

        private static long m_next_unique_id = -1;
        private static object m_next_unique_id_lock = new object();
        protected static long GetNextUniqueID()
        {
            lock(m_next_unique_id_lock)
                return m_next_unique_id--;
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