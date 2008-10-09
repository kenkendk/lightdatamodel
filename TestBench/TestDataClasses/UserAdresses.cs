/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.AccessDataProvider" connectionstring="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;" />
/// <type>Table</type>
/// <namespace>Datamodel.TestDB</namespace>
/// <name>UserAdresses</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;

namespace Datamodel.TestDB
{

	[DatabaseTable("UserAdresses")]
	public partial class UserAdresses : DataClassBase
	{

#region " private members "

		[PrimaryKey(), DatabaseField("ID")]
		private System.Int32 m_ID = rnd.Next(int.MinValue, -1);
		[DatabaseField("RoadName")]
		private System.String m_RoadName = "Vejnavn";
		[DatabaseField("HouseNumber")]
		private System.String m_HouseNumber = "";
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
			set{value = value != null && ((string)value).Length > 50 ? ((string)value).Substring(0, 50) : value;object oldvalue = m_RoadName;OnBeforeDataChange(this, "RoadName", oldvalue, value);m_RoadName = value;OnAfterDataChange(this, "RoadName", oldvalue, value);}
		}

		public System.String HouseNumber
		{
			get{return m_HouseNumber;}
			set{value = value != null && ((string)value).Length > 8 ? ((string)value).Substring(0, 8) : value;object oldvalue = m_HouseNumber;OnBeforeDataChange(this, "HouseNumber", oldvalue, value);m_HouseNumber = value;OnAfterDataChange(this, "HouseNumber", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}