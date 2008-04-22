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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Data.LightDatamodel;
using System.Collections.Generic;

namespace DataClassFileBuilder
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class DataClassFileBuilderDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button CreateViewButton;
		protected System.Windows.Forms.TextBox SQLText;
		protected System.Windows.Forms.TextBox ViewNameText;
		public IConfigureableDataProvider[] Providers;
		private System.Windows.Forms.GroupBox TableGroup;
		private System.Windows.Forms.GroupBox ViewGroup;
		public System.Windows.Forms.ComboBox ProviderList;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button BrowseDB;
		protected System.Windows.Forms.TextBox ConnectionStringText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button BrowseButton;
		protected System.Windows.Forms.TextBox DestinationDirText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button BuildButton;
		protected System.Windows.Forms.TextBox NamespaceStringText;
		private System.Windows.Forms.Label label3;
        private CheckBox UseConfigCheckBox;
        private Button EditConfigButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DataClassFileBuilderDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataClassFileBuilderDialog));
            this.TableGroup = new System.Windows.Forms.GroupBox();
            this.BrowseDB = new System.Windows.Forms.Button();
            this.ConnectionStringText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.DestinationDirText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BuildButton = new System.Windows.Forms.Button();
            this.NamespaceStringText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ViewGroup = new System.Windows.Forms.GroupBox();
            this.CreateViewButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.SQLText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ViewNameText = new System.Windows.Forms.TextBox();
            this.ProviderList = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.UseConfigCheckBox = new System.Windows.Forms.CheckBox();
            this.EditConfigButton = new System.Windows.Forms.Button();
            this.TableGroup.SuspendLayout();
            this.ViewGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableGroup
            // 
            this.TableGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TableGroup.Controls.Add(this.EditConfigButton);
            this.TableGroup.Controls.Add(this.BrowseDB);
            this.TableGroup.Controls.Add(this.ConnectionStringText);
            this.TableGroup.Controls.Add(this.label1);
            this.TableGroup.Controls.Add(this.BrowseButton);
            this.TableGroup.Controls.Add(this.DestinationDirText);
            this.TableGroup.Controls.Add(this.label2);
            this.TableGroup.Controls.Add(this.BuildButton);
            this.TableGroup.Controls.Add(this.NamespaceStringText);
            this.TableGroup.Controls.Add(this.label3);
            this.TableGroup.Enabled = false;
            this.TableGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.TableGroup.Location = new System.Drawing.Point(8, 72);
            this.TableGroup.Name = "TableGroup";
            this.TableGroup.Size = new System.Drawing.Size(600, 128);
            this.TableGroup.TabIndex = 4;
            this.TableGroup.TabStop = false;
            this.TableGroup.Text = "Create classes for all tables in datasource";
            // 
            // BrowseDB
            // 
            this.BrowseDB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseDB.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BrowseDB.Location = new System.Drawing.Point(568, 24);
            this.BrowseDB.Name = "BrowseDB";
            this.BrowseDB.Size = new System.Drawing.Size(24, 21);
            this.BrowseDB.TabIndex = 13;
            this.BrowseDB.Text = "...";
            this.BrowseDB.Click += new System.EventHandler(this.BrowseDB_Click);
            // 
            // ConnectionStringText
            // 
            this.ConnectionStringText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnectionStringText.Location = new System.Drawing.Point(104, 24);
            this.ConnectionStringText.Name = "ConnectionStringText";
            this.ConnectionStringText.Size = new System.Drawing.Size(464, 20);
            this.ConnectionStringText.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Connectionstring";
            // 
            // BrowseButton
            // 
            this.BrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BrowseButton.Location = new System.Drawing.Point(568, 48);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(24, 21);
            this.BrowseButton.TabIndex = 12;
            this.BrowseButton.Text = "...";
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // DestinationDirText
            // 
            this.DestinationDirText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DestinationDirText.Location = new System.Drawing.Point(104, 48);
            this.DestinationDirText.Name = "DestinationDirText";
            this.DestinationDirText.Size = new System.Drawing.Size(464, 20);
            this.DestinationDirText.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Dir";
            // 
            // BuildButton
            // 
            this.BuildButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BuildButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BuildButton.Location = new System.Drawing.Point(456, 96);
            this.BuildButton.Name = "BuildButton";
            this.BuildButton.Size = new System.Drawing.Size(136, 24);
            this.BuildButton.TabIndex = 9;
            this.BuildButton.Text = "Convert DB to classes";
            this.BuildButton.Click += new System.EventHandler(this.BuildButton_Click);
            // 
            // NamespaceStringText
            // 
            this.NamespaceStringText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NamespaceStringText.Location = new System.Drawing.Point(104, 72);
            this.NamespaceStringText.Name = "NamespaceStringText";
            this.NamespaceStringText.Size = new System.Drawing.Size(488, 20);
            this.NamespaceStringText.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Namespace";
            // 
            // ViewGroup
            // 
            this.ViewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ViewGroup.Controls.Add(this.CreateViewButton);
            this.ViewGroup.Controls.Add(this.label4);
            this.ViewGroup.Controls.Add(this.SQLText);
            this.ViewGroup.Controls.Add(this.label5);
            this.ViewGroup.Controls.Add(this.ViewNameText);
            this.ViewGroup.Enabled = false;
            this.ViewGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ViewGroup.Location = new System.Drawing.Point(8, 208);
            this.ViewGroup.Name = "ViewGroup";
            this.ViewGroup.Size = new System.Drawing.Size(600, 104);
            this.ViewGroup.TabIndex = 5;
            this.ViewGroup.TabStop = false;
            this.ViewGroup.Text = "Create non updateable view class";
            // 
            // CreateViewButton
            // 
            this.CreateViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateViewButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.CreateViewButton.Location = new System.Drawing.Point(456, 72);
            this.CreateViewButton.Name = "CreateViewButton";
            this.CreateViewButton.Size = new System.Drawing.Size(136, 24);
            this.CreateViewButton.TabIndex = 7;
            this.CreateViewButton.Text = "Create view class";
            this.CreateViewButton.Click += new System.EventHandler(this.CreateViewButton_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "SQL";
            // 
            // SQLText
            // 
            this.SQLText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SQLText.Location = new System.Drawing.Point(104, 48);
            this.SQLText.Name = "SQLText";
            this.SQLText.Size = new System.Drawing.Size(488, 20);
            this.SQLText.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 3;
            this.label5.Text = "Name";
            // 
            // ViewNameText
            // 
            this.ViewNameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ViewNameText.Location = new System.Drawing.Point(104, 24);
            this.ViewNameText.Name = "ViewNameText";
            this.ViewNameText.Size = new System.Drawing.Size(488, 20);
            this.ViewNameText.TabIndex = 4;
            // 
            // ProviderList
            // 
            this.ProviderList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProviderList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProviderList.Location = new System.Drawing.Point(112, 16);
            this.ProviderList.Name = "ProviderList";
            this.ProviderList.Size = new System.Drawing.Size(496, 21);
            this.ProviderList.TabIndex = 9;
            this.ProviderList.SelectedIndexChanged += new System.EventHandler(this.ProviderList_SelectedIndexChanged_1);
            // 
            // label6
            // 
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label6.Location = new System.Drawing.Point(8, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "Provider";
            // 
            // UseConfigCheckBox
            // 
            this.UseConfigCheckBox.AutoSize = true;
            this.UseConfigCheckBox.Checked = true;
            this.UseConfigCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseConfigCheckBox.Location = new System.Drawing.Point(112, 40);
            this.UseConfigCheckBox.Name = "UseConfigCheckBox";
            this.UseConfigCheckBox.Size = new System.Drawing.Size(168, 17);
            this.UseConfigCheckBox.TabIndex = 14;
            this.UseConfigCheckBox.Text = "Use a configuration document";
            this.UseConfigCheckBox.UseVisualStyleBackColor = true;
            this.UseConfigCheckBox.CheckedChanged += new System.EventHandler(this.UseConfigCheckBox_CheckedChanged);
            // 
            // EditConfigButton
            // 
            this.EditConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EditConfigButton.Location = new System.Drawing.Point(328, 96);
            this.EditConfigButton.Name = "EditConfigButton";
            this.EditConfigButton.Size = new System.Drawing.Size(120, 24);
            this.EditConfigButton.TabIndex = 14;
            this.EditConfigButton.Text = "Edit configuration";
            this.EditConfigButton.UseVisualStyleBackColor = true;
            this.EditConfigButton.Click += new System.EventHandler(this.EditConfigButton_Click);
            // 
            // DataClassFileBuilderDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(616, 320);
            this.Controls.Add(this.UseConfigCheckBox);
            this.Controls.Add(this.ProviderList);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ViewGroup);
            this.Controls.Add(this.TableGroup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataClassFileBuilderDialog";
            this.Text = "DataClassBuilder";
            this.Load += new System.EventHandler(this.DataClassFileBuilderDialog_Load);
            this.TableGroup.ResumeLayout(false);
            this.TableGroup.PerformLayout();
            this.ViewGroup.ResumeLayout(false);
            this.ViewGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.DoEvents();     //'the MAGIC trick

			DataClassFileBuilderDialog dlg = new DataClassFileBuilderDialog();

			dlg.Providers = GetAvalibleProviders();
			dlg.ProviderList.Items.Clear();
			foreach(IConfigureableDataProvider t in dlg.Providers)
				dlg.ProviderList.Items.Add(t.FriendlyName);

			for(int i = 0; i < dlg.Providers.Length; i++)
			{
				ConfigureProperties prop = dlg.Providers[i].AutoConfigure(args);
				if (prop != null)
				{
					dlg.ProviderList.SelectedIndex = i;
					dlg.ConnectionStringText.Text = prop.Connectionstring;
					dlg.DestinationDirText.Text = prop.DestinationDir;
					dlg.NamespaceStringText.Text = prop.Namespace;
					break;
				}
			}

			//check for params
			if(args.Length > 0 && File.Exists(args[0]) && dlg.ProviderList.SelectedIndex < 0)
			{
				if(Path.GetExtension(args[0]).ToLower() == ".cs")
				{
					try
					{
						XmlDocument meta = GetHiddenXml(args[0], "metadata");
						if(meta != null)
						{
							XmlNode provider = meta.SelectSingleNode("/metadata/provider");
							XmlNode namesp = meta.SelectSingleNode("/metadata/namespace");
							XmlNode name = meta.SelectSingleNode("/metadata/name");
							XmlNode sql = meta.SelectSingleNode("/metadata/sql");
							if(provider != null && provider.Attributes["connectionstring"] != null) dlg.ConnectionStringText.Text = provider.Attributes["connectionstring"].Value;
							if (provider != null && provider.Attributes["name"] != null) dlg.ProviderList.SelectedItem = ParseProvider((string)provider.Attributes["name"].Value, dlg.Providers);
							if(namesp != null) dlg.NamespaceStringText.Text = namesp.InnerText;
							if(name != null) dlg.ViewNameText.Text = name.InnerText;
							if(sql != null) dlg.SQLText.Text = sql.InnerText;
							dlg.DestinationDirText.Text = Path.GetDirectoryName(args[0]);
							string mappingFile = System.IO.Path.Combine(dlg.DestinationDirText.Text, "LightDataModel.Mapping.xml");
							if (System.IO.File.Exists(mappingFile))
								dlg.UseConfigCheckBox.Checked = true;
							else
								dlg.UseConfigCheckBox.Checked = false;
						}
					}
					catch(Exception ex)
					{
						MessageBox.Show("Error during file meta search\nError: " + ex.Message, "DataClassFileBuilderDialog.Main", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}

			System.Windows.Forms.Application.Run(dlg);
		}

		private static string ParseProvider(string providername, IConfigureableDataProvider[] providers)
		{
			foreach (IConfigureableDataProvider p in providers)
			{
				if (p.Name == providername) return p.FriendlyName;
			}
			return "";
		}

		private static XmlDocument GetHiddenXml(string path, string xmltag)
		{
			if(!File.Exists(path)) return null;
			try
			{
				XmlDocument ret = new XmlDocument();
				StreamReader sr = new StreamReader(path);
				string filecontent = sr.ReadToEnd();
				sr.Close();
				int p1 = filecontent.IndexOf("<" + xmltag + ">");
				int p2 = filecontent.IndexOf("</" + xmltag + ">");
				if(p1 >= 0 && p2 > p1)
				{
					string[] lines = filecontent.Substring(p1, p2 - p1 + 11).Split('\n');
					for(int i = 0; i < lines.Length; i++)
						lines[i] = lines[i].TrimStart('/').TrimStart(' ');
					string xml = "";
					for(int i = 0; i < lines.Length; i++)
						xml += lines[i];
					XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(xml));
					ret.Load(reader);
					return ret;
				}
				else return null;
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't read file \"" + path + "\"\nError: " + ex.Message);
			}
		}

		private void BuildButton_Click(object sender, System.EventArgs e)
		{
			//connection
			IDataProvider provider = Providers[ProviderList.SelectedIndex].GetProvider(ConnectionStringText.Text);

			try
			{
				//validate
				if(!DestinationDirText.Text.EndsWith("\\")) DestinationDirText.Text += "\\";
				if(NamespaceStringText.Text == "") throw new Exception("Namespace mustn't be empty");
				string assemblyname = NamespaceStringText.Text;
				int p = assemblyname.LastIndexOf('.');
				if(p >= 0) assemblyname = assemblyname.Substring(p+1);
				if (!System.IO.Directory.Exists(DestinationDirText.Text))
				{
					if (MessageBox.Show("Directory \"" + DestinationDirText.Text + "\" doesn't exists. Create it now?", "Create dir?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
						System.IO.Directory.CreateDirectory(DestinationDirText.Text);
					else
						return;
				}

                List<TypeConfiguration.MappedClass> tables = TypeConfiguration.DescribeDataSource(provider);
                string mappingFile = System.IO.Path.Combine(DestinationDirText.Text, "LightDataModel.Mapping.xml");
                if (UseConfigCheckBox.Checked)
                {
                    List<TypeConfiguration.IgnoredClass> ignored = new List<TypeConfiguration.IgnoredClass>();
                    if (System.IO.File.Exists(mappingFile))
                    {
                        ignored = TypeConfiguration.GetIgnoredTables(mappingFile);
                        TypeConfiguration.MergeSetups(TypeConfiguration.LoadXml(mappingFile), tables);
                        foreach(TypeConfiguration.IgnoredClass ic in ignored)
                            foreach(TypeConfiguration.MappedClass mc in tables)
                                if (mc.TableName == ic.Tablename)
                                {
                                    tables.Remove(mc);
                                    break;
                                }
                    }

                    TypeConfiguration.SaveXml(tables, ignored, mappingFile);
                }

				//tables
                Dictionary<string, TypeConfiguration.MappedClass> tableLookup = new Dictionary<string, TypeConfiguration.MappedClass>();
                foreach (TypeConfiguration.MappedClass mc in tables)
                    tableLookup.Add(mc.TableName, mc);

				//build files
                List<string> paths = new List<string>();
                for (int i = 0; i < tables.Count; i++)
				{
                    paths.Add(System.IO.Path.Combine(DestinationDirText.Text,  tables[i].TableName + ".cs"));
					BuildClassFile(tables[i], paths[paths.Count - 1], NamespaceStringText.Text, provider, typeof(DataClassBase), tableLookup);
				}

				//datahub
				//BuildDataHubFile(DestinationDirText.Text + "DataHub.cs", NamespaceStringText.Text, provider, tablenames, null, null);
				//string[] includes = new string[paths.Length + 1];
				//includes[includes.Length -1] = DestinationDirText.Text + "DataHub.cs";
                if (!UseConfigCheckBox.Checked)
                    mappingFile = null;

				//Build project
				BuildProjectFile(assemblyname, NamespaceStringText.Text, System.IO.Path.Combine(DestinationDirText.Text, assemblyname + ".csproj"), mappingFile, paths.ToArray());
				if(!File.Exists(DestinationDirText.Text + "AssemblyInfo.cs")) BuildAssemblyInfoFile(DestinationDirText.Text + "AssemblyInfo.cs", assemblyname);

				//copy LightDatamodel
				try
				{
					File.Copy(Path.Combine(Application.StartupPath, "LightDatamodel.dll"), Path.Combine(DestinationDirText.Text, "LightDatamodel.dll"), true);
				}
				catch(Exception ex)
				{
					throw new Exception("Couldn't copy LightDatamodel.dll\nError: " + ex.Message);
				}

				//copy certificate
				try
				{
					File.Copy(Path.Combine(Application.StartupPath, "LightDataModel.snk"), Path.Combine(DestinationDirText.Text, "LightDataModel.snk"), true);
				}
				catch(Exception ex)
				{
					throw new Exception("Couldn't copy LightDataModel.snk\nError: " + ex.Message);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Couldn't build files\nError: " + ex.Message);
			}
			finally
			{
				provider.Close();
			}
		}

		#region " project file "

		private void BuildAssemblyInfoFile(string path, string assemblyname)
		{
			StreamWriter sw = null;
			try
			{
				//get template AssemblyInfo
				string AssemblyInfo = "";
				using(StreamReader sr = new StreamReader(this.GetType().Assembly.GetManifestResourceStream("DataClassFileBuilder.TemplateAssemblyInfo.txt")))
					AssemblyInfo = sr.ReadToEnd();

				//insert values
				AssemblyInfo = AssemblyInfo.Replace("<TITLE>", "assemblyname");

				//save
				if(AssemblyInfo != "")
				{
					sw = new StreamWriter(path);
					sw.Write(AssemblyInfo);
				}
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't write AssemblyInfo file\nError: " + ex.Message);
			}
			finally
			{
				if(sw != null) sw.Close();
			}
		}

		private void BuildProjectFile(string assemblyname, string namespacestring, string path, string mappingfile, params string[] includefiles)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
				nm.AddNamespace("", "http://schemas.microsoft.com/developer/msbuild/2003");
				nm.AddNamespace("xs", "http://schemas.microsoft.com/developer/msbuild/2003");		//duh!

				//read or create
				if(File.Exists(path)) 
				{
					doc.Load(path);
				}
				else
				{
					doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));

					//create
					XmlElement tmp;
					XmlElement n = doc.CreateElement("Project", nm.DefaultNamespace);
					n.SetAttribute("DefaultTargets", "Build");
					n.SetAttribute("xmlns", "http://schemas.microsoft.com/developer/msbuild/2003");
					doc.AppendChild(n);
					n = (XmlElement)n.AppendChild(doc.CreateElement("PropertyGroup", nm.DefaultNamespace));
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("ProjectType", nm.DefaultNamespace));
					tmp.InnerText = "Local";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("ProductVersion", nm.DefaultNamespace));
					tmp.InnerText = "8.0.50727";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("SchemaVersion", nm.DefaultNamespace));
					tmp.InnerText = "2.0";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("ProjectGuid", nm.DefaultNamespace));
					tmp.InnerText = "{" + System.Guid.NewGuid().ToString() + "}";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("Configuration", nm.DefaultNamespace));
					tmp.InnerText = "Debug";
					tmp.SetAttribute("Condition", " '$(Configuration)' == '' ");
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("Platform", nm.DefaultNamespace));
					tmp.InnerText = "AnyCPU";
					tmp.SetAttribute("Condition", " '$(Platform)' == '' ");
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("ApplicationIcon", nm.DefaultNamespace));
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("AssemblyKeyContainerName", nm.DefaultNamespace));
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("AssemblyName", nm.DefaultNamespace));
					tmp.InnerText = assemblyname;
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("AssemblyOriginatorKeyFile", nm.DefaultNamespace));
					tmp.InnerText = "LightDatamodel.snk";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("DefaultClientScript", nm.DefaultNamespace));
					tmp.InnerText = "JScript";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("DefaultHTMLPageLayout", nm.DefaultNamespace));
					tmp.InnerText = "Grid";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("DefaultTargetSchema", nm.DefaultNamespace));
					tmp.InnerText = "IE50";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("DelaySign", nm.DefaultNamespace));
					tmp.InnerText = "false";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("OutputType", nm.DefaultNamespace));
					tmp.InnerText = "Library";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("RootNamespace", nm.DefaultNamespace));
					tmp.InnerText = namespacestring;
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("RunPostBuildEvent", nm.DefaultNamespace));
					tmp.InnerText = "OnBuildSuccess";
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("StartupObject", nm.DefaultNamespace));
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("FileUpgradeFlags", nm.DefaultNamespace));
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("UpgradeBackupLocation", nm.DefaultNamespace));
					tmp = (XmlElement)n.AppendChild(doc.CreateElement("SignAssembly", nm.DefaultNamespace));
					tmp.InnerText = "true";



					XmlElement debug = doc.CreateElement("PropertyGroup", nm.DefaultNamespace);
					debug.SetAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ");
					XmlElement release = doc.CreateElement("PropertyGroup", nm.DefaultNamespace);
					release.SetAttribute("Condition", " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ");
					debug.AppendChild(doc.CreateElement("OutputPath", nm.DefaultNamespace)).InnerText = @"bin\Debug\";
					debug.AppendChild(doc.CreateElement("AllowUnsafeBlocks", nm.DefaultNamespace)).InnerText = "false";
					debug.AppendChild(doc.CreateElement("BaseAddress", nm.DefaultNamespace)).InnerText = "285212672";
					debug.AppendChild(doc.CreateElement("CheckForOverflowUnderflow", nm.DefaultNamespace)).InnerText = "false";
					debug.AppendChild(doc.CreateElement("ConfigurationOverrideFile", nm.DefaultNamespace)).InnerText = "";
					debug.AppendChild(doc.CreateElement("DefineConstants", nm.DefaultNamespace)).InnerText = "DEBUG;TRACE";
					debug.AppendChild(doc.CreateElement("DocumentationFile", nm.DefaultNamespace)).InnerText = "";
					debug.AppendChild(doc.CreateElement("DebugSymbols", nm.DefaultNamespace)).InnerText = "true";
					debug.AppendChild(doc.CreateElement("FileAlignment", nm.DefaultNamespace)).InnerText = "4096";
					debug.AppendChild(doc.CreateElement("NoStdLib", nm.DefaultNamespace)).InnerText = "false";
					debug.AppendChild(doc.CreateElement("NoWarn", nm.DefaultNamespace)).InnerText = "";
					debug.AppendChild(doc.CreateElement("Optimize", nm.DefaultNamespace)).InnerText = "false";
					debug.AppendChild(doc.CreateElement("RegisterForComInterop", nm.DefaultNamespace)).InnerText = "false";
					debug.AppendChild(doc.CreateElement("RemoveIntegerChecks", nm.DefaultNamespace)).InnerText = "false";
					debug.AppendChild(doc.CreateElement("TreatWarningsAsErrors", nm.DefaultNamespace)).InnerText = "false";
					debug.AppendChild(doc.CreateElement("WarningLevel", nm.DefaultNamespace)).InnerText = "4";
					debug.AppendChild(doc.CreateElement("DebugType", nm.DefaultNamespace)).InnerText = "full";
					debug.AppendChild(doc.CreateElement("ErrorReport", nm.DefaultNamespace)).InnerText = "prompt";

					release.AppendChild(doc.CreateElement("OutputPath", nm.DefaultNamespace)).InnerText = @"bin\Release\";
					release.AppendChild(doc.CreateElement("AllowUnsafeBlocks", nm.DefaultNamespace)).InnerText = "false";
					release.AppendChild(doc.CreateElement("BaseAddress", nm.DefaultNamespace)).InnerText = "285212672";
					release.AppendChild(doc.CreateElement("CheckForOverflowUnderflow", nm.DefaultNamespace)).InnerText = "false";
					release.AppendChild(doc.CreateElement("ConfigurationOverrideFile", nm.DefaultNamespace)).InnerText = "";
					release.AppendChild(doc.CreateElement("DefineConstants", nm.DefaultNamespace)).InnerText = "TRACE";
					release.AppendChild(doc.CreateElement("DocumentationFile", nm.DefaultNamespace)).InnerText = "";
					release.AppendChild(doc.CreateElement("DebugSymbols", nm.DefaultNamespace)).InnerText = "false";
					release.AppendChild(doc.CreateElement("FileAlignment", nm.DefaultNamespace)).InnerText = "4096";
					release.AppendChild(doc.CreateElement("NoStdLib", nm.DefaultNamespace)).InnerText = "false";
					release.AppendChild(doc.CreateElement("NoWarn", nm.DefaultNamespace)).InnerText = "";
					release.AppendChild(doc.CreateElement("Optimize", nm.DefaultNamespace)).InnerText = "true";
					release.AppendChild(doc.CreateElement("RegisterForComInterop", nm.DefaultNamespace)).InnerText = "false";
					release.AppendChild(doc.CreateElement("RemoveIntegerChecks", nm.DefaultNamespace)).InnerText = "false";
					release.AppendChild(doc.CreateElement("TreatWarningsAsErrors", nm.DefaultNamespace)).InnerText = "false";
					release.AppendChild(doc.CreateElement("WarningLevel", nm.DefaultNamespace)).InnerText = "4";
					release.AppendChild(doc.CreateElement("DebugType", nm.DefaultNamespace)).InnerText = "none";
					release.AppendChild(doc.CreateElement("ErrorReport", nm.DefaultNamespace)).InnerText = "prompt";

					doc["Project"].AppendChild(debug);
					doc["Project"].AppendChild(release);

					XmlElement itemgroup = doc.CreateElement("ItemGroup", nm.DefaultNamespace);
					doc["Project"].AppendChild(itemgroup);
					XmlElement ref1 = doc.CreateElement("Reference", nm.DefaultNamespace);
					XmlElement ref2 = doc.CreateElement("Reference", nm.DefaultNamespace);
					XmlElement ref3 = doc.CreateElement("Reference", nm.DefaultNamespace);
					XmlElement ref4 = doc.CreateElement("Reference", nm.DefaultNamespace);
					itemgroup.AppendChild(ref1);
					itemgroup.AppendChild(ref2);
					itemgroup.AppendChild(ref3);
					itemgroup.AppendChild(ref4);
					ref1.SetAttribute("Include", "System");
					tmp = (XmlElement)ref1.AppendChild(doc.CreateElement("Name", nm.DefaultNamespace));
					tmp.InnerText = "System";
					ref2.SetAttribute("Include", "System.Data");
					tmp = (XmlElement)ref2.AppendChild(doc.CreateElement("Name", nm.DefaultNamespace));
					tmp.InnerText = "System.Data";
					ref3.SetAttribute("Include", "System.XML");
					tmp = (XmlElement)ref3.AppendChild(doc.CreateElement("Name", nm.DefaultNamespace));
					tmp.InnerText = "System.XML";
					ref4.SetAttribute("Include", "LightDatamodel");
					tmp = (XmlElement)ref4.AppendChild(doc.CreateElement("Name", nm.DefaultNamespace));
					tmp.InnerText = "LightDatamodel";
					tmp = (XmlElement)ref4.AppendChild(doc.CreateElement("HintPath", nm.DefaultNamespace));
					tmp.InnerText = ".\\LightDatamodel.dll";

					tmp = (XmlElement)doc["Project"].AppendChild(doc.CreateElement("ItemGroup", nm.DefaultNamespace));
					tmp = (XmlElement)tmp.AppendChild(doc.CreateElement("Compile", nm.DefaultNamespace));
					tmp.SetAttribute("Include", "AssemblyInfo.cs");

					tmp = (XmlElement)doc["Project"].AppendChild(doc.CreateElement("ItemGroup", nm.DefaultNamespace));
					tmp = (XmlElement)tmp.AppendChild(doc.CreateElement("None", nm.DefaultNamespace));
					tmp.SetAttribute("Include", "LightDatamodel.snk");

					tmp = (XmlElement)doc["Project"].AppendChild(doc.CreateElement("Import", nm.DefaultNamespace));
					tmp.SetAttribute("Project", @"$(MSBuildBinPath)\Microsoft.CSharp.targets");

				}

				//2005 project
				{
					XmlNode includes = doc.SelectSingleNode("/xs:Project/xs:ItemGroup/xs:Reference", nm).ParentNode;
					XmlNode files = doc.SelectSingleNode("/xs:Project/xs:ItemGroup/xs:Compile", nm).ParentNode;
					if (files.SelectSingleNode("xs:Compile[@Include='AssemblyInfo.cs']", nm) == null)
					{
						XmlElement file = doc.CreateElement("Compile", nm.DefaultNamespace);
						file.SetAttribute("Include", "AssemblyInfo.cs");
						files.AppendChild(file);
					}

					//insert includes
					string fn;
					foreach(string filepath in includefiles)
					{
						fn = Path.GetFileName(filepath);
						XmlNode n = files.SelectSingleNode("xs:Compile[@Include='" + fn + "']", nm);
						if(n == null)
						{
							XmlElement file = doc.CreateElement("Compile", nm.DefaultNamespace);
							file.SetAttribute("Include", fn);
							files.AppendChild(file);
						}
					}

                    if (mappingfile != null)
                    {
						XmlNode n = files.SelectSingleNode("xs:EmbeddedResource[@Include='" + mappingfile + "']", nm);
                        if (n == null)
                        {
							XmlElement file = doc.CreateElement("EmbeddedResource", nm.DefaultNamespace);
                            file.SetAttribute("Include", mappingfile);
                            files.AppendChild(file);
                        }
                    }

				}

				//save, one wonders why the default .Net encoding is not set to default...
				using(System.IO.StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
					sw.Write(doc.OuterXml);
			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't create project file\nError: " + ex.Message);
			}
		}

		#endregion

		private void BuildDataHubFile(string path, string namespacestring, IDataProvider provider, string[] tables, string[] views, string[] viewssql)
		{
			//Save (means keep) existing file
			string unsynchronizedcode = "";
			string unsynchronizedcodestart = "#region \" Unsynchronized Custom Code Region \"\n";
			string unsynchronizedcodeend = "#endregion\n";
			string unsynchronizedincludesstart = "#region \" Unsynchronized Includes \"\n";
			string unsynchronizedincludesend = "#endregion\n";
			string unsynchronizedincludes = "";
			string viewscode = "";
			string viewscodestart = "#region \" Views Syncronized Code Region \"\n";
			string viewscodeend = "#endregion\n";
			if(File.Exists(path))
			{
				StreamReader sr = new StreamReader(path);
				string filecontent = sr.ReadToEnd();
				sr.Close();

				//includes
				int p = filecontent.IndexOf(unsynchronizedincludesstart);
				if(p >= 0)
				{
					int p2 = filecontent.IndexOf(unsynchronizedincludesend, p);
					if(p2 >= 0)
						unsynchronizedincludes = filecontent.Substring(p + unsynchronizedincludesstart.Length, p2 - (p+unsynchronizedincludesstart.Length));
				}

				//code
				p = filecontent.IndexOf(unsynchronizedcodestart);
				if(p >= 0)
				{
					int p2 = filecontent.IndexOf(unsynchronizedcodeend, p);
					if(p2 >= 0)
						unsynchronizedcode = filecontent.Substring(p + unsynchronizedcodestart.Length, p2 - (p+unsynchronizedcodestart.Length));
				}

				//views
				p = filecontent.IndexOf(viewscodestart);
				if(p >= 0)
				{
					int p2 = filecontent.IndexOf(viewscodeend, p);
					if(p2 >= 0)
						viewscode = filecontent.Substring(p + viewscodestart.Length, p2 - (p+viewscodestart.Length));
				}
			}

			StreamWriter sw = null;
			try
			{

				//build file
				sw = new StreamWriter(path);

				//syncronized includes
				sw.Write("using System;\n");
				sw.Write("using System.Data.LightDatamodel;\n");

				//Unsynchronized includes
				sw.Write(unsynchronizedincludesstart);
				sw.Write(unsynchronizedincludes);
				if(unsynchronizedincludes == "") sw.Write("\n\t//Don't put any region sections in here\n\n");
				sw.Write(unsynchronizedincludesend + "\n");

				//metadata
				sw.Write("/// <metadata>\n");
				sw.Write("/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>\n");
				sw.Write("/// <provider name=\"" + provider.ToString() + "\" connectionstring=\"" + provider.ConnectionString + "\" />\n");
				sw.Write("/// <type>" + "DataHub" + "</type>\n");
				sw.Write("/// <namespace>" + namespacestring + "</namespace>\n");
				sw.Write("/// </metadata>\n\n");

				//namespace
				sw.Write("namespace " + namespacestring + "\n{\n\n");

				//class
				sw.Write("\tpublic class " + "DataHub" + " : System.Data.LightDatamodel.DataFetcher\n\t{\n\n");

				//private members
				sw.Write("#region \" private members \"\n\n");
				if(tables != null)
					for(int i = 0; i< tables.Length; i++)
					{
						sw.Write("\t\tprivate " + tables[i] + "Collection m_" + tables[i] + ";\n");
					}
				sw.Write("\n#endregion\n\n");

				//ctor
				sw.Write("\t\tpublic DataHub() : base(new " + provider.ToString() + "(\"" + provider.ConnectionString.Replace("\\","\\\\").Replace("\"","\\\"") + "\"))\n");
				sw.Write("\t\t{\n");
				sw.Write("\t\t}\n");
				sw.Write("\t\tpublic DataHub(IDataProvider provider) : base(provider)\n");
				sw.Write("\t\t{\n");
				sw.Write("\t\t}\n\n");
				sw.Write("\t\tpublic DataHub(string connectionstring) : base(new " +  provider.ToString() + "(connectionstring))\n");
				sw.Write("\t\t{\n");
				sw.Write("\t\t}\n\n");
				sw.Write("\t\tpublic DataHub(System.Data.IDbConnection connection) : base(new " +  provider.ToString() + "(connection))\n");
				sw.Write("\t\t{\n");
				sw.Write("\t\t}\n\n");

				//properties
				sw.Write("#region \" properties \"\n\n");
				if(tables != null)
					for(int i = 0; i< tables.Length; i++)
					{
						sw.Write("\t\tpublic " + tables[i] + "Collection " + tables[i] + "\n\t\t{\n");
						sw.Write("\t\t\tget\n\t\t\t{\n");
						sw.Write("\t\t\t\tif(m_" + tables[i] + " == null)\n");
						sw.Write("\t\t\t\t{\n");
						sw.Write("\t\t\t\t\tobject[] arr = GetObjects(typeof(" + tables[i] + "), \"\");\n");
						sw.Write("\t\t\t\t\tm_" + tables[i] + " = new " + tables[i] + "Collection();\n");
						sw.Write("\t\t\t\t\tm_" + tables[i] + ".AddRange(arr);\n");
						sw.Write("\t\t\t\t}\n");
						sw.Write("\t\t\t\treturn m_" + tables[i] + ";\n");
						sw.Write("\t\t\t}\n\t\t}\n\n");
					}
				sw.Write("#endregion\n\n");

				//views
				sw.Write(viewscodestart);
				sw.Write(viewscode);
				if(viewscode == "")
				{
					sw.Write("\n\t//Don't put any region sections in here\n\n");
					sw.Write("/// <views>\n");
					sw.Write("/// </views>\n\n");
				}
				sw.Write(viewscodeend + "\n");

				//Unsynchronized Custom Code Region
				sw.Write(unsynchronizedcodestart);
				sw.Write(unsynchronizedcode);
				if(unsynchronizedcode == "") sw.Write("\n\t//Don't put any region sections in here\n\n");
				sw.Write(unsynchronizedcodeend + "\n");

				sw.Write("\t}\n}");	//end namespace & class

			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't write file \"" + path + "\"\nError: " + ex.Message);
			}
			finally
			{
				//close file
				if(sw != null) sw.Close();
			}
		}

        private void BuildClassFile(TypeConfiguration.MappedClass mapping, string path, string namespacestring, IDataProvider provider, Type inheritclass, Dictionary<string, TypeConfiguration.MappedClass> tableLookup)
		{
			//get columns
            Dictionary<string, TypeConfiguration.MappedField> cols = mapping.Columns;

			//get primary key col
			string primarykeycol = mapping.PrimaryKey == null ? "" : mapping.UniqueColumn;

            string primaryAutoGenerate = "";
            if (mapping.PrimaryKey != null && mapping.PrimaryKey.IsAutoGenerated)
                primaryAutoGenerate = "\t\t[System.Data.LightDatamodel.MemberModifierAutoIncrement()]\n";

			try
			{

				//build file
                using (StreamWriter sw = new StreamWriter(path))
                {

                    //metadata
                    sw.Write("/// <metadata>\n");
                    sw.Write("/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>\n");
                    sw.Write("/// <provider name=\"" + provider.ToString() + "\" connectionstring=\"" + provider.ConnectionString + "\" />\n");
                    sw.Write("/// <type>" + (mapping.ViewSQL == null ? "Table" : "View") + "</type>\n");
                    sw.Write("/// <namespace>" + namespacestring + "</namespace>\n");
                    sw.Write("/// <name>" + mapping.TableName + "</name>\n");
                    sw.Write("/// <sql>" + (mapping.ViewSQL == null ? "" : mapping.ViewSQL)  + "</sql>\n");
                    sw.Write("/// </metadata>\n\n");

                    string classname = mapping.ClassName;
                    //namespace
                    if (classname.IndexOf(".") > 0)
                    {
                        classname = classname.Substring(classname.LastIndexOf(".") + 1);
                        sw.Write("namespace " + mapping.ClassName.Substring(0, mapping.ClassName.Length - classname.Length - 1) + "\n{\n\n");
                    }
                    else
                        sw.Write("namespace " + namespacestring + "\n{\n\n");

                    //class
                    sw.Write("\tpublic partial class " + classname + " : System.Data.LightDatamodel." + inheritclass.Name + "\n\t{\n\n");

                    //private members
                    sw.Write("#region \" private members \"\n\n");
                    foreach(TypeConfiguration.MappedField mf in mapping.Columns.Values)
                    {
                        if (mf.IsPrimaryKey)
                            sw.Write(primaryAutoGenerate);
                        else if (mf.IgnoreWithInsert)
                            sw.Write("\t\t[System.Data.LightDatamodel.MemberModifierIgnoreWithInsert()]\n");
                        if (mf.IgnoreWithUpdate)
                            sw.Write("\t\t[System.Data.LightDatamodel.MemberModifierIgnoreWithUpdate()]\n");
                        if (mf.IgnoreWithSelect)
                            sw.Write("\t\t[System.Data.LightDatamodel.MemberModifierIgnoreWithSelect()]\n");

                        sw.Write("\t\tprivate " + mf.DataType.FullName + " " + mf.FieldName + " = " + FormatObjectToCSharp(provider.GetDefaultValue(mapping.TableName, mf.ColumnName)) + ";\n");
                    }
                    sw.Write("#endregion\n\n");

                    //overrides
                    if (mapping.ViewSQL == null)
                    {
                        sw.Write("#region \" unique value \"\n\n");
                        sw.Write("\t\tpublic override object UniqueValue {get{return " + (mapping.PrimaryKey  != null ? mapping.PrimaryKey.FieldName : "null") + ";}}\n");
                        sw.Write("\t\tpublic override string UniqueColumn {get{return \"" + (mapping.PrimaryKey  != null ? mapping.UniqueColumn : "") + "\";}}\n");
                        sw.Write("#endregion\n\n");
                    }

                    //properties
                    sw.Write("#region \" properties \"\n\n");
                    foreach(TypeConfiguration.MappedField mf in mapping.Columns.Values)
                    {
                        if (mf.PropertyName == null || mf.PropertyName.Length == 0)
                            continue;

                        sw.Write("\t\tpublic " + mf.DataType.FullName + " " + mf.PropertyName + "\n\t\t{\n");
                        sw.Write("\t\t\tget{return " + mf.FieldName + ";}\n");
                        if (mapping.ViewSQL == null)
                            sw.Write("\t\t\tset{object oldvalue = " + mf.FieldName + ";OnBeforeDataChange(this, \"" + mf.ColumnName + "\", oldvalue, value);" + mf.FieldName + " = value;OnAfterDataChange(this, \"" + mf.ColumnName + "\", oldvalue, value);}\n");
                        else
                            sw.Write("\t\t\tset{" + mf.FieldName + " = value;}\n");
                        sw.Write("\t\t}\n\n");
                    }
                    sw.Write("#endregion\n\n");

                    //references
                    sw.Write("#region \" referenced properties \"\n\n");
                    foreach (TypeConfiguration.ReferenceField rf in mapping.ReferenceColumns.Values)
                    {
                        sw.Write("\t\tpublic " + tableLookup[rf.ReverseTablename].ClassName + " " + rf.PropertyName + "\n\t\t{\n");
                        sw.Write("\t\t\tget{ return base.RelationManager.GetReferenceObject<" + tableLookup[rf.ReverseTablename].ClassName + ">(this, \"" + rf.PropertyName + "\"); }\n");
                        sw.Write("\t\t\tset{ base.RelationManager.SetReferenceObject<" + tableLookup[rf.ReverseTablename].ClassName + ">(this, \"" + rf.PropertyName + "\", value); }\n");
                        sw.Write("\t\t}\n\n");
                    }

                    foreach(TypeConfiguration.MappedClass mc in tableLookup.Values)
                        if (mc != mapping)
                        {
                            foreach (TypeConfiguration.ReferenceField rf in mc.ReferenceColumns.Values)
                            {
                                if (rf.ReverseTablename == mapping.TableName)
                                {
                                    if (rf.IsCollection)
                                    {
                                        sw.Write("\t\tprivate System.Data.LightDatamodel.SyncCollectionBase<" + mc.ClassName + "> m_" + rf.ReversePropertyName + ";\n");
                                        sw.Write("\t\tpublic System.Data.LightDatamodel.SyncCollectionBase<" + mc.ClassName + "> " + rf.ReversePropertyName + "\n\t\t{\n");
                                        sw.Write("\t\t\tget\n\t\t\t{\n");
                                        sw.Write("\t\t\t\tif (m_" + rf.ReversePropertyName + " == null)\n");
                                        sw.Write("\t\t\t\t\tm_" + rf.ReversePropertyName + " = base.RelationManager.GetReferenceCollection<" + mc.ClassName + ">(this, \"" + rf.PropertyName + "\");\n");
                                        sw.Write("\t\t\t\treturn m_" + rf.ReversePropertyName + ";\n");
                                        sw.Write("\t\t\t}\n");
                                        sw.Write("\t\t}\n\n");
                                    }
                                    else
                                    {
                                        sw.Write("\t\tpublic " + mc.ClassName + " " + rf.ReversePropertyName + "\n\t\t{\n");
                                        sw.Write("\t\t\tget{ return base.RelationManager.GetReferenceObject<" + mc.ClassName + ">(this, \"" + rf.PropertyName + "\"); }\n");
                                        sw.Write("\t\t\tset{ base.RelationManager.SetReferenceObject<" + mc.ClassName + ">(this, \"" + rf.PropertyName + "\", value); }\n");
                                        sw.Write("\t\t}\n\n");
                                    }
                                }
                            }
                        }

                    sw.Write("#endregion\n\n");

                    sw.Write("\t}\n"); //end class
                    sw.Write("\n}"); //end namespace
                }

			}
			catch(Exception ex)
			{
				throw new Exception("Couldn't write file \"" + path + "\"\nError: " + ex.Message);
			}
		}

		private string FormatObjectToCSharp(object obj)
		{
			if (obj == null)
				return "null";
			else if (obj.GetType() == typeof(string))
				return "\"" + obj.ToString() + "\"";
			else if (obj.GetType() == typeof(DateTime))
			{
				DateTime d = (DateTime)obj;
				return "new System.DateTime(" + d.Year.ToString() + ", " + d.Month.ToString() + ", " + d.Day.ToString() + ")";
			}
			else return obj.ToString();
		}

		private void BrowseButton_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.ShowNewFolderButton = true;
			//OpenFileDialog dlg = new OpenFileDialog();
			//dlg.Filter = "Alle filer (*.*)|*.*";
			if(dlg.ShowDialog() == DialogResult.OK)
			{
				DestinationDirText.Text = dlg.SelectedPath; //Path.GetDirectoryName(dlg.FileName);
			}
		}

		private void CreateViewButton_Click(object sender, System.EventArgs e)
		{
			//connection
			IDataProvider provider = Providers[ProviderList.SelectedIndex].GetProvider(ConnectionStringText.Text);

			try
			{
				//validate
				if(!DestinationDirText.Text.EndsWith("\\")) DestinationDirText.Text += "\\";
				if(NamespaceStringText.Text == "") throw new Exception("Namespace mustn't be empty");
				string assemblyname = NamespaceStringText.Text;
				int p = assemblyname.LastIndexOf('.');
				if(p >= 0) assemblyname = assemblyname.Substring(p+1);

                TypeConfiguration.MappedClass config = TypeConfiguration.DescribeDataSource(provider, ViewNameText.Text, SQLText.Text);

				//Build view
				BuildClassFile(config, System.IO.Path.Combine(DestinationDirText.Text, ViewNameText.Text + ".cs"), NamespaceStringText.Text, provider, typeof(DataClassView), new Dictionary<string,TypeConfiguration.MappedClass>());

				//Build datahub
				BuildDataHubFile(DestinationDirText.Text + "DataHub.cs", NamespaceStringText.Text, provider, null, new string[]{ViewNameText.Text}, new string[]{SQLText.Text});

				//Build project
				BuildProjectFile(assemblyname, NamespaceStringText.Text, System.IO.Path.Combine(DestinationDirText.Text, assemblyname + ".csproj"), null, System.IO.Path.Combine(DestinationDirText.Text, ViewNameText.Text + ".cs"));
				if(!File.Exists(DestinationDirText.Text + "AssemblyInfo.cs")) BuildAssemblyInfoFile(DestinationDirText.Text + "AssemblyInfo.cs", assemblyname);

				//copy LightDatamodel
				try
				{
					File.Copy(Application.StartupPath + "\\LightDatamodel.dll", DestinationDirText.Text + "\\LightDatamodel.dll", true);
				}
				catch(Exception ex)
				{
					throw new Exception("Couldn't copy LightDatamodel.dll\nError: " + ex.Message);
				}

				//copy certificat
				try
				{
					File.Copy(Application.StartupPath + "\\..\\..\\GEOGRAF.snk", DestinationDirText.Text + "\\GEOGRAF.snk", true);
				}
				catch(Exception ex)
				{
					throw new Exception("Couldn't copy LightDatamodel.dll\nError: " + ex.Message);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show("Couldn't build files\nError: " + ex.Message);
			}
			finally
			{
				provider.Close();
			}
		}

		private static IConfigureableDataProvider[] GetAvalibleProviders()
		{
			ArrayList files = new ArrayList();
			ArrayList types = new ArrayList();


			files.Add(Path.Combine(Application.StartupPath, "LightDataModel.dll"));
			string folder = Path.Combine(Application.StartupPath, "Providers");
			if (System.IO.Directory.Exists(folder))
				files.AddRange(System.IO.Directory.GetFiles(folder, "*.dll"));

			foreach(string s in files)
				try 
				{
					System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFile(s);
					foreach(Type t in asm.GetExportedTypes())
						if (t.IsClass && t.GetInterface(typeof(System.Data.LightDatamodel.IConfigureableDataProvider).FullName) != null)
							types.Add(Activator.CreateInstance(t));
				}
				catch {	}

			return (IConfigureableDataProvider[])types.ToArray(typeof(IConfigureableDataProvider));
		}

		private void BrowseDB_Click(object sender, System.EventArgs e)
		{
			ConfigureProperties prop = new ConfigureProperties();
			prop.Connectionstring = ConnectionStringText.Text;
			prop.DestinationDir = DestinationDirText.Text;
			prop.Namespace = NamespaceStringText.Text;

			prop = Providers[ProviderList.SelectedIndex].Configure(this, prop);
			if (prop != null)
			{
				ConnectionStringText.Text = prop.Connectionstring;
				DestinationDirText.Text = prop.DestinationDir;
				NamespaceStringText.Text = prop.Namespace;
			}
		
		}

		private void ProviderList_SelectedIndexChanged_1(object sender, System.EventArgs e)
		{
			TableGroup.Enabled = ViewGroup.Enabled = ProviderList.SelectedIndex >= 0;
		}

        private void DataClassFileBuilderDialog_Load(object sender, EventArgs e)
        {
            EditConfigButton.Enabled = UseConfigCheckBox.Checked;
        }

        private void UseConfigCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            EditConfigButton.Enabled = UseConfigCheckBox.Checked;
        }

        private void EditConfigButton_Click(object sender, EventArgs e)
        {

            try
            {
                IDataProvider provider = Providers[ProviderList.SelectedIndex].GetProvider(ConnectionStringText.Text);
                if (!System.IO.Directory.Exists(DestinationDirText.Text))
                {
                    if (MessageBox.Show(this, "Directory \"" + DestinationDirText.Text + "\" doesn't exists. Create it now?", "Create dir?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        System.IO.Directory.CreateDirectory(DestinationDirText.Text);
                    else
                        return;
                }

                List<TypeConfiguration.MappedClass> tables = TypeConfiguration.DescribeDataSource(provider);
                string mappingFile = System.IO.Path.Combine(DestinationDirText.Text, "LightDataModel.Mapping.xml");

                List<TypeConfiguration.IgnoredClass> ignored = new List<TypeConfiguration.IgnoredClass>();
                if (System.IO.File.Exists(mappingFile))
                {
                    ignored = TypeConfiguration.GetIgnoredTables(mappingFile);
                    TypeConfiguration.MergeSetups(TypeConfiguration.LoadXml(mappingFile), tables);
                    foreach (TypeConfiguration.IgnoredClass ic in ignored)
                        foreach (TypeConfiguration.MappedClass mc in tables)
                            if (mc.TableName == ic.Tablename)
                            {
                                tables.Remove(mc);
                                break;
                            }
                }

                DataClassCustomizer dlg = new DataClassCustomizer(tables, ignored);
                if (dlg.ShowDialog() == DialogResult.OK)
                    TypeConfiguration.SaveXml(dlg.Tables, dlg.Ignored, mappingFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Error occured: {0}.\nChanges may not be correctly saved", ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


	}
}
