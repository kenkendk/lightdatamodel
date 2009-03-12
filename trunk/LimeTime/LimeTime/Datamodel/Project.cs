/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\Dokumenter\LightDatamodel\LimeTime\LimeTime\Datamodel\LimeTime.sqlite;" />
/// <type>Table</type>
/// <namespace>LimeTime.Datamodel</namespace>
/// <name>Project</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace LimeTime.Datamodel
{

	[DatabaseTable("Project")]
	public partial class Project : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, Relation("RegistrationProject", typeof(Registration), "ProjectID", false), DatabaseField("ID")]
		private System.Int64 m_ID = long.MinValue;
		[DatabaseField("Title")]
		private System.String m_Title = "";
		[DatabaseField("Type")]
		private System.String m_Type = "";
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.String Title
		{
			get{return m_Title;}
			set{object oldvalue = m_Title;OnBeforeDataChange(this, "Title", oldvalue, value);m_Title = value;OnAfterDataChange(this, "Title", oldvalue, value);}
		}

		public System.String Type
		{
			get{return m_Type;}
			set{object oldvalue = m_Type;OnBeforeDataChange(this, "Type", oldvalue, value);m_Type = value;OnAfterDataChange(this, "Type", oldvalue, value);}
		}

#endregion

#region " referenced properties "

		[Affects(typeof(Registration))]
		public System.Collections.Generic.IList<Registration> Registrations
		{
			get
			{
				return ((DataFetcherWithRelations)m_dataparent).GetRelatedObjects<Registration>("RegistrationProject", this);
			}
		}

#endregion

	}

}