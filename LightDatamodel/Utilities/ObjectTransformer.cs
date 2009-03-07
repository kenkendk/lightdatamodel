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
using System.Data.LightDatamodel.DataClassAttributes;

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Will aid in transforming datareaders to objects and back
	/// </summary>
	public class ObjectTransformer
	{

		/// <summary>
		/// This will create a copy of the array
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="arrary"></param>
		/// <returns></returns>
		public static IEnumerable<DATACLASS> CreateArrayCopy<DATACLASS>(IEnumerable<DATACLASS> array)
		{
			LinkedList<DATACLASS> list = new LinkedList<DATACLASS>();
			foreach (DATACLASS obj in array)
				list.AddLast(CreateCopy<DATACLASS>(obj));
			return list;
		}

        /// <summary>
        /// Creates a new copy of the given item
        /// </summary>
        /// <typeparam name="DATACLASS">The type of object to work on</typeparam>
        /// <param name="source">The source object</param>
        /// <returns>A fresh copy</returns>
        public static DATACLASS CreateCopy<DATACLASS>(DATACLASS source)
        {
            return (DATACLASS)CreateCopy(source as object);
        }

        /// <summary>
        /// Creates a new copy of the given item
        /// </summary>
        /// <param name="source">The source object</param>
        /// <returns>A fresh copy</returns>
        public static object CreateCopy(object source)
        {
			if (source == null) return null;
            object target = Activator.CreateInstance(source.GetType());
            CopyObject(source, target);
            return target;
        }

		/// <summary>
		/// This will copy one array onto another. Make sure that the size is right
		/// </summary>
		/// <typeparam name="DATACLASS"></typeparam>
		/// <param name="sourcearray"></param>
		/// <param name="targetarray"></param>
		public static void CopyArray<DATACLASS>(IEnumerable<DATACLASS> sourcearray, IEnumerable<DATACLASS> targetarray)
		{
			if (sourcearray == null) return;
			if (targetarray == null) throw new Exception("The target array is null. Don't be foolish!");

			IEnumerator<DATACLASS> s = sourcearray.GetEnumerator();
			IEnumerator<DATACLASS> t = targetarray.GetEnumerator();

			while (s.MoveNext())
			{
				if(!t.MoveNext()) throw new Exception("Target array are not as long as source");
				CopyObject(s.Current, t.Current);
			}
		}

		/// <summary>
		/// This will copy one array onto another. Make sure that the size is right
		/// </summary>
		/// <param name="sourcearray"></param>
		/// <param name="targetarray"></param>
		public static void CopyArray(IEnumerable sourcearray, IEnumerable targetarray)
		{
			if (sourcearray == null) return;
			if (targetarray == null) throw new Exception("The target array is null. Don't be foolish!");

			IEnumerator s = sourcearray.GetEnumerator();
			IEnumerator t = targetarray.GetEnumerator();

			while (s.MoveNext())
			{
				if (!t.MoveNext()) throw new Exception("Target array are not as long as source");
				CopyObject(s.Current, t.Current);
			}
		}

        /// <summary>
        /// Copies all data variables from one object into another
        /// </summary>
        /// <param name="source">The object to copy from</param>
        /// <param name="target">The object to copy to</param>
        public static void CopyObject(object source, object target)
        {
			if (source == null || target == null) throw new ArgumentNullException("source and target can't be null");
            if (target.GetType() != source.GetType()) throw new Exception("Objects must be of same type");

			FieldInfo[] fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly); ;
			object s, t;
			DataClassBase targetbase = target as DataClassBase;
			if(targetbase != null)
			{
				//fire events if possible
				foreach (FieldInfo fi in fields)
				{
					s = fi.GetValue(source);
					t = fi.GetValue(target);
					if (!object.Equals(s, t))		//only copy differating objects
					{
						DatabaseField dbf = TypeConfiguration.MappedClass.GetAttribute<DatabaseField>(fi);
						if(dbf == null)
						{
							fi.SetValue(target, s);
						}
						else
						{
							targetbase.OnBeforeDataChange(target,dbf.Fieldname , t, s);
							fi.SetValue(target, s);
							targetbase.OnAfterDataChange(target, dbf.Fieldname , t, s);
						}
					}
				}
			}
			else
			{
				foreach (FieldInfo fi in fields)
					fi.SetValue(target, fi.GetValue(source));
			}
        }



	}
}
