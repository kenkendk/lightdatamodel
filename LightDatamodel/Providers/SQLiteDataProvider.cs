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
using System.IO;
using System.Collections;

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Summary description for SQLiteDataProvider.
	/// </summary>
	public class SQLiteDataProvider : GenericDataProvider, IConfigureableDataProvider
	{
		#region " IConfigureableDataProvider "

		public ConfigureProperties Configure(System.Windows.Forms.Form owner, ConfigureProperties previousConnectionProperties)
		{
			System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
			dlg.Filter = "SQLite database (*.sqlite;*.sqlite3)|*.sqlite;*.sqlite3|Alle filer (*.*)|*.*";

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

		public string Name { get { return new SQLiteDataProvider().ToString(); } }

		public ConfigureProperties AutoConfigure(string[] args)
		{
			if (args.Length > 0 && File.Exists(args[0]) && (Path.GetExtension(args[0]).Equals(".sqlite", StringComparison.InvariantCultureIgnoreCase) || Path.GetExtension(args[0]).Equals(".sqlite3", StringComparison.InvariantCultureIgnoreCase)))
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

		#endregion

		/// <summary>
		/// To create a provider, using just a connectionstring, the SQLite provider must know what Connection class to create. SQLite is not part of the standard distribution, so we check that it was loaded by the caller.
		/// </summary>
		private static Type SQLiteConnectionType = null;

		public SQLiteDataProvider() : this("")
		{
		}

		public SQLiteDataProvider(string connectionstring)
		{
			if (SQLiteConnectionType == null)
                SQLiteConnectionType = Type.GetType("System.Data.SQLite.SQLiteConnection");
			if (SQLiteConnectionType == null)
				try { SQLiteConnectionType = Activator.CreateInstance("System.Data.SQLite", "System.Data.SQLite.SQLiteConnection").Unwrap().GetType(); }
				catch {}
			
			//.Net 2.0, uses the System.Data.SQLite provider
			if (SQLiteConnectionType == null)
				throw new Exception("Cannot find a suitable SQLite provider, try including the SQLite dll (System.Data.SQLite.dll)\n or manually set the connection type System.Data.LightDataModel.SQLiteDataProvider.SQLiteConnectionType = typeof(System.Data.SQLite.SQLiteConnection)");

            m_connection = (IDbConnection)Activator.CreateInstance(SQLiteConnectionType, new object[] {connectionstring});
			m_originalconnectionstring = connectionstring;
		}

		public SQLiteDataProvider(IDbConnection cmd)
		{
			m_connection = cmd;
			m_originalconnectionstring = cmd.ConnectionString;
		}

        public override bool IsAutoIncrement(string tablename, string column)
        {
            if (!column.Trim().Equals(GetPrimaryKey(tablename).Trim(), StringComparison.InvariantCultureIgnoreCase))
                return false;

            if (m_connection.State != ConnectionState.Open) m_connection.Open();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
				if (m_transactions.Count > 0) cmd.Transaction = m_transactions.Peek();
                cmd.CommandText = "SELECT SQL FROM SQLITE_MASTER WHERE name=" + AddParameter(cmd, "name", tablename) + " AND type='table'";
                using (IDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        throw new Exception("Failed to read SQL from SQLITE_MASTER for table " + tablename);
                    string sql = rd.GetValue(0).ToString();

                    //TODO: use a regexp for this, it will not get any uglier than this :D
                    //Basically we look for "[column name] datatype primary key," and extract "datatype" and check if it is integer
                    int p = sql.IndexOf(" primary", StringComparison.InvariantCultureIgnoreCase);

                    if (p > 0)
                    {
                        sql = sql.Substring(0, p + 1).Trim();
                        p = sql.LastIndexOfAny(new char[] { ',', '(' });
                        if (p >= 0)
                            sql = sql.Substring(p + 1).Trim();
                        p = sql.LastIndexOf(" ");
                        if (p > 0)
                            sql = sql.Substring(p).Trim();

                        if (sql.Trim().Equals("integer", StringComparison.InvariantCultureIgnoreCase))
                            return true;
                    }

                }
            }

            return false;
            
        }

		public override string GetPrimaryKey(string tablename)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
				if (m_transactions.Count > 0) cmd.Transaction = m_transactions.Peek();
                cmd.CommandText = "SELECT SQL FROM SQLITE_MASTER WHERE name=" + AddParameter(cmd, "name", tablename) + " AND type='table'";
                using (IDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        throw new Exception("Failed to read SQL from SQLITE_MASTER for table " + tablename);
                    string sql = rd.GetValue(0).ToString();

                    //TODO: use a regexp for this, it will not get any uglier than this :D
                    //Basically we look for "[column name] datatype primary key," and extract "column name"
                    int p = sql.IndexOf(" primary", StringComparison.InvariantCultureIgnoreCase);

                    if (p > 0)
                    {
                        sql = sql.Substring(0, p + 1).Trim();
                        p = sql.LastIndexOfAny(new char[] { ',', '(' });
                        if (p >= 0)
                            sql = sql.Substring(p + 1).Trim();
                        p = sql.LastIndexOf(" ");
                        p = sql.LastIndexOfAny(new char[] { '"', '\'', ']' });
                        if (p > 0)
                            sql = sql.Substring(0, p + 1);
                        else
                        {
                            p = sql.IndexOf(" ");
                            if (p > 0)
                                sql = sql.Substring(0, p);
                        }




                        if (sql.IndexOfAny(new char[] { '"', '\'', '[' }) == 0)
                            return sql.Substring(1, sql.Length - 2);
                        else
                            return sql;
                    }

                }
            }

			return "";
		}

		public override string[] GetTablenames()
		{
            ArrayList tb = new ArrayList();
            if (m_connection.State != ConnectionState.Open) m_connection.Open();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
				if (m_transactions.Count > 0) cmd.Transaction = m_transactions.Peek();
                cmd.CommandText = "SELECT Name FROM SQLITE_MASTER WHERE type='table' AND name NOT LIKE 'sqlite_%'";
                using (IDataReader rd = cmd.ExecuteReader())
                    while (rd.Read())
                        tb.Add(rd.GetValue(0).ToString());
            }

			return (string[])tb.ToArray(typeof(string));
		}

        /// <summary>
        /// Helper funtion that will insert a parameter in the commands parameter collection, and return a place holder for the value, to be used in the SQL string
        /// </summary>
        /// <param name="cmd">The command to use</param>
        /// <param name="value">The value to insert</param>
        /// <returns>A placeholder for the value, to be used in the SQL command</returns>
        protected override string AddParameter(IDbCommand cmd, string columnname, object value)
        {
            //This method is overridden because SQLite does not have to do tricks 
            // SQLite correctly handles dates, times, and empty strings!

            //TODO: update/insert has pre-cached strings, so we cannot return "NULL", but must insert the parameter
            if ((value == null || value == DBNull.Value) && !cmd.CommandText.StartsWith("insert into", StringComparison.InvariantCultureIgnoreCase) && !cmd.CommandText.StartsWith("update", StringComparison.InvariantCultureIgnoreCase))
            {
                //Unfortunately the "x is null" cannot accept a "x is ?" where ? is a DBNull parameter
                return "NULL";
            }
            else
            {
                IDataParameter p = cmd.CreateParameter();
                p.Value = value;
                cmd.Parameters.Add(p);
                return "?";
            }
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

		//public override void BeginTransaction(Guid id)
		//{
		//    OpenConnection();
		//    IDbCommand cmd = m_connection.CreateCommand();
		//    cmd.CommandText = "BEGIN TRANSACTION '" + id.ToString() + "'";
		//    cmd.ExecuteNonQuery();
		//    cmd.Dispose();
		//}

		//public override void CommitTransaction(Guid id)
		//{
		//    try
		//    {
		//        IDbCommand cmd = m_connection.CreateCommand();
		//        cmd.CommandText = "COMMIT TRANSACTION '" + id.ToString() + "'";			//SQLite will fail at random at these
		//        cmd.ExecuteNonQuery();
		//        cmd.Dispose();
		//    }
		//    catch
		//    {
		//        //let's try ... again. HIHIHI
		//        System.Threading.Thread.Sleep(1000);
		//        IDbCommand cmd = m_connection.CreateCommand();
		//        cmd.CommandText = "COMMIT TRANSACTION '" + id.ToString() + "'";
		//        cmd.ExecuteNonQuery();
		//        cmd.Dispose();
		//    }
		//}

		//public override void RollbackTransaction(Guid id)
		//{
		//    try
		//    {
		//        IDbCommand cmd = m_connection.CreateCommand();
		//        cmd.CommandText = "ROLLBACK TRANSACTION '" + id.ToString() + "'";		//SQLite will fail at random at these
		//        cmd.ExecuteNonQuery();
		//        cmd.Dispose();
		//    }
		//    catch
		//    {
		//        //let's try ... again. HIHIHI
		//        System.Threading.Thread.Sleep(1000);
		//        IDbCommand cmd = m_connection.CreateCommand();
		//        cmd.CommandText = "ROLLBACK TRANSACTION '" + id.ToString() + "'";
		//        cmd.ExecuteNonQuery();
		//        cmd.Dispose();
		//    }
		//}

		public override object GetLastAutogeneratedValue(string tablename)
		{
            IDbCommand cmd = m_connection.CreateCommand();
			if (m_transactions.Count > 0) cmd.Transaction = m_transactions.Peek();
            cmd.CommandText = "SELECT last_insert_rowid()";
            object o = cmd.ExecuteScalar();
            cmd.Dispose();
            return o;
		}

	}
}
