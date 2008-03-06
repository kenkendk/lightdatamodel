#region " Unsynchronized Includes "

	//Don't put any region sections in here
using System.Data.LightDatamodel;
#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>Table</type>
/// <namespace>UnitTest</namespace>
/// <name>Project</name>
/// <sql></sql>
/// </metadata>

namespace UnitTest
{

	public class Project : System.Data.LightDatamodel.DataClassExtended
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.Int64 m_CurrentTaskNoteID;
		private System.Int64 m_ProjectNoteID;
		private System.String m_Title;
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

		public System.Int64 CurrentTaskNoteID
		{
			get{return m_CurrentTaskNoteID;}
			set{object oldvalue = m_CurrentTaskNoteID;OnBeforeDataWrite(this, "CurrentTaskNoteID", oldvalue, value);m_CurrentTaskNoteID = value;OnAfterDataWrite(this, "CurrentTaskNoteID", oldvalue, value);}
		}

		public System.Int64 ProjectNoteID
		{
			get{return m_ProjectNoteID;}
			set{object oldvalue = m_ProjectNoteID;OnBeforeDataWrite(this, "ProjectNoteID", oldvalue, value);m_ProjectNoteID = value;OnAfterDataWrite(this, "ProjectNoteID", oldvalue, value);}
		}

		public System.String Title
		{
			get{return m_Title;}
			set{object oldvalue = m_Title;OnBeforeDataWrite(this, "Title", oldvalue, value);m_Title = value;OnAfterDataWrite(this, "Title", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here
		public Note ProjectNote
		{
			get { return (Note)(base.GetReferenceObject("ProjectNote", "m_ProjectNoteID")); }
			set { base.SetReferenceObject("ProjectNote", "m_ProjectNoteID", "ProjectNotes", value); }
		}

		public Note CurrentTaskNote
		{
			get { return (Note)base.GetReferenceObject("CurrentTaskNote", "m_CurrentTaskNoteID"); }
			set { base.SetReferenceObject("CurrentTaskNote", "m_CurrentTaskNoteID", "TaskNotes", value); }
		}

#endregion

	}


}