﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.LightDatamodel
{
    /// <summary>
    /// This class handles all relations on objects in a given fetcher
    /// </summary>
    public class RelationManager : IRelationManager
    {
        private Dictionary<Guid, IDataClass> m_guid;
        private Dictionary<IDataClass, Guid> m_reverseGuid;
        private Dictionary<Guid, bool> m_existsInDb;
        private Dictionary<Guid, Dictionary<string, Guid>> m_referenceObjects;
        private static Dictionary<Type, bool> m_isAutoGenerated = new Dictionary<Type, bool>();
        private static Dictionary<Type, string> m_primaryColumn = new Dictionary<Type, string>();
        private TypeConfiguration m_typeConfig;

        public RelationManager(TypeConfiguration typeConfig)
        {
            m_guid = new Dictionary<Guid, IDataClass>();
            m_reverseGuid = new Dictionary<IDataClass, Guid>();
            m_existsInDb = new Dictionary<Guid, bool>();
            m_referenceObjects = new Dictionary<Guid, Dictionary<string, Guid>>();
            m_typeConfig = typeConfig;
        }

        /// <summary>
        /// Returns a value indicating if the given object exists in the database. Throws an exception if the item is not registered
        /// </summary>
        /// <param name="item">The item to obtaion the state for</param>
        /// <returns>A value indicating if the given object exists in the database</returns>
        public bool ExistsInDb(IDataClass item)
        {
            if (m_reverseGuid.ContainsKey(item))
            {
                Guid g = m_reverseGuid[item];
                if (m_existsInDb.ContainsKey(g))
                    return m_existsInDb[g];
                else
                    return false;
            }

            throw new Exception("Unregistered object recieved");
        }

        /// <summary>
        /// Gets a list of all references an object has registered
        /// </summary>
        /// <param name="item">The item to retrive the references for</param>
        /// <returns>The items references</returns>
        public Dictionary<string, Guid> GetReferenceObjects(IDataClass item)
        {
            Guid g = GetGuidForObject(item);
            if (!m_referenceObjects.ContainsKey(g))
                m_referenceObjects[g] = new Dictionary<string, Guid>();
            return m_referenceObjects[g]; 
        }

        /// <summary>
        /// Sets an items refrences
        /// </summary>
        /// <param name="item">The items to update</param>
        /// <param name="references">The items references</param>
        public void SetReferenceObjects(IDataClass item, Dictionary<string, Guid> references)
        {
            Guid g = GetGuidForObject(item);
            m_referenceObjects[g] = new Dictionary<string, Guid>();
            foreach (string s in references.Keys)
                m_referenceObjects[g].Add(s, references[s]);
        }

        /// <summary>
        /// Sets the objects creation state
        /// </summary>
        /// <param name="item">The item to set the state for</param>
        /// <param name="state">The items state</param>
        public void SetExistsInDb(IDataClass item, bool state)
        {
            if (m_reverseGuid.ContainsKey(item))
            {
                Guid g = m_reverseGuid[item];
                m_existsInDb[g] = state;
            }
        }

        /// <summary>
        /// Returns the item with the given Guid
        /// </summary>
        /// <param name="g">The Guid of the object to look for</param>
        /// <returns>The item with the given Guid or null</returns>
        public IDataClass GetObjectByGuid(Guid g)
        {
            if (m_guid.ContainsKey(g) && g != Guid.Empty)
                return m_guid[g];
            else
                return null;
        }

        /// <summary>
        /// Gets a value indicating if the item is registered
        /// </summary>
        /// <param name="item">The item to query for</param>
        /// <returns>A value indicating if the item is registered</returns>
        public bool IsRegistered(IDataClass item)
        {
            return m_reverseGuid.ContainsKey(item);
        }

        /// <summary>
        /// Returns the Guid for an item, throws an exception if the item is not registered
        /// </summary>
        public Guid GetGuidForObject(IDataClass item)
        {
            if (m_reverseGuid.ContainsKey(item))
                return m_reverseGuid[item];

            throw new Exception("Object was not registered");
        }

        /// <summary>
        /// Get the primary column of a given type
        /// </summary>
        /// <param name="type">The type to get the column for</param>
        /// <returns>The unique column name</returns>
        public string GetUniqueColumn(Type type)
        {
            if (!m_primaryColumn.ContainsKey(type))
            {
                IDataClass obj = (IDataClass)Activator.CreateInstance(type);
                m_primaryColumn[type] = obj.UniqueColumn;
            }

            return m_primaryColumn[type];
        }

        public T GetReferenceObject<T>(IDataClass owner, string propertyname)
        {
            return (T)GetReferenceObject(owner, propertyname);
        }

        public object GetReferenceObject(IDataClass owner, string propertyname)
        {
            TypeConfiguration.ReferenceField rf = m_typeConfig.GetTypeInfo(owner).GetReferenceField(propertyname);
            return rf.GetReferencedItem(owner);
        }

        public void SetReferenceObject(IDataClass owner, string propertyname, IDataClass value)
        {
            TypeConfiguration.ReferenceField rf = m_typeConfig.GetTypeInfo(owner).GetReferenceField(propertyname);
            rf.SetReferencedItem(owner, value);
        }

        public void SetReferenceObject<T>(IDataClass owner, string propertyname, T value)
            where T : IDataClass
        {
            SetReferenceObject(owner, propertyname, (IDataClass)value);
        }

        public SyncCollectionBase<T> GetReferenceCollection<T>(IDataClass owner, string propertyname) where T : IDataClass
        {
            return new SyncCollectionBase<T>(owner, m_typeConfig.GetTypeInfo(typeof(T)).GetReferenceField(propertyname));
        }


        /// <summary>
        /// Returns a reference to an object
        /// </summary>
        /// <param name="propertyname">The property that is used to access the object</param>
        /// <param name="idfieldname">The name of the ID value in this instance</param>
        /// <param name="owner">The owner of the reference</param>
        /// <returns>The matching item</returns>
        public object GetReferenceObject(IDataClass owner, TypeConfiguration.ReferenceField reference)
        {
            Guid g = GetGuidForObject(owner);
            if (!m_referenceObjects.ContainsKey(g))
                m_referenceObjects.Add(g, new Dictionary<string, Guid>());

            if (!m_referenceObjects[g].ContainsKey(reference.PropertyName))
            {
                //Actually, this should be checked for the remote object, not this object
                if (!this.ExistsInDb(owner) && m_typeConfig.IsPrimaryKeyAutoGenerated(owner))
                    //Avoid doing a lookup if the id is not set
                    return null;

                if (reference.LocalField.Field == null)
                    throw new Exception("Bad config for reference object, field " + reference.LocalField.FieldName + " does not exist in class " + owner.GetType().FullName);
                if (reference.LocalProperty == null)
                    throw new Exception("Bad config for reference object, property " + reference.PropertyName + " does not exist in class " + owner.GetType().FullName);

                IDataClass item = (IDataClass)owner.DataParent.GetObjectById(reference.LocalProperty.PropertyType, reference.LocalField.Field.GetValue(owner));
                if (item != null)
                    m_referenceObjects[g][reference.PropertyName] = GetGuidForObject(item);
                else
                    m_referenceObjects[g][reference.PropertyName] = Guid.Empty;
            }

            return ((DataClassBase)owner).RelationManager.GetObjectByGuid(m_referenceObjects[g][reference.PropertyName]);
        }

        /// <summary>
        /// Sets a reference to an object
        /// </summary>
        /// <param name="propertyname">The property that is used to access the object</param>
        /// <param name="idfieldname">The name of the ID value in this instance</param>
        /// <param name="reversePropertyname">The name of the reverse property or null if there is no reverse property</param>
        /// <param name="value">The value to set the property to</param>
        /// <param name="owner">The owner of the reference</param>
        public void SetReferenceObject(IDataClass owner, TypeConfiguration.ReferenceField reference, IDataClass value)
        {
            Guid g = GetGuidForObject(owner);
            if (!m_referenceObjects.ContainsKey(g))
                m_referenceObjects.Add(g, new Dictionary<string, Guid>());

            if (value == null)
            {
                if (reference.ReverseProperty != null)
                {
                    object revObj = reference.GetReferencedItem(owner);
                    if (revObj != null)
                    {
                        if (reference.ReverseProperty.PropertyType.IsAssignableFrom(owner.GetType()))
                            reference.ReverseProperty.SetValue(revObj, null, null);
                        else
                        {
                            object revObjProp = reference.ReverseProperty.GetValue(revObj, null);
                            //TODO: What does this look like as a generic?
                            //if (revObjProp as ICollection != null)
                            //{
                                MethodInfo mi = revObjProp.GetType().GetMethod("Contains");
                                if (mi == null)
                                    throw new Exception("Type " + revObjProp.GetType().FullName + " did not have a Contains method");
                                bool res = Convert.ToBoolean(mi.Invoke(revObjProp, new object[] { owner }));

                                if (res)
                                {
                                    mi = revObjProp.GetType().GetMethod("Remove");
                                    if (mi == null)
                                        throw new Exception("Type " + revObjProp.GetType().FullName + " did not have a Remove method");
                                    mi.Invoke(revObjProp, new object[] { owner });
                                }
                            /*}
                            else
                                throw new Exception("Reverse Property " + reversePropertyname + " on type " + revObj.GetType().FullName + " must be either type " + ownerType.FullName + " or System.Collections.ICollection");*/
                        }
                    }
                }

                if (m_referenceObjects[g].ContainsKey(reference.PropertyName))
                    m_referenceObjects[g].Remove(reference.PropertyName);
                owner.SetDirty();
                return;
            }

            if (value.DataParent != owner.DataParent) throw new Exception("Cannot mix objects from differenct data contexts");

            m_referenceObjects[g][reference.PropertyName] = GetGuidForObject(value);
            owner.SetDirty();

            if (reference.LocalField.Field == null)
                throw new Exception("Bad config for reference object, field " + reference.LocalField.FieldName + " does not exist in class " + owner.GetType().FullName);

            reference.LocalField.Field.SetValue(owner, value.UniqueValue);

            if (reference.ReverseProperty != null)
            {
                object revval = reference.ReverseProperty.GetValue(value, null);

                if (reference.ReverseProperty.PropertyType.IsAssignableFrom(owner.GetType()))
                {
                    if (revval != owner)
                        reference.ReverseProperty.SetValue(value, owner, null);
                }
                else
                {
                    //TODO: What does this look like as a generic?
                    //if (revval as ICollection != null)
                    //{
                    MethodInfo mi = revval.GetType().GetMethod("Contains");
                    if (mi == null)
                        throw new Exception("Type " + revval.GetType().FullName + " did not have a Contains method");
                    bool res = Convert.ToBoolean(mi.Invoke(revval, new object[] { owner }));
                    if (!res)
                    {
                        mi = revval.GetType().GetMethod("Add");
                        if (mi == null)
                            throw new Exception("Type " + revval.GetType().FullName + " did not have an Add method");
                        mi.Invoke(revval, new object[] { owner });
                    }
                    /*}
                    else
                        throw new Exception("Reverse Property " + reversePropertyname + " on type " + value.GetType().FullName + " must be either type " + ownerType.FullName + " or System.Collections.ICollection"); */
                }
            }
        }

        public bool HasGuid(Guid g)
        {
            return m_guid.ContainsKey(g);
        }

        /// <summary>
        /// Registers an object
        /// </summary>
        /// <param name="g">The guid for the object</param>
        /// <param name="item">The item to register</param>
        /// <returns>The guid</returns>
        public Guid RegisterObject(Guid g, IDataClass item)
        {
            m_guid.Add(g, item);
            m_reverseGuid.Add(item, g);
           return g;
        }

        /// <summary>
        /// Registers an object with a new Guid
        /// </summary>
        /// <param name="item">The item to register</param>
        /// <returns>The newly assigned Guid</returns>
        public Guid RegisterObject(IDataClass item)
        {
            return RegisterObject(Guid.NewGuid(), item);
        }

        /// <summary>
        /// Unregisters an object
        /// </summary>
        /// <param name="item">The item to unregister</param>
        public void UnregisterObject(IDataClass item)
        {
            Guid g = GetGuidForObject(item);
            m_guid.Remove(g);
            m_reverseGuid.Remove(item);
        }

        /// <summary>
        /// Unregisters an object
        /// </summary>
        /// <param name="item">The item to unregister</param>
        public void UnregisterObject(Guid g)
        {
            IDataClass item = GetObjectByGuid(g);
            m_guid.Remove(g);
            m_reverseGuid.Remove(item);
        }

        /// <summary>
        /// Reassigns an items guid
        /// </summary>
        /// <param name="oldGuid">The items previous Guid</param>
        /// <param name="newGuid">The items new Guid</param>
        public void ReassignGuid(Guid oldGuid, Guid newGuid)
        {
            IDataClass item = GetObjectByGuid(oldGuid);
            m_guid.Remove(oldGuid);
            m_reverseGuid.Remove(item);
            RegisterObject(newGuid, item);
        }

        public void CommitItems(IDataFetcher fetcher, List<IDataClass> added, List<IDataClass> deleted, List<IDataClass> modified)
        {
            //Step 1, find types with autoincrement keys
            Dictionary<Guid, IDataClass> specialItems = new Dictionary<Guid, IDataClass>();
            foreach (IDataClass o in added)
            {
                if (m_typeConfig.IsPrimaryKeyAutoGenerated(o.GetType()))
                    specialItems.Add(GetGuidForObject(o), o);
                fetcher.Commit(o);
            }

            //Null update any deleted items
            foreach (IDataClass o in deleted)
                specialItems.Add(GetGuidForObject(o), null);

            //Step 2, re-assign to update internal ID's
            Dictionary<Type, List<PropertyInfo>> classProperties = new Dictionary<Type, List<PropertyInfo>>();

            List<object[]> subUpdates = new List<object[]>();
            foreach (Guid objKey in m_referenceObjects.Keys)
            {
                IDataClass item = GetObjectByGuid(objKey);
                foreach (KeyValuePair<string, Guid> pair in m_referenceObjects[objKey])
                    if (specialItems.ContainsKey(pair.Value))
                    {
                        if (!modified.Contains(item))
                            modified.Add(item);

                        PropertyInfo pi = item.GetType().GetProperty(pair.Key);
                        subUpdates.Add(new object[] { pi, item, specialItems[pair.Value] });
                    }
            }
            
            foreach(object[] s in subUpdates)
                ((PropertyInfo)s[0]).SetValue(s[1], s[2], null);
                    
            //Step 3, update all items
			foreach(IDataClass update in modified)
				fetcher.Commit(update);

			foreach (IDataClass delete in deleted)
				fetcher.Commit(delete);
        }

    }
}