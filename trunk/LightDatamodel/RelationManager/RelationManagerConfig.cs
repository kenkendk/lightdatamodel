using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.LightDatamodel
{
    public class RelationManagerConfig
    {
        /// <summary>
        ///This holds the actual relation, it is used when writing values back into the ID properties
        /// </summary>
        private Dictionary<string, KeyValuePair<TypeConfiguration.MappedField, TypeConfiguration.MappedField>> m_relations;
        /// <summary>
        /// This gives a reference between the Type/Property-key and the internal relation ID
        /// </summary>
        private Dictionary<Type, Dictionary<string, string>> m_lookups;
        /// <summary>
        /// This table provides fast lookup for detecting changes to the ID properties
        /// </summary>
        private Dictionary<Type, Dictionary<string, string>> m_propertyKeys;
        /// <summary>
        /// A lookup to determine if the relation is a collection
        /// </summary>
        private Dictionary<Type, Dictionary<string, bool>> m_isCollection;

        /// <summary>
        /// The basis type configuration
        /// </summary>
        private TypeConfiguration m_typeConfig;

        /// <summary>
        /// Gets a reference to the typeconfiguration this manager is using
        /// </summary>
        public TypeConfiguration TypeConfiguration { get { return m_typeConfig; } }
        
        public RelationManagerConfig(TypeConfiguration typeconfig)
        {
            m_relations = new Dictionary<string, KeyValuePair<TypeConfiguration.MappedField, TypeConfiguration.MappedField>>();
            m_lookups = new Dictionary<Type, Dictionary<string, string>>();
            m_propertyKeys = new Dictionary<Type, Dictionary<string, string>>();
            m_isCollection = new Dictionary<Type, Dictionary<string, bool>>();
            m_typeConfig = typeconfig;
        }

        public string[] GetAvaliblePropKeys()
        {
            string[] tmp = new string[m_relations.Keys.Count];
            int i = 0;
            foreach (string s in m_relations.Keys)
                tmp[i++] = s;
            return tmp;
        }

        public void AddRelation(TypeConfiguration.ReferenceField rf)
        {
            AddRelation(rf.PropertyName, rf.LocalField, rf.ReverseField, false, rf.IsCollection);
        }

        public void AddRelation(string relationkey, TypeConfiguration.MappedField owner, TypeConfiguration.MappedField child, bool ownerCollection, bool childCollection)
        {
            if (m_typeConfig.GetTypeInfo(child.Field.DeclaringType).PrimaryKey != child)
                throw new Exception("Invalid configuration, the child element must have the primary key as the reference field, consider swapping the two");

            if (m_typeConfig.GetTypeInfo(owner.Field.DeclaringType).PrimaryKey == owner)
                throw new Exception("Invalid configuration, the owner element cannot have the primary key as the reference field, consider swapping the two");

            string propKey = Guid.NewGuid().ToString();
            if (!m_propertyKeys.ContainsKey(owner.Field.DeclaringType))
                m_propertyKeys.Add(owner.Field.DeclaringType, new Dictionary<string, string>());
            m_propertyKeys[owner.Field.DeclaringType].Add(relationkey, propKey);

            if (!m_propertyKeys.ContainsKey(child.Field.DeclaringType))
                m_propertyKeys.Add(child.Field.DeclaringType, new Dictionary<string, string>());
            m_propertyKeys[child.Field.DeclaringType].Add(relationkey, propKey);

            m_relations.Add(propKey, new KeyValuePair<TypeConfiguration.MappedField, TypeConfiguration.MappedField>(owner, child));
            if (!m_lookups.ContainsKey(owner.Field.DeclaringType))
                m_lookups.Add(owner.Field.DeclaringType, new Dictionary<string, string>());
            if (!m_lookups.ContainsKey(child.Field.DeclaringType))
                m_lookups.Add(child.Field.DeclaringType, new Dictionary<string, string>());

            if (!m_isCollection.ContainsKey(owner.Field.DeclaringType))
                m_isCollection.Add(owner.Field.DeclaringType, new Dictionary<string, bool>());
            if (!m_isCollection.ContainsKey(child.Field.DeclaringType))
                m_isCollection.Add(child.Field.DeclaringType, new Dictionary<string, bool>());
            m_isCollection[owner.Field.DeclaringType].Add(relationkey, ownerCollection);
            m_isCollection[child.Field.DeclaringType].Add(relationkey, childCollection);

            //TODO: Handle this properly
            /*m_lookups[owner.Field.DeclaringType].Add(owner.ColumnName, propKey);
            m_lookups[child.Field.DeclaringType].Add(child.ColumnName, propKey);*/
        }

        public string[] GetPropKeysByID(Type type)
        {
            List<string> tmp = new List<string>();
            foreach(string s in m_propertyKeys[type].Values)
                //if (m_relations[s].Value.Field.DeclaringType == type)
                    tmp.Add(s);

            return tmp.ToArray();
        }

        public string GetPropKey(Type type, string propertyname)
        {
            TypeConfiguration.MappedClass mc = m_typeConfig.GetTypeInfo(type);

            if (m_propertyKeys.ContainsKey(type) && m_propertyKeys[type].ContainsKey(propertyname))
                return m_propertyKeys[type][propertyname];
            else
                return null;
        }

        public KeyValuePair<TypeConfiguration.MappedField, TypeConfiguration.MappedField> GetMapping(string propkey)
        {
            return m_relations[propkey];
        }

        public string GetRelationKey(Type type, string propkey)
        {
            foreach (KeyValuePair<string, string> s in m_propertyKeys[type])
                if (s.Value == propkey)
                    return s.Key;
            return null;
        }

        public TypeConfiguration.MappedField GetFieldInfo(Type type, string propkey)
        {
            if (m_relations[propkey].Key.Field.DeclaringType == type)
                return m_relations[propkey].Key;
            else
                return m_relations[propkey].Value;
        }

        public bool IsCollection(Type type, string relationKey)
        {
            return m_isCollection[type][relationKey];
        }
    }
}
