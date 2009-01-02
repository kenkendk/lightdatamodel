/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=D:\workspace\LightDatamodel\Unit test\Datamodel\UnitTest.sqlite3;" />
/// <type>Table</type>
/// <namespace>Datamodel.UnitTest</namespace>
/// <name>LeftSide</name>
/// <sql></sql>
/// </metadata>

using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{

	[DatabaseTable("LeftSide")]
	public partial class LeftSide : DataClassBase
	{

#region " private members "

		[AutoIncrement, PrimaryKey, DatabaseField("ID")]
		private System.Int64 m_ID = long.MinValue;
		[DatabaseField("Text")]
		private System.String m_Text = "";
#endregion

#region " properties "

		public System.Int64 ID
		{
			get{return m_ID;}
			set{object oldvalue = m_ID;OnBeforeDataChange(this, "ID", oldvalue, value);m_ID = value;OnAfterDataChange(this, "ID", oldvalue, value);}
		}

		public System.String Text
		{
			get{return m_Text;}
			set{value = value != null && ((string)value).Length > 255 ? ((string)value).Substring(0, 255) : value;object oldvalue = m_Text;OnBeforeDataChange(this, "Text", oldvalue, value);m_Text = value;OnAfterDataChange(this, "Text", oldvalue, value);}
		}

#endregion

#region " referenced properties "

#endregion

	}

}