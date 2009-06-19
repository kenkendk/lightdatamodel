using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LimeTime.Taskmanager
{
    public partial class CalendarRender : UserControl
    {
        private string[] MONTH_NAMES = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        private DateTime m_startDate = DateTime.Now;
        private int m_rowHeight = 15;

        public int RowHeight { get { return m_rowHeight; } }
        public int HeaderHeight 
        {
            get
            {
                int h = 0;
                foreach (CalendarObject co in m_headerItems)
                    h = Math.Max(h, co.Extent.Bottom);

                return h + 1;
            }
        }

        private List<CalendarObject> m_headerItems;
        private List<CalendarObject> m_userItems;

        public CalendarRender()
        {
            InitializeComponent();
        }

        private void RenderArea_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);

            m_headerItems = new List<CalendarObject>();
            m_userItems = new List<CalendarObject>();

            int spaceLeft = RenderArea.Width;
            DateTime d = m_startDate;

            while (spaceLeft > 0)
            {
                int dayWidth = 0;
                int monthOffset = RenderArea.Width - spaceLeft;

                MonthHeader mh = new MonthHeader(this, MONTH_NAMES[d.Month - 1], d);
                mh.Size = mh.CalculateSize(e.Graphics);
                m_headerItems.Add(mh);

                int m = d.Month;
                while (d.Month == m && spaceLeft > 0)
                {
                    DayHeader dh = new DayHeader(this, d);
                    dh.Size = dh.CalculateSize(e.Graphics);
                    dh.Location = new Point(RenderArea.Width - spaceLeft, mh.Size.Height);

                    m_headerItems.Add(dh);

                    d = d.AddDays(1);
                    dayWidth += dh.Size.Width;
                    spaceLeft -= dh.Size.Width;
                }

                int daysPassed = m_headerItems[m_headerItems.Count - 1].Size.Width * (mh.Date.Day - 1);

                mh.Location = new Point(monthOffset - daysPassed, 0);
                mh.Size = new Size(m_headerItems[m_headerItems.Count - 1].Size.Width * DateTime.DaysInMonth(mh.Date.Year, mh.Date.Month), mh.Size.Height);

                MonthBackground mbg = new MonthBackground(this, MONTH_NAMES[d.Month - 1], d);
                mbg.Size = mbg.CalculateSize(e.Graphics);
                mbg.Location = new Point(this.HeaderHeight, 0);
                //m_userItems.Add(mbg);
            }

            foreach (CalendarObject co in m_headerItems)
                co.Render(e.Graphics);

            spaceLeft = RenderArea.Height - this.HeaderHeight;
            int rownum = 0;

            while (spaceLeft > 0)
            {
                RowBackground rb = new RowBackground(this, rownum++);
                rb.Size = rb.CalculateSize(e.Graphics);
                rb.Location = new Point(0, RenderArea.Height - spaceLeft);
                spaceLeft -= rb.Size.Height;
                m_userItems.Add(rb);
            }

            foreach (CalendarObject co in m_userItems)
                co.Render(e.Graphics);
        }

        private SizeF GetLargestStringSize(string[] strings, Graphics g, Font f)
        {
            float w = 0;
            float h = 0;

            foreach(string s in strings)
            {
                SizeF sz = g.MeasureString(s, f);
                w = Math.Max(w, sz.Width);
                h = Math.Max(h, sz.Height);
            }

            return new SizeF(w, h);
        }
    }
}
