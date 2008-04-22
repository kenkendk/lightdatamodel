/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.AccessDataProvider" connectionstring="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;" />
/// <type>Table</type>
/// <namespace>Datamodel.TestDB</namespace>
/// <name>CompanyVisits</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.TestDB
{

	public partial class CompanyVisits : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		private System.Int32 m_ID = 0;
		private System.Int32 m_CompaniesID = 0;
		private System.Int32 m_UserAdressesID = 0;
		private System.DateTime m_VisitDate = new System.DateTime(1, 1, 1);
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

		public System.Int32 CompaniesID
		{
			get{return m_CompaniesID;}
			set{object oldvalue = m_CompaniesID;OnBeforeDataChange(this, "CompaniesID", oldvalue, value);m_CompaniesID = value;OnAfterDataChange(this, "CompaniesID", oldvalue, value);}
		}

		public System.Int32 UserAdressesID
		{
			get{return m_UserAdressesID;}
			set{object oldvalue = m_UserAdressesID;OnBeforeDataChange(this, "UserAdressesID", oldvalue, value);m_UserAdressesID = value;OnAfterDataChange(this, "UserAdressesID", oldvalue, value);}
		}

		public System.DateTime VisitDate
		{
			get{return m_VisitDate;}
			set{object oldvalue = m_VisitDate;OnBeforeDataChange(this, "VisitDate", oldvalue, value);m_VisitDate = value;OnAfterDataChange(this, "VisitDate", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}