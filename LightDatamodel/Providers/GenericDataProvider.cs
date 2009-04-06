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
using System.Collections.Generic;

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Summary description for GenericDataProvider.
	/// </summary>
	public abstract class GenericDataProvider : IDataProvider
	{
		protected IDbConnection m_connection;
		protected IDataFetcher m_parent;
		protected string m_originalconnectionstring;

		public IDbConnection Connection
		{
			get { return m_connection;}
			set { m_connection = value; }
		}

		public IDataFetcher Parent
		{
			get { return m_parent; }
			set { m_parent = value; }
		}

		/// <summary>
		/// The connectionstring will be obscured when opened by eg. SQL Server
		/// </summary>
		public string OriginalConnectionString
		{
			get { return m_originalconnectionstring; }
		}

        private class SQLFilterBuilder : QueryModel.OperationOrParameter
        {
			public SQLFilterBuilder(GenericDataProvider provider, IDbCommand cmd, QueryModel.OperationOrParameter op)
            {
                m_cmd = cmd;
                m_provider = provider;
                m_operation = op;
			
				if (m_cmd.CommandText == null)
					m_cmd.CommandText = "";
			}

            private IDbCommand m_cmd;
            private GenericDataProvider m_provider;
			private QueryModel.OperationOrParameter m_operation;
            private int m_colid = 4;

			//public IDbCommand Command
			//{
			//    get { return m_cmd; }
			//    set { m_cmd = value; }
			//}

			//public GenericDataProvider Provider
			//{
			//    get { return m_provider; }
			//    set { m_provider = value; }
			//}

            protected override string TranslateOperator(System.Data.LightDatamodel.QueryModel.Operators opr, string actualname)
            {
                return m_provider.TranslateOperator(opr, actualname);
            }

            protected override void QuoteColumnName(string columnname, System.Text.StringBuilder sb)
            {
                sb.Append(m_provider.QuoteColumnname(columnname));
            }

            protected override bool AddParameter(object o, bool allowNonprimitives, System.Text.StringBuilder sb)
            {
                sb.Append(m_provider.AddParameter(m_cmd, "col" + (m_colid++).ToString(), o));
                return true;
            }

            public override string ToString()
            {
                return this.ToString(false);
            }

            public override object Evaluate(object item, object[] parameters)
            {
                throw new NotImplementedException();
            }

            public override bool IsOperation
            {
                get { throw new NotImplementedException(); }
            }

            public override string ToString(bool allowNonprimitives)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (AsStringInternal(m_operation, allowNonprimitives, sb))
                    return sb.ToString();
                else
                    return null;
            }

        }

		#region Abstract Members

		public abstract string QuoteTablename(string tablename);
		public abstract string QuoteColumnname(string columnname);

		public abstract string GetPrimaryKey(string tablename);
		public abstract string[] GetTablenames();
        public abstract bool IsAutoIncrement(string tablename, string column);

		public abstract void BeginTransaction(Guid id);
		public abstract void CommitTransaction(Guid id);
		public abstract void RollbackTransaction(Guid id);

		public abstract object GetLastAutogeneratedValue(string tablename);

		#endregion

        #region SQL Command Cache Functions
        protected Dictionary<Type, string> m_cachedDelete = new Dictionary<Type, string>();
        protected Dictionary<Type, string> m_cachedSelect = new Dictionary<Type, string>();
        //protected Dictionary<Type, string> m_cachedUpdate = new Dictionary<Type, string>();
        protected Dictionary<Type, string> m_cachedInsert = new Dictionary<Type, string>();
        protected Dictionary<Type, string> m_identityWhere = new Dictionary<Type, string>();

        /// <summary>
        /// Returns a string of the form "DELETE FROM Table" for the given type
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <returns></returns>
        protected virtual string GetDeleteString(TypeConfiguration.MappedClass typeinfo)
        {
            if (!m_cachedDelete.ContainsKey(typeinfo.Type))
            {
				lock (m_cachedDelete)
				{
					if (m_cachedDelete.ContainsKey(typeinfo.Type)) return m_cachedDelete[typeinfo.Type];	//double lock
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append("DELETE FROM ");
					sb.Append(QuoteTablename(typeinfo.Tablename));
					m_cachedDelete[typeinfo.Type] = sb.ToString();
				}
            }

            return m_cachedDelete[typeinfo.Type];
        }

        /// <summary>
        /// Returns a string of the form " WHERE ID=?" for the given type
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <returns></returns>
        protected virtual string GetIdentityWhere(TypeConfiguration.MappedClass typeinfo)
        {
            if (!m_identityWhere.ContainsKey(typeinfo.Type))
            {
				lock (m_identityWhere)
				{
					if (m_identityWhere.ContainsKey(typeinfo.Type)) return m_identityWhere[typeinfo.Type]; //double lock

					if (typeinfo.PrimaryKey == null) throw new Exception("Cannot delete row from table \"" + typeinfo.Tablename + "\" because the table has no primary key");

					//Dummy parameter holder
					IDbCommand cmd = m_connection.CreateCommand();

					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append(" WHERE ");
					sb.Append(QuoteColumnname(typeinfo.PrimaryKey.Databasefield));
					sb.Append("=");
					sb.Append(AddParameter(cmd, "where" + typeinfo.PrimaryKey.Databasefield, ""));
					m_identityWhere[typeinfo.Type] = sb.ToString();
				}
            }

            return m_identityWhere[typeinfo.Type];
        }

        /// <summary>
        /// Returns a string in the form "SELECT Col1,Col2 FROM TABLE" for the given type
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <returns></returns>
        protected virtual string GetSelectString(TypeConfiguration.MappedClass typeinfo)
        {
            if (!m_cachedSelect.ContainsKey(typeinfo.Type))
            {
				lock (m_cachedSelect)
				{
					if (m_cachedSelect.ContainsKey(typeinfo.Type)) return m_cachedSelect[typeinfo.Type]; //double lock

					//check for view
					if (!String.IsNullOrEmpty(typeinfo.ViewSQL) && typeinfo.ViewSQL != "?") return typeinfo.ViewSQL;

					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append("SELECT ");
					foreach (TypeConfiguration.MappedField mf in typeinfo.MappedFields.Values)
						if (!mf.IgnoreWithSelect)
						{
							sb.Append(QuoteColumnname(mf.Databasefield));
							sb.Append(",");
						}
					sb.Length--;
					sb.Append(" FROM ");
					sb.Append(QuoteTablename(typeinfo.Tablename));
					m_cachedSelect[typeinfo.Type] = sb.ToString();
				}
            }

            return m_cachedSelect[typeinfo.Type];
        }

        /// <summary>
        /// Returns a string of the form "UPDATE Table SET Col1=?,Col2=?"
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <returns></returns>
		//protected virtual string GetUpdateString(TypeConfiguration.MappedClass typeinfo)
		//{
		//    if (!m_cachedUpdate.ContainsKey(typeinfo.Type))
		//    {
		//        //Dummy parameter holder
		//        IDbCommand cmd = m_connection.CreateCommand();

		//        System.Text.StringBuilder sb = new System.Text.StringBuilder();
		//        sb.Append("UPDATE ");
		//        sb.Append(QuoteTablename(typeinfo.TableName));
		//        sb.Append(" SET ");
		//        foreach (TypeConfiguration.MappedField mf in typeinfo.Columns.Values)
		//            if (!mf.IgnoreWithUpdate)
		//            {
		//                sb.Append(QuoteColumnname(mf.ColumnName));
		//                sb.Append("=");
		//                sb.Append(AddParameter(cmd, mf.ColumnName, null));
		//                sb.Append(",");
		//            }
		//        sb.Length--;
		//        m_cachedUpdate[typeinfo.Type] = sb.ToString();
		//    }

		//    return m_cachedUpdate[typeinfo.Type];
		//}

        /// <summary>
        /// Returns a string of the type "INSERT INTO Table (Col1,Col2) VALUES (?,?)"
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <returns></returns>
        protected virtual string GetInsertString(TypeConfiguration.MappedClass typeinfo)
        {
            if (!m_cachedInsert.ContainsKey(typeinfo.Type))
            {
				lock (m_cachedInsert)
				{
					if (m_cachedInsert.ContainsKey(typeinfo.Type)) return m_cachedInsert[typeinfo.Type]; //double lock

					//Dummy parameter holder
					IDbCommand cmd = m_connection.CreateCommand();

					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
					sb.Append("INSERT INTO ");
					sb.Append(QuoteTablename(typeinfo.Tablename));
					sb.Append(" (");
					foreach (TypeConfiguration.MappedField mf in typeinfo.MappedFields.Values)
						if (!mf.IgnoreWithInsert)
						{
							sb.Append(QuoteColumnname(mf.Databasefield));
							sb.Append(",");
							sb2.Append(AddParameter(cmd, mf.Databasefield, ""));
							sb2.Append(",");
						}
					sb.Length--;
					sb2.Length--;
					sb.Append(") VALUES (");
					sb.Append(sb2.ToString());
					sb.Append(")");
					m_cachedInsert[typeinfo.Type] = sb.ToString();
				}
            }

            return m_cachedInsert[typeinfo.Type];
        }
        #endregion

        #region IDataProvider Members

		public virtual bool IsIndexed(string tablename, string column)
		{
			OpenConnection();
			if (GetPrimaryKey(tablename) == column) return true;
			return false;
		}

        /// <summary>
		/// This will return the corresponding DBNull value. Eg. a date dbnull could be the classic date 1-1-1
		/// DBNull will be eliminated from the code. There's no such thing as a secondary meaning/use of a field. 
		/// If you need the implicit information on whether the field has been sat or not, add a boolean to the model!
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual object GetNullValue(Type type)
		{
			if(type == typeof(int))
				return int.MinValue;
            else if (type == typeof(long))
                return long.MinValue;
            else if (type == typeof(short))
				return short.MinValue;
			else if(type == typeof(string))
				return "";
			else if(type == typeof(double))
				return double.MinValue;
			else if(type == typeof(float))
				return float.MinValue;
			else if(type == typeof(decimal))
				return decimal.MinValue;
			else if(type == typeof(bool))
				return false;
			else if(type == typeof(DateTime))
				return new DateTime(1,1,1);

			return null;
		}

		/// <summary>
		/// Will return a single result
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="expression"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public virtual object Compute(string tablename, string expression, QueryModel.OperationOrParameter operation)
		{
			OpenConnection();
			IDbCommand cmd = m_connection.CreateCommand();
			string sqlfilter = new SQLFilterBuilder(this, cmd, operation).ToString();
			cmd.CommandText = "SELECT " + expression + " FROM " + QuoteTablename(tablename) + (!string.IsNullOrEmpty(sqlfilter) ? " WHERE " + sqlfilter : "");

			try
			{
				return cmd.ExecuteScalar();
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't load expression (" + expression.ToString() + ") from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		/// <summary>
		/// Will return the length of the given string column. Eg. 20 if it's a Char(20)
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="columnname"></param>
		/// <returns></returns>
		public virtual int GetColumnStringLength(string tablename, string columnname)
		{
			OpenConnection();

			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT " + QuoteColumnname(columnname) + " FROM " + QuoteTablename(tablename) + " WHERE 1 = 0";

			IDataReader r = null;
			try
			{
				r = cmd.ExecuteReader(CommandBehavior.KeyInfo);
				DataTable tbl = r.GetSchemaTable();
				if (tbl != null && tbl.Rows.Count > 0)
				{
					int length = (int)tbl.Rows[0]["ColumnSize"];
					return length < 256 ? length : int.MaxValue;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't get column string length\nError: " + ex.Message);
			}
			finally
			{
				if (r != null)
					r.Close();
			}

			return int.MaxValue;
		}

		/// <summary>
		/// Will get or set the connection string. All DBs I've meet is able to connect through the use of a single connection string. Even Oracle! (It can be rather huge though)
		/// </summary>
		public virtual string ConnectionString
		{
			get{return m_connection.ConnectionString;}
			set{m_connection.ConnectionString = value;}
		}

        /// <summary>
        /// Helper funtion that will insert a parameter in the commands parameter collection, and return a place holder for the value, to be used in the SQL string
        /// </summary>
        /// <param name="cmd">The command to use</param>
        /// <param name="value">The value to insert</param>
        /// <returns>A placeholder for the value, to be used in the SQL command</returns>
		protected virtual string AddParameter(IDbCommand cmd, string paramname, object value)
		{
			IDataParameter p = cmd.CreateParameter();
			if (value == null)
				p.Value = DBNull.Value;
			else if (value.GetType() == typeof(string) && String.IsNullOrEmpty((string)value))
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
			cmd.Parameters.Add(p);
			return "?";
		}

		/// <summary>
		/// Will delete a specified row from the DB. Will throw an exception if non or more than 1 are updated
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="primarycolumnname"></param>
		/// <param name="primaryvalue"></param>
		public virtual void DeleteRow(object item)
		{
			OpenConnection();
			IDbCommand cmd = m_connection.CreateCommand();
            TypeConfiguration.MappedClass typeinfo = m_parent.Mappings[item.GetType()];

            cmd.CommandText = GetDeleteString(typeinfo) + GetIdentityWhere(typeinfo);
			AddParameter(cmd, "where" + typeinfo.PrimaryKey.Databasefield, typeinfo.PrimaryKey.Field.GetValue(item));

			try
			{
				int r = cmd.ExecuteNonQuery();
                if (r != 1)
                {
                    throw new NoSuchObjectException("Delete was expected to delete 1 rows (" + typeinfo.PrimaryKey.Field.GetValue(item).ToString() + "), but deleted: " + r.ToString(), item);
                }
			}
			catch (NoSuchObjectException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't delete row (" + typeinfo.PrimaryKey.Field.GetValue(item).ToString() + ") from table \"" + typeinfo.Tablename + "\"\nError: " + ex.Message + "\nSQL: " + FullCommandText(cmd));
			}
		}

		/// <summary>
		/// This is a debug/error function
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		private static string FullCommandText(IDbCommand cmd)
		{
			try
			{
				string s = cmd.CommandText;
				int i = 0;
				while (s.IndexOf("?") >= 0)
				{
					int f = s.IndexOf("?");
					s = s.Substring(0, f) + ((IDataParameter)cmd.Parameters[i++]).Value.ToString() + s.Substring(f + 1);
				}
				return s;
			}
			catch
			{
				return cmd.CommandText;
			}
		}

		/// <summary>
		/// Will return a single row from the DB
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="primarycolumnname"></param>
		/// <param name="primaryvalue"></param>
		/// <returns></returns>
		public virtual object SelectRow(Type type, object primarykey)
		{
			OpenConnection();
            
			using(IDbCommand cmd = m_connection.CreateCommand())
            {
				TypeConfiguration.MappedClass typeinfo = m_parent.Mappings[type];
                
                cmd.CommandText = GetSelectString(typeinfo) + GetIdentityWhere(typeinfo);
                AddParameter(cmd, "where" + typeinfo.PrimaryKey.Databasefield, primarykey);

			    try
			    {
				    using(IDataReader dr = cmd.ExecuteReader())
                    {
						object[] results = (object[])TransformToObjects(type, dr);
						dr.Close();
                        if (results.Length == 0)
                            return null;
                        else if (results.Length == 1)
                            return results[0];
                        else
                            throw new Exception("Got " + results.Length.ToString() + " results after selection in table \"" + typeinfo.Tablename + "\" with primary key \"" + primarykey.ToString() + "\".");
                    }
			    }
			    catch(Exception ex)
			    {
                    throw new Exception("Couldn't load row (" + primarykey.ToString() + ") from table \"" + typeinfo.Tablename + "\"\nError: " + ex.Message);
			    }
            }
		}

		/// <summary>
		/// Will return the default value for the given column. Eg. 42
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="columname"></param>
		/// <returns></returns>
		public virtual object GetDefaultValue(string tablename, string columname)
		{
			OpenConnection();
			return GetNullValue( GetTableStructure(tablename)[columname]);
		}

		/// <summary>
		/// Will return the default value for the given column. Eg. 42
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="columname"></param>
		/// <returns></returns>
		public virtual object GetDefaultValue(string tablename, string columname, string sql)
		{
			if (String.IsNullOrEmpty(sql)) return GetDefaultValue(tablename, columname);
			OpenConnection();
			return GetNullValue(GetStructure(sql)[columname]);
		}

		/// <summary>
		/// Will return wheter the given column is unique. Eg. a primary key
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="columname"></param>
		/// <returns></returns>
		public virtual bool IsUnique(string tablename, string columname)
		{
			if (columname == GetPrimaryKey(tablename)) return true;		//by this we define that primary keys always will be unique ... uh oh
			else return false;
		}

		/// <summary>
		/// Will open the connection to the DB
		/// </summary>
		protected virtual void OpenConnection()
		{
			try
			{
				if (m_connection.State != ConnectionState.Open) m_connection.Open();
			}
			catch (Exception ex)
			{
				throw new Exception("Couldn't open the connection to the database\nError: " + ex.Message);
			}
		}

		/// <summary>
		/// Will select multiple rows from the DB through the use of QueryModel
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		public virtual object[] SelectRows(Type type, QueryModel.OperationOrParameter operation)
		{
			OpenConnection();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
				TypeConfiguration.MappedClass typeinfo = m_parent.Mappings[type];
                string filter = new SQLFilterBuilder(this, cmd, operation).ToString();
                cmd.CommandText = GetSelectString(typeinfo);

                if (!String.IsNullOrEmpty(filter)) cmd.CommandText += " WHERE " + filter;

                try
                {
					using (IDataReader dr = cmd.ExecuteReader())
					{
						object[] ret = (object[])TransformToObjects(type, dr);
						dr.Close();
						return ret;
					}
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't load rows (" + filter + ") from table \"" + typeinfo.Tablename + "\"\nError: " + ex.Message);
                }
            }
		}

		protected virtual string TranslateOperator(QueryModel.Operators opr, string actualname)
		{
			switch(opr)
			{
				case QueryModel.Operators.And:
					return "AND";
				case QueryModel.Operators.Equal:
					return "=";
				case QueryModel.Operators.GreaterThan:
					return ">";
				case QueryModel.Operators.GreaterThanOrEqual:
					return ">=";
				case QueryModel.Operators.IIF:
					return "IIF";
				case QueryModel.Operators.In:
					return "IN";
				case QueryModel.Operators.LessThan:
					return "<";
				case QueryModel.Operators.LessThanOrEqual:
					return "<=";
				case QueryModel.Operators.Like:
					return "LIKE";
				case QueryModel.Operators.Not:
					return "NOT";
				case QueryModel.Operators.NotEqual:
					return "<>";
				case QueryModel.Operators.Or:
					return "OR";
				case QueryModel.Operators.Xor:
					return "XOR";
                case QueryModel.Operators.Between:
                    return "BETWEEN";
                case QueryModel.Operators.Is:
                    return "IS";
                case QueryModel.Operators.Custom:
                    return actualname;
				default:
					throw new Exception("Bad operator: " + opr.ToString());
								   
			}
		}

		/// <summary>
		/// Will select multiple rows from the DB
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="filter"></param>
		/// <param name="values">Used if the filter is parametized. Eg. ... MyCol = ? AND YourCol = ? ...</param>
		/// <returns></returns>
		//public virtual object[] SelectRows(Type type, string filter, object[] values)
		//{
		//    if(m_connection.State != ConnectionState.Open) m_connection.Open();
		//    using (IDbCommand cmd = m_connection.CreateCommand())
		//    {
		//        TypeConfiguration.MappedClass typeinfo = m_transformer.TypeConfiguration.GetTypeInfo(type);

		//        cmd.CommandText = GetSelectString(typeinfo);
		//        cmd.Parameters.Clear();

		//        if (filter != null && filter != "") cmd.CommandText += " WHERE " + filter;
		//        if (values != null)
		//            foreach (object o in values)
		//                AddParameter(cmd, , o);

		//        try
		//        {
		//            using (IDataReader dr = cmd.ExecuteReader())
		//            {
		//                object[] ret= m_transformer.TransformToObjects(type, dr, this);
		//                dr.Close();
		//                return ret;
		//            }
		//        }
		//        catch (Exception ex)
		//        {
		//            throw new Exception("Couldn't load rows (" + filter + ") from table \"" + typeinfo.TableName + "\"\nError: " + ex.Message);
		//        }
		//    }
		//}

		/// <summary>
		/// Selects multiple rows from DB. 
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="filter">Non parameterized. Eg. MyCol = "Cheese" AND ...</param>
		/// <returns></returns>
		public virtual object[] SelectRows(Type type, string filter)
		{
			OpenConnection();
		    using (IDbCommand cmd = m_connection.CreateCommand())
		    {
				TypeConfiguration.MappedClass typeinfo = m_parent.Mappings[type];

		        cmd.CommandText = GetSelectString(typeinfo);
		        cmd.Parameters.Clear();

		        if (!String.IsNullOrEmpty(filter)) cmd.CommandText += " WHERE " + filter;

		        try
		        {
		            using (IDataReader dr = cmd.ExecuteReader())
		            {
						object[] ret = (object[])TransformToObjects(type, dr);
		                dr.Close();
		                return ret;
		            }
		        }
		        catch (Exception ex)
		        {
		            throw new Exception("Couldn't load rows (" + filter + ") from table \"" + typeinfo.Tablename + "\"\nError: " + ex.Message);
		        }
		    }
		}

		/// <summary>
		/// Will update a given row in the DB. Will throw an exception if none or more than 1 are updated
		/// </summary>
		/// <param name="item"></param>
		/// <param name="primaryvalue"></param>
		public virtual void UpdateRow(object item)
		{
			OpenConnection();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
				TypeConfiguration.MappedClass typeinfo = m_parent.Mappings[item.GetType()];
				DataClassBase orgitem = item as DataClassBase;

				//validate
				if ((orgitem.m_originalvalues == null || orgitem.m_originalvalues.Count == 0) && orgitem.IsDirty) throw new Exception("Update when there's no altered values? ... and yet it's dirty? Something's not right");

				//update sql
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("UPDATE ");
				sb.Append(QuoteTablename(typeinfo.Tablename));
				sb.Append(" SET ");
				bool hasedits = false;

                //Let the "AddParameter" command detect the type of command
                cmd.CommandText = sb.ToString();

				foreach (TypeConfiguration.MappedField mf in typeinfo.MappedFields.Values)
					if (!mf.IgnoreWithUpdate && (orgitem.m_originalvalues != null && orgitem.m_originalvalues.ContainsKey(mf.Databasefield)))
					{
						sb.Append(QuoteColumnname(mf.Databasefield));
						sb.Append("=");
						sb.Append(AddParameter(cmd, mf.Databasefield, mf.Field.GetValue(item)));
						sb.Append(",");
						hasedits = true;
					}
				if (!hasedits) return;
				sb.Length--;
                //cmd.CommandText = GetUpdateString(typeinfo) + GetIdentityWhere(typeinfo);
				cmd.CommandText = sb.ToString() + GetIdentityWhere(typeinfo);

				//values
				//foreach (TypeConfiguration.MappedField mf in typeinfo.Columns.Values)
				//    if (!mf.IgnoreWithUpdate)
				//    {
				//        if (orgitem.m_originalvalues.ContainsKey(mf.ColumnName))		//only update if it's changed
				//            AddParameter(cmd, mf.ColumnName, mf.Field.GetValue(item));
				//        else
				//        {
				//            IDataParameter p = cmd.CreateParameter();
				//            p.Value = null;
				//            cmd.Parameters.Add(p);
				//        }
				//    }

				//where
				if (orgitem.m_originalvalues != null && orgitem.m_originalvalues.ContainsKey(typeinfo.PrimaryKey.Databasefield))
					AddParameter(cmd, "where" + typeinfo.PrimaryKey.Databasefield, orgitem.m_originalvalues[typeinfo.PrimaryKey.Databasefield]);
				else
					AddParameter(cmd, "where" + typeinfo.PrimaryKey.Databasefield, typeinfo.PrimaryKey.Field.GetValue(item));

                try
                {
                    int r = cmd.ExecuteNonQuery();
                    if (r != 1)
                    {
                        string msg = "Command was: " + cmd.CommandText + "\r\n";
                        msg += "Primary key ";

                        if (orgitem.m_originalvalues != null && orgitem.m_originalvalues.ContainsKey(typeinfo.PrimaryKey.Databasefield))
                        {
                            msg += "(dirty key) ";
                            msg += string.Format("{0}, ", orgitem.m_originalvalues[typeinfo.PrimaryKey.Databasefield]);
                        }

                        msg += "(new key) ";
                        msg += string.Format("{0}", typeinfo.PrimaryKey.Field.GetValue(item));

                        throw new NoSuchObjectException("Row update was expected to update 1 row, but updated: " + r.ToString() + ".\r\n" + msg, item);
                    }
                }
				catch (NoSuchObjectException)
				{
					throw;
				}
                catch (Exception ex)
                {
					throw new Exception("Couldn't update row (" + typeinfo.PrimaryKey.Field.GetValue(item).ToString() + ") from table \"" + typeinfo.Tablename + "\"\nError: " + ex.Message + "\nSQL: " + FullCommandText(cmd));
                }
            }
		}

		/// <summary>
		/// Will insert a new row in the DB
		/// </summary>
		/// <param name="tablename"></param>
		/// <param name="values"></param>
		public virtual void InsertRow(object item)
		{
			OpenConnection();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
				TypeConfiguration.MappedClass typeinfo = m_parent.Mappings[item.GetType()];
                cmd.CommandText = GetInsertString(typeinfo);
                foreach (TypeConfiguration.MappedField mf in typeinfo.MappedFields.Values)
					if (!mf.IgnoreWithInsert)
					{
						object val = mf.Field.GetValue(item);
                        AddParameter(cmd, mf.Databasefield, val);
					}

                try
                {
                    int r = cmd.ExecuteNonQuery();
					if (r != 1) throw new NoSuchObjectException("The insert was expected to update 1 row, but updated: " + r.ToString(), item);
                }
				catch (NoSuchObjectException)
				{
					throw;
				}
                catch (Exception ex)
                {
					throw new Exception("Couldn't insert row in table \"" + typeinfo.Tablename + "\"\nError: " + ex.Message + "\nSQL: " + FullCommandText(cmd));
                }
            }
		}

        public virtual Dictionary<string, Type> GetStructure(string sql)
		{
			Dictionary<string, Type> res = new Dictionary<string, Type>();
			OpenConnection();
            using (IDbCommand cmd = m_connection.CreateCommand())
            {
                cmd.CommandText = sql;
                try
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        for (int i = 0; i < dr.FieldCount; i++)
                            res.Add(dr.GetName(i), dr.GetFieldType(i));
						dr.Close();
                        return res;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Couldn't get structure from sql (" + sql + ")\nError: " + ex.Message);
                }
            }
		}

        public virtual Dictionary<string, Type> GetTableStructure(string tablename)
		{
			return GetStructure("SELECT * FROM " + QuoteTablename(tablename) + " WHERE 1 = 0");
		}

		public virtual void Close()
		{
			try
			{
				m_connection.Close();
			}
			catch
			{
			}
		}

		#endregion


        /// <summary>
        /// This will insert the given data into an arbitary object (the private variables)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="data"></param>
        /// <returns>The item populated</returns>
        protected object PopulateDataClass(object obj, IDataReader reader)
        {
            TypeConfiguration.MappedClass typeinfo = this.Parent.Mappings[obj.GetType()];
            //This iteration model will enable the "forward only" type readers
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    TypeConfiguration.MappedField mf = typeinfo[reader.GetName(i)];
                    if (mf != null && !mf.IgnoreWithSelect)
                    {
                        object value = reader.GetValue(i);
                        if (value != DBNull.Value) mf.Field.SetValue(obj, value);		//no events
                        else mf.Field.SetValue(obj, this.GetNullValue(mf.Field.FieldType));		//no events
                    }
                }
                catch (Exception ex)
                {
                    string name = "<unknown>";
                    try { name = reader.GetName(i); }
                    catch { }
                    throw new Exception("Couldn't set field \"" + name + "\"\nError: " + ex.Message);
                }
            }
            return obj;
        }


        /// <summary>
        /// This will transform the DB-result to objects of the given type
        /// </summary>
        /// <param name="type">The type of object to create</param>
        /// <param name="reader">The reader with object data</param>
        /// <returns>An array of objects</returns>
        protected Array TransformToObjects(Type type, IDataReader reader)
        {
            LinkedList<object> items = new LinkedList<object>();
            while (reader.Read())
            {
                object newobj = Activator.CreateInstance(type);
                PopulateDataClass(newobj, reader);
                if (newobj is DataClassBase)
                    (newobj as DataClassBase).ObjectState = ObjectStates.Default;
                items.AddLast(newobj);
            }

            //conver to array
            Array ret = Array.CreateInstance(type, items.Count);
            items.CopyTo((object[])ret, 0);
            return ret;
        }

        /// <summary>
        /// This will transform the DB-result to objects of the given type
        /// </summary>
        /// <typeparam name="DATACLASS">The type of objects to return</typeparam>
        /// <param name="reader">The reader with the data to use</param>
        /// <returns>An array of objects</returns>
        protected DATACLASS[] TransformToObjects<DATACLASS>(IDataReader reader)
        {
            List<DATACLASS> items = new List<DATACLASS>();
            while (reader.Read())
            {
                DATACLASS newobj = Activator.CreateInstance<DATACLASS>();
                PopulateDataClass(newobj, reader);
                items.Add(newobj);
            }

            return items.ToArray();
        }

	}
}
