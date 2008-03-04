using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel
{
	public class SyncCollectionBase<DATACLASS> where DATACLASS : IDataClass
	{
		protected System.Collections.ArrayList m_baseList = new System.Collections.ArrayList();
		protected System.Reflection.PropertyInfo m_reverseProperty = null;
		protected object m_owner = null;
		protected DATACLASS m_baseType = default(DATACLASS);

		public SyncCollectionBase() { }

		public SyncCollectionBase(object owner, string reversePropertyname, string reversePropertyID)
		{
			if (owner == null)
				throw new System.ArgumentNullException("owner");
			if (m_baseType == null)
				throw new System.ArgumentNullException("baseType");
			m_owner = owner;
			m_reverseProperty = m_baseType.GetType().GetProperty(reversePropertyname);
			if (m_reverseProperty == null)
				throw new System.Exception("Class " + m_baseType.GetType().FullName + " does not contain the property " + reversePropertyname);

			DataClassExtended db = owner as DataClassExtended;
			if (db != null)
			{
				if (db.ExistsInDB)
					db.DataParent.GetObjects<DATACLASS>(reversePropertyID + "=?", db.UniqueValue);

				m_baseList.AddRange((db.DataParent as DataFetcherCached).GetObjectsFromCache<DATACLASS>(reversePropertyname + ".Guid=?", db.Guid));
			}
		}

		protected void UpdateReverse(object item, bool remove)
		{
			if (m_reverseProperty != null && m_owner != null)
				if (m_reverseProperty.PropertyType.IsAssignableFrom(m_owner.GetType()))
				{
					object nval = remove ? null : m_owner;
					if (m_reverseProperty.GetValue(item, null) != nval)
						m_reverseProperty.SetValue(item, remove ? null : m_owner, null);
				}
				else
				{
					object col = m_reverseProperty.GetValue(item, null);
					if (col == null)
						return;
					else if (col as System.Collections.ICollection == null)
						throw new System.Exception("Reverse property must be of type " + m_owner.GetType().FullName + " or ICollection");

					System.Reflection.MethodInfo mi = col.GetType().GetMethod("Contains");
					if (mi == null)
						throw new System.Exception("Reverse property type " + col.GetType().FullName + " does not contain a method called Contains");
					bool contains = System.Convert.ToBoolean(mi.Invoke(col, new object[] { m_owner }));
					if (!contains)
					{
						mi = col.GetType().GetMethod(remove ? "Remove" : "Add");
						if (mi == null)
							throw new System.Exception("Reverse property type " + col.GetType().FullName + " does not contain a method called " + (remove ? "Remove" : "Add"));
						mi.Invoke(col, new object[] { m_owner });
					}

				}

		}

		protected virtual void HookItem(object item)
		{
			UpdateReverse(item, false);
		}

		protected virtual void UnhookItem(object item)
		{
			UpdateReverse(item, true);
		}


	}
}
