/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\workspace\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>SpecificEvent</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{

	[DatabaseTable("SpecificEvent")]
	public partial class SpecificEvent : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = long.MinValue;
		[DatabaseField("Data")]
		private System.String m_Data = "";
		[DatabaseField("RegistrationID")]
		private System.Int64 m_RegistrationID = long.MinValue;
		[DatabaseField("Time")]
		private System.DateTime m_Time = new System.DateTime(1, 1, 1);
		[DatabaseField("EventType")]
		private System.Int64 m_EventType = long.MinValue;
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.String Data
		{
			get{return m_Data;}
			set{value = value != null && ((string)value).Length > 255 ? ((string)value).Substring(0, 255) : value;object oldvalue = m_Data;OnBeforeDataChange(this, "Data", oldvalue, value);m_Data = value;OnAfterDataChange(this, "Data", oldvalue, value);}
		}

		public System.Int64 RegistrationID
		{
			get{return m_RegistrationID;}
			set{object oldvalue = m_RegistrationID;OnBeforeDataChange(this, "RegistrationID", oldvalue, value);m_RegistrationID = value;OnAfterDataChange(this, "RegistrationID", oldvalue, value);}
		}

		public System.DateTime Time
		{
			get{return m_Time;}
			set{object oldvalue = m_Time;OnBeforeDataChange(this, "Time", oldvalue, value);m_Time = value;OnAfterDataChange(this, "Time", oldvalue, value);}
		}

		public System.Int64 EventType
		{
			get{return m_EventType;}
			set{object oldvalue = m_EventType;OnBeforeDataChange(this, "EventType", oldvalue, value);m_EventType = value;OnAfterDataChange(this, "EventType", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}