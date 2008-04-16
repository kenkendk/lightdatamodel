#region " Unsynchronized Includes "

	//Don't put any region sections in here
using System.Collections.Generic;

#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>Table</type>
/// <namespace>UnitTest</namespace>
/// <name>Note</name>
/// <sql></sql>
/// </metadata>

namespace UnitTest
{

	public class Note : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID;
		private System.String m_NoteText;
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

		public System.String NoteText
		{
			get{return m_NoteText;}
			set{object oldvalue = m_NoteText;OnBeforeDataWrite(this, "NoteText", oldvalue, value);m_NoteText = value;OnAfterDataWrite(this, "NoteText", oldvalue, value);}
		}

#endregion

#region " Unsynchronized Custom Code Region "

    //Don't put any region sections in here
	public IList<Project> ProjectNotes
	{
		get { return new System.Data.LightDatamodel.SyncCollectionBase<Project>(this, "ProjectNote", "ProjectNoteID"); }
	}

	public IList<Project> TaskNotes
	{
        get { return new System.Data.LightDatamodel.SyncCollectionBase<Project>(this, "CurrentTaskNote", "CurrentTaskNoteID"); }
	}

    public System.Guid Guid { get { return this.RelationManager.GetGuidForObject(this); } }
    public bool ExistsInDB { get { return this.RelationManager.ExistsInDb(this); } }

#endregion

	}

}