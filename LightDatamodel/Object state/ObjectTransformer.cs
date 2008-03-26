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
	/// Will aid in transforming datareaders to objects and back
	/// </summary>
	public class ObjectTransformer
	{
        /// <summary>
        /// Creates a new copy of the given item
        /// </summary>
        /// <typeparam name="DATACLASS">The type of object to work on</typeparam>
        /// <param name="source">The source object</param>
        /// <returns>A fresh copy</returns>
        public static DATACLASS CreateCopy<DATACLASS>(DATACLASS source)
        {
            return (DATACLASS)CreateCopy(source);
        }

        /// <summary>
        /// Creates a new copy of the given item
        /// </summary>
        /// <param name="source">The source object</param>
        /// <returns>A fresh copy</returns>
        public static object CreateCopy(object source)
        {
            object target = Activator.CreateInstance(source.GetType());
            CopyObject(source, target);
            return target;
        }

        /// <summary>
        /// Copies all data variables from one object into another
        /// </summary>
        /// <param name="source">The object to copy from</param>
        /// <param name="target">The object to copy to</param>
        public static void CopyObject(object source, object target)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (target == null)
                throw new ArgumentNullException("target");
            if (target.GetType() != source.GetType())
                throw new Exception("Objects must be of same type");

            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (FieldInfo fi in fields)
                fi.SetValue(target, fi.GetValue(source));

            IRelationManager sourceManager = null;
            IRelationManager targetManager = null;


            //take care of relations
            if (source as IDataClass != null)
                sourceManager = ((IDataClass)source).RelationManager;
            if (target as IDataClass != null)
                targetManager = ((IDataClass)target).RelationManager;
            if (sourceManager != null && targetManager != null)
            {
                if (targetManager.IsRegistered(target as IDataClass))
                    targetManager.ReassignGuid(targetManager.GetGuidForObject(target as IDataClass), sourceManager.GetGuidForObject(source as IDataClass));
                else
                    targetManager.RegisterObject(sourceManager.GetGuidForObject(source as IDataClass), target as IDataClass);

                targetManager.SetExistsInDb(target as IDataClass, sourceManager.ExistsInDb(source as IDataClass));
                targetManager.SetReferenceObjects(target as IDataClass, sourceManager.GetReferenceObjects(source as IDataClass));
            }
        }

		/// <summary>
		/// This will insert the given data into an arbitary object (the private variables)
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="data"></param>
        /// <returns>The item populated</returns>
		public static object PopulateDataClass(object obj, Data[] data, IDataProvider provider)
		{
			for (int i = 0; i < data.Length; i++)
			{
				try
				{
					FieldInfo field = obj.GetType().GetField("m_" + data[i].Name, BindingFlags.Instance | BindingFlags.NonPublic);
					if (field != null)
					{
						MemberModifierEnum m = MemberModifier.CalculateModifier(field);
						if ((m & MemberModifierEnum.IgnoreWithSelect) != MemberModifierEnum.IgnoreWithSelect)
						{
							if (data[i].Value != DBNull.Value)
								field.SetValue(obj, data[i].Value);
							else
								field.SetValue(obj, provider.GetNullValue(data[i].Type));
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Couldn't set field\nError: " + ex.Message);
				}
			}
            return obj;
		}

        public static Data[] GetDataFromObject(object obj)
        {
			//get private fields
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			ArrayList fieldList = new ArrayList();
			for (int i = 0; i < fields.Length; i++)
			{
				Data d = new Data(fields[i].Name, fields[i].GetValue(obj), fields[i].FieldType);
				if (d.Name.StartsWith("m_"))
					d.Name = d.Name.Substring(2);
				MemberModifierEnum m = MemberModifier.CalculateModifier(fields[i]);
                if (obj as IDataClass != null)
                {
                    if ((obj as IDataClass).ObjectState == ObjectStates.New)
                    {
                        if ((m & MemberModifierEnum.IgnoreWithInsert) != MemberModifierEnum.IgnoreWithInsert)
                            fieldList.Add(d);
                    }
                    else if ((obj as IDataClass).ObjectState == ObjectStates.Default)
                    {
                        if ((m & MemberModifierEnum.IgnoreWithUpdate) != MemberModifierEnum.IgnoreWithUpdate)
                            fieldList.Add(d);
                    }
                    else
                        fieldList.Add(d);
                }
                else
                    fieldList.Add(d);
			}

			return (Data[])fieldList.ToArray(typeof(Data));
        }

		public static DATACLASS[] TransformToObjects<DATACLASS>(Data[][] data, IDataProvider provider)
		{
			DATACLASS[] ret = new DATACLASS[data.Length];

			for (int i = 0; i < ret.Length; i++)
			{
				DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
				ObjectTransformer.PopulateDataClass(newobj, data[i], provider);
				ret[i] = newobj;
			}

			return ret;
		}
		public static object[] TransformToObjects(Type type, Data[][] data, IDataProvider provider)
		{
			object[] ret = new object[data.Length];

			for (int i = 0; i < ret.Length; i++)
			{
				object newobj = Activator.CreateInstance(type);
				ObjectTransformer.PopulateDataClass(newobj, data[i], provider);
				ret[i] = newobj;
			}

			return ret;
		}
	}

	#region " Attribute classes "
	[Flags]
	public enum MemberModifierEnum : int
	{
		None = 0,
		IgnoreWithInsert,
		IgnoreWithUpdate,
		IgnoreWithSelect,
		IgnoreAll = IgnoreWithInsert | IgnoreWithUpdate | IgnoreWithSelect,
		AutoIncrement = IgnoreWithInsert
	}

	public class MemberModifier : Attribute
	{
		private MemberModifierEnum m_m;
		public MemberModifier(MemberModifierEnum m)
		{
			m_m = m;
		}
		public MemberModifierEnum Modifier { get { return m_m; } }

		public static MemberModifierEnum CalculateModifier(MemberInfo mi)
		{
			MemberModifier[] m = (MemberModifier[])mi.GetCustomAttributes(typeof(MemberModifier), false);
			if (m == null || m.Length == 0)
				return MemberModifierEnum.None;

			MemberModifierEnum mf = m[0].Modifier;
			for (int i = 1; i < m.Length; i++)
				mf |= m[i].Modifier;

			return mf;
		}
	}

	public class MemberModifierIgnoreWithInsert : MemberModifier
	{
		public MemberModifierIgnoreWithInsert()
			: base(MemberModifierEnum.IgnoreWithInsert)
		{
		}
	}

	public class MemberModifierIgnoreWithUpdate : MemberModifier
	{
		public MemberModifierIgnoreWithUpdate()
			: base(MemberModifierEnum.IgnoreWithUpdate)
		{
		}
	}

	public class MemberModifierIgnoreWithSelect : MemberModifier
	{
		public MemberModifierIgnoreWithSelect()
			: base(MemberModifierEnum.IgnoreWithSelect)
		{
		}
	}

	public class MemberModifierIgnoreAll : MemberModifier
	{
		public MemberModifierIgnoreAll()
			: base(MemberModifierEnum.IgnoreAll)
		{
		}
	}

	public class MemberModifierAutoIncrement : MemberModifier
	{
		public MemberModifierAutoIncrement()
			: base(MemberModifierEnum.AutoIncrement)
		{
		}
	}

	#endregion
}
