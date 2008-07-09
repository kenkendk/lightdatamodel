using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Data.LightDatamodel;
using System.Data.LightDatamodel.QueryModel;

namespace Datamodel.UnitTest
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
			using(StreamReader sr = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTest.DB Schema.sql"), System.Text.Encoding.Default, true))
				cmd.CommandText = sr.ReadToEnd();

			cmd.ExecuteNonQuery();

			TestBasicOperations(con);
			TestCachedOperations(con);
			TestRelations(con);
			TestQueryModel(con);
            TestRelationsExtended(con);
		}

		/// <summary>
		/// This will test the second layer (the direct data fetcher)
		/// </summary>
		/// <param name="con"></param>
		public static void TestBasicOperations(IDbConnection con)
		{
			//conn
			DataFetcher fetcher = new DataFetcher(new SQLiteDataProvider(con));

			//create and compute
			Note newuser = new Note();
			newuser.ID = fetcher.Compute<int, Note>("MAX(ID)", "") + 1;
			newuser.NoteText = "Hans";
			fetcher.Commit(newuser);

			//retrive it
			Note vali = fetcher.GetObjectById<Note>(newuser.ID);
			if (vali == null) throw new Exception("Bah!");

			//update
			vali.NoteText = "Something's looking fenomenor...";
			fetcher.Commit(vali);

			//fetch
			Note[] u = fetcher.GetObjects<Note>();
			if (u == null || u.Length == 0) throw new Exception("Bah!");

			//validate update
			vali = fetcher.GetObjectById<Note>(u[0].ID);
			if (vali.NoteText != u[0].NoteText) throw new Exception("Bah!");

			//delete
			fetcher.DeleteObject(newuser);

			//validate
			vali = fetcher.GetObjectById<Note>(u[0].ID);
			if (vali != null) throw new Exception("Bah!");
		}

		/// <summary>
		/// This will test the third layer (the cache on top of the direct fetcher)
		/// </summary>
		/// <param name="con"></param>
		public static void TestCachedOperations(IDbConnection con)
		{
			//conn
			DataFetcherCached fetcher = new DataFetcherCached(new SQLiteDataProvider(con));

			//create and compute
			Note newuser = new Note();
			newuser.ID = fetcher.Compute<int, Note>("MAX(ID)", "") + 1;
			newuser.NoteText = "Hans";
			fetcher.Commit(newuser);

			//retrive it
			Note vali = fetcher.GetObjectById<Note>(newuser.ID);
			if (vali == null) throw new Exception("Bah!");

			//update
			vali.NoteText = "Something's looking fenomenor...";
			fetcher.Commit(vali);

			//fetch
			Note[] u = fetcher.GetObjects<Note>();
			if (u == null || u.Length == 0) throw new Exception("Bah!");

			//validate update
			vali = fetcher.GetObjectById<Note>(u[0].ID);
			if (vali.NoteText != u[0].NoteText) throw new Exception("Bah!");

			//delete
			fetcher.DeleteObject(newuser);

			//validate
			vali = fetcher.GetObjectById<Note>(u[0].ID);
			if (vali != null) throw new Exception("Bah!");

			//fetch new auto numbers
			Note[] old = fetcher.GetObjects<Note>();
			SpecificEvent e1 = fetcher.Add<SpecificEvent>();
			Note n1 = fetcher.Add<Note>();
			Note n2 = fetcher.Add<Note>();
			Note n3 = fetcher.Add<Note>();
			vali = fetcher.GetObjectById<Note>(n2.ID);
			if (vali == null) throw new Exception("This feature is used, when not using the relation system");
			Note[] newandold = fetcher.GetObjects<Note>();
			if (newandold.Length != old.Length + 3) throw new Exception("WHYYYYYYYYY!!!!!???????");

			//uh this be a tricky one
			int[] ids2 = { -5, -6 };
			n1.ID = -5;
			n2.ID = -6;
			n3.ID = -7;
			Note[] lst = fetcher.GetObjects<Note>("ID IN ?", ids2);
			if (lst.Length != 2) throw new Exception("WHYYYYYYYYYY!!!!!!!?????????");
		}

		public static void TestQueryModel(IDbConnection con)
		{
            DataFetcherCached hub = new DataFetcherCached(new SQLiteDataProvider(con));

			Operation op1 = new Operation(Operators.GreaterThan, new OperationOrParameter[] {new Parameter("ID", true), new Parameter(0, false)});
			Operation op2 = new Operation(Operators.LessThan, new OperationOrParameter[] {new Parameter("ID", true), new Parameter(1000, false)});
			Operation op3 = new Operation(Operators.And, new OperationOrParameter[] {op1, op2});

			object[] f = hub.GetObjects(typeof(Project), "");
            foreach (Project p in f)
                if (p.ID > 3)
                    hub.DeleteObject(p);
            hub.CommitAll();
            f = hub.GetObjects<Project>();

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

            //Avoid ID's being equal with note and project
            for(int i = 0; i < 100; i++)
                hub.Add(new Project());

            //hub.CommitAll();

            DataFetcherNested nd = new DataFetcherNested(hub);

            Note n = (Note)nd.Add(new Note());
            Project p = (Project)nd.Add(new Project());
			p.ProjectNote = n;
			Guid pg = p.Guid;
            Guid ng = n.Guid;

            if (p.DataParent != nd)
                throw new Exception("Bad dataparent");
            if (n.DataParent != nd)
                throw new Exception("Bad dataparent");

            if (p.ProjectNote != n)
                throw new Exception("Bad relation");
            if (n.ProjectNotes.Count != 1)
                throw new Exception("Bad relation");
            if (n.ProjectNotes[0] != p)
                throw new Exception("Bad relation");

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

            if (p.DataParent != hub)
                throw new Exception("Bad dataparent");
            if (n.DataParent != hub)
                throw new Exception("Bad dataparent");

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

			long pid = p.ID;
            long nid = n.ID;

			hub.DiscardObject(hub.GetObjectByGuid(p.Guid) as IDataClass);
			hub.DiscardObject(hub.GetObjectByGuid(n.Guid) as IDataClass);

            nd = new DataFetcherNested(hub);
			p = (Project)nd.GetObjectById(typeof(Project), pid);
			if (p == null)
				throw new Exception("Failed to load item from DB");
            if (p.DataParent != nd)
                throw new Exception("Invalid dataparent");
			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");

            if (p.ProjectNote == null)
                throw new Exception("Failed to load relation from DB");
            if (p.ProjectNote.ID != nid)
                throw new Exception("Loaded wrong relation");

            n = p.ProjectNote;

            hub.DiscardObject(hub.GetObjectByGuid(p.Guid) as IDataClass);
            hub.DiscardObject(hub.GetObjectByGuid(n.Guid) as IDataClass);
            nd = new DataFetcherNested(hub);
            n = (Note)nd.GetObjectById(typeof(Note), nid);
            if (n == null)
                throw new Exception("Failed to load item from DB");
            if (n.DataParent != nd)
                throw new Exception("Invalid dataparent");
            if (!n.ExistsInDB)
                throw new Exception("Note has wrong flag");
            if (n.ProjectNotes.Count != 1)
                throw new Exception("Failed to load relation from DB");
            if (n.ProjectNotes[0].ID != pid)
                throw new Exception("Loaded wrong relation");

            p = n.ProjectNotes[0];
			n = (Note)nd.Add(new Note());
			n.NoteText = "Newly created link";
            if (p.CurrentTaskNote != null)
                throw new Exception("Assignment before actual, this usually means you have caught the default object");
			p.CurrentTaskNote = n;
			if (p.CurrentTaskNote == null)
				throw new Exception("Failed to assign a new object on an existing project");
			if (p.CurrentTaskNote.NoteText != "Newly created link")
				throw new Exception("Failed to assign a new object on an existing project");
			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (n.ExistsInDB)
				throw new Exception("Note has wrong flag");

            if (!n.TaskNotes.Contains(p))
                throw new Exception("Failed to contain reverse");

			pg = p.Guid;
			ng = n.Guid;
						
			nd.CommitAll();

			p = (Project)hub.GetObjectByGuid(pg);
			n = (Note)hub.GetObjectByGuid(ng);

			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (n.ExistsInDB)
				throw new Exception("Note has wrong flag");

            if (p.CurrentTaskNote == null)
                throw new Exception("Failed to assign a new object on an existing project");
            if (p.CurrentTaskNote.NoteText != "Newly created link")
                throw new Exception("Failed to assign a new object on an existing project");
            if (!n.TaskNotes.Contains(p))
                throw new Exception("Failed to contain reverse");

			hub.CommitAll();

			if (!p.ExistsInDB)
				throw new Exception("Project has wrong flag");
			if (!n.ExistsInDB)
				throw new Exception("Note has wrong flag");

			if (p.CurrentTaskNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			nid = p.CurrentTaskNoteID;

            nd = new System.Data.LightDatamodel.DataFetcherNested(hub);

			n = (Note)nd.GetObjectById(typeof(Note), nid);
            p = (Project)nd.Add(new Project());
			p.Title = "A new project";
			p.ProjectNote = n;

            if (n.ProjectNotes == null || n.ProjectNotes.Count == 0)
                throw new Exception("Reverse property failed on create");

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

			p = (Project)hub.Add(new Project());
            n = (Note)hub.Add(new Note());
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

            hub.ClearCache();

            p = (Project)hub.Add(new Project());
            n = (Note)hub.Add(new Note());

            n.ProjectNotes.Add(p);

            if (p.ProjectNote == null)
                throw new Exception("Failed to update reverse collection");

            hub.CommitAll();

            nid = p.ProjectNoteID;
            p.ProjectNote = null;
            if (n.ProjectNotes.Count != 0)
                throw new Exception("Failed to update reverse item");

            if (p.ProjectNoteID == nid)
                throw new Exception("Failed to set reverse ID after removal");

            n = hub.GetObjectById<Note>(nid);
            p.ProjectNote = n;
            n.ProjectNotes.Remove(p);

            if (p.ProjectNote != null)
                throw new Exception("Failed to update reverse item");
            if (p.ProjectNoteID == nid)
                throw new Exception("Failed to set reverse ID after removal");

            

            hub.CommitAll();

            p.ProjectNote = n;

            if (n.ProjectNotes.Count != 1)
                throw new Exception("Failed to update reverse collection");

            hub.CommitAll();


            p.ProjectNote = null;
            hub.DeleteObject(n);

            hub.CommitAll();


            pid = p.ID;
            hub.ClearCache();

            nd = new DataFetcherNested(hub);
            p = nd.GetObjectById<Project>(pid);
            pg = nd.RelationManager.GetGuidForObject(p);
            Project test = hub.GetObjectByGuid<Project>(pg);
            pg = hub.RelationManager.GetGuidForObject(test);
            object delid = p.UniqueValue;
            nd.DeleteObject(p);
            Guid delg = nd.RelationManager.GetGuidForObject(p);
            pg = hub.RelationManager.GetGuidForObject(test);
            nd.CommitAll();
            test = (Project)hub.RelationManager.GetObjectByGuid(pg);
            pg = hub.RelationManager.GetGuidForObject(test);
            hub.CommitAll();

            p = hub.GetObjectById<Project>(delid);
            if (p != null)
                throw new Exception("Failed to actually remove item");

            p = hub.GetObjectByGuid<Project>(delg);
            if (p != null)
                throw new Exception("Failed to actually remove item");

            p = (Project)hub.Add(new Project());
            p.ProjectNoteID = 1;
            if (p.ProjectNote == null)
                throw new Exception("Failed to set relation through ID update");
            p.ProjectNoteID = -1;
            if (p.ProjectNote != null)
                throw new Exception("Failed to set relation through ID update");

		}

        public static void TestRelationsExtended(IDbConnection con)
        {
            DataFetcherCached hub = new DataFetcherCached(new SQLiteDataProvider(con));
            Project p = hub.Add<Project>();
            Note n = hub.Add<Note>();
            p.ProjectNote = n;
            hub.CommitAll();

            long i = p.ID;
            long j = n.ID;

            hub.ClearCache();

            DataFetcherNested nd = new DataFetcherNested(hub);
            p = nd.GetObjectById<Project>(i);

            if (p.ProjectNote == null)
                throw new Exception("Failed to load item");

            hub.ClearCache();

            nd = new DataFetcherNested(hub);
            n = nd.GetObjectById<Note>(j);

            if (n.ProjectNotes.Count != 1)
                throw new Exception("Failed to load item");

            hub.ClearCache();

            n = hub.GetObjectById<Note>(j);

            nd = new DataFetcherNested(hub);
            n = nd.GetObjectById<Note>(j);

            if (n.ProjectNotes.Count != 1)
                throw new Exception("Failed to load item");

            p = nd.Add<Project>();
            n = nd.Add<Note>();
            n.ProjectNotes.Add(p);

            nd.CommitAll();
            hub.CommitAll();

            foreach(Note nx in hub.GetObjects<Note>())
                n = nx;

            if (n.ProjectNotes.Count != 1)
                throw new Exception("Failed to set item");


            hub.ClearCache();
            n = nd.Add<Note>();
            p = nd.Add<Project>();
            n.ProjectNotes.Add(p);

            hub.CommitAll();

            if (n.ProjectNotes.Count != 1)
                throw new Exception("Failed to set item");

            if (p.ProjectNote == null)
                throw new Exception("Failed to set item");


        }
	}
}
