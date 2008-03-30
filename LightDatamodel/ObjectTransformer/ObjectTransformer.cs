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
	public class ObjectTransformer : IObjectTransformer
	{
        private TypeConfiguration m_configuration;

        public TypeConfiguration TypeConfiguration { get { return m_configuration; } }

        /// <summary>
        /// Constructs a new object transformer
        /// </summary>
        public ObjectTransformer()
        {
            m_configuration = new TypeConfiguration();
        }

        /// <summary>
        /// Creates a new copy of the given item
        /// </summary>
        /// <typeparam name="DATACLASS">The type of object to work on</typeparam>
        /// <param name="source">The source object</param>
        /// <returns>A fresh copy</returns>
        public DATACLASS CreateCopy<DATACLASS>(DATACLASS source)
        {
            return (DATACLASS)CreateCopy(source);
        }

        /// <summary>
        /// Creates a new copy of the given item
        /// </summary>
        /// <param name="source">The source object</param>
        /// <returns>A fresh copy</returns>
        public object CreateCopy(object source)
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
        public void CopyObject(object source, object target)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (target == null)
                throw new ArgumentNullException("target");
            if (target.GetType() != source.GetType())
                throw new Exception("Objects must be of same type");

            FieldInfo[] fields = m_configuration.GetFields(source.GetType());
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
        public object PopulateDataClass(object obj, IDataReader reader, IDataProvider provider)
        {
            TypeConfiguration.MappedClass typeinfo = m_configuration.GetTypeInfo(obj);
            //This iteration model will enable the "forward only" type readers
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    TypeConfiguration.MappedField mf = typeinfo.GetColumn(reader.GetName(i));
                    if (mf != null && !mf.IgnoreWithSelect)
                    {
                        object value = reader.GetValue(i);
                        if (value != DBNull.Value)
                            mf.Field.SetValue(obj, value);
                        else
                            mf.Field.SetValue(obj, provider.GetNullValue(mf.Field.FieldType));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't set field\nError: " + ex.Message);
                }
            }
            return obj;
        }

        public object[] TransformToObjects(Type type, IDataReader reader, IDataProvider provider)
        {
            List<object> items = new List<object>();
            while (reader.Read())
            {
                object newobj = Activator.CreateInstance(type);
                this.PopulateDataClass(newobj, reader, provider);
                items.Add(newobj);
            }

            return items.ToArray();
        }

        public DATACLASS[] TransformToObjects<DATACLASS>(IDataReader reader, IDataProvider provider)
        {
            List<DATACLASS> items = new List<DATACLASS>();
            while (reader.Read())
            {
                DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
                this.PopulateDataClass(newobj, reader, provider);
                items.Add(newobj);
            }

            return items.ToArray();
        }

	}
}
