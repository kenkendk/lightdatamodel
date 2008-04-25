/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>RightSide</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class RightSide : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID = -9223372036854775808;
		private System.String m_Text = "";
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

		public System.String Text
		{
			get{return m_Text;}
			set{object oldvalue = m_Text;OnBeforeDataChange(this, "Text", oldvalue, value);m_Text = value;OnAfterDataChange(this, "Text", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}