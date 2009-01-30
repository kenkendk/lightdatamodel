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
using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
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
			System.Console.WriteLine("Starting test...");
			int start = System.Environment.TickCount;

			string filename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "unittest.sqlite3");
			if (File.Exists(filename)) File.Delete(filename);

			System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("New=True;Version=3;Data Source=" + filename);
			con.Open();


			IDbCommand cmd = con.CreateCommand();
			using (StreamReader sr = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTest.DB Schema.sql"), System.Text.Encoding.Default, true))
				cmd.CommandText = sr.ReadToEnd();
			cmd.ExecuteNonQuery();

			System.Console.WriteLine("Basic operations...");
			int basicstart = System.Environment.TickCount;
			TestBasicOperations(con);
			System.Console.WriteLine("" + (System.Environment.TickCount - basicstart) + " ms");

			System.Console.WriteLine("MultiDictionary...");
			int multistart = System.Environment.TickCount;
			TestMultiDictionary();
			TestCache();
			System.Console.WriteLine("" + (System.Environment.TickCount - multistart) + " ms");

			System.Console.WriteLine("Cached operations...");
			int cachedstart = System.Environment.TickCount;
			TestCachedOperations(con);
			System.Console.WriteLine("" + (System.Environment.TickCount - cachedstart) + " ms");

			System.Console.WriteLine("Query model...");
			int querystart = System.Environment.TickCount;
			TestQueryModel(con);
			System.Console.WriteLine("" + (System.Environment.TickCount - querystart) + " ms");

			//System.Console.WriteLine("Assyncron fetcher...");
			//int assynstart = System.Environment.TickCount;
			//TestFetcherAssyncron(con);
			//System.Console.WriteLine("" + (System.Environment.TickCount - assynstart) + " ms");

			//System.Console.WriteLine("Cached assyncron fetcher...");
			//assynstart = System.Environment.TickCount;
			//TestCachedFetcherAssyncron(con);
			//System.Console.WriteLine("" + (System.Environment.TickCount - assynstart) + " ms");

			System.Console.WriteLine("Relations operations...");
			int relationsstart = System.Environment.TickCount;
			TestRelations(con);
			System.Console.WriteLine("" + (System.Environment.TickCount - relationsstart) + " ms");

			System.Console.WriteLine("Relations extended...");
			int extendedstart = System.Environment.TickCount;
			TestRelationsExtended(con);
			System.Console.WriteLine("" + (System.Environment.TickCount - extendedstart) + " ms");

            System.Console.WriteLine("Nested fetcher model...");
            int nestedstart = System.Environment.TickCount;
            TestNestedFetcher(con);
            System.Console.WriteLine("" + (System.Environment.TickCount - nestedstart) + " ms");
            
            System.Console.WriteLine("Done! Whole process took " + (System.Environment.TickCount - start) + " ms");
			System.Console.ReadKey();
		}

		public static void TestMultiDictionary()
		{
			MultiDictionary<int, string> test = new MultiDictionary<int, string>();

			//test the empty one
			foreach (KeyValuePair<int, string> k in test)
			{
				int dummy = test.Count;
			}
			foreach (string str in test.Values)
			{
				int dummy = test.Count;
			}
			try
			{
				//this one is meant to fail
				foreach (string ko in test.Values[0])
				{
					int dummy = test.Count;
				}
			}
			catch
			{
			}

			test.Add(1, "ost1");
			test.Add(1, "ko1");
			test.Add(1, "fisk1");
			test.Add(2, "ost2");
			test.Add(3, "ost3");
			test.Add(2, "ko2");

			if (test.Count != 6) throw new Exception("Bah!");

			int c = 0;
			foreach (KeyValuePair<int, string> k in test)
			{
				if (k.Key < 1 || k.Key > 3) throw new Exception("Bah!");
				if (k.Value != "ost" + k.Key && k.Value != "ko" + k.Key && k.Value != "fisk" + k.Key) throw new Exception("Bah!");
				c++;
			}
			if (c != 6) throw new Exception("Bah!");

			c = 0;
			foreach (string str in test.Values)
			{
				c++;
			}
			if (c != 6) throw new Exception("Bah!");

			if (test.Items[1].Count != 3) throw new Exception("Bah!");

			test.Remove(2, "ost2");
			if (test.Count != 5) throw new Exception("Bah!");

			test.Remove(1);
			if (test.Count != 2) throw new Exception("Bah!");

			c = 0;
			foreach (string str in test.Items)
			{
				c++;
			}
			if (c != 2) throw new Exception("Bah!");

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

			//only one must exist
			u = fetcher.GetObjects<Note>("ID = ?", newuser.ID);
			if (u.Length != 1) throw new Exception("Unacceptable!!!!!");

			//validate update
			vali = fetcher.GetObjectById<Note>(u[0].ID);
			if (vali.NoteText != u[0].NoteText) throw new Exception("Bah!");

			//delete
			fetcher.DeleteObject(newuser);

			//validate
			vali = fetcher.GetObjectById<Note>(u[0].ID);
			if (vali != null) throw new Exception("Bah!");

			//test for NULL and ""
			IDbCommand cmd = con.CreateCommand();
			cmd.CommandText = "INSERT INTO Note (ID) VALUES (666)";		//NoteText is now null
			cmd.ExecuteNonQuery();
			cmd.CommandText = "INSERT INTO Note (NoteText) VALUES (NULL)";
			cmd.ExecuteNonQuery();
			cmd.CommandText = "INSERT INTO Note (NoteText) VALUES (\"\")";
			cmd.ExecuteNonQuery();
			Note[] tmp = fetcher.GetObjects<Note>("NoteText = ? OR NoteText Is ?", "", DBNull.Value);
			if (tmp.Length != 3) throw new Exception("NOOOOOO!!!!! HOW DO I LOCATE ALL EMPTY POSTS?????????");
			foreach (Note n in tmp)
				fetcher.DeleteObject(n);
			tmp = fetcher.GetObjects<Note>("NoteText = ? OR NoteText Is ?", "", DBNull.Value);
			if (tmp.Length != 0) throw new Exception("Bah!");

			//test view
			Project p1 = fetcher.Add<Project>();
			Project p2 = fetcher.Add<Project>();
			fetcher.Commit(p1);
			fetcher.Commit(p2);
			int np = fetcher.GetObjects<Project>().Length;
			int nv = fetcher.GetObjects<TestView>().Length;
			if (np != nv) throw new Exception("Bah");

			//test non autoincrementing table
			TableWithNoAutoincremetion tableno = new TableWithNoAutoincremetion();
			tableno.ID = "MUFLEBASSE";
			tableno.Meh = 42;		//answer to the universe
			fetcher.Add(tableno);
			fetcher.Commit(tableno);
			tableno = fetcher.GetObjectById<TableWithNoAutoincremetion>("MUFLEBASSE");
			if (tableno == null || tableno.Meh != 42) throw new Exception("Bah");
			tableno.ID = "YAKKATYKKY";
			fetcher.Commit(tableno);
			tableno = fetcher.GetObjectById<TableWithNoAutoincremetion>("YAKKATYKKY");
			if (tableno == null || tableno.Meh != 42) throw new Exception("Bah");
			fetcher.DeleteObject(tableno);
			tableno = fetcher.GetObjectById<TableWithNoAutoincremetion>("YAKKATYKKY");
			if (tableno != null) throw new Exception("Bah");
		
		}

		public static void TestCache()
		{
			DataFetcherCached.Cache test = new DataFetcherCached.Cache();
			try
			{
				test.Lock.AcquireWriterLock(-1);

				//test the empty one
				foreach (IDataClass k in test)
				{
					int dummy = test.Count;
				}
				foreach (IDataClass k in test.GetObjects(typeof(Project)))
				{
					int dummy = test.Count;
				}

				test.Add(typeof(Project), "ID", 1, new Project());
				test.Add(typeof(Project), "ID", 2, new Project());
				test.Add(typeof(Project), "ID", 3, new Project());
				test.Add(typeof(Project), "ID", 4, new Project());
				test.Add(typeof(Project), "ID", 5, new Project());
				test.Add(typeof(Project), "ID", 4, new Project());		//the same OoOOoOoOOooo

				if (test.Count != 6) throw new Exception("Bah!");

				int c = 0;
				foreach (IDataClass k in test)
				{
					c++;
					if (c > 10) throw new Exception("Bah!");
				}
				if (c != 6) throw new Exception("Bah!");

				c = 0;
				foreach (IDataClass k in test.GetObjects(typeof(Project)))
				{
					c++;
				}
				if (c != 6) throw new Exception("Bah!");

				Project p = (Project)test[typeof(Project), "ID", 1];

				test.RemoveObjectFromIndex(typeof(Project), "ID", 1, p);
				if (test.Count != 5) throw new Exception("Bah!");

				p = (Project)test[typeof(Project), "ID", 4];
				test.RemoveObjectFromIndex(typeof(Project), "ID", 4, p);
				if (test.Count != 4) throw new Exception("Bah!");

				c = 0;
				foreach (IDataClass k in test)
				{
					c++;
				}
				if (c != 4) throw new Exception("Bah!");

				c = 0;
				foreach (IDataClass k in test.GetObjects(typeof(Project)))
				{
					c++;
				}
				if (c != 4) throw new Exception("Bah!");

			}
			finally
			{
				test.Lock.ReleaseWriterLock();
			}

		}

		/// <summary>
		/// This will test the nested fetcher
		/// </summary>
		/// <param name="con"></param>
        public static void TestNestedFetcher(IDbConnection con)
        {
            DataFetcherCached baseFetcher = new DataFetcherWithRelations(new SQLiteDataProvider(con));
            DataFetcherNested fetcher = new DataFetcherNested(baseFetcher);

            Project p1 = fetcher.Add<Project>();
            Project p2 = fetcher.Add<Project>();

            fetcher.CommitAll();
            baseFetcher.CommitAll();

            p1 = fetcher.Add<Project>();
            Note n1 = fetcher.Add<Note>();
            p1.CurrentTaskNote = n1;

            fetcher.CommitWithRelations(p1);
            if (n1.ObjectState == ObjectStates.New)
                throw new Exception("Bad relation commit");

            n1 = baseFetcher.GetObjectById<Note>(n1.ID);
            if (n1.ObjectState != ObjectStates.New)
                throw new Exception("Bad relation commit");

            p1 = fetcher.Add<Project>();
            n1 = fetcher.Add<Note>();
            p1.CurrentTaskNote = n1;

            fetcher.CommitRecursiveWithRelations(p1);

            n1 = baseFetcher.GetObjectById<Note>(n1.ID);
            if (n1.ObjectState == ObjectStates.New)
                throw new Exception("Bad recursive relation commit");

            p1 = baseFetcher.GetObjectById<Project>(p1.ID);
            if (p1.ObjectState == ObjectStates.New)
                throw new Exception("Bad recursive relation commit");

            if (p1.CurrentTaskNote != n1)
                throw new Exception("Bad recursive update");
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
			newuser.ID = fetcher.Compute<long, Note>("MAX(ID)", "") + 1;
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

			//only one must exist
			u = fetcher.GetObjects<Note>("ID = ?", newuser.ID);
			if (u.Length != 1) throw new Exception("Unacceptable!!!!!");

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
			Note n1 = new Note();
			Note n2 = new Note();
			Note n3 = new Note();
			n1.ID = -5;		//auto numbers
			n2.ID = -6;		//auto numbers
			n3.ID = -7;		//auto numbers
			fetcher.Add(n1);
			fetcher.Add(n2);
			fetcher.Add(n3);
			vali = fetcher.GetObjectById<Note>(n2.ID);
			if (vali == null) throw new Exception("This feature is used, when not using the relation system. THE OBJECT MUST RETURN! IT IS FAITH!");
			Note[] newandold = fetcher.GetObjects<Note>();
			if (newandold.Length != old.Length + 3) throw new Exception("WHYYYYYYYYY!!!!!???????");

			//uh this be a tricky one
            int[] ids2 = { (int)n1.ID, (int)n2.ID }; //Cannot use { -5, -6 } because the ID is autogenerated
			Note[] lst = fetcher.GetObjects<Note>("ID IN ?", ids2);
			if (lst.Length != 2) throw new Exception("WHYYYYYYYYYY!!!!!!!?????????");

			//test for NULL and ""
			Note[] tmp = fetcher.GetObjects<Note>("NoteText = ? OR NoteText Is ?", "", DBNull.Value);
			if (tmp.Length != 3) throw new Exception("NOOOOOO!!!!! HOW DO I LOCATE ALL EMPTY POSTS?????????");
			tmp = fetcher.GetObjects<Note>("NoteText = ? OR NoteText Is ?", "", DBNull.Value);	//retry to test the cache
			if (tmp.Length != 3) throw new Exception("NOOOOOO!!!!! HOW DO I LOCATE ALL EMPTY POSTS?????????");
			foreach (Note n in tmp)
				fetcher.DeleteObject(n);

			//test case sentivity
			Note casesentivity = new Note();
			casesentivity.NoteText = "HerErEnText";
			fetcher.Add(casesentivity);
			fetcher.Commit(casesentivity);
			Note[] casetest = fetcher.GetObjects<Note>("NoteText = ?", "Hererentext");
			if (casetest == null || casetest.Length == 0) throw new Exception("This is most likely wrong");

			//test index
			fetcher.AddIndex(typeof(Note), "NoteText");
			Note n4 = fetcher.GetObjectByIndex<Note>("NoteText", "HerErEnText");
			if (n4 == null) throw new Exception("Bah!");
			for (int i = 0; i < 10; i++)
			{
				Note n5 = new Note();
				n5.NoteText = "HerErEnText";
				fetcher.Add(n5);
			}
			Note[] ns = fetcher.GetObjectsByIndex<Note>("NoteText", "HerErEnText");
			if (ns == null || ns.Length < 11) throw new Exception("Bah");

			//test view
			Project p1 = fetcher.Add<Project>();
			Project p2 = fetcher.Add<Project>();
			fetcher.Commit(p1);
			fetcher.Commit(p2);
			int np = fetcher.GetObjects<Project>().Length;
			int nv = fetcher.GetObjects<TestView>().Length;
			if (np != nv) throw new Exception("Bah");

			//test non autoincrementing table
			TableWithNoAutoincremetion tableno = new TableWithNoAutoincremetion();
			tableno.ID = "MUFLEBASSE";
			tableno.Meh = 42;		//answer to the universe
			fetcher.Add(tableno);
			fetcher.Commit(tableno);
			tableno = fetcher.GetObjectById<TableWithNoAutoincremetion>("MUFLEBASSE");
			if (tableno == null || tableno.Meh != 42) throw new Exception("Bah");
			tableno.ID = "YAKKATYKKY";
			fetcher.Commit(tableno);
			tableno = fetcher.GetObjectById<TableWithNoAutoincremetion>("YAKKATYKKY");
			if (tableno == null || tableno.Meh != 42) throw new Exception("Bah");
			fetcher.DeleteObject(tableno);
			fetcher.CommitAll();
			tableno = fetcher.GetObjectById<TableWithNoAutoincremetion>("YAKKATYKKY");
			if (tableno != null) throw new Exception("Bah");

			//test cache and refresh
			p1 = fetcher.GetObjectById<Project>(p1.ID);
			fetcher.ClearCache();
			fetcher.RefreshObject(p1);		//refresing a non cached object should cache it
			p1 = fetcher.GetObjectFromCacheById<Project>(p1.ID);
			if (p1 == null) throw new Exception("Bah!");

			//test the assyncron loading
			fetcher.LoadAndCacheObjects(typeof(TableWithNoAutoincremetion), typeof(Project), typeof(Note), typeof(Registration));
			p1 = fetcher.GetObjectFromCacheById<Project>(p1.ID);
			if (p1 == null) throw new Exception("Bah!");
			fetcher.ClearCache();
			fetcher.LoadAndCacheObjects(typeof(TableWithNoAutoincremetion), typeof(Project), typeof(Note), typeof(Registration));
			p1 = fetcher.GetObjectFromCacheById<Project>(p1.ID);
			if (p1 == null) throw new Exception("Bah!");

			//test delete from index (note has index on NoteText)
			n1 = new Note();
			n1.NoteText = "aaa";
			fetcher.Add(n1);
			n1 = new Note();
			n1.NoteText = "aaa";
			fetcher.Add(n1);
			n1 = new Note();
			n1.NoteText = "aaa";
			fetcher.Add(n1);
			fetcher.CommitAll();
			fetcher.ClearCache();
			Note[] notes = fetcher.GetObjectsByIndex<Note>("NoteText", "aaa");
			if (notes.Length != 3) throw new Exception("Bah");
			fetcher.DeleteObject(notes[0]);
			notes = fetcher.GetObjectsByIndex<Note>("NoteText", "aaa");
			if (notes.Length != 2) throw new Exception("Bah");


			//small test with Nested fetcher
			bool isdeity = fetcher.IsDirty;
			if (isdeity) fetcher.CommitAll();
			isdeity = fetcher.IsDirty;
			if (isdeity) throw new Exception("Bah");
			DataFetcherNested nf = new DataFetcherNested(fetcher);
			notes = nf.GetObjects<Note>();
			notes[0].NoteText = "YummyYummyYummy";
			if (fetcher.IsDirty) throw new Exception("Bah");
			if (!nf.IsDirty) throw new Exception("Bah");
			nf.CommitAll();
			if (!fetcher.IsDirty) throw new Exception("Bah");

		}

		public static void TestQueryModel(IDbConnection con)
		{
			DataFetcherCached hub = new DataFetcherCached(new SQLiteDataProvider(con));

			//Avoid ID's being equal with note and project
			for (int i = 0; i < 100; i++)
			{
				hub.Add(new Project());
			}
			hub.CommitAll();

			Operation op1 = Query.GreaterThan(Query.Property("ID"), Query.Value(0));
			Operation op2 = Query.LessThan(Query.Property("ID"), Query.Value(1000));
            Operation op3 = Query.And(op1, op2);

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

            //Basic function invocation
            Operation opy1 = Query.Parse("GetType() Is ?", typeof(string));
            //Function invocation and use of a property on the result
            Operation opy2 = Query.Parse("GetType().FullName = ?", typeof(string).FullName);
            //Longer nesting of the above, the left and right hand side are equvalent
            Operation opy3 = Query.Parse("GetType().Assembly.GetType().FullName = ?", typeof(string).Assembly.GetType().FullName);
            //Static function invocation, and use of the this column
            Operation opy4 = Query.Parse("::System.Convert.ToString(this) = ?", "4");
            //The left and right hand side are equvalent
            Operation opy5 = Query.Parse("this.GetType() is GetType()");

            object[] testitems = new object[] { "", 4, 'c' };

            if (opy1.EvaluateList(testitems).Count != 1)
                throw new Exception("Invalid function call");
            if (opy2.EvaluateList(testitems).Count != 1)
                throw new Exception("Invalid function call");
            if (opy3.EvaluateList(testitems).Count != 3)
                throw new Exception("Invalid function call");
            if (opy4.EvaluateList(testitems).Count != 1)
                throw new Exception("Invalid function call");
            if (opy5.EvaluateList(testitems).Count != 3)
                throw new Exception("Invalid function call");

			Operation op = Query.Parse("ID = 5 AND X = \"test\" AND Y = 5.3");

			System.Data.LightDatamodel.QueryModel.Operation opx =
				new Operation(Operators.In, new OperationOrParameter[] {
																		   new Parameter("Left", true),
																		   new Parameter(new Parameter[] { new Parameter(1, false), new Parameter(2, false) }, false),
																		   new Parameter("Right", true) });

			op = Query.Parse("ID IN (1,2,3,4,5) ORDER BY ID DESC");
            if (op.EvaluateList(f).Count != listlen)
				throw new Exception("IN operator failure");

            op = Query.Parse("ID IN (\"1\",\"2\",\"3\",\"4\",\"5\") ORDER BY ID ASC");
            if (op.EvaluateList(f).Count != listlen)
				throw new Exception("IN operator failure with strings");

            op = Query.Parse("ORDER BY ID DESC");
            if (op.EvaluateList(f).Count != listlen)
				throw new Exception("Missing elements af sorting");
            op = Query.Parse("ID BETWEEN ? AND ?", 3, 2);
            if (op.EvaluateList(f).Count != 2)
				throw new Exception("Bad result from the BETWEEN operator");

            op = Query.Parse("IIF(ID=1,\"1\",\"2\") = 2");
            if (op.EvaluateList(f).Count != 2)
				throw new Exception("Bad result from the IIF operator");

            op = Query.Parse("GetType().FullName = \"" + typeof(Project).FullName + "\" AND ID = 1");
            if (op.EvaluateList(f).Count != 1)
				throw new Exception("Bad result from the method operator");

			ArrayList l = new ArrayList(f);
			l.Add("invalid obj");

			//This will break if the evaluation is not lazy
            op = Query.Parse("GetType().FullName = \"" + typeof(Project).FullName + "\" AND ID = 1");
            if (op.EvaluateList(l).Count != 1)
				throw new Exception("Bad result from the lazy method operator");

			ArrayList ids = new ArrayList();
			ids.Add(1);
			ids.Add(listlen + 1);
            op = Query.Parse("ID IN (?)", ids);
            if (op.EvaluateList(f).Count != 1)
				throw new Exception("Bad result from the IN operator with a parameter and a list");

            op = Query.Parse("ID IN (?)");
            if (op.EvaluateList(f, ids).Count != 1)
				throw new Exception("Bad result from the IN operator with a parameter and a list, late bind");

            op = Query.Parse("(ID IN (?)) OR ID = ?", ids);
            if (op.EvaluateList(f, 2).Count != 2)
				throw new Exception("Bad result from the IN operator with a parameter and a list, plus a bind argument");

		}

		public static void TestRelations(IDbConnection con)
		{
			DataFetcherWithRelations hub = new DataFetcherWithRelations(new SQLiteDataProvider(con));

			//Avoid ID's being equal with note and project
			//for (int i = 0; i < 100; i++)
			//    hub.Add(new Project());

			//hub.CommitAll();

			DataFetcherNested nd = new DataFetcherNested(hub);

			Note n = (Note)nd.Add(new Note());
			Project p = (Project)nd.Add(new Project());
			p.ProjectNote = n;
			//Guid pg = p.Guid;
			//Guid ng = n.Guid;


			if (n.ProjectNotes.Count == 0) throw new Exception("Bad something");

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

			TestRelationCache(hub);

			hub.LocalCache.Lock.AcquireReaderLock(-1);
			if (hub.LocalCache.GetAllChanged().Length != 2) throw new Exception("We've just created two new objects");
			hub.LocalCache.Lock.ReleaseReaderLock();

			//p = (Project)hub.GetObjectByGuid(pg);
			p = hub.GetObjectById<Project>(p.ID);
			if (p == null)
				throw new Exception("Failed to put item through Context");
			n = p.CurrentTaskNote;
			if (n != null)
			    throw new Exception("Bad assignment, is ID autoincrement?");
			//if (n == null) throw new Exception("All objects are added, pointing with default value ... this is bound to give a hit");
			n = p.ProjectNote;
			if (n == null)
				throw new Exception("Failed to assign note through Context");

			//if (p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			if (p.DataParent != hub)
				throw new Exception("Bad dataparent");
			if (n.DataParent != hub)
				throw new Exception("Bad dataparent");

			hub.CommitAll();

			TestRelationCache(hub);

			//if (!p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (!n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			nd = new DataFetcherNested(hub);
			//p = (Project)nd.GetObjectByGuid(pg);
			p = nd.GetObjectById<Project>(p.ID);
			if (p == null)
				throw new Exception("Failed to load item from guid");
			//n = (Note)nd.GetObjectByGuid(ng);
			n = nd.GetObjectById<Note>(n.ID);

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			long pid = p.ID;
			long nid = n.ID;

			//hub.DiscardObject(hub.GetObjectByGuid(p.Guid) as IDataClass);
			//hub.DiscardObject(hub.GetObjectByGuid(n.Guid) as IDataClass);
			hub.DiscardObject(hub.GetObjectById<Project>(pid));
			hub.DiscardObject(hub.GetObjectById<Note>(nid));

			nd = new DataFetcherNested(hub);
			p = (Project)nd.GetObjectById(typeof(Project), pid);
			if (p == null)
				throw new Exception("Failed to load item from DB");
			if (p.DataParent != nd)
				throw new Exception("Invalid dataparent");
			//if (!p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");

			if (p.ProjectNote == null)
				throw new Exception("Failed to load relation from DB");
			if (p.ProjectNote.ID != nid)
				throw new Exception("Loaded wrong relation");

			n = p.ProjectNote;

			//hub.DiscardObject(hub.GetObjectByGuid(p.Guid) as IDataClass);
			//hub.DiscardObject(hub.GetObjectByGuid(n.Guid) as IDataClass);
			hub.DiscardObject(hub.GetObjectById<Project>(pid));
			hub.DiscardObject(hub.GetObjectById<Note>(nid));
			nd = new DataFetcherNested(hub);
			n = (Note)nd.GetObjectById(typeof(Note), nid);
			if (n == null)
				throw new Exception("Failed to load item from DB");
			if (n.DataParent != nd)
				throw new Exception("Invalid dataparent");
			//if (!n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");
			if (n.ProjectNotes.Count != 1)
				throw new Exception("Failed to load relation from DB");
			if (n.ProjectNotes[0].ID != pid)
				throw new Exception("Loaded wrong relation");

			p = n.ProjectNotes[0];
			n = (Note)nd.Add(new Note());
			n.NoteText = "Newly created link";
			//if (p.CurrentTaskNote != null)
			//    throw new Exception("Assignment before actual, this usually means you have caught the default object");
			p.CurrentTaskNote = n;
			if (p.CurrentTaskNote == null)
				throw new Exception("Failed to assign a new object on an existing project");
			if (p.CurrentTaskNote.NoteText != "Newly created link")
				throw new Exception("Failed to assign a new object on an existing project");
			//if (!p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			if (!n.TaskNotes.Contains(p))
				throw new Exception("Failed to contain reverse");

			//pg = p.Guid;
			//ng = n.Guid;

			nd.CommitAll();

			TestRelationCache(hub);

			//p = (Project)hub.GetObjectByGuid(pg);
			//n = (Note)hub.GetObjectByGuid(ng);
			p = hub.GetObjectById<Project>(p.ID);
			n = hub.GetObjectById<Note>(n.ID);

			//if (!p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			if (p.CurrentTaskNote == null)
				throw new Exception("Failed to assign a new object on an existing project");
			if (p.CurrentTaskNote.NoteText != "Newly created link")
				throw new Exception("Failed to assign a new object on an existing project");
			if (!n.TaskNotes.Contains(p))
				throw new Exception("Failed to contain reverse");

			hub.CommitAll();

			TestRelationCache(hub);

			//if (!p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (!n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			if (p.CurrentTaskNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			nid = p.CurrentTaskNoteID;

			nd = new DataFetcherNested(hub);
			n = (Note)nd.GetObjectById(typeof(Note), nid);
			p = (Project)nd.Add(new Project());
			p.Title = "A new project";
			p.ProjectNote = n;
			n = nd.Add<Note>();
			p = (Project)nd.Add(new Project());
			p.Title = "A new project2";
			p.ProjectNote = n;
			if (n.ProjectNotes == null || n.ProjectNotes.Count == 0) throw new Exception("Reverse property failed on create");
			nd.CommitAll();

			TestRelationCache(hub);

			//p = (Project)hub.GetObjectByGuid(pg);
			p = hub.GetObjectById<Project>(p.ID);

			if (p == null)
				throw new Exception("Newly created object did not pass through the Context");
			n = p.ProjectNote;
			if (n == null)
				throw new Exception("Failed to perists relation through Context");


			//if (p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (!n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			hub.CommitAll();

			TestRelationCache(hub);

			//if (!p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (!n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			p = (Project)hub.Add(new Project());
			n = (Note)hub.Add(new Note());
			n.NoteText = "1";
			p.Title = "2";
			p.ProjectNote = n;

			//pg = p.Guid;
			//ng = n.Guid;

			nd = new DataFetcherNested(hub);
			//p = (Project)nd.GetObjectByGuid(pg);
			p = hub.GetObjectById<Project>(p.ID);
			if (p.ProjectNote == null)
				throw new Exception("Failed to pass on created object");
			//if (p.ProjectNote.Guid != ng)
			//    throw new Exception("Failed to pass on created object");

			p.Title = "4";
			p.ProjectNote.NoteText = "5";

			nd.CommitAll();

			TestRelationCache(hub);

			hub.CommitAll();

			TestRelationCache(hub);

			//p = (Project)hub.GetObjectByGuid(pg);
			//n = (Note)hub.GetObjectByGuid(ng);
			p = hub.GetObjectById<Project>(p.ID);
			n = hub.GetObjectById<Note>(n.ID);

			//if (!p.ExistsInDB)
			//    throw new Exception("Project has wrong flag");
			//if (!n.ExistsInDB)
			//    throw new Exception("Note has wrong flag");

			if (p.ProjectNoteID != n.ID)
				throw new Exception("Failed to update reverse ID");

			TestRelationCache(hub);

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
			//hub.DeleteObjectFromReferences(n);
			hub.DiscardObject(n);

			hub.CommitAll();


			pid = p.ID;
			hub.ClearCache();

			nd = new DataFetcherNested(hub);
			p = nd.GetObjectById<Project>(pid);
			//pg = nd.GetGuidForObject(p);
			//Project test = hub.GetObjectByGuid<Project>(pg);
			Project test = hub.GetObjectById<Project>(pid);
			//pg = hub.GetGuidForObject(test);
			object delid = p.ID;
			//nd.DeleteObjectFromReferences(p);
			nd.DiscardObject(p);
			//Guid delg = nd.GetGuidForObject(p);		//aren't this supposed to fail?
			//Sort of, but since it's a nested fetcher, and not committed, 
			//the basefetcher can still retrieve the item
			//But it ONLY works for Guid access
			//pg = hub.GetGuidForObject(test);
			nd.CommitAll();
			//test = (Project)hub.GetObjectByGuid(pg);
			test = hub.GetObjectById<Project>(pid);
			//pg = hub.GetGuidForObject(test);
			hub.CommitAll();

			p = hub.GetObjectById<Project>(delid);
			//if (p != null)
			//    throw new Exception("Failed to actually remove item");

			//p = hub.GetObjectByGuid<Project>(delg);
			p = hub.GetObjectById<Project>(pid);
			//if (p != null)
			//    throw new Exception("Failed to actually remove item");

			p = (Project)hub.Add(new Project());
            long nnID = -1;
			if (hub.GetObjectById<Note>(1) == null)
			{
				Note newnote1 = new Note();
				newnote1.ID = 1;
				hub.Add(newnote1);
                nnID = newnote1.ID;
			}
			p.ProjectNoteID = nnID;
			if (p.ProjectNote == null)
				throw new Exception("Failed to set relation through ID update");
			p.ProjectNoteID = -1;
			if (p.ProjectNote != null)
				throw new Exception("Failed to set relation through ID update");

			Note n1 = hub.GetObjects<Note>()[0];
			p.ProjectNoteID = n1.ID;
			hub.CommitAll();
			if (p.ProjectNote == null) throw new Exception("Failed to set relation through ID update");

			//test the assyncron loading
			hub.LoadAndCacheObjects(typeof(TableWithNoAutoincremetion), typeof(Project), typeof(Note), typeof(Registration));
			p = hub.GetObjectFromCacheById<Project>(p.ID);
			if (p == null || p.ProjectNote == null) throw new Exception("Bah!");
			hub.ClearCache();
			hub.LoadAndCacheObjects(typeof(TableWithNoAutoincremetion), typeof(Project), typeof(Note), typeof(Registration));
			p = hub.GetObjectFromCacheById<Project>(p.ID);
			if (p == null || p.ProjectNote == null) throw new Exception("Bah!");

			nd = new DataFetcherNested(hub);
			nd.LoadAndCacheObjects(typeof(TableWithNoAutoincremetion), typeof(Project), typeof(Note), typeof(Registration));
			p = nd.GetObjectFromCacheById<Project>(p.ID);
			if (p == null || p.ProjectNote == null) throw new Exception("Bah!");
			nd.ClearCache();
			nd.LoadAndCacheObjects(typeof(TableWithNoAutoincremetion), typeof(Project), typeof(Note), typeof(Registration));
			p = nd.GetObjectFromCacheById<Project>(p.ID);
			if (p == null || p.ProjectNote == null) throw new Exception("Bah!");


			//test lazy added relations
			DataFetcherWithRelations lazyrelations = new DataFetcherWithRelations(hub.Provider);
			lazyrelations.AddRelation("Testrelation", typeof(Registration), "NoteID", typeof(Note), "ID");
			Note lazynote = new Note();
			lazynote.NoteText = "lazy";
			lazyrelations.Add(lazynote);
			Registration lazyreg = new Registration();
			lazyreg.ActiveAcknowledge = true;
			lazyrelations.Add(lazyreg);
			lazyrelations.AddRelatedObject("Testrelation", lazynote, lazyreg);
			lazyreg = new Registration();
			lazyreg.ActiveAcknowledge = true;
			lazyrelations.Add(lazyreg);
			lazyrelations.AddRelatedObject("Testrelation", lazynote, lazyreg);
			lazyreg = new Registration();
			lazyreg.ActiveAcknowledge = true;
			lazyrelations.Add(lazyreg);
			lazyrelations.AddRelatedObject("Testrelation", lazynote, lazyreg);
			if(lazyrelations.GetRelatedObjects<Registration>("Testrelation", lazynote).Count != 3) throw new Exception("Bah!");
			if (lazyrelations.GetRelatedObject<Note>("Testrelation", lazyreg) != lazynote) throw new Exception("Bah!");
			lazyrelations.CommitAll();
			if (lazyrelations.GetRelatedObjects<Registration>("Testrelation", lazynote).Count != 3) throw new Exception("Bah!");
			if (lazyrelations.GetRelatedObject<Note>("Testrelation", lazyreg) != lazynote) throw new Exception("Bah!");
			lazyrelations.ClearCache();
			lazynote = lazyrelations.GetObjectById<Note>(lazynote.ID);
			lazyreg = lazyrelations.GetObjectById<Registration>(lazyreg.ID);
			if (lazyrelations.GetRelatedObjects<Registration>("Testrelation", lazynote).Count != 3) throw new Exception("Bah!");
			if (lazyrelations.GetRelatedObject<Note>("Testrelation", lazyreg) != lazynote) throw new Exception("Bah!");
		}

		private static void TestRelationCache(DataFetcherWithRelations hub)
		{
			hub.LocalCache.Lock.AcquireReaderLock(-1);
			foreach (IDataClass obj in hub.LocalCache)
			{
				if (!hub.ObjectRelationCache.ContainsKey(obj)) throw new Exception("Bah!");

				foreach (DataFetcherWithRelations.ObjectConnection rel in hub.ObjectRelationCache[obj].Values)
				{
					foreach (IDataClass child in rel.SubObjects.Values)
						if (!hub.ObjectRelationCache[child][rel.Relation.Name].SubObjects.ContainsKey(obj.GetHashCode())) throw new Exception("Bah");

					//test for doubles
					//Dictionary<IDataClass, IDataClass> tmp = new Dictionary<IDataClass, IDataClass>();
					//foreach (IDataClass child in rel.SubObjects)
					//{
					//    try
					//    {
					//        tmp.Add(child, child);
					//    }
					//    catch
					//    {
					//        throw new Exception("Double relation");
					//    }
					//}
				}

			}
			hub.LocalCache.Lock.ReleaseReaderLock();
		}

		public static void TestRelationsExtended(IDbConnection con)
		{
			DataFetcherWithRelations hub = new DataFetcherWithRelations(new SQLiteDataProvider(con));
			Project p = hub.Add<Project>();
			Note n = hub.Add<Note>();
			p.ProjectNote = n;
			hub.CommitAll();

			long i = p.ID;
			long j = n.ID;

			hub.ClearCache();

			n = hub.GetObjectById<Note>(j);
			IList<Project> l = hub.GetRelatedObjects<Project>("ProjectProjectNote", n);
			if (l.Count != 1) throw new Exception("Failed to load item");

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

			long maxid = hub.Compute<long, Note>("MAX(ID)", "");
			n = hub.GetObjectById<Note>(maxid);

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

		private static void TestFetcherAssyncron(IDbConnection con)
		{
			//conn
			DataFetcher fetcher = new DataFetcher(new SQLiteDataProvider(con));

			int count = 10;
			AssyncronWorker[] aw = new AssyncronWorker[count];
			for (int i = 0; i < count; i++)
			{
				aw[i] = new AssyncronWorker(fetcher);
				aw[i].StartAssyncronTest("TestTråd " + i);
			}

			//wait for them to exit
			bool wait = true;
			while (wait)
			{
				wait = false;
				for (int i = 0; i < count; i++)
				{
					if (aw[i].Thread.IsAlive)
					{
						wait = true;
						break;
					}
				}
				if (wait) System.Threading.Thread.Sleep(100);
			}
		}

		private static void TestCachedFetcherAssyncron(IDbConnection con)
		{
			//conn
			DataFetcherCached fetcher = new DataFetcherCached(new SQLiteDataProvider(con));

			int count = 10;
			AssyncronWorker[] aw = new AssyncronWorker[count];
			for (int i = 0; i < count; i++)
			{
				aw[i] = new AssyncronWorker(fetcher);
				aw[i].StartAssyncronTest("TestTråd " + i);
			}

			//wait for them to exit
			bool wait = true;
			while (wait)
			{
				wait = false;
				for (int i = 0; i < count; i++)
				{
					if (aw[i].Thread.IsAlive)
					{
						wait = true;
						break;
					}
				}
				if (wait) System.Threading.Thread.Sleep(100);
			}
		}

		/// <summary>
		/// This will perform a serie of fetcher operations, meant for stressing the fetcher in assyncron mode
		/// </summary>
		private class AssyncronWorker
		{
			private IDataFetcher m_conn;
			private DataFetcherCached m_cached;
			private System.Threading.Thread m_thread;
			private const int m_count = 10;

			public System.Threading.Thread Thread { get { return m_thread; } }

			public AssyncronWorker(IDataFetcher connection)
			{
				m_conn = connection;
				m_cached = connection as DataFetcherCached;
			}

			private void PerformTest()
			{
				Random rnd = new Random();

				if (m_thread.Name == "TestTråd 5")
				{
					int ko = 1;		// <-- place break point here
                    ko++; //Remove compile warning
				}

				//insert
				Project[] ps = new Project[m_count];
				Note[] ns = new Note[m_count];
				for (int i = 0; i < m_count; i++)
				{
					try
					{
						ps[i] = new Project();
						m_conn.Commit(ps[i]);
						ns[i] = new Note();
						m_conn.Commit(ns[i]);
					}
					catch (NoSuchObjectException)
					{
						//this can happen if the object is deleted, right after creation, but before refresh
						//I'm not sure wheter we should deal with this or not
					}
				}

				////load 
				//for (int i = 0; i < 100; i++)
				//{
				//    m_conn.GetObjectById<Project>(ps[rnd.Next(0, m_count - 1)].ID);
				//    m_conn.GetObjectById<Note>(ns[rnd.Next(0, m_count - 1)].ID);
				//}

				//m_conn.GetObjects<Project>();
				//m_conn.GetObjects<Note>();

				//for (int i = 0; i < 100; i++)
				//{
				//    m_conn.GetObjectById<Project>(ps[rnd.Next(0, m_count - 1)].ID);
				//    m_conn.GetObjectById<Note>(ns[rnd.Next(0, m_count - 1)].ID);
				//}

				//delete all!!!
				ps = m_conn.GetObjects<Project>();
				ns = m_conn.GetObjects<Note>();
				foreach (Project p in ps)
				{
					try
					{
						m_conn.DeleteObject(p);
					}
					catch (NoSuchObjectException)
					{
						//Again, this might already be deleted
                    }
				}
				foreach (Note n in ns)
				{
					try
					{
						m_conn.DeleteObject(n);
					}
					catch (NoSuchObjectException)
					{
						//Again, this might already be deleted
					}
				}

				//cache
				if (m_cached != null)
				{
					bool success = false;
					while (!success)
					{
						try
						{
							m_cached.CommitAll();
							success = true;
						}
						catch (NoSuchObjectException ex)
						{
							//discard the object and try again
							m_cached.DiscardObject(ex.Object as IDataClass);
						}
					}
				}

				m_thread.Abort();	//end
			}

			public void StartAssyncronTest(string threadname)
			{
				m_thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.PerformTest));
				m_thread.Name = threadname;
				m_thread.Start();
			}
		}
	}
}
