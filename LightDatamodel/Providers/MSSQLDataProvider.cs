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
using System.Data;
using System.Data.SqlClient;
using System.Data.LightDatamodel;
using System.Collections;
using System.IO;

namespace System.Data.LightDatamodel
{
	public class MSSQLDataProvider : GenericDataProvider
	{
		Stack m_transactions = new Stack();
		
		public MSSQLDataProvider(string connectionstring)
		{
			//the ODBC way !!!!
			if (!String.IsNullOrEmpty(connectionstring))
			{
				if (connectionstring.StartsWith("Driver={SQL Server};", StringComparison.InvariantCultureIgnoreCase))
				{
					try
					{
						//Driver={SQL Server};Server=Kursus24;Database=Byggesag;Uid=mkv;Pwd=kvist;
						int i1 = -1, i2 = -1;
						i1 = connectionstring.IndexOf("Server=", StringComparison.InvariantCultureIgnoreCase);
						i2 = connectionstring.IndexOf(";", i1);
						string server = connectionstring.Substring(i1 + 7, i2 - i1 - 7);
						i1 = connectionstring.IndexOf("Database=", StringComparison.InvariantCultureIgnoreCase);
						i2 = connectionstring.IndexOf(";", i1);
						string catalog = connectionstring.Substring(i1 + 9, i2 - i1 - 9);
						i1 = connectionstring.IndexOf("Uid=", StringComparison.InvariantCultureIgnoreCase);
						i2 = connectionstring.IndexOf(";", i1);
						string user = connectionstring.Substring(i1 + 4, i2 - i1 - 4);
						i1 = connectionstring.IndexOf("Pwd=", StringComparison.InvariantCultureIgnoreCase);
						i2 = connectionstring.IndexOf(";", i1);
						string password = connectionstring.Substring(i1 + 4, i2 - i1 - 4);

						//the SQL way
						connectionstring = "Data Source=" + server + ";Initial Catalog=" + catalog + ";User Id=" + user + ";Password=" + password + ";";
					}
					catch (Exception ex)
					{
						throw new Exception("Couldn't convert the ODBC connectionstring (" + connectionstring + ") to the SQL-variant\nError: " + ex.Message);
					}
				}
			}

			m_connection = new SqlConnection(connectionstring);
		}

		public MSSQLDataProvider() : this("")
		{
		}

        public override bool IsAutoIncrement(string tablename, string column)
        {
            if (m_connection.State != ConnectionState.Open) m_connection.Open();

			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM " + QuoteTablename(tablename) + " WHERE 1 = 0";

			try
			{
				IDataReader dr = cmd.ExecuteReader( CommandBehavior.KeyInfo );
				DataTable schema = dr.GetSchemaTable();
				dr.Close();
				DataRow[] row = schema.Select("ColumnName = '" + column + "'");
				if (row == null || row.Length == 0) throw new Exception("Couldn't find column for table");
				return (bool)row[0]["IsAutoIncrement"];
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't load IsAutoIncrement from table \"" + tablename + "\"\nError: " + ex.Message);
			}
        }

		public override object GetDefaultValue(string tablename, string columname)
		{
			if (m_connection.State != ConnectionState.Open) m_connection.Open();

			//get from schema
			SqlConnection conn = (SqlConnection)m_connection;
			DataTable tbl = conn.GetSchema("Columns", new string[] { null, null, tablename, columname });
			object def = null;
			if(tbl != null && tbl.Rows.Count > 0) def = tbl.Rows[0]["COLUMN_DEFAULT"];

			//convert to .net value
			if (def == DBNull.Value || def == null || def.ToString() == "NULL")
			{
				//don't do anything. Let the base handle it
			}
			else
			{
				try
				{
					Type columntype = GetTableStructure(tablename)[columname];
					def = def.ToString().Trim('\"', '\'');
					if (columntype == typeof(DateTime) && def.ToString().ToLower() == "date()" || def.ToString().ToLower() == "curdate()" || def.ToString().ToLower() == "now()") return DateTime.Now;
					return Convert.ChangeType(def, columntype, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch
				{
					//don't do anything really. If it sucks, it sucks
				}
			}

			return base.GetDefaultValue(tablename, columname);
		}

		public override bool IsUnique(string tablename, string columname)
		{
			bool unique = base.IsUnique(tablename, columname);
			if (unique) return true;

			//check if it's a unique 
			//if (m_connection.State != ConnectionState.Open) m_connection.Open();
			//IDbCommand cmd = m_connection.CreateCommand();
			//cmd.CommandText = "SELECT sysobjects.Name FROM sysobjects INNER JOIN syscolumns ON sysobjects.id = syscolumns.id WHERE ";
			
			//TODO

			return unique;
		}

		public override string GetPrimaryKey(string tablename)
		{
			try
			{
				if(m_connection.State != ConnectionState.Open) m_connection.Open();
				IDbCommand cmd = m_connection.CreateCommand();
				cmd.CommandText = "SELECT sysobjects.name as TABLE_NAME, syscolumns.name as COLUMN_NAME, syscolumns.colid as COLUMN_ORDINAL FROM ((sysobjects INNER JOIN sysindexes ON sysobjects.id = sysindexes.id) INNER JOIN sysindexkeys ON sysindexes.indid = sysindexkeys.indid AND sysindexes.id = sysindexkeys.id) INNER JOIN syscolumns ON sysindexkeys.colid = syscolumns.colid AND sysindexkeys.id = syscolumns.id WHERE sysobjects.name = '" + tablename + "' AND sysindexes.name Like 'PK__%'";
				IDataReader r = cmd.ExecuteReader();
				string ret = "";
				if (r.Read()) ret = (string)r.GetValue(1);
				r.Close();
				return ret;
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't load primary key schema\nError: " + ex.Message);
			}
		}

		public override string[] GetTablenames()
		{
			try
			{
				if (m_connection.State != ConnectionState.Open) m_connection.Open();
				DataTable tablesschema = ((SqlConnection)m_connection).GetSchema("Tables", new string[] { null, null, null, "BASE TABLE" });
				string[] tablenames = new string[tablesschema.Rows.Count];
				for (int i = 0; i < tablenames.Length; i++)
					tablenames[i] = tablesschema.Rows[i][2].ToString();
				return tablenames;
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't load table schema\nError: " + ex.Message);
			}
		}

		public override string QuoteColumnname(string columnname)
		{
			return columnname;
		}

		public override string QuoteTablename(string tablename)
		{
			return tablename;
		}

		public override void BeginTransaction(Guid id)
		{
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "BEGIN TRANSACTION";
			cmd.ExecuteNonQuery();
			m_transactions.Push(id);
		}

		public override void CommitTransaction(Guid id)
		{
			if (m_transactions.Count == 0)
				throw new Exception("There were no active transactions");
			if (((Guid)m_transactions.Peek()) != id)
				throw new Exception("Nested transactions must be commited or rolled back in the same order they were created");

			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "COMMIT TRANSACTION";
			cmd.ExecuteNonQuery();
			m_transactions.Pop();
		}

		public override void RollbackTransaction(Guid id)
		{
			if (m_transactions.Count == 0)
				throw new Exception("There were no active transactions");
			if (((Guid)m_transactions.Peek()) != id)
				throw new Exception("Nested transactions must be commited or rolled back in the same order they were created");

			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "ROLLBACK TRANSACTION";
			cmd.ExecuteNonQuery();
            cmd.Dispose();
			m_transactions.Pop();
		}

		public override object GetLastAutogeneratedValue(string tablename)
		{
			try
			{
				IDbCommand cmd = m_connection.CreateCommand();
				cmd.CommandText = "Select CAST(IDENT_CURRENT('" + tablename + "') AS INTEGER)";
				object o = cmd.ExecuteScalar();
				cmd.Dispose();
				return o;
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't load last autonumber from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		protected override string AddParameter(IDbCommand cmd, string columnname, object value)
		{
			IDataParameter p = cmd.CreateParameter();
			if (value == null)
				p.Value = DBNull.Value;
			else if (value.GetType() == typeof(string) && (string)value == "")
				p.Value = DBNull.Value;
			else if (value.GetType() == typeof(DateTime))
			{
				DateTime d = (DateTime)value;
				if (d.Equals(GetNullValue(typeof(DateTime))))
					p.Value = DBNull.Value;
				else
					p.Value = d.ToOADate();
			}
			else
				p.Value = value;
			p.ParameterName = columnname;
			cmd.Parameters.Add(p);
			return "@" + columnname;
		}
	}

	public class MSSQLProviderConfiguration : IConfigureableDataProvider
	{
		public ConfigureProperties Configure(System.Windows.Forms.Form owner, ConfigureProperties previousConnectionProperties)
		{
			ConfigureProperties prop = new ConfigureProperties();
			prop.Connectionstring = "Data Source=Kursus24;Initial Catalog=Byggesag;User Id=mkv;Password=kvist;";
			return prop;
		}

		public string FriendlyName { get { return "MSSQL database"; } }

		public string Name { get { return new MSSQLDataProvider().ToString(); } }

		public ConfigureProperties AutoConfigure(string[] args)
		{
			return null;
		}

		public IDataProvider GetProvider(string connectionstring)
		{
			return new MSSQLDataProvider(connectionstring);
		}
	}
}
