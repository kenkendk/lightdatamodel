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
		private System.Int64 m_ID = 0;
		private System.Int64 m_CurrentTaskNoteID = 0;
		private System.Int64 m_ProjectNoteID = 0;
		private System.String m_Title = "";
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

		public System.Int64 CurrentTaskNoteID
		{
			get{return m_CurrentTaskNoteID;}
			set{object oldvalue = m_CurrentTaskNoteID;OnBeforeDataChange(this, "CurrentTaskNoteID", oldvalue, value);m_CurrentTaskNoteID = value;OnAfterDataChange(this, "CurrentTaskNoteID", oldvalue, value);}
		}

		public System.Int64 ProjectNoteID
		{
			get{return m_ProjectNoteID;}
			set{object oldvalue = m_ProjectNoteID;OnBeforeDataChange(this, "ProjectNoteID", oldvalue, value);m_ProjectNoteID = value;OnAfterDataChange(this, "ProjectNoteID", oldvalue, value);}
		}

		public System.String Title
		{
			get{return m_Title;}
			set{value = value != null && ((string)value).Length > 255 ? ((string)value).Substring(0, 255) : value;object oldvalue = m_Title;OnBeforeDataChange(this, "Title", oldvalue, value);m_Title = value;OnAfterDataChange(this, "Title", oldvalue, value);}
		}

#endregion

#region " referenced properties "

		public Note ProjectNote
		{
			get{ return base.RelationManager.GetReferenceObject<Note>("ProjectNote", this); }
			set{ base.RelationManager.SetReferenceObject<Note>("ProjectNote", this, value); }
		}

		public Note CurrentTaskNote
		{
			get{ return base.RelationManager.GetReferenceObject<Note>("CurrentTaskNote", this); }
			set{ base.RelationManager.SetReferenceObject<Note>("CurrentTaskNote", this, value); }
		}

#endregion

	}

}