namespace LimeTime
{
    partial class SearchForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GOButton = new System.Windows.Forms.Button();
            this.ActionImage = new System.Windows.Forms.PictureBox();
            this.HelperText = new System.Windows.Forms.Label();
            this.m_cancelbutton = new System.Windows.Forms.Button();
            this.m_displayAnnouClockCheck = new System.Windows.Forms.CheckBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActionImage)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(56, 8);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(312, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LimeTime.Properties.Resources.SearchIcon;
            this.pictureBox1.Location = new System.Drawing.Point(8, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // GOButton
            // 
            this.GOButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GOButton.Location = new System.Drawing.Point(213, 261);
            this.GOButton.Name = "GOButton";
            this.GOButton.Size = new System.Drawing.Size(75, 23);
            this.GOButton.TabIndex = 3;
            this.GOButton.Text = "GO!";
            this.GOButton.UseVisualStyleBackColor = true;
            this.GOButton.Click += new System.EventHandler(this.GOButton_Click);
            // 
            // ActionImage
            // 
            this.ActionImage.Location = new System.Drawing.Point(376, 8);
            this.ActionImage.Name = "ActionImage";
            this.ActionImage.Size = new System.Drawing.Size(32, 32);
            this.ActionImage.TabIndex = 4;
            this.ActionImage.TabStop = false;
            // 
            // HelperText
            // 
            this.HelperText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelperText.Location = new System.Drawing.Point(56, 32);
            this.HelperText.Name = "HelperText";
            this.HelperText.Size = new System.Drawing.Size(312, 16);
            this.HelperText.TabIndex = 5;
            this.HelperText.TextChanged += new System.EventHandler(this.HelperText_TextChanged);
            this.HelperText.Click += new System.EventHandler(this.HelperText_Click);
            // 
            // m_cancelbutton
            // 
            this.m_cancelbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_cancelbutton.Location = new System.Drawing.Point(133, 261);
            this.m_cancelbutton.Name = "m_cancelbutton";
            this.m_cancelbutton.Size = new System.Drawing.Size(75, 24);
            this.m_cancelbutton.TabIndex = 4;
            this.m_cancelbutton.Text = "Cancel";
            this.m_cancelbutton.UseVisualStyleBackColor = true;
            this.m_cancelbutton.Click += new System.EventHandler(this.m_cancelbutton_Click);
            // 
            // m_displayAnnouClockCheck
            // 
            this.m_displayAnnouClockCheck.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_displayAnnouClockCheck.AutoSize = true;
            this.m_displayAnnouClockCheck.Checked = true;
            this.m_displayAnnouClockCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_displayAnnouClockCheck.Location = new System.Drawing.Point(144, 237);
            this.m_displayAnnouClockCheck.Name = "m_displayAnnouClockCheck";
            this.m_displayAnnouClockCheck.Size = new System.Drawing.Size(133, 17);
            this.m_displayAnnouClockCheck.TabIndex = 2;
            this.m_displayAnnouClockCheck.Text = "Display \"Annoy Clock\"";
            this.m_displayAnnouClockCheck.UseVisualStyleBackColor = true;
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(8, 56);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(400, 173);
            this.listBox.TabIndex = 6;
            this.listBox.Visible = false;
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            this.listBox.Click += new System.EventHandler(this.listBox_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // SearchForm
            // 
            this.AcceptButton = this.GOButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_cancelbutton;
            this.ClientSize = new System.Drawing.Size(421, 299);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.m_displayAnnouClockCheck);
            this.Controls.Add(this.m_cancelbutton);
            this.Controls.Add(this.HelperText);
            this.Controls.Add(this.ActionImage);
            this.Controls.Add(this.GOButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find or create a project";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActionImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button GOButton;
        private System.Windows.Forms.PictureBox ActionImage;
        private System.Windows.Forms.Label HelperText;
		private System.Windows.Forms.Button m_cancelbutton;
		private System.Windows.Forms.CheckBox m_displayAnnouClockCheck;
        private System.Windows.Forms.ListBox listBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}

