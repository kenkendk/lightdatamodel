namespace LimeTime.Taskmanager
{
    partial class CalendarRender
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.RenderArea = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar1.LargeChange = 7;
            this.hScrollBar1.Location = new System.Drawing.Point(0, 404);
            this.hScrollBar1.Maximum = 20;
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(266, 19);
            this.hScrollBar1.TabIndex = 0;
            this.hScrollBar1.Value = 7;
            // 
            // RenderArea
            // 
            this.RenderArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderArea.Location = new System.Drawing.Point(0, 0);
            this.RenderArea.Name = "RenderArea";
            this.RenderArea.Size = new System.Drawing.Size(266, 404);
            this.RenderArea.TabIndex = 1;
            this.RenderArea.Paint += new System.Windows.Forms.PaintEventHandler(this.RenderArea_Paint);
            // 
            // CalendarRender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RenderArea);
            this.Controls.Add(this.hScrollBar1);
            this.Name = "CalendarRender";
            this.Size = new System.Drawing.Size(266, 423);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Panel RenderArea;
    }
}
