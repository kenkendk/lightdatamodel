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
/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.AccessDataProvider" connectionstring="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;" />
/// <type>Table</type>
/// <namespace>Datamodel.TestDB</namespace>
/// <name>Users</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.TestDB
{

	public partial class Users : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		private System.Int32 m_ID = 0;
		private System.String m_Name = "Ny bruger";
		private System.DateTime m_CreatedDate = new System.DateTime(1, 1, 1);
		private System.Int32 m_UserAdressesID = 0;
#endregion

#region " unique value "

		public override object UniqueValue {get{return m_ID;}}
		public override string UniqueColumn {get{return "ID";}}
#endregion

#region " properties "

		public System.Int32 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.String Name
		{
			get{return m_Name;}
			set{object oldvalue = m_Name;OnBeforeDataChange(this, "Name", oldvalue, value);m_Name = value;OnAfterDataChange(this, "Name", oldvalue, value);}
		}

		public System.DateTime CreatedDate
		{
			get{return m_CreatedDate;}
			set{object oldvalue = m_CreatedDate;OnBeforeDataChange(this, "CreatedDate", oldvalue, value);m_CreatedDate = value;OnAfterDataChange(this, "CreatedDate", oldvalue, value);}
		}

		public System.Int32 UserAdressesID
		{
			get{return m_UserAdressesID;}
			set{object oldvalue = m_UserAdressesID;OnBeforeDataChange(this, "UserAdressesID", oldvalue, value);m_UserAdressesID = value;OnAfterDataChange(this, "UserAdressesID", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}