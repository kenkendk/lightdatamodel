using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
	public class TextBoxWithFancyAutoComplete : TextBox
	{
		private Form m_dropdownlist;
		private System.ComponentModel.BackgroundWorker m_backgroundWorker;
		public delegate List<ListEntry> GetItemsHandler(string searchtext);
		public GetItemsHandler SearchItems;

		public TextBoxWithFancyAutoComplete()
		{
			m_backgroundWorker = new System.ComponentModel.BackgroundWorker();
			m_backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(m_backgroundWorker_RunWorkerCompleted);
			m_backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_backgroundWorker_DoWork);
		}

		void m_backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			if (SearchItems != null)
			{
				e.Result = SearchItems((string)e.Argument);
			}
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			this.Parent.Move += new EventHandler(Parent_Move);
		}

		void Parent_Move(object sender, EventArgs e)
		{
			if (m_dropdownlist != null && m_dropdownlist.Visible)
			{
				Point p = new Point(this.Location.X, this.Location.Y + this.Height);
				p = this.Parent.PointToScreen(p);
				m_dropdownlist.Location = p;
			}
		}

		void m_backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if (e.Result == null || ((List<ListEntry>)e.Result).Count == 0)
			{
				if (m_dropdownlist != null && m_dropdownlist.Visible) m_dropdownlist.Close();
				return;
			}
			ShowListWindow(e.Result as List<ListEntry>);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((this.Text.Length == 1 && e.KeyCode == Keys.Back || this.SelectionLength == this.Text.Length && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)) && m_dropdownlist != null)
				m_dropdownlist.Close();
			else
			{
				if (!m_backgroundWorker.IsBusy) m_backgroundWorker.RunWorkerAsync(this.Text + (char)e.KeyValue);
			}
			base.OnKeyDown(e);
		}

		/// <summary>
		/// This just *doesn't* seem to work the way I want
		/// </summary>
		private class FormUnfocused : Form
		{
			public FormUnfocused()
			{
				this.DoubleBuffered = true;
			}

			protected override bool ShowWithoutActivation
			{
				get { return true; }
			}
			protected override CreateParams CreateParams
			{
				get
				{
					CreateParams baseParams = base.CreateParams;
					const int WS_EX_NOACTIVATE = 0x08000000;
					const int WS_EX_TOOLWINDOW = 0x00000080;
					baseParams.ExStyle |= (int)(WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
					return baseParams;
				}
			}
			protected override void OnGotFocus(EventArgs e)
			{
				//this.Owner.Focus();
				base.OnGotFocus(e);
			}
		}

		private void ShowListWindow(List<ListEntry> list)
		{
			if (m_dropdownlist == null || !m_dropdownlist.Visible)
			{
				m_dropdownlist = new FormUnfocused();
				m_dropdownlist.BackColor = Color.White;
				m_dropdownlist.AutoScroll = true;
				m_dropdownlist.FormBorderStyle = FormBorderStyle.None;
				m_dropdownlist.ShowInTaskbar = false;
				m_dropdownlist.StartPosition = FormStartPosition.Manual;
				Point p = new Point(this.Location.X, this.Location.Y + this.Height);
				p = this.Parent.PointToScreen(p);
				m_dropdownlist.Location = p;
				m_dropdownlist.Width = this.Width;
				FillList(m_dropdownlist, list);
				m_dropdownlist.Show(this);
			}
			else
				FillList(m_dropdownlist, list);
		}

		protected override void OnResize(EventArgs e)
		{
			if (m_dropdownlist != null && m_dropdownlist.Visible)
			{
				m_dropdownlist.Width = this.Width;
			}
			base.OnResize(e);
		}

		public struct ListEntry
		{
			public Image Icon;
			public string Header;
			public string SubText;
			public ListEntry(Image icon, string header, string subtext)
			{
				this.Icon = icon;
				this.Header = header;
				this.SubText = subtext;
			}
		}

		private void FillList(Form frm, List<ListEntry> list)
		{
			frm.SuspendLayout();

			//fill in controls
			Control[] cons = new Control[list.Count * 3];
			int cury = 0;
			for (int i = 0; i < list.Count; i++)
			{
				//image
				PictureBox p = new PictureBox();
				p.Image = list[i].Icon;
				p.Location = new Point(0, cury);
				p.Size = new Size(32, 32);
				cons[i * 3] = p;

				//header
				Label l = new Label();
				l.Text = list[i].Header;
				l.Location = new Point(37, cury);
				l.Size = new Size(this.Width - 37, 16);
				l.Font = new Font(this.Font.FontFamily, 10, FontStyle.Regular);
				l.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				cons[i * 3 + 1] = l;

				//sub header
				Label ls = new Label();
				ls.Text = list[i].SubText;
				ls.Location = new Point(37, cury + 16);
				ls.Size = new Size(this.Width - 37, 16);
				ls.Font = new Font(this.Font.FontFamily, 8, FontStyle.Regular);
				ls.ForeColor = Color.Gray;
				ls.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				cons[i * 3 + 2] = ls;

				cury += 32;
			}
			frm.Height = Math.Min( cury, 100);

			frm.Controls.Clear();
			frm.Controls.AddRange(cons);
			frm.ResumeLayout(true);
		}
	}
}
