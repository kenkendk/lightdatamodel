/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\workspace\Ny version2 LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>Project</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{

	[DatabaseTable("Project")]
	public partial class Project : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = rnd.Next(int.MinValue, -1);
		[Relation("c3a06cc7-9649-4cb9-beca-db6ad822cf94", typeof(Note), "ID", "CurrentTaskNote", "TaskNotes"), DatabaseField("CurrentTaskNoteID")]
		private System.Int64 m_CurrentTaskNoteID = long.MinValue;
		[Relation("227fe561-039d-4337-b2b7-67ed95f32637", typeof(Note), "ID", "ProjectNote", "ProjectNotes"), DatabaseField("ProjectNoteID")]
		private System.Int64 m_ProjectNoteID = long.MinValue;
		[DatabaseField("Title")]
		private System.String m_Title = "";
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

		[Affects(typeof(Note))]
		public Note CurrentTaskNote
		{
			get{ return ((DataFetcherWithRelations)m_dataparent).GetReferenceObject<Note>("c3a06cc7-9649-4cb9-beca-db6ad822cf94", this); }
			set{ ((DataFetcherWithRelations)m_dataparent).SetReferenceObject("c3a06cc7-9649-4cb9-beca-db6ad822cf94", this, value); }
		}

		[Affects(typeof(Note))]
		public Note ProjectNote
		{
			get{ return ((DataFetcherWithRelations)m_dataparent).GetReferenceObject<Note>("227fe561-039d-4337-b2b7-67ed95f32637", this); }
			set{ ((DataFetcherWithRelations)m_dataparent).SetReferenceObject("227fe561-039d-4337-b2b7-67ed95f32637", this, value); }
		}

#endregion

	}

}