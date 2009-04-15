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
        private string m_lastSearch = "";
        private object m_lock = new object();

        public SearchForm()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.TrayIcon;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            /*if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();*/

            HelperText.Text = textBox1.Text;
            listBox.Items.Clear();
            listBox.Visible = false;

            if (textBox1.Text.Trim().Length == 0)
            {
                GOButton.Enabled = false;
				return;
            }
            else
                GOButton.Enabled = true;

            if (!backgroundWorker.IsBusy)
                backgroundWorker.RunWorkerAsync(textBox1.Text);
        }

        private void GOButton_Click(object sender, EventArgs e)
        {
            Datamodel.Project p = System.Data.LightDatamodel.Query.FindFirst<Datamodel.Project>(System.Data.LightDatamodel.Query.Parse("Title LIKE ?"), Program.DataConnection.GetObjects<Datamodel.Project>(), HelperText.Text);
            if (p == null)
            {
                p = Program.DataConnection.Add<Datamodel.Project>();
                p.Title = HelperText.Text;
                p.Type = "Project";
                Program.DataConnection.Commit(p);
            }

            Datamodel.Registration r = Program.DataConnection.Add<Datamodel.Registration>();
            r.Time = DateTime.Now;
            r.Note = "";
            r.Project = p;
            p.UseAnnoyClock = m_displayAnnouClockCheck.Checked;

            if (p.Title.ToLower().Trim() != textBox1.Text.Trim().ToLower())
            {
                Datamodel.RecentEntry ent = System.Data.LightDatamodel.Query.FindFirst<Datamodel.RecentEntry>(System.Data.LightDatamodel.Query.Parse("TypedText LIKE ? AND Project.Title LIKE ?"), Program.DataConnection.GetObjects<Datamodel.RecentEntry>(), textBox1.Text, HelperText.Text);
                if (ent == null)
                    ent =  Program.DataConnection.Add<Datamodel.RecentEntry>();
                ent.Time = DateTime.Now;
                ent.TypedText = textBox1.Text;
                ent.Project = p;

                Datamodel.RecentEntry[] existing = Program.DataConnection.GetObjects<Datamodel.RecentEntry>("ORDER BY Time ASC");
                int deleted = 0;
                while (existing.Length - deleted > 50)
                {
                    existing[deleted].DataParent.DeleteObject(existing[deleted]);
                    deleted++;
                }

                if (deleted > 0)
                    Program.DataConnection.CommitAllRecursive();
            }

            Program.DataConnection.CommitWithRelations(r);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

		private void m_cancelbutton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            lock(m_lock)
                m_lastSearch = (string)e.Argument;

            e.Result = null;

            if (!string.IsNullOrEmpty((string)e.Argument))
            {
                List<Datamodel.Project> res = new List<LimeTime.Datamodel.Project>(Program.DataConnection.GetObjects<Datamodel.Project>("ORDER BY ::LimeTime.Search.IntelligentSearch.Evaluate(Title, ?) DESC", (string)e.Argument));
                foreach (Datamodel.RecentEntry r in Program.DataConnection.GetObjects<Datamodel.RecentEntry>("TypedText LIKE ? ORDER BY Time DESC", (string)e.Argument))
                {
                    res.Remove(r.Project);
                    res.Insert(0, r.Project);
                }
                e.Result = res;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            listBox.Items.Clear();
            listBox.Visible = false;

            if (e.Cancelled || e.Error != null || e.Result == null)
            {
                //Report it?
            }
            else
            {

                foreach (Datamodel.Project p in (List<Datamodel.Project>)e.Result)
                {
                    listBox.Items.Add(p.Title);
                    if (listBox.Items.Count > 10)
                        break;
                }

                if (listBox.Items.Count > 0)
                {
                    listBox.Tag = e.Result;
                    listBox.Visible = true;
                }
            }

            lock(m_lock)
                if (m_lastSearch != textBox1.Text)
                    backgroundWorker.RunWorkerAsync(textBox1.Text);


        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex >= 0)
                HelperText.Text = listBox.SelectedItem.ToString();
        }

        private void listBox_Click(object sender, EventArgs e)
        {
        }

        private void HelperText_Click(object sender, EventArgs e)
        {

        }

        private void HelperText_TextChanged(object sender, EventArgs e)
        {
            if (HelperText.Text.Trim().Length == 0)
            {
                ActionImage.Image = null;
                return;
            }

            Datamodel.Project p = System.Data.LightDatamodel.Query.FindFirst<Datamodel.Project>(System.Data.LightDatamodel.Query.Parse("Title LIKE ?"), Program.DataConnection.GetObjects<Datamodel.Project>(), HelperText.Text);
            if (p == null)
            {
                ActionImage.Image = Properties.Resources.AddProject;
                m_displayAnnouClockCheck.Checked = true;
            }
            else
            {
                ActionImage.Image = Properties.Resources.SelectProject;
                m_displayAnnouClockCheck.Checked = p.UseAnnoyClock;
            }

        }
    }
}