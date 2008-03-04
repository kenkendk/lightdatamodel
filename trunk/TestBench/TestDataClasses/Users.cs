#region " Unsynchronized Includes "

	//Don't put any region sections in here
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

	public class Users : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		private System.Int32 m_ID;
		private System.String m_Name;
		private System.DateTime m_CreatedDate;
		private System.Int32 m_UserAdressesID;
#endregion

#region " unique value "

		public override object UniqueValue {get{return m_ID;}}
		public override string UniqueColumn {get{return "ID";}}
#endregion

#region " properties "

		public System.Int32 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataWrite(this, "ID", oldvalue, value);m_ID = value;OnAfterDataWrite(this, "ID", oldvalue, value);}
		}

		public System.String Name
		{
			get{return m_Name;}
			set{object oldvalue = m_Name;OnBeforeDataWrite(this, "Name", oldvalue, value);m_Name = value;OnAfterDataWrite(this, "Name", oldvalue, value);}
		}

		public System.DateTime CreatedDate
		{
			get{return m_CreatedDate;}
			set{object oldvalue = m_CreatedDate;OnBeforeDataWrite(this, "CreatedDate", oldvalue, value);m_CreatedDate = value;OnAfterDataWrite(this, "CreatedDate", oldvalue, value);}
		}

		public System.Int32 UserAdressesID
		{
			get{return m_UserAdressesID;}
			set{object oldvalue = m_UserAdressesID;OnBeforeDataWrite(this, "UserAdressesID", oldvalue, value);m_UserAdressesID = value;OnAfterDataWrite(this, "UserAdressesID", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

		public UserAdresses Address
		{
			get
			{
				return m_dataparent.GetObjectById<UserAdresses>(m_UserAdressesID);
			}
			set
			{
				m_UserAdressesID = value.ID;
			}
		}

#endregion

	}

}