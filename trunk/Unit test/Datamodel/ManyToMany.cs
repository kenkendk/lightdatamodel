#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>Table</type>
/// <namespace>UnitTest</namespace>
/// <name>ManyToMany</name>
/// <sql></sql>
/// </metadata>

namespace UnitTest
{

	public class ManyToMany : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.Int64 m_LeftID;
		private System.Int64 m_RightID;
#endregion

#region " unique value "

		public override object UniqueValue {get{return m_ID;}}
		public override string UniqueColumn {get{return "ID";}}
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataWrite(this, "ID", oldvalue, value);m_ID = value;OnAfterDataWrite(this, "ID", oldvalue, value);}
		}

		public System.Int64 LeftID
		{
			get{return m_LeftID;}
			set{object oldvalue = m_LeftID;OnBeforeDataWrite(this, "LeftID", oldvalue, value);m_LeftID = value;OnAfterDataWrite(this, "LeftID", oldvalue, value);}
		}

		public System.Int64 RightID
		{
			get{return m_RightID;}
			set{object oldvalue = m_RightID;OnBeforeDataWrite(this, "RightID", oldvalue, value);m_RightID = value;OnAfterDataWrite(this, "RightID", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

#endregion

	}

}