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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(DataClassFileBuilderDialog));
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
			this.TableGroup.SuspendLayout();
			this.ViewGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// TableGroup
			// 
			this.TableGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
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
			this.TableGroup.Location = new System.Drawing.Point(8, 48);
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
			this.ConnectionStringText.Text = "";
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
			this.DestinationDirText.Text = "";
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
			this.NamespaceStringText.Text = "";
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
			this.ViewGroup.Location = new System.Drawing.Point(8, 184);
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
			this.SQLText.Text = "";
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
			this.ViewNameText.Text = "";
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
			// DataClassFileBuilderDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(616, 301);
			this.Controls.Add(this.ProviderList);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.ViewGroup);
			this.Controls.Add(this.TableGroup);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DataClassFileBuilderDialog";
			this.Text = "DataClassBuilder";
			this.TableGroup.ResumeLayout(false);
			this.ViewGroup.ResumeLayout(false);
			this.ResumeLayout(false);

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
							//TODO: Select correct provider on the list
							XmlNode provider = meta.SelectSingleNode("/metadata/provider");
							XmlNode namesp = meta.SelectSingleNode("/metadata/namespace");
							XmlNode name = meta.SelectSingleNode("/metadata/name");
							XmlNode sql = meta.SelectSingleNode("/metadata/sql");
							if(provider != null && provider.Attributes["connectionstring"] != null) dlg.ConnectionStringText.Text = provider.Attributes["connectionstring"].Value;
							if(namesp != null) dlg.NamespaceStringText.Text = namesp.InnerText;
							if(name != null) dlg.ViewNameText.Text = name.InnerText;
							if(sql != null) dlg.SQLText.Text = sql.InnerText;
							dlg.DestinationDirText.Text = Path.GetDirectoryName(args[0]);
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

				//tables
				string[] tablenames = provider.GetTablenames();

				//build files
				string[] paths = new string[tablenames.Length];
				for(int i = 0; i< tablenames.Length; i++)
				{
					paths[i] = DestinationDirText.Text + tablenames[i] + ".cs";
					BuildClassFile(tablenames[i], null, paths[i], NamespaceStringText.Text, provider, typeof(DataClassBase));
				}

				//datahub
				//BuildDataHubFile(DestinationDirText.Text + "DataHub.cs", NamespaceStringText.Text, provider, tablenames, null, null);
				//string[] includes = new string[paths.Length + 1];
				string[] includes = new string[paths.Length];
				Array.Copy(paths, includes, paths.Length);
				//includes[includes.Length -1] = DestinationDirText.Text + "DataHub.cs";

				//Build project
				BuildProjectFile(assemblyname, NamespaceStringText.Text, DestinationDirText.Text + assemblyname + ".csproj", includes);
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

				//copy certificat
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

		private void BuildProjectFile(string assemblyname, string namespacestring, string path, params string[] includefiles)
		{
			try
			{
				XmlDocument doc = new XmlDocument();

				//read or create
				if(File.Exists(path)) 
				{
					 //one wonders why the default .Net encoding is not set to default...
					using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default, true))
						doc.Load(sr);
				}
				else
				{
					//create
					XmlElement root = doc.CreateElement("VisualStudioProject");
					doc.AppendChild(root);
					XmlElement csharp = doc.CreateElement("CSHARP");
					csharp.SetAttribute("ProjectType","Local");
					csharp.SetAttribute("ProductVersion", "7.10.3077");
					csharp.SetAttribute("SchemaVersion", "2.0");
					csharp.SetAttribute("ProjectGuid", "{" + System.Guid.NewGuid().ToString() + "}");
					root.AppendChild(csharp);
					XmlElement build = doc.CreateElement("Build");
					csharp.AppendChild(build);
					XmlElement settings = doc.CreateElement("Settings");
					build.AppendChild(settings);
					settings.SetAttribute("ApplicationIcon","");
					settings.SetAttribute("AssemblyKeyContainerName","");
					settings.SetAttribute("AssemblyName",assemblyname);
					settings.SetAttribute("AssemblyOriginatorKeyFile","");
					settings.SetAttribute("DefaultClientScript","JScript");
					settings.SetAttribute("DefaultHTMLPageLayout","Grid");
					settings.SetAttribute("DefaultTargetSchema","IE50");
					settings.SetAttribute("DelaySign","false");
					settings.SetAttribute("OutputType","Library");
					settings.SetAttribute("PreBuildEvent","");
					settings.SetAttribute("PostBuildEvent","");
					settings.SetAttribute("RootNamespace",namespacestring);
					settings.SetAttribute("RunPostBuildEvent","OnBuildSuccess");
					settings.SetAttribute("StartupObject","");
					XmlElement debug = doc.CreateElement("Config");
					XmlElement release = doc.CreateElement("Config");
					debug.SetAttribute("Name","Debug");
					debug.SetAttribute("AllowUnsafeBlocks","false");
					debug.SetAttribute("BaseAddress","285212672");
					debug.SetAttribute("CheckForOverflowUnderflow","false");
					debug.SetAttribute("ConfigurationOverrideFile","");
					debug.SetAttribute("DefineConstants","DEBUG;TRACE");
					debug.SetAttribute("DocumentationFile","");
					debug.SetAttribute("DebugSymbols","true");
					debug.SetAttribute("FileAlignment","4096");
					debug.SetAttribute("IncrementalBuild","false");
					debug.SetAttribute("NoStdLib","false");
					debug.SetAttribute("NoWarn","");
					debug.SetAttribute("Optimize","false");
					debug.SetAttribute("OutputPath","bin\\Debug\\");
					debug.SetAttribute("RegisterForComInterop","false");
					debug.SetAttribute("RemoveIntegerChecks","false");
					debug.SetAttribute("TreatWarningsAsErrors","false");
					debug.SetAttribute("WarningLevel","4");
					release.SetAttribute("Name","Release");
					release.SetAttribute("AllowUnsafeBlocks","false");
					release.SetAttribute("BaseAddress","285212672");
					release.SetAttribute("CheckForOverflowUnderflow","false");
					release.SetAttribute("ConfigurationOverrideFile","");
					release.SetAttribute("DefineConstants","TRACE");
					release.SetAttribute("DocumentationFile","");
					release.SetAttribute("DebugSymbols","false");
					release.SetAttribute("FileAlignment","4096");
					release.SetAttribute("IncrementalBuild","false");
					release.SetAttribute("NoStdLib","false");
					release.SetAttribute("NoWarn","");
					release.SetAttribute("Optimize","true");
					release.SetAttribute("OutputPath","bin\\Release\\");
					release.SetAttribute("RegisterForComInterop","false");
					release.SetAttribute("RemoveIntegerChecks","false");
					release.SetAttribute("TreatWarningsAsErrors","false");
					release.SetAttribute("WarningLevel","4");
					settings.AppendChild(debug);
					settings.AppendChild(release);
					XmlElement references = doc.CreateElement("References");
					build.AppendChild(references);
					XmlElement ref1 = doc.CreateElement("Reference");
					XmlElement ref2 = doc.CreateElement("Reference");
					XmlElement ref3 = doc.CreateElement("Reference");
					XmlElement ref4 = doc.CreateElement("Reference");
					references.AppendChild(ref1);
					references.AppendChild(ref2);
					references.AppendChild(ref3);
					references.AppendChild(ref4);
					ref1.SetAttribute("Name","System");
					ref1.SetAttribute("AssemblyName","System");
					ref2.SetAttribute("Name","System.Data");
					ref2.SetAttribute("AssemblyName","System.Data");
					ref3.SetAttribute("Name","System.XML");
					ref3.SetAttribute("AssemblyName","System.XML");
					ref4.SetAttribute("Name","LightDatamodel");
					ref4.SetAttribute("AssemblyName","LightDatamodel");
					ref4.SetAttribute("HintPath",".\\LightDatamodel.dll");
					XmlElement files = doc.CreateElement("Files");
					csharp.AppendChild(files);
					XmlElement include = doc.CreateElement("Include");
					files.AppendChild(include);
				}

				if (doc.SelectSingleNode("/VisualStudioProject") != null)
				{

					XmlNode includes = doc.SelectSingleNode("/VisualStudioProject/CSHARP/Files/Include");

					//check for AssemblyInfo
					if(doc.SelectSingleNode("/VisualStudioProject/CSHARP/Files/Include/File[@RelPath='AssemblyInfo.cs']") == null)
					{
						XmlElement file = doc.CreateElement("File");
						file.SetAttribute("RelPath","AssemblyInfo.cs");
						file.SetAttribute("SubType","Code");
						file.SetAttribute("BuildAction","Compile");
						includes.AppendChild(file);
					}

					//insert includes
					string fn;
					foreach(string filepath in includefiles)
					{
						fn = Path.GetFileName(filepath);
						XmlNode n = doc.SelectSingleNode("/VisualStudioProject/CSHARP/Files/Include/File[@RelPath='" + fn + "']");
						if(n == null)
						{
							XmlElement file = doc.CreateElement("File");
							file.SetAttribute("RelPath",fn);
							file.SetAttribute("SubType","Code");
							file.SetAttribute("BuildAction","Compile");
							includes.AppendChild(file);
						}
					}
				}
				else //2005 project
				{
					XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
					nm.AddNamespace("xs", "http://schemas.microsoft.com/developer/msbuild/2003");
					XmlNode includes = doc.SelectSingleNode("/xs:Project/xs:ItemGroup/xs:Reference", nm).ParentNode;
					XmlNode files = doc.SelectSingleNode("/xs:Project/xs:ItemGroup/xs:Compile", nm).ParentNode;
					if (files.SelectSingleNode("xs:Compile[@Include='AssemblyInfo.cs']", nm) == null)
					{
						XmlElement file = doc.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
						file.SetAttribute("Include", "AssemblyInfo.cs");
						XmlElement type = doc.CreateElement("SubType", "http://schemas.microsoft.com/developer/msbuild/2003");
						type.InnerText = "Code";
						file.AppendChild(type);
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
							XmlElement file = doc.CreateElement("Compile", "http://schemas.microsoft.com/developer/msbuild/2003");
							file.SetAttribute("Include", fn);
							XmlElement type = doc.CreateElement("SubType", "http://schemas.microsoft.com/developer/msbuild/2003");
							type.InnerText = "Code";
							file.AppendChild(type);
							files.AppendChild(file);
						}
					}

				}

				//save, one wonders why the default .Net encoding is not set to default...
				using(System.IO.StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
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

		private void BuildClassFile(string name, string sql, string path, string namespacestring, IDataProvider provider, Type inheritclass)
		{
			//Save (means keep) existing file
			string unsynchronizedcode = "";
			string unsynchronizedcodestart = "#region \" Unsynchronized Custom Code Region \"\n";
			string unsynchronizedcodeend = "#endregion\n";
			string unsynchronizedincludesstart = "#region \" Unsynchronized Includes \"\n";
			string unsynchronizedincludesend = "#endregion\n";
			string unsynchronizedincludes = "";
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
			}

			//get columns
			Data[] cols = null;
			if(sql != null) cols= provider.GetStructure(sql);
			else cols = provider.GetTableStructure(name);

			//get primary key col
			string primarykeycol = "";
			if(sql == null) primarykeycol= provider.GetPrimaryKey(name);

			StreamWriter sw = null;
			try
			{

				//build file
				sw = new StreamWriter(path);

				//Unsynchronized includes
				sw.Write(unsynchronizedincludesstart);
				sw.Write(unsynchronizedincludes);
				if(unsynchronizedincludes == "") sw.Write("\n\t//Don't put any region sections in here\n\n");
				sw.Write(unsynchronizedincludesend + "\n");

				//metadata
				sw.Write("/// <metadata>\n");
				sw.Write("/// <creator>This class was created by DataClassFileBuilder (LightDatamodel)</creator>\n");
				sw.Write("/// <provider name=\"" + provider.ToString() + "\" connectionstring=\"" + provider.ConnectionString + "\" />\n");
				sw.Write("/// <type>" + (sql == null ? "Table" : "View") + "</type>\n");
				sw.Write("/// <namespace>" + namespacestring + "</namespace>\n");
				sw.Write("/// <name>" + (string)name + "</name>\n");
				sw.Write("/// <sql>" + (string)sql + "</sql>\n");
				sw.Write("/// </metadata>\n\n");

				//namespace
				sw.Write("namespace " + namespacestring + "\n{\n\n");

				//class
				sw.Write("\tpublic class " + name + " : System.Data.LightDatamodel." + inheritclass.Name + "\n\t{\n\n");

				//private members
				sw.Write("#region \" private members \"\n\n");
				for(int i = 0; i< cols.Length; i++)
				{
					sw.Write("\t\tprivate " + cols[i].Type.ToString() + " " + "m_" + cols[i].Name + ";\n");
				}
				sw.Write("#endregion\n\n");

				//overrides
				if(sql == null)
				{
					sw.Write("#region \" unique value \"\n\n");
					sw.Write("\t\tpublic override object UniqueValue {get{return " + (primarykeycol != "" ? "m_" + primarykeycol : "null") + ";}}\n");
					sw.Write("\t\tpublic override string UniqueColumn {get{return \"" + primarykeycol + "\";}}\n");
					//sw.Write("\t\tpublic override void Commit() {if(m_dataparent!=null)m_dataparent.PopulateDatabase(this);}\n");
					//sw.Write("\t\tpublic override void Update() {if(m_dataparent!=null)m_dataparent.UpdateDataClass(this);}\n\n");
					sw.Write("#endregion\n\n");
				}

				//properties
				sw.Write("#region \" properties \"\n\n");
				for(int i = 0; i< cols.Length; i++)
				{
					sw.Write("\t\tpublic " + cols[i].Type.ToString() + " " + cols[i].Name + "\n\t\t{\n");
					sw.Write("\t\t\tget{return m_" + cols[i].Name + ";}\n");
					if(sql == null)
						sw.Write("\t\t\tset{object oldvalue = m_" + cols[i].Name + ";OnBeforeDataWrite(this, \"" + cols[i].Name + "\", oldvalue, value);m_" + cols[i].Name + " = value;OnAfterDataWrite(this, \"" + cols[i].Name + "\", oldvalue, value);}\n");
					else
						sw.Write("\t\t\tset{m_" + cols[i].Name + " = value;}\n");
					sw.Write("\t\t}\n\n");
				}
				sw.Write("#endregion\n\n");


				//Unsynchronized Custom Code Region
				sw.Write(unsynchronizedcodestart);
				sw.Write(unsynchronizedcode);
				if(unsynchronizedcode == "") sw.Write("\n\t//Don't put any region sections in here\n\n");
				sw.Write(unsynchronizedcodeend + "\n");

				sw.Write("\t}\n"); //end class
				sw.Write("\n}"); //end namespace

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

				//Build view
				BuildClassFile(ViewNameText.Text, SQLText.Text, DestinationDirText.Text + "\\" + ViewNameText.Text + ".cs", NamespaceStringText.Text, provider, typeof(DataClassView));

				//Build datahub
				BuildDataHubFile(DestinationDirText.Text + "DataHub.cs", NamespaceStringText.Text, provider, null, new string[]{ViewNameText.Text}, new string[]{SQLText.Text});

				//Build project
				BuildProjectFile(assemblyname, NamespaceStringText.Text, DestinationDirText.Text + assemblyname + ".csproj", DestinationDirText.Text + "\\" + ViewNameText.Text + ".cs");
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


	}
}
