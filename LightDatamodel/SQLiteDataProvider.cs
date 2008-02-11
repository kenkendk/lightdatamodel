using System;
using System.IO;
using System.Collections;

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Summary description for SQLiteDataProvider.
	/// </summary>
	public class SQLiteDataProvider : GenericDataProvider
	{
		/// <summary>
		/// To create a provider, using just a connectionstring, the SQLite provider must know what Connection class to create.
		/// The two &quot;regular&quot; providers are tried, depending on framework version if this is not set.
		/// </summary>
		public static Type SQLiteConnectionType = null;

		public SQLiteDataProvider(string connectionstring)
		{
			if (System.Environment.Version < new Version(2,0,0,0))
			{
				if (SQLiteConnectionType == null)
					SQLiteConnectionType = Type.GetType("Finisar.SQLite.SQLiteConnection");
				if (SQLiteConnectionType == null)
					try { SQLiteConnectionType = Activator.CreateInstance("SQLite.NET", "Finisar.SQLite.SQLiteConnection").Unwrap().GetType(); }
					catch {}
				
				//.Net 1.1, uses the Finisar.SQLite provider
				if (SQLiteConnectionType == null)
					throw new Exception("Cannot find a suitable SQLite provider, try including the Finisar SQLite dll (SQLite.NET.dll)\n or manually set the connection type System.Data.LightDataModel.SQLiteDataProvider.SQLiteConnectionType = typeof(Finisar.SQLite.SQLiteConnection)");
			}
			else
			{
				if (SQLiteConnectionType == null)
                    SQLiteConnectionType = Type.GetType("System.Data.SQLite.SQLiteConnection");
				if (SQLiteConnectionType == null)
					try { SQLiteConnectionType = Activator.CreateInstance("System.Data.SQLite", "System.Data.SQLite.SQLiteConnection").Unwrap().GetType(); }
					catch {}
				
				//.Net 2.0, uses the System.Data.SQLite provider
				if (SQLiteConnectionType == null)
					throw new Exception("Cannot find a suitable SQLite provider, try including the SQLite dll (System.Data.SQLite.dll)\n or manually set the connection type System.Data.LightDataModel.SQLiteDataProvider.SQLiteConnectionType = typeof(System.Data.SQLite.SQLiteConnection)");
			}
			m_connection = (IDbConnection)Activator.CreateInstance(SQLiteConnectionType, new object[] {connectionstring});
		}

		public SQLiteDataProvider(IDbConnection cmd)
		{
			m_connection = cmd;
		}

		public override string GetPrimaryKey(string tablename)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT SQL FROM SQLITE_MASTER WHERE name=" + AddParameter(cmd, tablename) + " AND type='table'";
			IDataReader rd = null; 
			try
			{
				rd = cmd.ExecuteReader();
				if (!rd.Read())
					throw new Exception("Failed to read SQL from SQLITE_MASTER for table " + tablename);
				string sql = rd.GetValue(0).ToString();

				//TODO: use a regexp for this, it will not get any uglier than this :D
				//Basically we look for "[column name] datatype primary key," and extract "column name"
				int p = sql.ToLower().IndexOf(" primary");

				if (p > 0)
				{
					sql = sql.Substring(0, p + 1).Trim();
					p = sql.LastIndexOfAny(new char[] {',', '('});
					if (p >= 0)
						sql = sql.Substring(p + 1).Trim();
					p = sql.LastIndexOf(" ");
					p = sql.LastIndexOfAny(new char[] {'"', '\'', ']' });
					if (p > 0)
						sql = sql.Substring(0, p + 1);
					else
					{
						p = sql.IndexOf(" ");
						if (p > 0)
							sql = sql.Substring(0, p);
					}
						



					if (sql.IndexOfAny(new char[] {'"', '\'', '[' }) == 0)
						return sql.Substring(1, sql.Length - 2);
					else
						return sql;
				}
	
			} 
			finally 
			{
				try { if (rd != null) rd.Close(); }
				catch {}
			}

			return "";
		}

		public override string[] GetTablenames()
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT Name FROM SQLITE_MASTER WHERE type='table' AND name NOT LIKE 'sqlite_%'";
			IDataReader rd = null;
			ArrayList tb = new ArrayList();
			try
			{
				rd = cmd.ExecuteReader();
				while(rd.Read())
					tb.Add(rd.GetValue(0).ToString());
				rd.Close();
			}
			finally
			{
				try { if (rd != null) rd.Close(); }
				catch {}
			}
			return (string[])tb.ToArray(typeof(string));
		}

		public override string QuoteColumnname(string columnname)
		{
			return "\"" + columnname.Replace("\"", "\\\"") + "\"";
		}

		public override string QuoteTablename(string tablename)
		{
			//TODO: Is this the correct way to escape the table names?
			return "\"" + tablename.Replace("\"", "\\\"") + "\"";
		}

		public override void BeginTransaction(Guid id)
		{
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "BEGIN TRANSACTION '" + id.ToString() + "'";
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public override void CommitTransaction(Guid id)
		{
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "COMMIT TRANSACTION '" + id.ToString() + "'";
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public override void RollbackTransaction(Guid id)
		{
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "ROLLBACK TRANSACTION '" + id.ToString() + "'";
			cmd.ExecuteNonQuery();
			cmd.Dispose();
		}

		public override object GetLastAutogeneratedValue(string tablename)
		{
            IDbCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "SELECT last_insert_rowid()";
            object o = cmd.ExecuteScalar();
            cmd.Dispose();
            return o;
		}

	}

	public class AccessDataProviderConfiguration : IConfigureAbleDataProvider
	{
		public ConfigureProperties Configure(System.Windows.Forms.Form owner, ConfigureProperties previousConnectionProperties)
		{
			System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
			dlg.Filter = "SQLite database (*.sqlite;*.sqlite3)|*.sqlite;*.sqlite3|Alle filer (*.*)|*.*";

			if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ConfigureProperties prop = new ConfigureProperties();
				prop.Connectionstring = "Version=3;Data Source=" + dlg.FileName + ";";
				prop.DestinationDir = Path.GetDirectoryName(dlg.FileName);
				prop.Namespace = "Datamodel." + Path.GetFileNameWithoutExtension(dlg.FileName);
				return prop;
			}
			else
				return previousConnectionProperties;
		}

		public string FriendlyName { get { return "SQLite database"; } }

		public ConfigureProperties AutoConfigure(string[] args)
		{
			if(args.Length > 0 && File.Exists(args[0]) && (Path.GetExtension(args[0]).ToLower() == ".sqlite" || Path.GetExtension(args[0]).ToLower() == ".sqlite3"))
			{
				ConfigureProperties prop = new ConfigureProperties();
				prop.Connectionstring = "Version=3;Data Source=" + args[0] + ";";
				prop.DestinationDir = Path.GetDirectoryName(args[0]);
				prop.Namespace = "Datamodel." + Path.GetFileNameWithoutExtension(args[0]);
				return prop;
			}

			return null;
		}

		public IDataProvider GetProvider(string connectionstring)
		{
			return new SQLiteDataProvider(connectionstring);
		}
	}
}
