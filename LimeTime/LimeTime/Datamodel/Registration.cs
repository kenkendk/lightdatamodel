/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\Dokumenter\LightDatamodel\LimeTime\LimeTime\Datamodel\LimeTime.sqlite;" />
/// <type>Table</type>
/// <namespace>LimeTime.Datamodel</namespace>
/// <name>Registration</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace LimeTime.Datamodel
{

	[DatabaseTable("Registration")]
	public partial class Registration : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = long.MinValue;
		[DatabaseField("Time")]
		private System.DateTime m_Time = new System.DateTime(1, 1, 1);
		[Relation("RegistrationProject", typeof(Project), "ID"), DatabaseField("ProjectID")]
		private System.Int64 m_ProjectID = long.MinValue;
		[DatabaseField("Note")]
		private System.String m_Note = "";
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.DateTime Time
		{
			get{return m_Time;}
			set{object oldvalue = m_Time;OnBeforeDataChange(this, "Time", oldvalue, value);m_Time = value;OnAfterDataChange(this, "Time", oldvalue, value);}
		}

		public System.Int64 ProjectID
		{
			get{return m_ProjectID;}
			set{object oldvalue = m_ProjectID;OnBeforeDataChange(this, "ProjectID", oldvalue, value);m_ProjectID = value;OnAfterDataChange(this, "ProjectID", oldvalue, value);}
		}

		public System.String Note
		{
			get{return m_Note;}
			set{object oldvalue = m_Note;OnBeforeDataChange(this, "Note", oldvalue, value);m_Note = value;OnAfterDataChange(this, "Note", oldvalue, value);}
		}

#endregion

#region " referenced properties "

		[Affects(typeof(Project))]
		public Project Project
		{
			get{ return ((DataFetcherWithRelations)m_dataparent).GetRelatedObject<Project>("RegistrationProject", this); }
			set{ ((DataFetcherWithRelations)m_dataparent).SetRelatedObject("RegistrationProject", this, value); }
		}

#endregion

	}

}