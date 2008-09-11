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