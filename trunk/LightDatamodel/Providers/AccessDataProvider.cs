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
using System.Data.OleDb;
using System.Data.LightDatamodel;
using System.Collections;
using System.IO;

namespace System.Data.LightDatamodel
{
	public class AccessDataProvider : GenericDataProvider
	{
		Stack m_transactions = new Stack();
		
		public AccessDataProvider(string connectionstring)
		{
			m_connection = new OleDb.OleDbConnection(connectionstring);
		}

		public AccessDataProvider() : this("")
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
			OleDb.OleDbConnection conn = (OleDb.OleDbConnection)m_connection;
			DataTable tbl = conn.GetOleDbSchemaTable(OleDb.OleDbSchemaGuid.Columns, new object[] {null, null, tablename, columname });
			object def = tbl.Rows[0]["COLUMN_DEFAULT"];

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

			//check if it's a unique index
			if (m_connection.State != ConnectionState.Open) m_connection.Open();
			OleDb.OleDbConnection conn = (OleDb.OleDbConnection)m_connection;
			DataTable tbl = conn.GetOleDbSchemaTable(OleDb.OleDbSchemaGuid.Indexes, new object[] { null, null, null, null, tablename });
			if (tbl != null && tbl.Rows.Count > 0)
			{
				DataRow[] rows = tbl.Select("COLUMN_NAME = '" + columname + "'");
				if (rows != null && rows.Length > 0)
				{
					foreach (DataRow row in rows)
						if ((bool)row["UNIQUE"]) unique = true;
				}
			}
			return unique;
		}

		public override string GetPrimaryKey(string tablename)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			DataTable primsch = ((OleDb.OleDbConnection)m_connection).GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new object[] {null, null, tablename});
			if( primsch == null || primsch.Rows.Count == 0) return "";
			return primsch.Rows[0].ItemArray[3].ToString();
		}

		public override string[] GetTablenames()
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			DataTable tablesschema = ((OleDb.OleDbConnection)m_connection).GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {null, null, null, "TABLE"});
			string[] tablenames = new string[tablesschema.Rows.Count];
			for(int i = 0; i< tablenames.Length; i++)
				tablenames[i] = tablesschema.Rows[i][2].ToString();
			return tablenames;
		}

		public override string QuoteColumnname(string columnname)
		{
			return "[" + columnname + "]";
		}

		public override string QuoteTablename(string tablename)
		{
			return "[" + tablename + "]";
		}

		public override void BeginTransaction(Guid id)
		{
			if (m_transactions.Count == 5)
				throw new Exception("Access databases have a limit of 5 nested transactions");

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
            IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "Select @@IDENTITY";
            object o = cmd.ExecuteScalar();
            cmd.Dispose();
            return o;
		}

	}

	public class AccessProviderConfiguration : IConfigureableDataProvider
	{
		public ConfigureProperties Configure(System.Windows.Forms.Form owner, ConfigureProperties previousConnectionProperties)
		{
			System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
			dlg.Filter = "Access database (*.mdb;*.mde)|*.mdb;*.mde|Alle filer (*.*)|*.*";

			if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ConfigureProperties prop = new ConfigureProperties();
				prop.Connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dlg.FileName + ";";
				prop.DestinationDir = Path.GetDirectoryName(dlg.FileName);
				prop.Namespace = "Datamodel." + Path.GetFileNameWithoutExtension(dlg.FileName);
				return prop;
			}
			else
				return previousConnectionProperties;
		}

		public string FriendlyName { get { return "Access database"; } }

		public string Name { get { return new AccessDataProvider().ToString(); } }

		public ConfigureProperties AutoConfigure(string[] args)
		{
			if(args.Length > 0 && File.Exists(args[0]) && (Path.GetExtension(args[0]).ToLower() == ".mdb" || Path.GetExtension(args[0]).ToLower() == ".mde"))
			{
				ConfigureProperties prop = new ConfigureProperties();
				prop.Connectionstring = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + args[0] + ";";
				prop.DestinationDir = Path.GetDirectoryName(args[0]);
				prop.Namespace = "Datamodel." + Path.GetFileNameWithoutExtension(args[0]);
				return prop;
			}

			return null;
		}

		public IDataProvider GetProvider(string connectionstring)
		{
			return new AccessDataProvider(connectionstring);
		}
	}
}