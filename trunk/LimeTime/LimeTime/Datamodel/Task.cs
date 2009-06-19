/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\Dokumenter\LightDatamodel\LimeTime\LimeTime\Datamodel\LimeTime.sqlite;" />
/// <type>Table</type>
/// <namespace>LimeTime.Datamodel</namespace>
/// <name>Task</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace LimeTime.Datamodel
{

	[DatabaseTable("Task")]
	public partial class Task : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = long.MinValue;
		[DatabaseField("Name")]
		private System.String m_Name = "";
		[DatabaseField("Duration")]
		private System.Int64 m_Duration = long.MinValue;
		[DatabaseField("SortOrder")]
		private System.Int64 m_SortOrder = long.MinValue;
		[DatabaseField("Note")]
		private System.String m_Note = "";
		[DatabaseField("StartDate")]
		private System.DateTime m_StartDate = new System.DateTime(1, 1, 1);
		[DatabaseField("EndDate")]
		private System.DateTime m_EndDate = new System.DateTime(1, 1, 1);
		[DatabaseField("Fixed")]
		private System.Boolean m_Fixed = false;
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.String Name
		{
			get{return m_Name;}
			set{object oldvalue = m_Name;OnBeforeDataChange(this, "Name", oldvalue, value);m_Name = value;OnAfterDataChange(this, "Name", oldvalue, value);}
		}

		public System.Int64 Duration
		{
			get{return m_Duration;}
			set{object oldvalue = m_Duration;OnBeforeDataChange(this, "Duration", oldvalue, value);m_Duration = value;OnAfterDataChange(this, "Duration", oldvalue, value);}
		}

		public System.Int64 SortOrder
		{
			get{return m_SortOrder;}
			set{object oldvalue = m_SortOrder;OnBeforeDataChange(this, "SortOrder", oldvalue, value);m_SortOrder = value;OnAfterDataChange(this, "SortOrder", oldvalue, value);}
		}

		public System.String Note
		{
			get{return m_Note;}
			set{object oldvalue = m_Note;OnBeforeDataChange(this, "Note", oldvalue, value);m_Note = value;OnAfterDataChange(this, "Note", oldvalue, value);}
		}

		public System.DateTime StartDate
		{
			get{return m_StartDate;}
			set{object oldvalue = m_StartDate;OnBeforeDataChange(this, "StartDate", oldvalue, value);m_StartDate = value;OnAfterDataChange(this, "StartDate", oldvalue, value);}
		}

		public System.DateTime EndDate
		{
			get{return m_EndDate;}
			set{object oldvalue = m_EndDate;OnBeforeDataChange(this, "EndDate", oldvalue, value);m_EndDate = value;OnAfterDataChange(this, "EndDate", oldvalue, value);}
		}

		public System.Boolean Fixed
		{
			get{return m_Fixed;}
			set{object oldvalue = m_Fixed;OnBeforeDataChange(this, "Fixed", oldvalue, value);m_Fixed = value;OnAfterDataChange(this, "Fixed", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}