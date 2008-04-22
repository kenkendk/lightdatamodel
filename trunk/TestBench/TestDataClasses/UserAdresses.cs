/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.AccessDataProvider" connectionstring="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;" />
/// <type>Table</type>
/// <namespace>Datamodel.TestDB</namespace>
/// <name>UserAdresses</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.TestDB
{

	public partial class UserAdresses : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		private System.Int32 m_ID = 0;
		private System.String m_RoadName = "Vejnavn";
		private System.String m_HouseNumber = "";
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

		public System.String RoadName
		{
			get{return m_RoadName;}
			set{object oldvalue = m_RoadName;OnBeforeDataChange(this, "RoadName", oldvalue, value);m_RoadName = value;OnAfterDataChange(this, "RoadName", oldvalue, value);}
		}

		public System.String HouseNumber
		{
			get{return m_HouseNumber;}
			set{object oldvalue = m_HouseNumber;OnBeforeDataChange(this, "HouseNumber", oldvalue, value);m_HouseNumber = value;OnAfterDataChange(this, "HouseNumber", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}