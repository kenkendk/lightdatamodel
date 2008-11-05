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
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Data.LightDatamodel.DataClassAttributes;

namespace System.Data.LightDatamodel
{


    /// <summary>
    /// This class reads and caches information about runtime types
    /// </summary>
    public class TypeConfiguration : IEnumerable<TypeConfiguration.MappedClass>
    {
        private Dictionary<Type, MappedClass> m_knownTypes;

		public event EventHandler TypesInitialized;

        /// <summary>
        /// Constructs a new type configuration instance
        /// </summary>
        public TypeConfiguration()
        {
            m_knownTypes = new Dictionary<Type, MappedClass>();
        }

        /// <summary>
        /// Returns the mapping information for a given type
        /// </summary>
        /// <param name="item">The type to obtain the mapping information for</param>
        /// <returns>The requested mapping information</returns>
        public MappedClass this[Type type]
        {
			get
			{
					if (!m_knownTypes.ContainsKey(type))
					{
						lock (m_knownTypes)
						{
							if (m_knownTypes.ContainsKey(type)) return m_knownTypes[type];		//do a recheck due to the lock


							//When no mapping info is avalible, just use reflection
							foreach (Type t in type.Assembly.GetExportedTypes())
								if (t.IsClass && typeof(IDataClass).IsAssignableFrom(t))
									m_knownTypes.Add(t, new MappedClass(this, t));

							//initialize relations
							foreach (MappedClass mc in m_knownTypes.Values)
								mc.InitializeRelations();

						}

							if (!m_knownTypes.ContainsKey(type))
								throw new Exception(string.Format("The supplied type '{0}' could not be mapped!", type.FullName));

						if (TypesInitialized != null) TypesInitialized(this, null);
					}

				return m_knownTypes[type];
			}
        }

		public IEnumerator<TypeConfiguration.MappedClass> GetEnumerator()
		{
			return m_knownTypes.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

        /// <summary>
        /// This class contains the type info for a single class/table
        /// </summary>
        public class MappedClass
        {
			private TypeConfiguration m_parent;
            private string m_tablename;
            private Type m_type;
            private MappedField m_primaryKey;
            private Dictionary<string, MappedField> m_fields;
            private Dictionary<string, ReferenceField> m_referenceFields;
			private LinkedList<MappedField> m_indexes;
            private string m_viewSql;

			public static ATTRIBUTE GetAttribute<ATTRIBUTE>(ICustomAttributeProvider type) where ATTRIBUTE : Attribute
			{
				object[] objs = type.GetCustomAttributes(typeof(ATTRIBUTE), false);
				if (objs != null && objs.Length > 0) return (ATTRIBUTE)objs[0];
				return null;
			}

			public static ATTRIBUTE[] GetAttributes<ATTRIBUTE>(ICustomAttributeProvider type) where ATTRIBUTE : Attribute
			{
				ATTRIBUTE[] objs = (ATTRIBUTE[])type.GetCustomAttributes(typeof(ATTRIBUTE), false);
				return objs;
			}

            /// <summary>
            /// Constructs the mapping info, from the type through reflection
            /// </summary>
            /// <param name="type">The type to map</param>
            public MappedClass(TypeConfiguration parent, Type type)
            {
				m_parent = parent;
                m_fields = new Dictionary<string, MappedField>();
				m_indexes = new LinkedList<MappedField>();
				DatabaseTable t = GetAttribute<DatabaseTable>(type);
				if (t != null) m_tablename = t.Tablename;
				else m_tablename = type.Name;
				DatabaseView view = GetAttribute<DatabaseView>(type);
				if (view != null) m_viewSql = view.SQL;

                m_type = type;
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

				foreach (FieldInfo fi in fields)
				{
					if (GetAttribute<DatabaseField>(fi) != null)
					{
						MappedField mf = new MappedField(this, fi);
						if (mf.IsPrimaryKey) m_primaryKey = mf;
						if (mf.Index || mf.IsPrimaryKey) m_indexes.AddLast(new LinkedListNode<MappedField>(mf));
						m_fields.Add(mf.Databasefield, mf);
					}
				}
            }

			protected internal void InitializeRelations()
			{
				m_referenceFields = new Dictionary<string, ReferenceField>();
				foreach (MappedField fi in m_fields.Values)
				{
					if (GetAttribute<DatabaseField>(fi.Field) != null)
					{
						//search for relations
						Relation[] list = GetAttributes<Relation>(fi.Field);
						if(list != null)
						foreach(Relation rel in list)
						{
							MappedClass child = this;
							MappedClass parent = m_parent[rel.TargetClass];
							MappedField childfield = fi;
							MappedField parentfield = parent[rel.TargetDatabasefield];

							//swap?
							if (!rel.TargetIsParent)
							{
								MappedClass tmp = child;
								child = parent;
								parent = tmp;
								MappedField tmp2 = childfield;
								childfield = parentfield;
								parentfield = tmp2;
							}

							ReferenceField newref = new ReferenceField(rel.Name, childfield, parentfield, child, parent);
							m_referenceFields.Add(rel.Name, newref);
						}
					}
				}
			}

            /// <summary>
            /// Returns the name of the table this class is mapped to
            /// </summary>
            public string Tablename{get { return m_tablename; }}

            /// <summary>
            /// Returns the view sql for view based mappings
            /// </summary>
            public string ViewSQL{get { return m_viewSql; }}

            /// <summary>
            /// Gets the mapped type
            /// </summary>
            public Type Type { get { return m_type; } }

            /// <summary>
            /// Gets or sets the tables primary key
            /// </summary>
            public MappedField PrimaryKey{get { return m_primaryKey; }}

            /// <summary>
            /// Gets or sets the mapping information
            /// </summary>
            /// <param name="columnName">The name of the column</param>
            /// <returns>The mapping information</returns>
            public MappedField this[string columnName]{get { return m_fields[columnName]; }}

            /// <summary>
            /// Gets the list of mapped fields
            /// </summary>
            public Dictionary<string, MappedField> MappedFields { get { return m_fields; } }

			/// <summary>
			/// Get the list of indexed fields
			/// </summary>
			public LinkedList<MappedField> IndexFields { get { return m_indexes; } }

            /// <summary>
            /// Gets the list of reference fields
            /// </summary>
            public Dictionary<string, ReferenceField> ReferenceFields { get { return m_referenceFields; }}
        
        }

        /// <summary>
        /// This class contains the type info for a single column/field/property
        /// </summary>
        public class MappedField
        {
            private string m_databasefield;
            private FieldInfo m_field;
            private PropertyInfo m_property;
            private bool m_isAutoGenerated;
            private bool m_isPrimaryKey;
			private bool m_index;
            private object m_defaultValue;
			private MappedClass m_parent;

            private bool m_ignoreWithInsert;
            private bool m_ignoreWithUpdate;
            private bool m_ignoreWithSelect;

            /// <summary>
            /// Constructs a new field mapping from the type info through reflection
            /// </summary>
            /// <param name="fi">The item to map</param>
            public MappedField(MappedClass parent, FieldInfo fi)
            {
				m_parent = parent;
                m_field = fi;
				m_databasefield = MappedClass.GetAttribute<DatabaseField>(fi).Fieldname;
                m_property = fi.DeclaringType.GetProperty(m_databasefield);
                m_isAutoGenerated = MappedClass.GetAttribute<AutoIncrement>(fi) != null;
				m_isPrimaryKey = MappedClass.GetAttribute<PrimaryKey>(fi) != null;
				m_defaultValue = MappedClass.GetAttribute<Default>(fi) != null ? MappedClass.GetAttribute<Default>(fi).Value : null;
				m_index = MappedClass.GetAttribute<Index>(fi) != null || m_isPrimaryKey;

                m_ignoreWithInsert = MappedClass.GetAttribute<IgnoreWithInsert>(fi) != null;
				m_ignoreWithUpdate = MappedClass.GetAttribute<IgnoreWithUpdate>(fi) != null; 
				m_ignoreWithSelect = MappedClass.GetAttribute<IgnoreWithSelect>(fi) != null;
				if (m_isAutoGenerated)
				{
					m_ignoreWithInsert = true;
					m_ignoreWithUpdate = true;
				}
            }

            public object DefaultValue{get { return m_defaultValue; }}

            public object GetDefaultValue(IDataProvider provider)
            {
                if (m_defaultValue == null)
                    return provider.GetNullValue(m_field.FieldType);
                else
                    return m_defaultValue;
            }

            /// <summary>
            /// Gets or sets the name of the column that this field is mapped to
            /// </summary>
            public string Databasefield { get { return m_databasefield; }}

            /// <summary>
            /// Gets or sets the field assigned to this mapping information
            /// </summary>
            public FieldInfo Field{get { return m_field; }}

            /// <summary>
            /// Gets or sets the property accessor for this field
            /// </summary>
            public PropertyInfo Property { get { return m_property; } }

            /// <summary>
            /// Gets a value indicating if the field/column value is assigned by the data source
            /// </summary>
            public bool IsAutoGenerated{ get { return m_isAutoGenerated; } }

            /// <summary>
            /// Gets a boolean value indicating if this field/column is the primary key
            /// </summary>
            public bool IsPrimaryKey {get { return m_isPrimaryKey; } }

			public bool Index
			{
				get { return m_index; }
				set { m_index = value; }		//this is used by AddIndex
			}

            /// <summary>
            /// Gets a value indicating if this field should be omitted when creating a new row in the data source
            /// </summary>
            public bool IgnoreWithInsert { get { return m_ignoreWithInsert || m_isAutoGenerated; } }

            /// <summary>
            /// Gets a value indicating if this field should be omitted when updating a row in the data source
            /// </summary>
            public bool IgnoreWithUpdate { get { return m_ignoreWithUpdate; } }

            /// <summary>
            /// Gets a value indicating if this field should be omitted when reading a row from the data source
            /// </summary>
            public bool IgnoreWithSelect { get { return m_ignoreWithSelect; } }

			public MappedClass Parent { get { return m_parent; } }

        }

        /// <summary>
        /// This class contains mapping information for a relational field
        /// </summary>
        public class ReferenceField
        {
			private MappedField m_childField;
			private MappedField m_parentField;
            private string m_name;
			private MappedClass m_parent;
			private MappedClass m_child;

			public ReferenceField(string name, MappedField childField, MappedField parentField, MappedClass child, MappedClass parent)
			{
				m_name = name;
				m_childField = childField;
				m_parentField = parentField;
				m_parent = parent;
				m_child = child;
			}

			public MappedField ChildField{get { return m_childField; }}
			public MappedClass Child { get { return m_child; } }
			public MappedClass Parent { get { return m_parent; } }
			public MappedField ParentField{	get { return m_parentField; }}
			public string Name {get { return m_name; }}
        }
	}

}

namespace System.Data.LightDatamodel.DataClassAttributes
{

	#region " Attribute classes "

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class IgnoreWithInsert : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class IgnoreWithUpdate : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class IgnoreWithSelect : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class AutoIncrement : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class PrimaryKey : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class Index : Attribute
	{
	}

	/// <summary>
	/// Used for classes
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DatabaseView : Attribute
	{
		string m_viewsql = "";
		public DatabaseView(string sql)
		{
			m_viewsql = sql;
		}
		public string SQL { get { return m_viewsql; } }
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class Default : Attribute
	{
		object m_value = null;
		public Default(object value)
		{
			m_value = value;
		}
		public object Value { get { return m_value; } }
	}

	/// <summary>
	/// Used for classes
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DatabaseTable : Attribute
	{
		string m_tablename = "";
		public DatabaseTable(string tablename)
		{
			m_tablename = tablename;
		}
		public string Tablename { get { return m_tablename; } }
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class DatabaseField : Attribute
	{
		string m_fieldname = "";
		public DatabaseField(string fieldname)
		{
			m_fieldname = fieldname;
		}
		public string Fieldname { get { return m_fieldname; } }
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class Relation : Attribute
	{
		string m_name = "";
		string m_targetdatabasefield = "";
		Type m_targetclass = null;
		bool m_targetisparent;
		public Relation(string name, Type targetclass, string targetdatabasefield) : this(name, targetclass, targetdatabasefield, true){}
		public Relation(string name, Type targetclass, string targetdatabasefield, bool targetisparent)
		{
			m_name = name;
			m_targetclass = targetclass;
			m_targetdatabasefield = targetdatabasefield;
			m_targetisparent = targetisparent;
		}
		public string Name { get { return m_name; } }
		public Type TargetClass { get { return m_targetclass; } }
		public string TargetDatabasefield { get { return m_targetdatabasefield; } }
		public bool TargetIsParent { get { return m_targetisparent; } }
	}

	/// <summary>
	/// Used for properties to indicate that the property is affecting or affected by a given class. 
	/// This will give the user a chance to prefetch eventual objects
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public sealed class Affects : Attribute
	{
		Type m_targettype = null;
		public Affects(Type targettype)
		{
			m_targettype = targettype;
		}
		public Type TargetType { get { return m_targettype; } }
	}

	#endregion

}
