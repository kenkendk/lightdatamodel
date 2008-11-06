/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\workspace\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>TableWithNoAutoincremetion</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{

	[DatabaseTable("TableWithNoAutoincremetion")]
	public partial class TableWithNoAutoincremetion : DataClassBase
	{

#region " private members "

		[PrimaryKey, DatabaseField("ID")]
		private System.String m_ID = "";
		[DatabaseField("Meh")]
		private System.Int64 m_Meh = long.MinValue;
#endregion

#region " properties "

		public System.String ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.Int64 Meh
		{
			get{return m_Meh;}
			set{object oldvalue = m_Meh;OnBeforeDataChange(this, "Meh", oldvalue, value);m_Meh = value;OnAfterDataChange(this, "Meh", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}