/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>Project</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class Project : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.Int64 m_CurrentTaskNoteID;
		private System.Int64 m_ProjectNoteID;
		private System.String m_Title;
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

#region " referenced properties "

		public Note ProjectNote
		{
			get{ return base.RelationManager.GetReferenceObject<Note>(this, "ProjectNote"); }
			set{ base.RelationManager.SetReferenceObject<Note>(this, "ProjectNote", value); }
		}

		public Note CurrentTaskNote
		{
			get{ return base.RelationManager.GetReferenceObject<Note>(this, "CurrentTaskNote"); }
			set{ base.RelationManager.SetReferenceObject<Note>(this, "CurrentTaskNote", value); }
		}

#endregion

	}

}