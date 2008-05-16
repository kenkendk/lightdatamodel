/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>ManyToMany</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class ManyToMany : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID = 0;
		private System.Int64 m_LeftID = 0;
		private System.Int64 m_RightID = 0;
#endregion

#region " unique value "

		public override object UniqueValue {get{return m_ID;}}
		public override string UniqueColumn {get{return "ID";}}
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