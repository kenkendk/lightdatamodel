/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\workspace\Ny version2 LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>Note</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{

	[DatabaseTable("Note")]
	public partial class Note : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, Relation("c3a06cc7-9649-4cb9-beca-db6ad822cf94", typeof(Project), "CurrentTaskNoteID", "TaskNotes", "CurrentTaskNote"), Relation("227fe561-039d-4337-b2b7-67ed95f32637", typeof(Project), "ProjectNoteID", "ProjectNotes", "ProjectNote"), DatabaseField("ID")]
		private System.Int64 m_ID = rnd.Next(int.MinValue, -1);
		[DatabaseField("NoteText")]
		private System.String m_NoteText = "";
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

		private System.Collections.Generic.IList<Project> m_TaskNotes;
		[Affects(typeof(Project))]
		public System.Collections.Generic.IList<Project> TaskNotes
		{
			get
			{
				if (m_TaskNotes == null)
					m_TaskNotes = ((DataFetcherWithRelations)m_dataparent).GetReferenceCollection<Project>("c3a06cc7-9649-4cb9-beca-db6ad822cf94", this);
				return m_TaskNotes;
			}
		}

		private System.Collections.Generic.IList<Project> m_ProjectNotes;
		[Affects(typeof(Project))]
		public System.Collections.Generic.IList<Project> ProjectNotes
		{
			get
			{
				if (m_ProjectNotes == null)
					m_ProjectNotes = ((DataFetcherWithRelations)m_dataparent).GetReferenceCollection<Project>("227fe561-039d-4337-b2b7-67ed95f32637", this);
				return m_ProjectNotes;
			}
		}

#endregion

	}

}