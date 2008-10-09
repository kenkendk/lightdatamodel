/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.AccessDataProvider" connectionstring="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;" />
/// <type>Table</type>
/// <namespace>Datamodel.TestDB</namespace>
/// <name>CompanyVisits</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;

namespace Datamodel.TestDB
{

	[DatabaseTable("CompanyVisits")]
	public partial class CompanyVisits : DataClassBase
	{

#region " private members "

		[PrimaryKey(), DatabaseField("ID")]
		private System.Int32 m_ID = rnd.Next(int.MinValue, -1);
		[DatabaseField("CompaniesID")]
		private System.Int32 m_CompaniesID = 0;
		[DatabaseField("UserAdressesID")]
		private System.Int32 m_UserAdressesID = 0;
		[DatabaseField("VisitDate")]
		private System.DateTime m_VisitDate = new System.DateTime(1, 1, 1);
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