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
		//private TypeConfiguration m_configuration = new TypeConfiguration();

		//public TypeConfiguration TypeConfiguration { get { return m_configuration; } }

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
			if (source == null || target == null) throw new ArgumentNullException("source and target can't be null");
            if (target.GetType() != source.GetType()) throw new Exception("Objects must be of same type");

			FieldInfo[] fields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly); ;
            foreach (FieldInfo fi in fields)
                fi.SetValue(target, fi.GetValue(source));

			DataFetcherWithRelations sourceManager = null;
			DataFetcherWithRelations targetManager = null;

            //take care of relations
			if (source as IDataClass != null) sourceManager = (((IDataClass)source).DataParent as DataFetcherWithRelations) != null ? (((IDataClass)source).DataParent as DataFetcherWithRelations) : null;
			if (target as IDataClass != null) targetManager = (((IDataClass)source).DataParent as DataFetcherWithRelations) != null ? (((IDataClass)target).DataParent as DataFetcherWithRelations) : null; 
            if (sourceManager != null && targetManager != null)
            {
                if (targetManager.IsRegistered(target as IDataClass)) targetManager.ReassignGuid(targetManager.GetGuidForObject(target as IDataClass), sourceManager.GetGuidForObject(source as IDataClass));
                else targetManager.RegisterObject(sourceManager.GetGuidForObject(source as IDataClass), target as IDataClass);

                targetManager.SetExistsInDb(target as IDataClass, sourceManager.ExistsInDb(source as IDataClass));
                targetManager.SetReferenceObjects(target.GetType(), targetManager.GetGuidForObject(target as IDataClass), sourceManager.GetReferenceObjects(source.GetType(), sourceManager.GetGuidForObject(source as IDataClass)));
            }
        }

        /// <summary>
        /// This will insert the given data into an arbitary object (the private variables)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="data"></param>
        /// <returns>The item populated</returns>
        public static object PopulateDataClass(object obj, IDataReader reader, IDataProvider provider)
        {
			TypeConfiguration.MappedClass typeinfo = provider.Parent.Mappings[obj.GetType()];
            //This iteration model will enable the "forward only" type readers
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    TypeConfiguration.MappedField mf = typeinfo[reader.GetName(i)];
                    if (mf != null && !mf.IgnoreWithSelect)
                    {
                        object value = reader.GetValue(i);
                        if (value != DBNull.Value) mf.Field.SetValue(obj, value);
                        else mf.Field.SetValue(obj, provider.GetNullValue(mf.Field.FieldType));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't set field\nError: " + ex.Message);
                }
            }
            return obj;
        }

        public static object[] TransformToObjects(Type type, IDataReader reader, IDataProvider provider)
        {
            List<object> items = new List<object>();
            while (reader.Read())
            {
                object newobj = Activator.CreateInstance(type);
				ObjectTransformer.PopulateDataClass(newobj, reader, provider);
                items.Add(newobj);
            }

            return items.ToArray();
        }

        public static DATACLASS[] TransformToObjects<DATACLASS>(IDataReader reader, IDataProvider provider)
        {
            List<DATACLASS> items = new List<DATACLASS>();
            while (reader.Read())
            {
                DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
				ObjectTransformer.PopulateDataClass(newobj, reader, provider);
                items.Add(newobj);
            }

            return items.ToArray();
        }

	}
}
