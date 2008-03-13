using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Data.LightDatamodel;
using System.Data.LightDatamodel.QueryModel;

namespace UnitTest
{
	/// <summary>
	/// Summary description for UnitTest.
	/// </summary>
	class UnitTest
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "unittest.sqlite3");
			if (File.Exists(filename))
				File.Delete(filename);

            System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("New=True;Version=3;Data Source=" + filename);
			con.Open();


			IDbCommand cmd = con.CreateCommand();
			using(StreamReader sr = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(UnitTest), "DB Schema.sql"), System.Text.Encoding.Default, true))
				cmd.CommandText = sr.ReadToEnd();

			cmd.ExecuteNonQuery();

			TestRelations(con);
			TestQueryModel(con);
		}

		public static void TestQueryModel(IDbConnection con)
		{
            DataFetcherCached hub = new DataFetcherCached(new SQLiteDataProvider(con));

			Operation op1 = new Operation(Operators.GreaterThan, new OperationOrParameter[] {new Parameter("ID", true), new Parameter(0, false)});
			Operation op2 = new Operation(Operators.LessThan, new OperationOrParameter[] {new Parameter("ID", true), new Parameter(1000, false)});
			Operation op3 = new Operation(Operators.And, new OperationOrParameter[] {op1, op2});

			object[] f = hub.GetObjects(typeof(Project), "");
			object[] q = hub.GetObjects(typeof(Project), op3);
			if (q.Length != f.Length)
				throw new Exception("Bad evaluation, should have returned " + f.Length.ToString() + " objects");

			int listlen = f.Length;

			Operation op = Parser.ParseQuery("ID = 5 AND X = \"test\" AND Y = 5.3");

			System.Data.LightDatamodel.QueryModel.Operation opx =
				new Operation(Operators.In, new OperationOrParameter[] {
																		   new Parameter("Left", true),
																		   new Parameter(new Parameter[] { new Parameter(1, false), new Parameter(2, false) }, false),
																		   new Parameter("Right", true) });

			op = Parser.ParseQuery("ID IN (1,2,3,4,5) ORDER BY ID DESC");
			if (op.EvaluateList(f).Length != listlen)
				throw new Exception("IN operator failure");

			op = Parser.ParseQuery("ID IN (\"1\",\"2\",\"3\",\"4\",\"5\") ORDER BY ID ASC");
			if (op.EvaluateList(f).Length != listlen)
				throw new Exception("IN operator failure with strings");

			op = Parser.ParseQuery("ORDER BY ID DESC");
			if (op.EvaluateList(f).Length != listlen)
				throw new Exception("Missing elements af sorting");
			op = Parser.ParseQuery("ID BETWEEN ? AND ?", 3, 2);
			if (op.EvaluateList(f).Length != 2)
				throw new Exception("Bad result from the BETWEEN operator");

			op = Parser.ParseQuery("IIF(ID=1,\"1\",\"2\") = 2");
			if (op.EvaluateList(f).Length != 2)
				throw new Exception("Bad result from the IIF operator");

			op = Parser.ParseQuery("GetType.FullName = \"" + typeof(Project).FullName + "\" AND ID = 1");
			if (op.EvaluateList(f).Length != 1)
				throw new Exception("Bad result from the method operator");

			ArrayList l = new ArrayList(f);
			l.Add("invalid obj");

			//This will break if the evaluation is not lazy
			op = Parser.ParseQuery("GetType.FullName = \"" + typeof(Project).FullName + "\" AND ID = 1");
			if (op.EvaluateList(l).Length != 1)
				throw new Exception("Bad result from the lazy method operator");

			ArrayList ids = new ArrayList();
			ids.Add(1);
			ids.Add(listlen + 1);
			op = Parser.ParseQuery("ID IN (?)", ids);
			if (op.EvaluateList(f).Length != 1)
				throw new Exception("Bad result from the IN operator with a parameter and a list");

			op = Parser.ParseQuery("ID IN (?)");
			if (op.EvaluateList(f, ids).Length != 1)
				throw new Exception("Bad result from the IN operator with a parameter and a list, late bind");

			op = Parser.ParseQuery("(ID IN (?)) OR ID = ?", ids);
			if (op.EvaluateList(f, 2).Length != 2)
				throw new Exception("Bad result from the IN operator with a parameter and a list, plus a bind argument");

		}

		public static void TestRelations(IDbConnection con)
		{
            DataFetcherCached hub = new DataFetcherCached(new SQLiteDataProvider(con));

            DataFetcherNested nd = new DataFetcherNested(hub);

			Note n = (Note)nd.CreateObject(typeof(Note));
			Project p = (Project)nd.CreateObject(typeof(Project));
			p.ProjectNote = n;
			Guid pg = p.Guid;
			Guid ng = n.Guid;

			nd.CommitAll();

			p = (Project)hub.GetObjectByGuid(pg);
			if (p == null)
				throw new Exception("Failed to put item through Context");
			n = p.CurrentTaskNote;
			if (n != null)
				throw new Exception("Bad assignment, is ID autoincrement?");
			n = p.ProjectNote;
			if (n == null)
				throw new Exception("Failed to assign note through Context");

			if (p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			hub.CommitAll();

			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (!n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

            nd = new DataFetcherNested(hub);
			p = (Project)nd.GetObjectByGuid(pg);
			if (p == null)
				throw new Exception("Failed to load item from guid");
			n = (Note)nd.GetObjectByGuid(ng);

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			long id = p.ID;
			hub.DiscardObject(p);
			hub.DiscardObject(n);

            nd = new DataFetcherNested(hub);
			p = (Project)nd.GetObjectById(typeof(Project), id);
			if (p == null)
				throw new Exception("Failed to load item from DB");
			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");

			n = (Note)nd.CreateObject(typeof(Note));
			n.NoteText = "Newly created link";
			p.CurrentTaskNote = n;
			if (p.CurrentTaskNote == null)
				throw new Exception("Failed to assign a new object on an existing project");
			if (p.CurrentTaskNote.NoteText != "Newly created link")
				throw new Exception("Failed to assign a new object on an existing project");
			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			pg = p.Guid;
			ng = n.Guid;
						
			nd.CommitAll();

			p = (Project)hub.GetObjectByGuid(pg);
			n = (Note)hub.GetObjectByGuid(ng);

			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			hub.CommitAll();

			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (!n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			if (p.CurrentTaskNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			id = p.ProjectNoteID;

            nd = new System.Data.LightDatamodel.DataFetcherNested(hub);

			n = (Note)nd.GetObjectById(typeof(Note), id);
			p = (Project)nd.CreateObject(typeof(Project));
			p.Title = "A new project";
			p.ProjectNote = n;

			pg = p.Guid;
			ng = n.Guid;

			nd.CommitAll();

			p = (Project)hub.GetObjectByGuid(pg);
			if (p == null)
				throw new Exception("Newly created object did not pass through the Context");
			n = p.ProjectNote;
			if (n == null)
				throw new Exception("Failed to perists relation through Context");


			if (p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (!n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			hub.CommitAll();

			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (!n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			p = (Project)hub.CreateObject(typeof(Project));
			n = (Note)hub.CreateObject(typeof(Note));
			n.NoteText = "1";
			p.Title = "2";
			p.ProjectNote = n;

			pg = p.Guid;
			ng = n.Guid;

            nd = new DataFetcherNested(hub);
			p = (Project)nd.GetObjectByGuid(pg);
			if (p.ProjectNote == null)
				throw new Exception("Failed to pass on created object");
			if (p.ProjectNote.Guid != ng)
				throw new Exception("Failed to pass on created object");

			p.Title = "4";
			p.ProjectNote.NoteText = "5";

			nd.CommitAll();

			hub.CommitAll();

			p = (Project)hub.GetObjectByGuid(pg);
			n = (Note)hub.GetObjectByGuid(ng);

			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (!n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");
		}
	}
}
