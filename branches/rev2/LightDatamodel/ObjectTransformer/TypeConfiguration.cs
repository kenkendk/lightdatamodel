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
        //private Dictionary<Assembly, Assembly> m_loadedAssemblies;
        //private RelationManagerConfig m_relationConfig;

		public event EventHandler TypesInitialized;

        /// <summary>
        /// Constructs a new type configuration instance
        /// </summary>
        public TypeConfiguration()
        {
            m_knownTypes = new Dictionary<Type, MappedClass>();
            //m_loadedAssemblies = new Dictionary<Assembly, Assembly>();
            //m_relationConfig = new RelationManagerConfig(this);
        }

		//public MappedClass[] MappedClasses
		//{
		//    get
		//    {
		//        MappedClass[] item = new MappedClass[m_knownTypes.Count];
		//        int i = 0;
		//        foreach (MappedClass mc in m_knownTypes.Values)
		//            item[i++] = mc;
		//        return item;
		//    }   
		//}

        //public RelationManagerConfig RelationConfig { get { return m_relationConfig; } }

        /// <summary>
        /// Creates a reference field. This is only used in the editor, and to avoid problems with calling the default constructor,
        /// this is implemented as a method rather than a constructor
        /// </summary>
        /// <returns>a new instance of a ReferenceField</returns>
		//public static ReferenceField CreateReferenceField()
		//{
		//    return ReferenceField.CreateInstance();
		//}

        /// <summary>
        /// Merges two setups
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="current"></param>
		//public static void MergeSetups(List<MappedClass> previous, List<MappedClass> current)
		//{
		//    Dictionary<string, MappedClass> prev = new Dictionary<string, MappedClass>();
		//    foreach (MappedClass mc in previous)
		//        prev.Add(mc.Tablename, mc);

		//    foreach(MappedClass mc in current)
		//        if (prev.ContainsKey(mc.Tablename))
		//        {
		//            if (prev[mc.Tablename].Classname != prev[mc.Tablename].Tablename) mc.Classname = prev[mc.Tablename].Classname;

		//            List<MappedField> toremove = new List<MappedField>();
		//            foreach(MappedField mf in mc.MappedFields.Values)
		//                if (prev[mc.Tablename].MappedFields.ContainsKey(mf.Databasefield))
		//                {
		//                    MappedField prevCol = prev[mc.Tablename].MappedFields[mf.Databasefield];
		//                    if (prevCol.FieldName != "m_" + prevCol.Databasefield) mf.FieldName = prevCol.FieldName;
		//                    if (prevCol.PropertyName != prevCol.Databasefield) mf.PropertyName = prevCol.PropertyName;
		//                    mf.IgnoreWithInsert = prevCol.IgnoreWithInsert;
		//                    mf.IgnoreWithUpdate = prevCol.IgnoreWithUpdate;
		//                    mf.IgnoreWithSelect = prevCol.IgnoreWithSelect;
		//                    mf.DefaultValue = prevCol.DefaultValue;
		//                    if (mf.DataTypeName == null) mf.DataTypeName = prevCol.DataTypeName;
		//                    if (mf.DataType == null) mf.DataType = prevCol.DataType;
		//                }
		//                else if (prev[mc.Tablename].IgnoredFields.ContainsKey(mf.Databasefield)) toremove.Add(mf);

		//            foreach (MappedField mf in toremove)
		//                mc.MappedFields.Remove(mf.Databasefield);

		//            mc.ReferenceFields = prev[mc.Tablename].ReferenceFields;
		//            mc.IgnoredFields = prev[mc.Tablename].IgnoredFields;

		//            prev.Remove(mc.Tablename);
		//        }

		//    foreach (MappedClass mc in prev.Values)
		//        if (mc.ViewSQL != null)
		//            current.Add(mc);
		//}

        /// <summary>
        /// Saves a mapping to an Xml file
        /// </summary>
        /// <param name="maps">The mapping setup</param>
        /// <param name="filename">The filename to save the setup with</param>
		//public static void SaveXml(List<MappedClass> maps, List<IgnoredClass> ignored, string filename)
		//{
		//    XmlDocument doc = new XmlDocument();
		//    XmlNode root = doc.AppendChild(doc.CreateElement("mapping"));
		//    foreach (MappedClass mc in maps)
		//        mc.Serialize(root.AppendChild(doc.CreateElement("class")));
		//    foreach (IgnoredClass ic in ignored)
		//        ic.Serialize(root.AppendChild(doc.CreateElement("ignoredclass")));

		//    doc.Save(filename);
		//}


        /// <summary>
        /// Loads all ignored files from an Xml file, used to merge existing file when database upgrades
        /// </summary>
        /// <param name="filename">The file with mapping info</param>
        /// <returns></returns>
		//public static List<IgnoredClass> GetIgnoredTables(string filename)
		//{
		//    XmlDocument doc = new XmlDocument();
		//    doc.Load(filename);
		//    List<IgnoredClass> res = new List<IgnoredClass>();

		//    foreach (XmlNode n in doc.SelectNodes("/mapping/ignoredclass"))
		//        res.Add(new IgnoredClass(n));

		//    return res;
		//}

        /// <summary>
        /// Loads a config from an Xml file, used to merge existing file when database upgrades
        /// </summary>
        /// <param name="filename">The file with mapping info</param>
        /// <returns></returns>
		//public static List<MappedClass> LoadXml(string filename)
		//{
		//    XmlDocument doc = new XmlDocument();
		//    doc.Load(filename);
		//    List<MappedClass> res = new List<MappedClass>();

		//    foreach (XmlNode n in doc.SelectNodes("/mapping/class"))
		//        res.Add(new MappedClass(n));

		//    return res;
		//}

        /// <summary>
        /// Builds configuration for a provider
        /// </summary>
        /// <param name="provider">The provider to use</param>
		//public static List<MappedClass> DescribeDataSource(IDataProvider provider)
		//{
		//    List<MappedClass> res = new List<MappedClass>();
		//    foreach (string s in provider.GetTablenames())
		//       res.Add(new MappedClass(s, provider));

		//    return res;
		//}

        /// <summary>
        /// Builds a mapping for a view
        /// </summary>
        /// <param name="provider">The provider to use</param>
		//public static MappedClass DescribeDataSource(IDataProvider provider, string viewname, string sql)
		//{
		//    return new MappedClass(provider, viewname, sql);
		//}

        /// <summary>
        /// Returns the mapping information for a given item
        /// </summary>
        /// <param name="item">The item to obtain the mapping information for</param>
        /// <returns>The requested mapping information</returns>
		//public MappedClass GetTypeInfo(object item)
		//{
		//    return GetTypeInfo(item.GetType());
		//}

        /// <summary>
        /// Loads the mapping information for an assembly from the given manifest resource stream
        /// </summary>
        /// <param name="asm">The assembly containing the defined types</param>
        /// <param name="docname">The name of the resource stream with the mapping document</param>
		//public void LoadMapping(Assembly asm, string docname)
		//{
		//    XmlDocument doc = new XmlDocument();
		//    doc.Load(asm.GetManifestResourceStream(docname));
		//    LoadMapping(asm, doc);
		//}

        /// <summary>
        /// Loads the mapping information from the supplied document
        /// </summary>
        /// <param name="asm">The assembly containing the defined types</param>
        /// <param name="config">The mapping document</param>
		//public void LoadMapping(Assembly asm, XmlDocument config)
		//{
		//    m_loadedAssemblies[asm] = null;

		//    Dictionary<string, Type> partial = new Dictionary<string, Type>();
		//    Dictionary<string, Type> full = new Dictionary<string, Type>();

		//    foreach(Type t in asm.GetExportedTypes())
		//        if (t.IsClass)
		//        {
		//            //If more than one class has the same name, they can only be acessed by the fully qualified name
		//            if (partial.ContainsKey(t.Name))
		//                partial[t.Name] = null;
		//            else
		//                partial.Add(t.Name, t);
		//            full.Add(t.FullName, t);
		//        }

		//    Dictionary<XmlNode, MappedClass> tmp = new Dictionary<XmlNode, MappedClass>();
		//    foreach (XmlNode n in config.SelectNodes("/mapping/class"))
		//    {
		//        if (n.Attributes["tablename"] == null || n.Attributes["classname"] == null)
		//            throw new Exception("Bad document, all \"class\" nodes must have both the tablename and classname attributes");

		//        Type t = null;
		//        if (partial.ContainsKey(n.Attributes["classname"].Value))
		//            t = partial[n.Attributes["classname"].Value];
		//        if (t == null && full.ContainsKey(n.Attributes["classname"].Value))
		//            t = full[n.Attributes["classname"].Value];
		//        if (t == null)
		//            throw new Exception("Unable to map \"" + n.Attributes["classname"].Value + "\" to a class." + (partial.ContainsKey(n.Attributes["classname"].Value) ? "\nMultiple classes share the name, so please use the fully qualified classname" : ""));

		//        m_knownTypes.Add(t, new MappedClass(t, n));
		//        tmp.Add(n, m_knownTypes[t]);
		//    }

		//    foreach (XmlNode n in tmp.Keys)
		//        foreach (XmlNode n2 in n.SelectNodes("referencecolumn"))
		//        {
		//            ReferenceField rf = new ReferenceField(tmp[n].Type, this, n2);
		//            tmp[n].ReferenceFields.Add(rf.PropertyName, rf);
		//            m_relationConfig.AddRelation(rf);
		//        }

		//}

        /// <summary>
        /// Returns a mapping document for all types from a given assembly
        /// </summary>
        /// <param name="asm">The assembly to obtain the mapping for</param>
        /// <returns>A mapping document</returns>
		//public XmlDocument GetMapping(Assembly asm)
		//{
		//    XmlDocument doc = new XmlDocument();
		//    XmlNode root = doc.AppendChild(doc.CreateElement("mappping"));
		//    foreach(MappedClass c in m_knownTypes.Values)
		//        if (c.Type.Assembly == asm)
		//        {
		//            XmlNode n = root.AppendChild(doc.CreateElement("table"));
		//            c.Serialize(n);
		//        }

		//    return doc;
		//}

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

							//if (!m_loadedAssemblies.ContainsKey(type.Assembly))
							//{
							//bool mapped = false;
							//foreach(string s in type.Assembly.GetManifestResourceNames())
							//    if (s.EndsWith("LightDataModel.Mapping.xml"))
							//    {
							//        LoadMapping(type.Assembly, s);
							//        mapped = true;
							//    }


							//When no mapping info is avalible, just use reflection
							//if (!mapped)
							//{
							foreach (Type t in type.Assembly.GetExportedTypes())
								if (t.IsClass && typeof(IDataClass).IsAssignableFrom(t))
									m_knownTypes.Add(t, new MappedClass(this, t));
							//}

							//initialize relations
							foreach (MappedClass mc in m_knownTypes.Values)
								mc.InitializeRelations();

							//foreach (MappedClass mc in m_knownTypes.Values)
							//    foreach (ReferenceField rf in mc.ReferenceFields.Values)
							//        m_relationConfig.AddRelation(rf);


							//m_loadedAssemblies[type.Assembly] = null;
							//}

						}

							if (!m_knownTypes.ContainsKey(type))
								throw new Exception(string.Format("The supplied type '{0}' could not be mapped!", type.FullName));

						if (TypesInitialized != null) TypesInitialized(this, null);
					}

				return m_knownTypes[type];
			}
        }

        /// <summary>
        /// Returns the database table name for the given type
        /// </summary>
        /// <param name="type">The type to query for</param>
        /// <returns>The name of the table</returns>
		//public string GetTableName(Type type)
		//{
		//    return GetTypeInfo(type).Tablename;
		//}

        /// <summary>
        /// Returns the database table name for the given type
        /// </summary>
        /// <param name="item">The item to query for</param>
        /// <returns>The name of the table</returns>
		//public string GetTableName(object item)
		//{
		//    return GetTableName(item.GetType());
		//}

        /// <summary>
        /// Gets the name of the unique column
        /// </summary>
        /// <param name="item">The item to obtain the unique column name from</param>
        /// <returns>The unique column name</returns>
		//public string UniqueColumn(object item)
		//{
		//    return GetTypeInfo(item).PrimaryKey.Databasefield;
		//}

        /// <summary>
        /// Gets the name of the unique column
        /// </summary>
        /// <param name="type">The type to obtain the unique column name from</param>
        /// <returns>The unique column name</returns>
		//public string UniqueColumn(Type type)
		//{
		//    return GetTypeInfo(type).PrimaryKey.Databasefield;
		//}

        /// <summary>
        /// Gets the name of the unique column
        /// </summary>
        /// <param name="item">The type to obtain the unique column name from</param>
        /// <returns>The unique column name</returns>
		//public object UniqueValue(object item)
		//{
		//    return GetTypeInfo(item).PrimaryKey.Field.GetValue(item);
		//}

        /// <summary>
        /// Returns a value indicating if the supplied types primary key is assigned by the data source 
        /// </summary>
        /// <param name="type">The type to obtain the information from</param>
        /// <returns>True or false</returns>
		//public bool IsPrimaryKeyAutoGenerated(Type type)
		//{
		//    MappedClass mc = GetTypeInfo(type);
		//    return mc.PrimaryKey != null && mc.PrimaryKey.IsAutoGenerated;
		//}

        /// <summary>
        /// Returns a value indicating if the supplied items primary key is assigned by the data source 
        /// </summary>
        /// <param name="type">The item to obtain the information from</param>
        /// <returns>True or false</returns>
		//public bool IsPrimaryKeyAutoGenerated(object item)
		//{
		//    return IsPrimaryKeyAutoGenerated(item.GetType());
		//}

        /// <summary>
        /// Returns a list of fields for a given type
        /// </summary>
        /// <param name="type">The type to obtain the fields from</param>
        /// <returns>A list of fields</returns>
		//public FieldInfo[] GetFields(Type type)
		//{
		//    List<FieldInfo> fi = new List<FieldInfo>();
		//    foreach (MappedField mf in this[type].MappedFields.Values)
		//        fi.Add(mf.Field);
		//    return fi.ToArray();
		//}



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
            //private Dictionary<string, IgnoredField> m_ignoredFields;
            private string m_viewSql;
            //private string m_className;

			//public MappedClass(IDataProvider provider, string viewname, string sql)
			//{
			//    m_tablename = viewname;
			//    //m_className = viewname;
			//    m_type = null;
			//    m_primaryKey = null;
			//    m_viewSql = sql;
			//    m_fields = new Dictionary<string, MappedField>();
			//    m_referenceFields = new Dictionary<string, ReferenceField>();
			//    m_ignoredFields = new Dictionary<string, IgnoredField>();

			//    foreach (KeyValuePair<string, Type> columns in provider.GetStructure(sql))
			//        m_fields.Add(columns.Key, new MappedField(columns.Key, columns.Value, provider));
			//}

            /// <summary>
            /// Constructs mapping information from a data provider
            /// </summary>
            /// <param name="tablename">The name of the table to build the configuration for</param>
            /// <param name="provider">The provider to use for the table</param>
			//public MappedClass(string tablename, IDataProvider provider)
			//{
			//    m_tablename = tablename;
			//    //m_className = tablename;
			//    m_type = null;
			//    m_viewSql = null;
			//    m_fields = new Dictionary<string, MappedField>();
			//    m_referenceFields = new Dictionary<string, ReferenceField>();
			//    m_ignoredFields = new Dictionary<string, IgnoredField>();
			//    string primaryKey = provider.GetPrimaryKey(tablename);
			//    foreach (KeyValuePair<string, Type> columns in provider.GetTableStructure(tablename))
			//    {
			//        MappedField mf = new MappedField(columns.Key, columns.Value, provider);
			//        if (mf.Databasefield == primaryKey)
			//        {
			//            mf.IsPrimaryKey = true;
			//            this.PrimaryKey = mf;
			//        }
			//        m_fields.Add(mf.Databasefield, mf);
			//        mf.IsAutoGenerated = provider.IsAutoIncrement(tablename, columns.Key);
			//    }

			//}

            /// <summary>
            /// Constructs mapping information for a given type, using the given Xml mapping information
            /// </summary>
            /// <param name="type">The type to map</param>
            /// <param name="mapping">The mapping information</param>
			//public MappedClass(XmlNode mapping)
			//{
			//    if (mapping.Attributes["tablename"] == null || mapping.Attributes["classname"] == null) throw new Exception("All \"class\" nodes must have the columnname and fieldname attributes");
			//    m_tablename = mapping.Attributes["tablename"].Value;
			//    m_className = mapping.Attributes["classname"].Value;
			//    m_type = null;
                
			//    if (mapping.Attributes["viewSql"] != null)
			//        m_viewSql = mapping.Attributes["viewSql"].Value;
			//    else
			//        m_viewSql = null;

			//    m_fields = new Dictionary<string, MappedField>();
			//    foreach (XmlNode n in mapping.SelectNodes("column"))
			//    {
			//        MappedField mf = new MappedField(n);
			//        if (mf.IsPrimaryKey)
			//            m_primaryKey = mf;
			//        m_fields.Add(mf.Databasefield, mf);
			//    }

			//    m_referenceFields = new Dictionary<string, ReferenceField>();
			//    foreach (XmlNode n in mapping.SelectNodes("referencecolumn"))
			//    {
			//        ReferenceField rf = new ReferenceField(n);
			//        m_referenceFields.Add(rf.PropertyName, rf);
			//    }

			//    m_ignoredFields = new Dictionary<string, IgnoredField>();
			//    foreach (XmlNode n in mapping.SelectNodes("ignoredcolumn"))
			//    {
			//        IgnoredField i = new IgnoredField(n);
			//        m_ignoredFields.Add(i.Fieldname, i);
			//    }

			//}

            /// <summary>
            /// Constructs mapping information for a given type, using the given Xml mapping information
            /// </summary>
            /// <param name="type">The type to map</param>
            /// <param name="mapping">The mapping information</param>
			//public MappedClass(Type type, XmlNode mapping)
			//{
			//    m_tablename = mapping.Attributes["tablename"].Value;
			//    m_type = type;
			//    m_className = type.FullName;
                
			//    m_fields = new Dictionary<string, MappedField>();
			//    foreach (XmlNode n in mapping.SelectNodes("column"))
			//    {
			//        MappedField mf = new MappedField(type, n);
			//        if (mf.IsPrimaryKey)
			//            m_primaryKey = mf;
			//        m_fields.Add(mf.Databasefield, mf);
			//    }

			//    m_referenceFields = new Dictionary<string, ReferenceField>();
			//    m_ignoredFields = new Dictionary<string, IgnoredField>();
			//}

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

				//IDataClass item = (IDataClass)Activator.CreateInstance(type);
				//foreach (FieldInfo fi in fields)
				//{
				//    MappedField mf = new MappedField(fi);
				//    if (item.UniqueColumn == mf.Databasefield)
				//    {
				//        mf.IsPrimaryKey = true;
				//        m_primaryKey = mf;
				//    }
				//    m_fields.Add(mf.Databasefield, mf);
				//}

                //m_referenceFields = new Dictionary<string, ReferenceField>();
                //m_ignoredFields = new Dictionary<string, IgnoredField>();
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
							MappedClass local = this;
							MappedClass reverse = m_parent[rel.TargetClass];
							MappedField localfield = fi;
							MappedField reversefield = reverse[rel.TargetDatabasefield];
							PropertyInfo localproperty = this.m_type.GetProperty( rel.ThisProperty);
							PropertyInfo reverseproperty = reverse.m_type.GetProperty(rel.TargetProperty);
							ReferenceField newref = new ReferenceField(rel.Name, localfield, localproperty, reversefield, reverseproperty);
							m_referenceFields.Add(rel.Name, newref);
						}
					}
				}
			}

            /// <summary>
            /// Returns the name of the table this class is mapped to
            /// </summary>
            public string Tablename
            {
                get { return m_tablename; }
                set { m_tablename = value; }
            }

			//public string Classname
			//{
			//    get { return m_className; }
			//    set { m_className = value; }
			//}

            /// <summary>
            /// Returns the view sql for view based mappings
            /// </summary>
            public string ViewSQL
            {
                get { return m_viewSql; }
                set { m_viewSql = value; }
            }

            /// <summary>
            /// Gets the mapped type
            /// </summary>
            public Type Type { get { return m_type; } }

            /// <summary>
            /// Gets or sets the tables primary key
            /// </summary>
            public MappedField PrimaryKey
            {
                get { return m_primaryKey; }
                set { m_primaryKey = value; }
            }

            /// <summary>
            /// Gets a reference field by name
            /// </summary>
            /// <param name="name">The name of the field</param>
            /// <returns>The matching reference field</returns>
            public ReferenceField GetReferenceField(string name)
            {
                if (!m_referenceFields.ContainsKey(name)) throw new Exception(string.Format("Failed to locate a reference property name {0} in class {1}", name, this.m_type.FullName));
                return m_referenceFields[name];
            }

			/// <summary>
			/// Will add a reference to the engine
			/// </summary>
			/// <param name="reversetable"></param>
			/// <param name="reversepropertyname"></param>
			/// <param name="localpropertyname"></param>
			/// <param name="localiscollection"></param>
			//public void AddReferenceField(string reversetable, string reversekeypropertyname, string localkeypropertyname, string localrelationproperty, bool localiscollection)
			//{
			//    string name = localrelationproperty;
			//    if(m_referenceFields.ContainsKey(name))m_referenceFields.Remove(name);
			//    m_referenceFields.Add(name, new ReferenceField(reversetable, reversekeypropertyname, localkeypropertyname, localiscollection));

			//}

            /// <summary>
            /// Gets or sets the mapping information
            /// </summary>
            /// <param name="columnName">The name of the column</param>
            /// <returns>The mapping information</returns>
            public MappedField this[string columnName]
            {
                get { return m_fields[columnName]; }
                set { m_fields[columnName] = value; }
            }

            /// <summary>
            /// Gets the lisf of mapped fields
            /// </summary>
            public Dictionary<string, MappedField> MappedFields { get { return m_fields; } }

			public LinkedList<MappedField> IndexFields { get { return m_indexes; } }

            /// <summary>
            /// Gets the list of reference fields
            /// </summary>
            public Dictionary<string, ReferenceField> ReferenceFields 
            { 
                get { return m_referenceFields; }
                //set { m_referenceFields = value; }
            }


            /// <summary>
            /// Gets the lisf of ignored fields
            /// </summary>
			//public Dictionary<string, IgnoredField> IgnoredFields 
			//{ 
			//    get { return m_ignoredFields; }
			//    set { m_ignoredFields = value; }
			//}
            
            /// <summary>
            /// Writes all mapping information into an empty Xml node
            /// </summary>
            /// <param name="mapping">The node to update with mapping information</param>
			//public void Serialize(XmlNode mapping)
			//{
			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("tablename")).Value = m_tablename;
			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("classname")).Value = m_className;
			//    if (m_viewSql != null)
			//        mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("viewSql")).Value = m_viewSql;
			//    foreach (MappedField mf in m_fields.Values)
			//    {
			//        XmlNode n = mapping.AppendChild(mapping.OwnerDocument.CreateElement("column"));
			//        mf.Serialize(n);
			//    }

			//    foreach (ReferenceField rf in m_referenceFields.Values)
			//    {
			//        XmlNode n = mapping.AppendChild(mapping.OwnerDocument.CreateElement("referencecolumn"));
			//        rf.Serialize(n);
			//    }
			//}
        
        }

        /// <summary>
        /// This class contains the type info for a single column/field/property
        /// </summary>
        public class MappedField
        {
            private string m_databasefield;
            private FieldInfo m_field;
            private PropertyInfo m_property;
            //private Type m_dataType;
            private bool m_isAutoGenerated;
            private bool m_isPrimaryKey;
			private bool m_index;
            private object m_defaultValue;
			private MappedClass m_parent;

            private bool m_ignoreWithInsert;
            private bool m_ignoreWithUpdate;
            private bool m_ignoreWithSelect;

            //private string m_fieldName;
            //private string m_propertyName;
            //private string m_datatypename;

            /// <summary>
            /// Constructs mapping information from the database
            /// </summary>
            /// <param name="columnname">The name of the column</param>
            /// <param name="type">The data type</param>
            /// <param name="provider"></param>
			//public MappedField(string columnname, Type type, IDataProvider provider)
			//{
			//    m_databasefield = columnname;
			//    m_field = null;
			//    m_property = null;
			//    m_isAutoGenerated = false; //Set in table/class
			//    m_isPrimaryKey = false; //Set in table/class
			//    m_dataType = type;
			//    m_defaultValue = provider.GetNullValue(m_dataType);

			//    m_ignoreWithInsert = false;
			//    m_ignoreWithUpdate = false;
			//    m_ignoreWithSelect = false;

			//    m_fieldName = "m_" + columnname;
			//    m_propertyName = columnname;
			//}

            /// <summary>
            /// Constructs a new fieldmapping, based on the xml info
            /// </summary>
            /// <param name="type">The type containing the field</param>
            /// <param name="mapping">The mapping information</param>
			//public MappedField(XmlNode mapping)
			//{
			//    if (mapping.Attributes["columnname"] == null || mapping.Attributes["fieldname"] == null)
			//        throw new Exception("All \"column\" nodes must have the columnname and fieldname attributes");

			//    m_databasefield = mapping.Attributes["columnname"].Value;
			//    m_field = null;
			//    m_property = null;
			//    m_fieldName = mapping.Attributes["fieldname"].Value;
			//    if (mapping.Attributes["propertyname"] != null)
			//        m_propertyName = mapping.Attributes["propertyname"].Value;

			//    if (mapping.Attributes["datatype"] != null)
			//    {
			//        m_datatypename = mapping.Attributes["datatype"].Value;
			//        m_dataType = Type.GetType(m_datatypename);

			//        if (mapping.Attributes["nullvalue"] != null)
			//        {
			//            if (m_dataType != null)
			//                m_defaultValue = Convert.ChangeType(mapping.Attributes["nullvalue"].Value, m_dataType);
			//            else
			//                throw new Exception("Invalid datatype: " + m_datatypename);
			//        }
			//        else
			//            m_defaultValue = null;
			//    }
			//    else
			//        m_defaultValue = null;

			//    m_isAutoGenerated = mapping.Attributes["isAutoGenerated"] != null ? bool.Parse(mapping.Attributes["isAutoGenerated"].Value) : false;
			//    m_isPrimaryKey = mapping.Attributes["isPrimaryKey"] != null ? bool.Parse(mapping.Attributes["isPrimaryKey"].Value) : false;

			//    m_ignoreWithInsert = mapping.Attributes["ignoreWithInsert"] != null ? bool.Parse(mapping.Attributes["ignoreWithInsert"].Value) : false;
			//    m_ignoreWithUpdate = mapping.Attributes["ignoreWithUpdate"] != null ? bool.Parse(mapping.Attributes["ignoreWithUpdate"].Value) : false;
			//    m_ignoreWithSelect = mapping.Attributes["ignoreWithSelect"] != null ? bool.Parse(mapping.Attributes["ignoreWithSelect"].Value) : false;
			//}

            /// <summary>
            /// Constructs a new fieldmapping, based on the xml info
            /// </summary>
            /// <param name="type">The type containing the field</param>
            /// <param name="mapping">The mapping information</param>
			//public MappedField(Type type, XmlNode mapping) : this(mapping)
			//{
			//    m_field = type.GetField(m_fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			//    if (m_field == null)
			//        m_field = type.GetField(m_fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

			//    if (m_field == null)
			//        throw new Exception("Cannot find field with name \"" + m_fieldName + "\" in type \"" + type.FullName + "\".");

			//    m_dataType = m_field.FieldType;
			//    if (mapping.Attributes["nullvalue"] != null)
			//        m_defaultValue = Convert.ChangeType(mapping.Attributes["nullvalue"].Value, m_dataType);
			//    else
			//        m_defaultValue = null;

			//    if (m_propertyName != null && m_propertyName.Length > 0)
			//    {
			//        m_property = type.GetProperty(m_propertyName);
			//        if (m_property == null)
			//            throw new Exception("Cannot find property with name \"" + m_propertyName + "\" in type \"" + type.FullName + "\".");
			//    }
			//}

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
				m_index = MappedClass.GetAttribute<Index>(fi) != null;

                m_ignoreWithInsert = MappedClass.GetAttribute<IgnoreWithInsert>(fi) != null;
				m_ignoreWithUpdate = MappedClass.GetAttribute<IgnoreWithUpdate>(fi) != null; 
				m_ignoreWithSelect = MappedClass.GetAttribute<IgnoreWithSelect>(fi) != null;
				if (m_isPrimaryKey)
				{
					m_ignoreWithInsert = true;
					m_ignoreWithUpdate = true;
				}
            }

            public object DefaultValue
            {
                get { return m_defaultValue; }
                set { m_defaultValue = value; }
            }

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
            public string Databasefield 
            { 
                get { return m_databasefield; }
                //set { m_databasefield = value; }
            }

            /// <summary>
            /// Gets or sets the field assigned to this mapping information
            /// </summary>
            public FieldInfo Field
            {
                get { return m_field; }
                //set { m_field = value; }
            }

            /// <summary>
            /// Gets or sets the property accessor for this field
            /// </summary>
            public PropertyInfo Property
            {
                get { return m_property; }
                //set { m_property = value; }
            }

			//public string DataTypeName
			//{
			//    get { return m_datatypename; }
			//    set { m_datatypename = value; }
			//}

			//public Type DataType
			//{
			//    get { return m_dataType; }
			//    set { m_dataType = value; }
			//}

            /// <summary>
            /// Gets a value indicating if the field/column value is assigned by the data source
            /// </summary>
            public bool IsAutoGenerated
            {
                get { return m_isAutoGenerated; }
                //set { m_isAutoGenerated = value; }
            }

            /// <summary>
            /// Gets a boolean value indicating if this field/column is the primary key
            /// </summary>
            public bool IsPrimaryKey
            {
                get { return m_isPrimaryKey; }
                //set { m_isPrimaryKey = value; }
            }

			public bool Index
			{
				get { return m_index; }
				//set { m_isPrimaryKey = value; }
			}

            /// <summary>
            /// Gets a value indicating if this field should be omitted when creating a new row in the data source
            /// </summary>
            public bool IgnoreWithInsert
            {
                get { return m_ignoreWithInsert || m_isAutoGenerated; }
                //set { m_ignoreWithInsert = value; }
            }

            /// <summary>
            /// Gets a value indicating if this field should be omitted when updating a row in the data source
            /// </summary>
            public bool IgnoreWithUpdate
            {
                get { return m_ignoreWithUpdate; }
                //set { m_ignoreWithUpdate = value; }
            }

            /// <summary>
            /// Gets a value indicating if this field should be omitted when reading a row from the data source
            /// </summary>
            public bool IgnoreWithSelect
            {
                get { return m_ignoreWithSelect; }
                //set { m_ignoreWithSelect = value; }
            }

            /// <summary>
            /// Gets or sets the name of the field used in this mapping
            /// </summary>
			//public string FieldName
			//{
			//    get { return m_fieldName; }
			//    set { m_fieldName = value; }
			//}

            /// <summary>
            /// Gets or sets the name of the property in this mapping
            /// </summary>
			//public string PropertyName
			//{
			//    get { return m_propertyName; }
			//    set { m_propertyName = value; }
			//}

            /// <summary>
            /// Writes mapping info back into an xml node
            /// </summary>
            /// <param name="mapping">The empty node to update with mapping info</param>
			//public void Serialize(XmlNode mapping)
			//{
			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("columnname")).Value = m_databasefield;
			//    if (m_field == null)
			//    {
			//        mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("fieldname")).Value = m_fieldName;
			//        if (m_propertyName != null && m_propertyName.Length > 0)
			//            mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("propertyname")).Value = m_propertyName;
			//    }
			//    else
			//    {
			//        mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("fieldname")).Value = m_field.Name;
			//        if (m_property != null)
			//            mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("propertyname")).Value = m_property.Name;
			//    }


			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("isAutoGenerated")).Value = m_isAutoGenerated.ToString(); 
			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("isPrimaryKey")).Value = m_isPrimaryKey.ToString();

			//    if (m_datatypename != null)
			//        mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("datatype")).Value = m_datatypename;
			//    else if (m_dataType != null)
			//        mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("datatype")).Value = m_dataType.FullName;

			//    if (m_defaultValue != null)
			//        mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("nullvalue")).Value = m_defaultValue.ToString();

			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("ignoreWithInsert")).Value = m_ignoreWithInsert.ToString();
			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("ignoreWithUpdate")).Value = m_ignoreWithUpdate.ToString();
			//    mapping.Attributes.Append(mapping.OwnerDocument.CreateAttribute("ignoreWithSelect")).Value = m_ignoreWithSelect.ToString();
			//}

        }

		//public class IgnoredClass
		//{
		//    public string m_tablename;

		//    public IgnoredClass()
		//    {
		//    }

		//    public IgnoredClass(XmlNode node)
		//    {
		//        if (node.Attributes["tablename"] == null)
		//            throw new Exception("All ignoredclass tags must have a table parameter");
		//        m_tablename = node.Attributes["tablename"].Value;
		//    }

		//    public string Tablename
		//    {
		//        get { return m_tablename; }
		//        set { m_tablename = value; }
		//    }

		//    public void Serialize(XmlNode node)
		//    {
		//        node.Attributes.Append(node.OwnerDocument.CreateAttribute("tablename")).Value = m_tablename;
		//    }
		//}

        /// <summary>
        /// This class represent an ignored field.
        /// This ensures that the class generator does not re-create a previously hidden field.
        /// </summary>
		//public class IgnoredField
		//{
		//    public string m_fieldname;

		//    public IgnoredField()
		//    {
		//    }

		//    public IgnoredField(XmlNode node)
		//    {
		//        if (node.Attributes["column"] == null)
		//            throw new Exception("All ignoredfield tags must have a column parameter");
		//        m_fieldname = node.Attributes["column"].Value;
		//    }

		//    public string Fieldname
		//    {
		//        get { return m_fieldname; }
		//        set { m_fieldname = value; }
		//    }

		//    public void Serialize(XmlNode node)
		//    {
		//        node.Attributes.Append(node.OwnerDocument.CreateAttribute("column")).Value = m_fieldname;
		//    }
		//}


        /// <summary>
        /// This class contains mapping information for a relational field
        /// </summary>
        public class ReferenceField
        {
            //private bool m_isCollection;

			//private string m_reverse_table;
			//private string m_reverse_column;
			//private string m_reverse_propertyName;

			//private string m_columnname;
			//private string m_propertyName;

			//private MappedClass m_parent;
			private MappedField m_localField;
			private PropertyInfo m_localProperty;
			private MappedField m_reverseField;
			private PropertyInfo m_reverseProperty;
            private string m_name;

			//public ReferenceField()
			//{
			//}

			public ReferenceField(string name, MappedField localField, PropertyInfo localProperty, MappedField reverseField, PropertyInfo reverseProperty)
			{
				m_name = name;
				m_localField = localField;
				m_localProperty = localProperty;
				m_reverseField = reverseField;
				m_reverseProperty = reverseProperty;
			}

			//public ReferenceField(Type type, TypeConfiguration parent, XmlNode node)
			//    : this(node)
			//{
			//    m_parent = parent;
			//    m_localProperty = type.GetProperty(m_propertyName);
			//    m_localField = parent[type][m_columnname];

			//    if (m_localProperty == null)
			//        throw new Exception(string.Format("Failed to locate property {0} in class {1}", m_propertyName, type.FullName));
			//    if (m_localField == null)
			//        throw new Exception(string.Format("Failed to locate field {0} in class {1}", m_columnname, type.FullName));

			//    foreach (MappedClass mc in parent.m_knownTypes.Values)
			//        if (mc.Tablename == m_reverse_table)
			//        {
			//            m_reverseField = mc[m_reverse_column];
			//            m_reverseProperty = mc.Type.GetProperty(m_reverse_propertyName);
			//            break;
			//        }
			//}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="reversetable">These are for the remote table/class</param>
			/// <param name="reversepropertyname">These are for the remote table/class</param>
			/// <param name="localpropertyname">These are for the local table/class</param>
			/// <param name="localiscollection">True if the local property is a collection</param>
			//public ReferenceField(string reversetable, string reversepropertyname, string localpropertyname, bool localiscollection)
			//{
			//    //These are for the remote table/class
			//    m_reverse_table = reversetable;
			//    m_reverse_column = reversepropertyname;
			//    m_reverse_propertyName = null;

			//    //These are for the local table/class
			//    m_columnname = localpropertyname;
			//    m_propertyName = localpropertyname;

			//    //True if the local property is a collection
			//    m_isCollection = localiscollection;
			//}

			//public ReferenceField(XmlNode node) : this()
			//{
			//    //These are for the remote table/class
			//    m_reverse_table = node.Attributes["reverse_table"].Value;
			//    m_reverse_column = node.Attributes["reverse_column"].Value;
			//    m_reverse_propertyName = node.Attributes["reverse_property"].Value;

			//    //These are for the local table/class
			//    m_columnname = node.Attributes["column"].Value;
			//    m_propertyName = node.Attributes["property"].Value;

			//    //True if the local property is a collection
			//    m_isCollection = bool.Parse(node.Attributes["iscollection"].Value);

			//    //Backwards compatible propertykey
			//    m_relationKey = node.Attributes["relation_key"] == null ? m_propertyName : node.Attributes["relation_key"].Value;
			//}


            /// <summary>
            /// Creates a reference field. This is only used in the editor, and to avoid problems with calling the default constructor,
            /// this is implemented as a method rather than a constructor
            /// </summary>
            /// <returns>a new instance of a ReferenceField</returns>
			//internal static ReferenceField CreateInstance()
			//{
			//    return new ReferenceField();
			//}

            /*public object GetReferencedItem(IDataClass owner)
            {
                return owner.RelationManager.GetReferenceObject(owner, this);
            }

            public void SetReferencedItem(IDataClass owner, IDataClass remoteobject)
            {
                owner.RelationManager.SetReferenceObject(owner, this, remoteobject);
            }*/

			//public string ReverseTablename
			//{
			//    get { return m_reverse_table; }
			//    set { m_reverse_table = value; }
			//}

			//public string PropertyName
			//{
			//    get { return m_propertyName; }
			//    set { m_propertyName = value; }
			//}

			//public string Column
			//{
			//    get { return m_columnname; }
			//    set { m_columnname = value; }
			//}

			//public string ReverseColumn
			//{
			//    get { return m_reverse_column; }
			//    set { m_reverse_column = value; }
			//}

			//public string ReversePropertyName
			//{
			//    get { return m_reverse_propertyName; }
			//    set { m_reverse_propertyName = value; }
			//}

			//public bool IsCollection
			//{
			//    get { return m_isCollection; }
			//    set { m_isCollection = value; }
			//}

			public MappedField LocalField
			{
				get { return m_localField; }
			}

			public PropertyInfo LocalProperty
			{
				get { return m_localProperty; }
			}

			public MappedField ReverseField
			{
				get { return m_reverseField; }
			}

			public PropertyInfo ReverseProperty
			{
				get { return m_reverseProperty; }
			}

            public string Name
            {
                get { return m_name; }
                //set { m_name = value; }
            }

			public bool LocalIsCollection
			{
				get
				{
					return m_localProperty.PropertyType.IsArray;
				}
			}

			public bool ReverseIsCollection
			{
				get
				{
					return m_reverseProperty.PropertyType.IsArray;
				}
			}

			//public void Serialize(XmlNode node)
			//{
			//    node.Attributes.Append(node.OwnerDocument.CreateAttribute("reverse_table")).Value = m_reverse_table;
			//    node.Attributes.Append(node.OwnerDocument.CreateAttribute("reverse_column")).Value = m_reverse_column;
			//    if (m_reverse_propertyName != null)
			//        node.Attributes.Append(node.OwnerDocument.CreateAttribute("reverse_property")).Value = m_reverse_propertyName;

			//    node.Attributes.Append(node.OwnerDocument.CreateAttribute("column")).Value = m_columnname;
			//    node.Attributes.Append(node.OwnerDocument.CreateAttribute("property")).Value = m_propertyName;

			//    node.Attributes.Append(node.OwnerDocument.CreateAttribute("iscollection")).Value = m_isCollection ? "true" : "false";

			//    if (m_relationKey != null)
			//        node.Attributes.Append(node.OwnerDocument.CreateAttribute("relation_key")).Value = m_relationKey;
			//}
        }

		#region IEnumerable<MappedClass> Members

		public IEnumerator<TypeConfiguration.MappedClass> GetEnumerator()
		{
			return m_knownTypes.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}

}

namespace System.Data.LightDatamodel.DataClassAttributes
{

	#region " Attribute classes "

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class IgnoreWithInsert : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class IgnoreWithUpdate : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class IgnoreWithSelect : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoIncrement : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class PrimaryKey : Attribute
	{
	}

	/// <summary>
	/// Used for fields
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class Index : Attribute
	{
	}

	/// <summary>
	/// Used for classes
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DatabaseView : Attribute
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
	public class Default : Attribute
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
	public class DatabaseTable : Attribute
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
	public class DatabaseField : Attribute
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
	public class Relation : Attribute
	{
		string m_name = "";
		string m_targetdatabasefield = "";
		Type m_targetclass = null;
		string m_targetproperty = "";
		string m_thisproperty = "";
		public Relation(string name, Type targetclass, string targetdatabasefield, string thisproperty, string targetproperty)
		{
			m_name = name;
			m_targetclass = targetclass;
			m_targetproperty = targetproperty;
			m_targetdatabasefield = targetdatabasefield;
			m_thisproperty = thisproperty;
		}
		public string Name { get { return m_name; } }
		public Type TargetClass { get { return m_targetclass; } }
		public string TargetProperty { get { return m_targetproperty; } }
		public string ThisProperty { get { return m_thisproperty; } }
		public string TargetDatabasefield { get { return m_targetdatabasefield; } }
	}

	/// <summary>
	/// Used for properties to indicate that the property is affecting or affected by a given class. 
	/// This will give the user a chance to prefetch eventual objects
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class Affects : Attribute
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
