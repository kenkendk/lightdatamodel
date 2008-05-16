/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>SpecificEvent</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class SpecificEvent : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID = 0;
		private System.String m_Data = "";
		private System.Int64 m_RegistrationID = 0;
		private System.DateTime m_Time = new System.DateTime(1, 1, 1);
		private System.Int64 m_EventType = -1;
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