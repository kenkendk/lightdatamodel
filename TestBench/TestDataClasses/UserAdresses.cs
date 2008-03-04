#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

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

	public class UserAdresses : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		private System.Int32 m_ID;
		private System.String m_RoadName;
		private System.String m_HouseNumber;
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

		public System.String RoadName
		{
			get{return m_RoadName;}
			set{object oldvalue = m_RoadName;OnBeforeDataWrite(this, "RoadName", oldvalue, value);m_RoadName = value;OnAfterDataWrite(this, "RoadName", oldvalue, value);}
		}

		public System.String HouseNumber
		{
			get{return m_HouseNumber;}
			set{object oldvalue = m_HouseNumber;OnBeforeDataWrite(this, "HouseNumber", oldvalue, value);m_HouseNumber = value;OnAfterDataWrite(this, "HouseNumber", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

#endregion

	}

}