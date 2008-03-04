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
	/// Will aid in transforming datareaders to objects and back
	/// </summary>
	public class ObjectTransformer
	{
		/// <summary>
		/// This will insert the given data into an arbitary object (the private variables)
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="data"></param>
		public static void PopulateDataClass(object obj, Data[] data, IDataProvider provider)
		{
			for (int i = 0; i < data.Length; i++)
			{
				bool isDataClassBase = obj as DataClassBase != null;
				try
				{
					if (data[i].Type != null)
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
					else if (isDataClassBase)
					{
						FieldInfo bfi = typeof(DataClassBase).GetField(data[i].Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
						if (bfi != null)
							bfi.SetValue(obj, data[i].Value);
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Couldn't set field\nError: " + ex.Message);
				}
			}
		}

		public static void PopulateDatabase(IDataClass obj, IDataProvider provider)
		{
			string tablename = obj.GetType().Name;

			//get private fields
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			ArrayList fieldList = new ArrayList();
			for (int i = 0; i < fields.Length; i++)
			{
				Data d = new Data(fields[i].Name, fields[i].GetValue(obj), fields[i].FieldType);
				if (d.Name.StartsWith("m_"))
					d.Name = d.Name.Substring(2);
				MemberModifierEnum m = MemberModifier.CalculateModifier(fields[i]);
				if (obj.ObjectState == ObjectStates.New)
				{
					if ((m & MemberModifierEnum.IgnoreWithInsert) != MemberModifierEnum.IgnoreWithInsert)
						fieldList.Add(d);
				}
				else if (obj.ObjectState == ObjectStates.Default)
				{
					if ((m & MemberModifierEnum.IgnoreWithUpdate) != MemberModifierEnum.IgnoreWithUpdate)
						fieldList.Add(d);
				}
				else
					fieldList.Add(d);
			}

			//take care of system fields
			if (obj as DataClassBase != null)
				foreach (string s in new string[] { "m_guid", "m_existsInDB", "m_referenceObjects" })
				{
					FieldInfo bfi = typeof(DataClassBase).GetField(s, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
					if (bfi != null)
						fieldList.Add(new Data(bfi.Name, bfi.GetValue(obj), null));
					//All 'special' values are tagged with type == null
				}


			Data[] data = (Data[])fieldList.ToArray(typeof(Data));

			//sql
			if (obj.ObjectState == ObjectStates.Default)
			{
				provider.UpdateRow(tablename, obj.UniqueColumn, obj.UniqueValue, data);
			}
			else if (obj.ObjectState == ObjectStates.New)
			{
				provider.InsertRow(tablename, data);
			}
			else if (obj.ObjectState == ObjectStates.Deleted)
			{
				provider.DeleteRow(tablename, obj.UniqueColumn, obj.UniqueValue);
			}
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
