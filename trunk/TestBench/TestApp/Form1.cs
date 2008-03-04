using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.LightDatamodel;
using Datamodel.TestDB;

namespace TestApp
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//conn
			DataFetcher fetcher = new DataFetcher(new AccessDataProvider(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\workspace\LightDatamodel\TestBench\TestDB.mdb;"));

			//fetch
			Users[] u = fetcher.GetObjects<Users>();

			//update
			u[0].CreatedDate = u[0].CreatedDate.AddDays(1);
			fetcher.Commit(u[0]);

			//validate update
			Users vali = fetcher.GetObjectById<Users>(u[0].ID);
			if (vali.CreatedDate != u[0].CreatedDate) throw new Exception("Bah!");

			//create and compute
			Users newuser = new Users();
			newuser.ID = fetcher.Compute<int, Users>("MAX(ID)", "") + 1;
			newuser.Name = "Hans";
			fetcher.Commit(newuser);

			//retrive it
			vali = fetcher.GetObjectById<Users>(newuser.ID);

			//delete
			fetcher.DeleteObject<Users>(newuser.ID);

			//test joins
			string husnr = u[0].Address.HouseNumber;


			int i = 0;
		}
	}
}