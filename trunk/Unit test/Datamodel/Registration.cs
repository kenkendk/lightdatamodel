#region Disclaimer / License
// Copyright (C) 2008, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>Registration</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class Registration : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID = 0;
		private System.Boolean m_ActiveAcknowledge = false;
		private System.Int64 m_NoteID = 0;
		private System.Int64 m_ProjectID = 0;
		private System.DateTime m_Time = new System.DateTime(1, 1, 1);
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