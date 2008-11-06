using System.Data.LightDatamodel;
using System.Data.LightDatamodel.DataClassAttributes;

namespace Datamodel.UnitTest
{
	[DatabaseView("SELECT ID FROM Project")]
	public class TestView : DataClassView
	{
		[DatabaseField("ID")]
		private System.Int64 m_ID = 0;

		public System.Int64 ID
		{
			get { return m_ID; }
			set { m_ID = value; }
		}
	}
}
