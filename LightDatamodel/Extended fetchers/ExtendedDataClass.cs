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
	/// Base class for all 'database classes'
	/// </summary>
	public abstract class DataClassExtended : DataClassBase, IDataClass
	{
		internal protected Guid m_guid = Guid.NewGuid();
		internal protected bool m_existsInDB = false;
		protected Hashtable m_referenceObjects = new Hashtable();

		public Guid Guid { get { return m_guid; } }
		public bool ExistsInDB { get { return m_existsInDB; } }

		public bool IsPrimaryKeyAutogenerated
		{
			get 
			{
				FieldInfo fi = this.GetType().GetField("m_" + this.UniqueColumn, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
				if (fi == null)
					return false;
				MemberModifierEnum m = MemberModifier.CalculateModifier(fi);
				return ((m & MemberModifierEnum.IgnoreWithInsert) == MemberModifierEnum.IgnoreWithInsert) && ((m & MemberModifierEnum.IgnoreWithSelect) != MemberModifierEnum.IgnoreWithSelect);
			}
		}

		internal void SetExistsInDB(bool nv) { m_existsInDB = nv; }
		internal void SetPrimaryKey(object o) 
		{ 
			FieldInfo fi = this.GetType().GetField("m_" + this.UniqueColumn, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
			if (fi == null)
				return;
			else
				fi.SetValue(this, o);
		}

		/// <summary>
		/// Returns a reference to an object
		/// </summary>
		/// <param name="propertyname">The property that is used to access the object</param>
		/// <param name="idfieldname">The name of the ID value in this instance</param>
		/// <returns>The matching item</returns>
		//protected DataClassExtended GetReferenceObject(string propertyname, string idfieldname)
		//{
		//    bool found = m_referenceObjects.ContainsKey(propertyname);
		//    if (!found)
		//    {
		//        //Actually, this should be checked for the remote object, not this object
		//        if (!ExistsInDB && IsPrimaryKeyAutogenerated)
		//            //Avoid doing a lookup if the id is not set
		//            return null;

		//        PropertyInfo pi = GetType().GetProperty(propertyname);
		//        FieldInfo fi = GetType().GetField(idfieldname, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

		//        if (fi == null)
		//            throw new Exception("Bad config for reference object, field " + idfieldname + " does not exist in class " + GetType().FullName);
		//        if (pi == null)
		//            throw new Exception("Bad config for reference object, property " + propertyname + " does not exist in class " + GetType().FullName);

		//        DataClassExtended c = m_dataparent.GetObjectById(pi.PropertyType, fi.GetValue(this));
		//        if (c == null)
		//            m_referenceObjects[propertyname] = null;
		//        else
		//            m_referenceObjects[propertyname] = c.Guid;
		//    }

		//    return (DataClassExtended)(m_dataparent as DataFetcherCached).GetObjectByGuid(m_referenceObjects[propertyname]);
		//}

		/// <summary>
		/// Sets a reference to an object
		/// </summary>
		/// <param name="propertyname">The property that is used to access the object</param>
		/// <param name="idfieldname">The name of the ID value in this instance</param>
		/// <param name="reversePropertyname">The name of the reverse property or null if there is no reverse property</param>
		/// <param name="value">The value to set the property to</param>
		//protected void SetReferenceObject(string propertyname, string idfieldname, string reversePropertyname, DataClassExtended value)
		//{
		//    if (value == null)
		//    {
		//        if (reversePropertyname != null)
		//        {
		//            DataClassExtended revObj = GetReferenceObject(propertyname, idfieldname);
		//            if (revObj != null)
		//            {
		//                PropertyInfo pi = revObj.GetType().GetProperty(reversePropertyname);
		//                if (pi.PropertyType.IsAssignableFrom(this.GetType()))
		//                    pi.SetValue(revObj, null, null);
		//                else
		//                {
		//                    object revObjProp = pi.GetValue(revObj, null);
		//                    if (revObjProp as ICollection != null)
		//                    {
		//                        MethodInfo mi = revObjProp.GetType().GetMethod("Remove");
		//                        if (mi == null)
		//                            throw new Exception("Type " + revObjProp.GetType().FullName + " did not have a Remove method");
		//                        mi.Invoke(revObjProp, new object[] { this });
		//                    }
		//                    else
		//                        throw new Exception("Reverse Property " + reversePropertyname + " on type " + revObj.GetType().FullName + " must be either type " + this.GetType().FullName + " or System.Collections.ICollection");
		//                }
		//            }
		//        }

		//        m_referenceObjects[propertyname] = null;
		//        m_isdirty = true;
		//        return;
		//    }

		//    if (value.m_dataparent != this.m_dataparent)
		//        throw new Exception("Cannot mix objects from differenct data contexts");

		//    m_referenceObjects[propertyname] = value.m_guid;

		//    FieldInfo fi = GetType().GetField(idfieldname, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

		//    if (fi == null)
		//        throw new Exception("Bad config for reference object, field " + idfieldname + " does not exist in class " + GetType().FullName);
		//    m_isdirty = true;
		//    fi.SetValue(this, value.UniqueValue);

		//    if (reversePropertyname != null)
		//    {
		//        PropertyInfo pi = value.GetType().GetProperty(reversePropertyname);
		//        object revval = pi.GetValue(value, null);

		//        if (pi.PropertyType.IsAssignableFrom(this.GetType()))
		//        {
		//            if (revval != this)
		//                pi.SetValue(value, this, null);
		//        }
		//        else
		//        {
		//            if (revval as ICollection != null)
		//            {
		//                MethodInfo mi = revval.GetType().GetMethod("Contains");
		//                if (mi == null)
		//                    throw new Exception("Type " + revval.GetType().FullName + " did not have a Contains method");
		//                bool res = Convert.ToBoolean(mi.Invoke(revval, new object[] { this }));
		//                if (!res)
		//                {
		//                    mi = revval.GetType().GetMethod("Add");
		//                    if (mi == null)
		//                        throw new Exception("Type " + revval.GetType().FullName + " did not have an Add method");
		//                    mi.Invoke(revval, new object[] { this });
		//                }
		//            }
		//            else
		//                throw new Exception("Reverse Property " + reversePropertyname + " on type " + value.GetType().FullName + " must be either type " + this.GetType().FullName + " or System.Collections.ICollection");
		//        }
		//    }
		//}
	}
}
