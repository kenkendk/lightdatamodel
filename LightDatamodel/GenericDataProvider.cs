using System;
using System.Collections;

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// Summary description for GenericDataProvider.
	/// </summary>
	public abstract class GenericDataProvider : IDataProvider
	{
		protected IDbConnection m_connection;

		#region Abstract Members

		public abstract string QuoteTablename(string tablename);
		public abstract string QuoteColumnname(string columnname);

		public abstract string GetPrimaryKey(string tablename);
		public abstract string[] GetTablenames();

		public abstract void BeginTransaction(Guid id);
		public abstract void CommitTransaction(Guid id);
		public abstract void RollbackTransaction(Guid id);

		public abstract string GetLastAutogeneratedValue(string tablename);

		#endregion


		#region IDataProvider Members

		public virtual object GetNullValue(Type type)
		{
			//in case of DBNULL
			if(type == typeof(int))
				return int.MinValue;
			else if(type == typeof(short))
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
				new DateTime(1,1,1);

			return null;
		}


		public virtual string ConnectionString
		{
			get{return m_connection.ConnectionString;}
			set{m_connection.ConnectionString = value;}
		}

		protected virtual string AddParameter(IDbCommand cmd, object value)
		{
			IDataParameter p = cmd.CreateParameter();
			p.Value = value;
			cmd.Parameters.Add(p);
			return "?";
		}

		public virtual void DeleteRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "DELETE FROM " + QuoteTablename(tablename) + " WHERE " + QuoteColumnname(primarycolumnname) + " = " + AddParameter(cmd, primaryvalue);

			try
			{
				int r = cmd.ExecuteNonQuery();
				if (r != 1)
					throw new Exception("Delete was expected to delete 1 rows, but deleted: " + r.ToString());
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't delete row (" + primaryvalue.ToString() + ") from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public virtual  Data[] SelectRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM " + QuoteTablename(tablename) + " WHERE " + QuoteColumnname(primarycolumnname) + " = " + AddParameter(cmd, primaryvalue);

			try
			{
				IDataReader dr = cmd.ExecuteReader();
				if(!dr.Read()) {dr.Close();return new Data[0];};
				Data[] ret = new Data[dr.FieldCount];
				for(int i = 0; i < dr.FieldCount; i++)
					ret[i] = new Data(dr.GetName(i), dr.GetValue(i), dr.GetFieldType(i));
				dr.Close();
				return ret;
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't load row (" + primaryvalue.ToString() + ") from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public virtual Data[][] SelectRows(string tablename, QueryModel.Operation operation)
		{
			ArrayList values = new ArrayList();
			string filter = EvalTree(operation, values);
			return SelectRows(tablename, filter, (object[])values.ToArray(typeof(object)));
		}

		protected virtual string TranslateOperator(QueryModel.Operators opr)
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
				default:
					throw new Exception("Bad operator: " + opr.ToString());
								   
			}
		}

		protected virtual string EvalTree(QueryModel.OperationOrParameter opm, ArrayList cmds)
		{
			if (opm.IsOperation)
			{
				QueryModel.Operation operation = opm as QueryModel.Operation;
				switch(operation.Operator)
				{
					case QueryModel.Operators.Not:
						return "Not (" + EvalTree(operation.Parameters[0], cmds) + ")";
					case QueryModel.Operators.IIF:
						return "IIF((" + EvalTree(operation.Parameters[0], cmds) + "),(" + EvalTree(operation.Parameters[1], cmds) + "),(" + EvalTree(operation.Parameters[2], cmds) + "))";
					case QueryModel.Operators.In:
						string[] tmp = new string[operation.Parameters.Length - 1];
						for(int i = 0; i < tmp.Length; i++)
							tmp[i] = EvalTree(operation.Parameters[i+1], cmds);
						return "(" + EvalTree(operation.Parameters[0], cmds) + ") In (" + string.Join(",", tmp) + ")";
					default:
						return "(" + EvalTree(operation.Parameters[0], cmds) + ") " + TranslateOperator(operation.Operator) + " (" +  EvalTree(operation.Parameters[1], cmds) + ")";
				}
			}
			else
			{
				QueryModel.Parameter parameter = opm as QueryModel.Parameter;
				if (parameter.IsColumn)
					return "[" + (string)parameter.Value + "]";
				else
				{
					cmds.Add(parameter.Value);
					return "?";
				}
			}
		}

		public virtual Data[][] SelectRows(string tablename, string filter, object[] values)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM " + QuoteTablename(tablename) + "";
			cmd.Parameters.Clear();

			if(filter != null && filter != "") cmd.CommandText += " WHERE " + filter;
			if (values != null)
				foreach(object o in values)
				{
					IDbDataParameter p = cmd.CreateParameter();
					p.Value = o;
					cmd.Parameters.Add(p);
				}

			try
			{
				IDataReader dr = cmd.ExecuteReader();
				if(!dr.Read()) {dr.Close();return new Data[0][];};
				ArrayList list = new ArrayList();
				do
				{
					Data[] row = new Data[dr.FieldCount];
					for(int i = 0; i < dr.FieldCount; i++)
						row[i] = new Data(dr.GetName(i), dr.GetValue(i), dr.GetFieldType(i));
					list.Add(row);
				} while(dr.Read());
				dr.Close();
				return (Data[][])list.ToArray(typeof(Data[]));
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't load rows (" + filter + ") from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public virtual Data[][] SelectRows(string tablename, string filter)
		{
			return SelectRows(tablename, filter, null);
		}

		public virtual void UpdateRow(string tablename, string primarycolumnname, object primaryvalue, params Data[] values)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "UPDATE " + QuoteTablename(tablename) + " SET ";
			foreach(Data col in values)
				cmd.CommandText += QuoteColumnname(col.Name) + " = " + AddParameter(cmd, col.Value) + ",";

			cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 2);
			cmd.CommandText += " WHERE " + QuoteColumnname(primarycolumnname) + " = " + AddParameter(cmd, primaryvalue);

			try
			{
				int r = cmd.ExecuteNonQuery();
				if (r != 1)
					throw new Exception("Row update was expected to update 1 row, but updated: " + r.ToString());
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't update row (" + primaryvalue.ToString() + ") from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public virtual void InsertRow(string tablename, params Data[] values)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "INSERT INTO " + QuoteTablename(tablename) + " (";
			foreach(Data col in values)
				cmd.CommandText += QuoteColumnname(col.Name) + ", ";
			cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 2)+ ") VALUES (";
			foreach(Data col in values)
				cmd.CommandText += AddParameter(cmd, col.Value) + ", ";
			cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 2) + ")";
			try
			{
				int r = cmd.ExecuteNonQuery();
				if (r != 1)
					throw new Exception("The insert was expected to update 1 row, but updated: " + r.ToString());
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't insert row in table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public virtual Data[] GetStructure(string sql)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			IDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = sql;
			try
			{
				IDataReader dr = cmd.ExecuteReader();
				Data[] ret = new Data[dr.FieldCount];
				for(int i = 0; i < dr.FieldCount; i++)
					ret[i] = new Data(dr.GetName(i), null, dr.GetFieldType(i));
				dr.Close();
				return ret;
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't get structure from sql (" + sql + ")\nError: " + ex.Message);
			}
		}

		public virtual Data[] GetTableStructure(string tablename)
		{
			return GetStructure("SELECT * FROM [" + tablename + "] WHERE 1 = 0");
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
	}
}
