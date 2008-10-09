/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.AccessDataProvider" connectionstring="Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;" />
/// <type>Table</type>
/// <namespace>Datamodel.TestDB</namespace>
/// <name>Companies</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;

namespace Datamodel.TestDB
{

	[DatabaseTable("Companies")]
	public partial class Companies : DataClassBase
	{

#region " private members "

		[PrimaryKey(), DatabaseField("ID")]
		private System.Int32 m_ID = rnd.Next(int.MinValue, -1);
		[DatabaseField("Name")]
		private System.String m_Name = "";
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

#endregion

#region " referenced properties "

#endregion

	}

}