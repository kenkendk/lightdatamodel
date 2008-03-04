#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

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

	public class CompanyVisits : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		private System.Int32 m_ID;
		private System.Int32 m_CompaniesID;
		private System.Int32 m_UserAdressesID;
		private System.DateTime m_VisitDate;
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

		public System.Int32 CompaniesID
		{
			get{return m_CompaniesID;}
			set{object oldvalue = m_CompaniesID;OnBeforeDataWrite(this, "CompaniesID", oldvalue, value);m_CompaniesID = value;OnAfterDataWrite(this, "CompaniesID", oldvalue, value);}
		}

		public System.Int32 UserAdressesID
		{
			get{return m_UserAdressesID;}
			set{object oldvalue = m_UserAdressesID;OnBeforeDataWrite(this, "UserAdressesID", oldvalue, value);m_UserAdressesID = value;OnAfterDataWrite(this, "UserAdressesID", oldvalue, value);}
		}

		public System.DateTime VisitDate
		{
			get{return m_VisitDate;}
			set{object oldvalue = m_VisitDate;OnBeforeDataWrite(this, "VisitDate", oldvalue, value);m_VisitDate = value;OnAfterDataWrite(this, "VisitDate", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

#endregion

	}

}