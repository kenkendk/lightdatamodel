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

namespace System.Data.LightDatamodel
{
	/// <summary>
	/// All data providers must implement this interface
	/// </summary>
	public interface IDataProvider
	{
		void DeleteRow(string tablename, string primarycolumnname, object primaryvalue);
		Data[] SelectRow(string tablename, string primarycolumnname, object primaryvalue);
		Data[][] SelectRows(string tablename, string filter);
		Data[][] SelectRows(string tablename, string filter, object[] values);
		Data[][] SelectRows(string tablename, QueryModel.Operation operation);
		void UpdateRow(string tablename, string primarycolumnname, object primaryvalue, params Data[] values);
		void InsertRow(string tablename, params Data[] values);
		string GetPrimaryKey(string tablename);
		string[] GetTablenames();
		Data[] GetStructure(string sql);
		Data[] GetTableStructure(string tablename);
		void Close();
		string ConnectionString{get;set;}
		object GetNullValue(Type type);
		object Compute(string tablename, string expression, string filter);

		void BeginTransaction(Guid id);
		void CommitTransaction(Guid id);
		void RollbackTransaction(Guid id);

		object GetLastAutogeneratedValue(string tablename);
	}
}
