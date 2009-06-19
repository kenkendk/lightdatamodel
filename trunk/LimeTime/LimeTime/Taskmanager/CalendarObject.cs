using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LimeTime.Taskmanager
{
    public abstract class CalendarObject
    {
        protected Rectangle m_extent;
        protected CalendarRender m_owner;

        public CalendarObject(CalendarRender owner)
        {
            m_owner = owner;
        }

        public Size Size 
        { 
            get { return m_extent.Size; } 
            set { m_extent = new Rectangle(m_extent.Location, value);  } 
        }

        public Point Location
        {
            get { return m_extent.Location; }
            set { m_extent = new Rectangle(value, m_extent.Size); } 
        }

        public Rectangle Extent
        {
            get { return m_extent; }
            set { m_extent = value; }
        }

        public abstract void Render(Graphics g);
        public abstract Size CalculateSize(Graphics g);
    }

    public class MonthHeader : CalendarObject
    {
        protected DateTime m_date;
        protected string m_name;

        public DateTime Date { get { return m_date; } }

        public MonthHeader(CalendarRender owner, string name, DateTime month)
            : base(owner)
        {
            m_date = month;
            m_name = name;
        }

        public override void Render(Graphics g)
        {
            g.DrawRectangle(Pens.Black, this.m_extent);

            SizeF asz = g.MeasureString(m_name, m_owner.Font);
            g.DrawString(m_name, m_owner.Font, Brushes.Black, new RectangleF((m_extent.Width - asz.Width) / 2 + m_extent.X, (m_extent.Height - asz.Height) / 2 + m_extent.Y, m_extent.Width, m_extent.Height));
        }

        public override Size CalculateSize(Graphics g)
        {
            SizeF monthSize = g.MeasureString(m_name, m_owner.Font);
            return new Size((int)monthSize.Width, (int)monthSize.Height);
        }
    }

    public class MonthBackground : MonthHeader
    {
        public MonthBackground(CalendarRender owner, string name, DateTime month)
            : base(owner, name, month)
        {
        }

        public override void  Render(Graphics g)
        {
 	        g.DrawRectangle(Pens.LightGray, this.m_extent);
        }

        public override Size CalculateSize(Graphics g)
        {
 	        Size b = base.CalculateSize(g);
            return new Size(b.Width, m_owner.Height - m_owner.HeaderHeight);
        }
    }

    public class DayHeader : CalendarObject
    {
        protected DateTime m_date;

        public DayHeader(CalendarRender owner, DateTime date)
            : base(owner)
        {
            m_date = date;
        }

        public override void Render(Graphics g)
        {
            g.DrawRectangle(Pens.Black, this.m_extent);

            SizeF asz = g.MeasureString(m_date.Day.ToString(), m_owner.Font);
            g.DrawString(m_date.Day.ToString(), m_owner.Font, Brushes.Black, new RectangleF((m_extent.Width - asz.Width) / 2 + m_extent.X, (m_extent.Height - asz.Height) / 2 + m_extent.Y, m_extent.Width, m_extent.Height));
        }

        public override Size CalculateSize(Graphics g)
        {
            SizeF daySize = g.MeasureString("30", m_owner.Font);
            return new Size((int)Math.Ceiling(daySize.Width), (int)Math.Ceiling(daySize.Height));
        }
    }

    public class RowBackground : CalendarObject
    {
        protected int m_rowIndex;

        public RowBackground(CalendarRender owner, int rowIndex)
            : base(owner)
        {
            m_rowIndex = rowIndex;
        }

        public override void Render(Graphics g)
        {
            g.FillRectangle(m_rowIndex % 2 == 0 ? Brushes.LightCyan : Brushes.White, m_extent);
        }

        public override Size CalculateSize(Graphics g)
        {
            return new Size(m_owner.Width, m_owner.RowHeight);
        }
    }

    /*public class CustomItem : CalendarObject
    {
        protected DateTime m_date;
        protected object m_item;

        public DateTime Date { get { return m_date; } }

        public CustomItem(CalendarRender owner, DateTime date, object item)
            : base(owner)
        {
            m_rowIndex = rowIndex;
        }

    }*/
         

}
