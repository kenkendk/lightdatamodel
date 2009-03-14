using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimeTime
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.TrayIcon;

            textBox1.AutoCompleteCustomSource.Clear();
            foreach (Datamodel.Project p in Program.DataConnection.GetObjects<Datamodel.Project>())
                textBox1.AutoCompleteCustomSource.Add(p.Title);

            textBox1_TextChanged(null, null);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length == 0)
            {
                ActionImage.Image = null;
                HelperText.Text = "";
                GOButton.Enabled = false;
            }
            else if (System.Data.LightDatamodel.Query.FindFirst(System.Data.LightDatamodel.Query.Parse("Title LIKE ?"), Program.DataConnection.GetObjects<Datamodel.Project>(), textBox1.Text) != null)
            {
                ActionImage.Image = Properties.Resources.SelectProject;
                HelperText.Text = "Select the current project as active";
                GOButton.Enabled = true;
            }
            else
            {
                ActionImage.Image = Properties.Resources.AddProject;
                HelperText.Text = "Add a new project";
                GOButton.Enabled = true;
            }
        }

        private void GOButton_Click(object sender, EventArgs e)
        {
            Datamodel.Project p = System.Data.LightDatamodel.Query.FindFirst<Datamodel.Project>(System.Data.LightDatamodel.Query.Parse("Title LIKE ?"), Program.DataConnection.GetObjects<Datamodel.Project>(), textBox1.Text);
            if (p == null)
            {
                p = Program.DataConnection.Add<Datamodel.Project>();
                p.Title = textBox1.Text;
                p.Type = "Project";
                Program.DataConnection.Commit(p);
            }

            Datamodel.Registration r = Program.DataConnection.Add<Datamodel.Registration>();
            r.Time = DateTime.Now;
            r.Note = "";
            r.Project = p;
            Program.DataConnection.CommitWithRelations(r);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}