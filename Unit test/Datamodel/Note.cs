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
/// <name>Note</name>
/// <sql></sql>
/// </metadata>

namespace Datamodel.UnitTest
{

	public partial class Note : System.Data.LightDatamodel.DataClassBase
	{

#region " private members "

		[System.Data.LightDatamodel.MemberModifierAutoIncrement()]
		private System.Int64 m_ID = 0;
		private System.String m_NoteText = "";
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

		public System.String NoteText
		{
			get{return m_NoteText;}
			set{object oldvalue = m_NoteText;OnBeforeDataChange(this, "NoteText", oldvalue, value);m_NoteText = value;OnAfterDataChange(this, "NoteText", oldvalue, value);}
		}

#endregion

#region " referenced properties "

		private System.Collections.Generic.IList<Project> m_ProjectNotes;
		public System.Collections.Generic.IList<Project> ProjectNotes
		{
			get
			{
				if (m_ProjectNotes == null)
					m_ProjectNotes = base.RelationManager.GetReferenceCollection<Project>("ProjectNote", this);
				return m_ProjectNotes;
			}
		}

		private System.Collections.Generic.IList<Project> m_TaskNotes;
		public System.Collections.Generic.IList<Project> TaskNotes
		{
			get
			{
				if (m_TaskNotes == null)
					m_TaskNotes = base.RelationManager.GetReferenceCollection<Project>("CurrentTaskNote", this);
				return m_TaskNotes;
			}
		}

#endregion

	}

}