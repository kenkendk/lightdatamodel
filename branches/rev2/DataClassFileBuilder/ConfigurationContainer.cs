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
using System.Data.LightDatamodel;

namespace DataClassFileBuilder
{
	public class ConfigurationContainer
	{
		public Table[] m_tables;

		public class Table
		{
			public string Name;
			public string Classname;
			public string PrimaryKey;
			public string ViewSQL;
			public Column[] Columns;
			public bool Ignore;
			public List<Relation> Relations = new List<Relation>();
		}

		public class Column
		{
			public string Typename;
			public string Name;
			public string Fieldname;
			public bool Autonumber;
			public bool PrimaryKey;
			public object Default;
			public bool IgnoreWithSelect;
			public bool IgnoreWithUpdate;
			public bool IgnoreWithInsert;
			public bool Index;
			public Type GetFieldType(){	return Type.GetType(Typename);}
		}

		public class Relation
		{
			public enum RelationType
			{
				OneToOne,
				OneToMany,
				ManyToOne,
			}

			public string Name;
			public string Databasefield;
			public string Propertyname;
			public RelationType Type;
			public string ReverseTablename;
			public string ReverseDatabasefield;
			public string ReversePropertyname;
			public string Tablename;

			private bool m_targetisparent = true;
			public bool TargetIsParent { get { return m_targetisparent; } }

			public Relation GetOppositDirection()
			{
				Relation tmp = new Relation();
				tmp.Name = Name;
				switch (Type)
				{
					case RelationType.ManyToOne:
						tmp.Type = RelationType.OneToMany;
						break;
					case RelationType.OneToMany:
						tmp.Type = RelationType.ManyToOne;
						break;
					case RelationType.OneToOne:
						tmp.Type = RelationType.OneToOne;
						break;
				}
				tmp.Databasefield = ReverseDatabasefield;
				tmp.Tablename = ReverseTablename;
				tmp.Propertyname = ReversePropertyname;
				tmp.ReverseDatabasefield = Databasefield;
				tmp.ReversePropertyname = Propertyname;
				tmp.ReverseTablename = Tablename;
				tmp.m_targetisparent = false;
				return tmp;
			}
		}

		/// <summary>
		/// Builds configuration for a provider
		/// </summary>
		/// <param name="provider">The provider to use</param>
		public static Table[] DescribeDataSource(IDataProvider provider)
		{
			string[] tables = provider.GetTablenames();
			Table[] ret = new Table[tables.Length];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = new Table();
				ret[i].Name = tables[i];
				ret[i].Classname = tables[i];
				ret[i].PrimaryKey = provider.GetPrimaryKey(tables[i]);
				ret[i].Ignore = false;

				//columns
				Dictionary<string, Type> columns = provider.GetTableStructure(ret[i].Name);
				ret[i].Columns = new Column[columns.Count];
				int k = 0;
				foreach (KeyValuePair<string, Type> col in columns)
				{
					ret[i].Columns[k] = new Column();
					ret[i].Columns[k].Name = col.Key;
					ret[i].Columns[k].Fieldname = col.Key;
					ret[i].Columns[k].Typename = col.Value.FullName;
					ret[i].Columns[k].PrimaryKey = ret[i].PrimaryKey == ret[i].Columns[k].Name;
					ret[i].Columns[k].Autonumber = provider.IsAutoIncrement(ret[i].Name, ret[i].Columns[k].Name);
					k++;
				}
			}

			return ret;
		}

		/// <summary>
		/// Builds configuration for a provider
		/// </summary>
		/// <param name="provider">The provider to use</param>
		public static Table DescribeDataSource(IDataProvider provider, string viewname, string sql)
		{
			Table ret = new Table();
			ret.Name = viewname;
			ret.ViewSQL = sql;
			ret.Classname = viewname;
			ret.Ignore = false;

			//columns
			Dictionary<string, Type> columns = provider.GetStructure(sql);
			ret.Columns = new Column[columns.Count];
			int k = 0;
			foreach (KeyValuePair<string, Type> col in columns)
			{
				ret.Columns[k] = new Column();
				ret.Columns[k].Name = col.Key;
				ret.Columns[k].Fieldname = col.Key;
				ret.Columns[k].Typename = col.Value.FullName;
				k++;
			}

			return ret;
		}

		public static Table[] MergeSetups(Table[] userdefinedtables, Table[] databasetables)
		{
			if (userdefinedtables == null) return databasetables;

			//index the suckers
			Dictionary<string, Table> userdefined = new Dictionary<string, Table>(userdefinedtables.Length);
			foreach (Table t in userdefinedtables) userdefined.Add(t.Name, t);
			Dictionary<string, Table> database = new Dictionary<string, Table>(databasetables.Length);
			foreach (Table t in databasetables) database.Add(t.Name, t);

			List<Table> ret = new List<Table>();

			//go throuh user tables
			foreach (Table user in userdefinedtables)
			{
				if (!String.IsNullOrEmpty(user.ViewSQL))
					ret.Add(user);		//blindly transfer all views
				else
				{
					if (database.ContainsKey(user.Name))
					{
						//index columns
						Dictionary<string, Column> usercolumns = new Dictionary<string, Column>(user.Columns.Length);
						foreach (Column t in user.Columns) usercolumns.Add(t.Name, t);
						Dictionary<string, Column> datacolumns = new Dictionary<string, Column>(database[user.Name].Columns.Length);
						foreach (Column t in database[user.Name].Columns) datacolumns.Add(t.Name, t);

						List<Column> retcols = new List<Column>();

						foreach (Column col in user.Columns)
						{
							if (datacolumns.ContainsKey(col.Name))
							{
								col.Typename = datacolumns[col.Name].Typename;
								retcols.Add(col);
							}
						}
						foreach (Column col in database[user.Name].Columns)
						{
							if (!usercolumns.ContainsKey(col.Name))
								retcols.Add(col);
						}


						user.Columns = retcols.ToArray();

						//add table
						ret.Add(user);
					}
				}
			}

			//go through database
			foreach (Table data in databasetables)
			{
				if(!userdefined.ContainsKey(data.Name))
					ret.Add(data);
			}


			return ret.ToArray();
		}

		#region " Save/Load "

		/// <summary>
		/// Save to disk
		/// </summary>
		public static void Save(Table[] tables, string path)
		{
			ConfigurationContainer tmp = new ConfigurationContainer();
			tmp.m_tables = tables;
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationContainer));
			System.IO.FileStream fs = null;
			try
			{
				fs = new System.IO.FileStream(path, System.IO.FileMode.Create);
				ser.Serialize(fs, tmp);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if(fs != null)fs.Close();
			}
		}

		/// <summary>
		/// Load from disk
		/// </summary>
		/// <returns></returns>
		public static Table[] Load(string path)
		{
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(ConfigurationContainer));
			System.IO.FileStream fs = null;
			try
			{
				fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
				ConfigurationContainer tmp = (ConfigurationContainer)ser.Deserialize(fs);
				return tmp.m_tables;
			}
			catch
			{
			}
			finally
			{
				if (fs != null) fs.Close();
			}
			return null;
		}
		#endregion
	}
}
