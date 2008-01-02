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

namespace System.Data.LightDatamodel
{
	public class AccessDataProvider : IDataProvider
	{
		private OleDbConnection m_connection = new OleDbConnection();

		public AccessDataProvider(string connectionstring)
		{
			m_connection.ConnectionString = connectionstring;
		}

		public AccessDataProvider() : this("")
		{

		}

		#region IDataProvider Members

		public object GetNullValue(Type type)
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


		public string ConnectionString
		{
			get{return m_connection.ConnectionString;}
			set{m_connection.ConnectionString = value;}
		}

		public void DeleteRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			OleDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "DELETE FROM [" + tablename + "] WHERE " + primarycolumnname + " = " + FormatValue(primaryvalue);
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't delete row (" + primaryvalue.ToString() + ") from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public Data[] SelectRow(string tablename, string primarycolumnname, object primaryvalue)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			OleDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM [" + tablename + "] WHERE " + primarycolumnname + " = " + FormatValue(primaryvalue);
			try
			{
				OleDbDataReader dr = cmd.ExecuteReader();
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

		public Data[][] SelectRows(string tablename, QueryModel.Operation operation)
		{
			ArrayList values = new ArrayList();
			string filter = EvalTree(operation, values);
			return SelectRows(tablename, filter, (object[])values.ToArray(typeof(object)));
		}

		private string TranslateOperator(QueryModel.Operators opr)
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

		private string EvalTree(QueryModel.OperationOrParameter opm, ArrayList cmds)
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

		public Data[][] SelectRows(string tablename, string filter, object[] values)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			OleDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "SELECT * FROM [" + tablename + "]";
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
				OleDbDataReader dr = cmd.ExecuteReader();
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

		public Data[][] SelectRows(string tablename, string filter)
		{
			return SelectRows(tablename, filter, null);
		}

		public void UpdateRow(string tablename, string primarycolumnname, object primaryvalue, params Data[] values)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			OleDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "UPDATE [" + tablename + "] SET ";
			foreach(Data col in values)
				cmd.CommandText += col.Name + " = " + FormatValue(col.Value) + ", ";
			cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 2);
			cmd.CommandText += " WHERE " + primarycolumnname + " = " + FormatValue(primaryvalue);
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't update row (" + primaryvalue.ToString() + ") from table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public void InsertRow(string tablename, params Data[] values)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			OleDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = "INSERT INTO [" + tablename + "] (";
			foreach(Data col in values)
				cmd.CommandText += col.Name + ", ";
			cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 2)+ ") VALUES (";
			foreach(Data col in values)
				cmd.CommandText += FormatValue(col.Value) + ", ";
			cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 2) + ")";
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't insert row in table \"" + tablename + "\"\nError: " + ex.Message);
			}
		}

		public string GetPrimaryKey(string tablename)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			DataTable primsch = m_connection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new Object[] {null, null, tablename});
			if( primsch == null || primsch.Rows.Count == 0) return "";
			return primsch.Rows[0].ItemArray[3].ToString();
		}

		public string[] GetTablenames()
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			DataTable tablesschema = m_connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {null, null, null, "TABLE"});
			string[] tablenames = new string[tablesschema.Rows.Count];
			for(int i = 0; i< tablenames.Length; i++)
				tablenames[i] = tablesschema.Rows[i][2].ToString();
			return tablenames;
		}

		public Data[] GetStructure(string sql)
		{
			if(m_connection.State != ConnectionState.Open) m_connection.Open();
			OleDbCommand cmd = m_connection.CreateCommand();
			cmd.CommandText = sql;
			try
			{
				OleDbDataReader dr = cmd.ExecuteReader();
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

		public Data[] GetTableStructure(string tablename)
		{
			return GetStructure("SELECT * FROM [" + tablename + "] WHERE 1 = 0");
		}

		public string FormatValue(object value)
		{
			if(value == null || value == GetNullValue(value.GetType())) return "NULL";
			if(value.GetType() == typeof(string)) return "\"" + (string)value + "\"";
			else if(value.GetType() == typeof(int) || value.GetType() == typeof(short)) 
				return ((int)value).ToString();
			else if(value.GetType() == typeof(double) || value.GetType() == typeof(float) || value.GetType() == typeof(decimal))
				return ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
			else if(value.GetType() == typeof(DateTime))
				return ((DateTime)value).ToString();		//prolly need to specify this
			else if(value.GetType() == typeof(DateTime))
				return ((bool)value).ToString().ToUpper();
			else return value.ToString();
		}

		public void Close()
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
