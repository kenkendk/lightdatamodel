/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\workspace\Ny version2 LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>Registration</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{

	[DatabaseTable("Registration")]
	public partial class Registration : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = rnd.Next(int.MinValue, -1);
		[DatabaseField("ActiveAcknowledge")]
		private System.Boolean m_ActiveAcknowledge = false;
		[DatabaseField("NoteID")]
		private System.Int64 m_NoteID = long.MinValue;
		[DatabaseField("ProjectID")]
		private System.Int64 m_ProjectID = long.MinValue;
		[DatabaseField("Time")]
		private System.DateTime m_Time = new System.DateTime(1, 1, 1);
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.Boolean ActiveAcknowledge
		{
			get{return m_ActiveAcknowledge;}
			set{object oldvalue = m_ActiveAcknowledge;OnBeforeDataChange(this, "ActiveAcknowledge", oldvalue, value);m_ActiveAcknowledge = value;OnAfterDataChange(this, "ActiveAcknowledge", oldvalue, value);}
		}

		public System.Int64 NoteID
		{
			get{return m_NoteID;}
			set{object oldvalue = m_NoteID;OnBeforeDataChange(this, "NoteID", oldvalue, value);m_NoteID = value;OnAfterDataChange(this, "NoteID", oldvalue, value);}
		}

		public System.Int64 ProjectID
		{
			get{return m_ProjectID;}
			set{object oldvalue = m_ProjectID;OnBeforeDataChange(this, "ProjectID", oldvalue, value);m_ProjectID = value;OnAfterDataChange(this, "ProjectID", oldvalue, value);}
		}

		public System.DateTime Time
		{
			get{return m_Time;}
			set{object oldvalue = m_Time;OnBeforeDataChange(this, "Time", oldvalue, value);m_Time = value;OnAfterDataChange(this, "Time", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}