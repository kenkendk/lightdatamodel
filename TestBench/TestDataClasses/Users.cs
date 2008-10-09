/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.AccessDataProvider" connectionstring="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;" />
/// <type>Table</type>
/// <namespace>Datamodel.TestDB</namespace>
/// <name>Users</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;

namespace Datamodel.TestDB
{

	[DatabaseTable("Users")]
	public partial class Users : DataClassBase
	{

#region " private members "

		[PrimaryKey(), DatabaseField("ID")]
		private System.Int32 m_ID = rnd.Next(int.MinValue, -1);
		[DatabaseField("Name")]
		private System.String m_Name = "Ny bruger";
		[DatabaseField("CreatedDate")]
		private System.DateTime m_CreatedDate = new System.DateTime(1, 1, 1);
		[DatabaseField("UserAdressesID")]
		private System.Int32 m_UserAdressesID = 0;
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
			set{value = value != null && ((string)value).Length > 50 ? ((string)value).Substring(0, 50) : value;object oldvalue = m_Name;OnBeforeDataChange(this, "Name", oldvalue, value);m_Name = value;OnAfterDataChange(this, "Name", oldvalue, value);}
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