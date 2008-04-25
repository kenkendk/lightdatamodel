/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>Note</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class Note : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID = -9223372036854775808;
		private System.String m_NoteText = "";
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

		public System.String NoteText
		{
			get{return m_NoteText;}
			set{object oldvalue = m_NoteText;OnBeforeDataChange(this, "NoteText", oldvalue, value);m_NoteText = value;OnAfterDataChange(this, "NoteText", oldvalue, value);}
		}

#endregion

#region " referenced properties "

		private System.Data.LightDatamodel.SyncCollectionBase<Project> m_ProjectNotes;
		public System.Data.LightDatamodel.SyncCollectionBase<Project> ProjectNotes
		{
			get
			{
				if (m_ProjectNotes == null)
					m_ProjectNotes = base.RelationManager.GetReferenceCollection<Project>(this, "ProjectNote");
				return m_ProjectNotes;
			}
		}

		private System.Data.LightDatamodel.SyncCollectionBase<Project> m_TaskNotes;
		public System.Data.LightDatamodel.SyncCollectionBase<Project> TaskNotes
		{
			get
			{
				if (m_TaskNotes == null)
					m_TaskNotes = base.RelationManager.GetReferenceCollection<Project>(this, "CurrentTaskNote");
				return m_TaskNotes;
			}
		}

#endregion

	}

}