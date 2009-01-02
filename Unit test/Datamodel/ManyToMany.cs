/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\workspace\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>ManyToMany</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{

	[DatabaseTable("ManyToMany")]
	public partial class ManyToMany : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = long.MinValue;
		[DatabaseField("LeftID")]
		private System.Int64 m_LeftID = long.MinValue;
		[DatabaseField("RightID")]
		private System.Int64 m_RightID = long.MinValue;
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.Int64 LeftID
		{
			get{return m_LeftID;}
			set{object oldvalue = m_LeftID;OnBeforeDataChange(this, "LeftID", oldvalue, value);m_LeftID = value;OnAfterDataChange(this, "LeftID", oldvalue, value);}
		}

		public System.Int64 RightID
		{
			get{return m_RightID;}
			set{object oldvalue = m_RightID;OnBeforeDataChange(this, "RightID", oldvalue, value);m_RightID = value;OnAfterDataChange(this, "RightID", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}