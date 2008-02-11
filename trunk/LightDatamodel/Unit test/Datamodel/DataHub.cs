using System;
using System.Data.LightDatamodel;
#region " Unsynchronized Includes "

	//Don't put any region sections in here

#endregion

/// <metadata>
/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>
/// <provider name="System.Data.LightDatamodel.SQLiteDataProvider" connectionstring="Version=3;Data Source=C:\Documents and Settings\Kenneth\Dokumenter\LightDatamodel\LightDatamodel\Unit test\bin\Debug\unittest.sqlite3;" />
/// <type>DataHub</type>
/// <namespace>UnitTest</namespace>
/// </metadata>

namespace UnitTest
{

	public class DataHub : System.Data.LightDatamodel.DataFetcher
	{

#region " private members "

		private ProjectCollection m_Project;
		private RegistrationCollection m_Registration;
		private SpecificEventCollection m_SpecificEvent;
		private NoteCollection m_Note;
		private LeftSideCollection m_LeftSide;
		private RightSideCollection m_RightSide;
		private ManyToManyCollection m_ManyToMany;

#endregion

		public DataHub() : base(new System.Data.LightDatamodel.SQLiteDataProvider("Version=3;Data Source=C:\\Documents and Settings\\Kenneth\\Dokumenter\\LightDatamodel\\LightDatamodel\\Unit test\\bin\\Debug\\unittest.sqlite3;"))
		{
		}
		public DataHub(IDataProvider provider) : base(provider)
		{
		}

		public DataHub(string connectionstring) : base(new System.Data.LightDatamodel.SQLiteDataProvider(connectionstring))
		{
		}

		public DataHub(System.Data.IDbConnection connection) : base(new System.Data.LightDatamodel.SQLiteDataProvider(connection))
		{
		}

#region " properties "

		public ProjectCollection Project
		{
			get
			{
				if(m_Project == null)
				{
					object[] arr = GetObjects(typeof(Project), "");
					m_Project = new ProjectCollection();
					m_Project.AddRange(arr);
				}
				return m_Project;
			}
		}

		public RegistrationCollection Registration
		{
			get
			{
				if(m_Registration == null)
				{
					object[] arr = GetObjects(typeof(Registration), "");
					m_Registration = new RegistrationCollection();
					m_Registration.AddRange(arr);
				}
				return m_Registration;
			}
		}

		public SpecificEventCollection SpecificEvent
		{
			get
			{
				if(m_SpecificEvent == null)
				{
					object[] arr = GetObjects(typeof(SpecificEvent), "");
					m_SpecificEvent = new SpecificEventCollection();
					m_SpecificEvent.AddRange(arr);
				}
				return m_SpecificEvent;
			}
		}

		public NoteCollection Note
		{
			get
			{
				if(m_Note == null)
				{
					object[] arr = GetObjects(typeof(Note), "");
					m_Note = new NoteCollection();
					m_Note.AddRange(arr);
				}
				return m_Note;
			}
		}

		public LeftSideCollection LeftSide
		{
			get
			{
				if(m_LeftSide == null)
				{
					object[] arr = GetObjects(typeof(LeftSide), "");
					m_LeftSide = new LeftSideCollection();
					m_LeftSide.AddRange(arr);
				}
				return m_LeftSide;
			}
		}

		public RightSideCollection RightSide
		{
			get
			{
				if(m_RightSide == null)
				{
					object[] arr = GetObjects(typeof(RightSide), "");
					m_RightSide = new RightSideCollection();
					m_RightSide.AddRange(arr);
				}
				return m_RightSide;
			}
		}

		public ManyToManyCollection ManyToMany
		{
			get
			{
				if(m_ManyToMany == null)
				{
					object[] arr = GetObjects(typeof(ManyToMany), "");
					m_ManyToMany = new ManyToManyCollection();
					m_ManyToMany.AddRange(arr);
				}
				return m_ManyToMany;
			}
		}

#endregion

#region " Views Syncronized Code Region "

	//Don't put any region sections in here

/// <views>
/// </views>

#endregion

#region " Unsynchronized Custom Code Region "

	//Don't put any region sections in here

#endregion

	}
}