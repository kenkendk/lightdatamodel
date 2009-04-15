/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\Dokumenter\LightDatamodel\LimeTime\LimeTime\Datamodel\LimeTime.sqlite;" />
/// <type>Table</type>
/// <namespace>LimeTime.Datamodel</namespace>
/// <name>RecentEntry</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace LimeTime.Datamodel
{

	[DatabaseTable("RecentEntry")]
	public partial class RecentEntry : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = long.MinValue;
		[DatabaseField("Time")]
		private System.DateTime m_Time = new System.DateTime(1, 1, 1);
		[Relation("RecentEntryProject", typeof(Project), "ID"), DatabaseField("ProjectID")]
		private System.Int64 m_ProjectID = long.MinValue;
		[DatabaseField("TypedText")]
		private System.String m_TypedText = "";
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

		public System.String TypedText
		{
			get{return m_TypedText;}
			set{object oldvalue = m_TypedText;OnBeforeDataChange(this, "TypedText", oldvalue, value);m_TypedText = value;OnAfterDataChange(this, "TypedText", oldvalue, value);}
		}

#endregion

#region " referenced properties "

		[Affects(typeof(Project))]
		public Project Project
		{
			get{ return ((DataFetcherWithRelations)m_dataparent).GetRelatedObject<Project>("RecentEntryProject", this); }
			set{ ((DataFetcherWithRelations)m_dataparent).SetRelatedObject("RecentEntryProject", this, value); }
		}

#endregion

	}

}