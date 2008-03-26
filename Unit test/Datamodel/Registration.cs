#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>Table</type>
/// <namespace>UnitTest</namespace>
/// <name>Registration</name>
/// <sql></sql>
/// </metadata>

namespace UnitTest
{

    public class Registration : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.Boolean m_ActiveAcknowledge;
		private System.Int64 m_NoteID;
		private System.Int64 m_ProjectID;
		private System.DateTime m_Time;
#endregion

#region " unique value "

		public override object UniqueValue {get{return ID;}}
		public override string UniqueColumn {get{return "ID";}}
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataWrite(this, "ID", oldvalue, value);m_ID = value;OnAfterDataWrite(this, "ID", oldvalue, value);}
		}

		public System.Boolean ActiveAcknowledge
		{
			get{return m_ActiveAcknowledge;}
			set{object oldvalue = m_ActiveAcknowledge;OnBeforeDataWrite(this, "ActiveAcknowledge", oldvalue, value);m_ActiveAcknowledge = value;OnAfterDataWrite(this, "ActiveAcknowledge", oldvalue, value);}
		}

		public System.Int64 NoteID
		{
			get{return m_NoteID;}
			set{object oldvalue = m_NoteID;OnBeforeDataWrite(this, "NoteID", oldvalue, value);m_NoteID = value;OnAfterDataWrite(this, "NoteID", oldvalue, value);}
		}

		public System.Int64 ProjectID
		{
			get{return m_ProjectID;}
			set{object oldvalue = m_ProjectID;OnBeforeDataWrite(this, "ProjectID", oldvalue, value);m_ProjectID = value;OnAfterDataWrite(this, "ProjectID", oldvalue, value);}
		}

		public System.DateTime Time
		{
			get{return m_Time;}
			set{object oldvalue = m_Time;OnBeforeDataWrite(this, "Time", oldvalue, value);m_Time = value;OnAfterDataWrite(this, "Time", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

#endregion

	}


}