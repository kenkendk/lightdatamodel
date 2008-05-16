/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>Registration</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class Registration : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID = 0;
		private System.Boolean m_ActiveAcknowledge = false;
		private System.Int64 m_NoteID = 0;
		private System.Int64 m_ProjectID = 0;
		private System.DateTime m_Time = new System.DateTime(1, 1, 1);
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

		public System.Boolean ActiveAcknowledge
		{
			get{return m_ActiveAcknowledge;}
			set{object oldvalue = m_ActiveAcknowledge;OnBeforeDataChange(this, "ActiveAcknowledge", oldvalue, value);m_ActiveAcknowledge = value;OnAfterDataChange(this, "ActiveAcknowledge", oldvalue, value);}
		}

		public System.Int64 NoteID
		{
			get{return m_NoteID;}
			set{object oldvalue = m_NoteID;OnBeforeDataChange(this, "NoteID", oldvalue, value);m_NoteID = value;OnAfterDataChange(this, "NoteID", oldvalue, value);}
		}

		public System.Int64 ProjectID
		{
			get{return m_ProjectID;}
			set{object oldvalue = m_ProjectID;OnBeforeDataChange(this, "ProjectID", oldvalue, value);m_ProjectID = value;OnAfterDataChange(this, "ProjectID", oldvalue, value);}
		}

		public System.DateTime Time
		{
			get{return m_Time;}
			set{object oldvalue = m_Time;OnBeforeDataChange(this, "Time", oldvalue, value);m_Time = value;OnAfterDataChange(this, "Time", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}