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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GOButton = new System.Windows.Forms.Button();
            this.ActionImage = new System.Windows.Forms.PictureBox();
            this.HelperText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActionImage)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBox1.Location = new System.Drawing.Point(56, 16);
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
            this.GOButton.Location = new System.Drawing.Point(176, 64);
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
            this.HelperText.AutoSize = true;
            this.HelperText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HelperText.Location = new System.Drawing.Point(56, 40);
            this.HelperText.Name = "HelperText";
            this.HelperText.Size = new System.Drawing.Size(0, 13);
            this.HelperText.TabIndex = 5;
            // 
            // SearchForm
            // 
            this.AcceptButton = this.GOButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 102);
            this.Controls.Add(this.HelperText);
            this.Controls.Add(this.ActionImage);
            this.Controls.Add(this.GOButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox1);
            this.Name = "SearchForm";
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
    }
}

