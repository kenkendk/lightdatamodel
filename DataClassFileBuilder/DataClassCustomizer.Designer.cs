namespace DataClassFileBuilder
{
    partial class DataClassCustomizer
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataClassCustomizer));
			this.panel1 = new System.Windows.Forms.Panel();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.OKBtn = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
			this.addReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.RemoveButton = new System.Windows.Forms.ToolStripButton();
			this.treeView = new System.Windows.Forms.TreeView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.IgnoredFieldProperties = new System.Windows.Forms.GroupBox();
			this.IgnoredFieldName = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.ReferenceProperties = new System.Windows.Forms.GroupBox();
			this.ReferenceType = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.GenerateRelationKey = new System.Windows.Forms.Button();
			this.ReferenceRelationKey = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.ReferenceReversePropertyname = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.ReferenceReverseColumnname = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.ReferencePropertyname = new System.Windows.Forms.ComboBox();
			this.ReferenceReverseTablename = new System.Windows.Forms.ComboBox();
			this.ReferenceColumnname = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.TableProperties = new System.Windows.Forms.GroupBox();
			this.TableTablename = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.FieldProperties = new System.Windows.Forms.GroupBox();
			this.FieldDefaultValue = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.FieldPropertyname = new System.Windows.Forms.ComboBox();
			this.FieldFieldname = new System.Windows.Forms.ComboBox();
			this.FieldDatatype = new System.Windows.Forms.ComboBox();
			this.FieldColumnname = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.FieldExcludeSelect = new System.Windows.Forms.CheckBox();
			this.FieldExcludeUpdate = new System.Windows.Forms.CheckBox();
			this.FieldAutogenerate = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.panel1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.IgnoredFieldProperties.SuspendLayout();
			this.ReferenceProperties.SuspendLayout();
			this.TableProperties.SuspendLayout();
			this.FieldProperties.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.CancelBtn);
			this.panel1.Controls.Add(this.OKBtn);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 523);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(553, 41);
			this.panel1.TabIndex = 0;
			// 
			// CancelBtn
			// 
			this.CancelBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(273, 8);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(80, 24);
			this.CancelBtn.TabIndex = 1;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			// 
			// OKBtn
			// 
			this.OKBtn.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.OKBtn.Location = new System.Drawing.Point(177, 8);
			this.OKBtn.Name = "OKBtn";
			this.OKBtn.Size = new System.Drawing.Size(80, 24);
			this.OKBtn.TabIndex = 0;
			this.OKBtn.Text = "OK";
			this.OKBtn.UseVisualStyleBackColor = true;
			this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.toolStrip);
			this.splitContainer1.Panel1.Controls.Add(this.treeView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.IgnoredFieldProperties);
			this.splitContainer1.Panel2.Controls.Add(this.ReferenceProperties);
			this.splitContainer1.Panel2.Controls.Add(this.TableProperties);
			this.splitContainer1.Panel2.Controls.Add(this.FieldProperties);
			this.splitContainer1.Size = new System.Drawing.Size(553, 523);
			this.splitContainer1.SplitterDistance = 200;
			this.splitContainer1.TabIndex = 1;
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.RemoveButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(200, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip1";
			// 
			// toolStripSplitButton1
			// 
			this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addReferenceToolStripMenuItem});
			this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
			this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSplitButton1.Name = "toolStripSplitButton1";
			this.toolStripSplitButton1.Size = new System.Drawing.Size(58, 22);
			this.toolStripSplitButton1.Text = "Add";
			this.toolStripSplitButton1.ToolTipText = "Add new item";
			// 
			// addReferenceToolStripMenuItem
			// 
			this.addReferenceToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addReferenceToolStripMenuItem.Image")));
			this.addReferenceToolStripMenuItem.Name = "addReferenceToolStripMenuItem";
			this.addReferenceToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.addReferenceToolStripMenuItem.Text = "Reference field";
			this.addReferenceToolStripMenuItem.Click += new System.EventHandler(this.addReferenceToolStripMenuItem_Click);
			// 
			// RemoveButton
			// 
			this.RemoveButton.Enabled = false;
			this.RemoveButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveButton.Image")));
			this.RemoveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.RemoveButton.Name = "RemoveButton";
			this.RemoveButton.Size = new System.Drawing.Size(66, 22);
			this.RemoveButton.Text = "Remove";
			this.RemoveButton.ToolTipText = "Remove the selected item";
			this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
			// 
			// treeView
			// 
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.treeView.ImageIndex = 0;
			this.treeView.ImageList = this.imageList;
			this.treeView.Location = new System.Drawing.Point(8, 32);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = 0;
			this.treeView.Size = new System.Drawing.Size(184, 482);
			this.treeView.TabIndex = 0;
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "application_form.png");
			this.imageList.Images.SetKeyName(1, "application_form_magnify.png");
			this.imageList.Images.SetKeyName(2, "application_form_delete.png");
			this.imageList.Images.SetKeyName(3, "application.png");
			this.imageList.Images.SetKeyName(4, "application_link.png");
			this.imageList.Images.SetKeyName(5, "application_delete.png");
			// 
			// IgnoredFieldProperties
			// 
			this.IgnoredFieldProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.IgnoredFieldProperties.Controls.Add(this.IgnoredFieldName);
			this.IgnoredFieldProperties.Controls.Add(this.label11);
			this.IgnoredFieldProperties.Location = new System.Drawing.Point(8, 464);
			this.IgnoredFieldProperties.Name = "IgnoredFieldProperties";
			this.IgnoredFieldProperties.Size = new System.Drawing.Size(335, 48);
			this.IgnoredFieldProperties.TabIndex = 2;
			this.IgnoredFieldProperties.TabStop = false;
			this.IgnoredFieldProperties.Text = "Field properties";
			// 
			// IgnoredFieldName
			// 
			this.IgnoredFieldName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.IgnoredFieldName.Location = new System.Drawing.Point(88, 16);
			this.IgnoredFieldName.Name = "IgnoredFieldName";
			this.IgnoredFieldName.Size = new System.Drawing.Size(239, 20);
			this.IgnoredFieldName.TabIndex = 1;
			this.IgnoredFieldName.TextChanged += new System.EventHandler(this.IgnoredFieldName_TextChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(16, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(35, 13);
			this.label11.TabIndex = 0;
			this.label11.Text = "Name";
			// 
			// ReferenceProperties
			// 
			this.ReferenceProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferenceProperties.Controls.Add(this.ReferenceType);
			this.ReferenceProperties.Controls.Add(this.label14);
			this.ReferenceProperties.Controls.Add(this.GenerateRelationKey);
			this.ReferenceProperties.Controls.Add(this.ReferenceRelationKey);
			this.ReferenceProperties.Controls.Add(this.label13);
			this.ReferenceProperties.Controls.Add(this.ReferenceReversePropertyname);
			this.ReferenceProperties.Controls.Add(this.label10);
			this.ReferenceProperties.Controls.Add(this.ReferenceReverseColumnname);
			this.ReferenceProperties.Controls.Add(this.label9);
			this.ReferenceProperties.Controls.Add(this.ReferencePropertyname);
			this.ReferenceProperties.Controls.Add(this.ReferenceReverseTablename);
			this.ReferenceProperties.Controls.Add(this.ReferenceColumnname);
			this.ReferenceProperties.Controls.Add(this.label6);
			this.ReferenceProperties.Controls.Add(this.label7);
			this.ReferenceProperties.Controls.Add(this.label8);
			this.ReferenceProperties.Location = new System.Drawing.Point(8, 208);
			this.ReferenceProperties.Name = "ReferenceProperties";
			this.ReferenceProperties.Size = new System.Drawing.Size(335, 192);
			this.ReferenceProperties.TabIndex = 0;
			this.ReferenceProperties.TabStop = false;
			this.ReferenceProperties.Text = "Reference properties";
			// 
			// ReferenceType
			// 
			this.ReferenceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferenceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ReferenceType.FormattingEnabled = true;
			this.ReferenceType.Items.AddRange(new object[] {
            "",
            "OneToOne",
            "ManyToOne",
            "OneToMany"});
			this.ReferenceType.Location = new System.Drawing.Point(104, 88);
			this.ReferenceType.Name = "ReferenceType";
			this.ReferenceType.Size = new System.Drawing.Size(104, 21);
			this.ReferenceType.TabIndex = 28;
			this.ReferenceType.SelectedIndexChanged += new System.EventHandler(this.ReferenceType_SelectedIndexChanged);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(16, 88);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(31, 13);
			this.label14.TabIndex = 27;
			this.label14.Text = "Type";
			// 
			// GenerateRelationKey
			// 
			this.GenerateRelationKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.GenerateRelationKey.Location = new System.Drawing.Point(256, 64);
			this.GenerateRelationKey.Name = "GenerateRelationKey";
			this.GenerateRelationKey.Size = new System.Drawing.Size(72, 20);
			this.GenerateRelationKey.TabIndex = 26;
			this.GenerateRelationKey.Text = "Generate";
			this.GenerateRelationKey.UseVisualStyleBackColor = true;
			this.GenerateRelationKey.Click += new System.EventHandler(this.GenerateRelationKey_Click);
			// 
			// ReferenceRelationKey
			// 
			this.ReferenceRelationKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferenceRelationKey.Location = new System.Drawing.Point(104, 64);
			this.ReferenceRelationKey.Name = "ReferenceRelationKey";
			this.ReferenceRelationKey.Size = new System.Drawing.Size(144, 20);
			this.ReferenceRelationKey.TabIndex = 25;
			this.ReferenceRelationKey.TextChanged += new System.EventHandler(this.ReferenceRelationKey_TextChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(16, 64);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(64, 13);
			this.label13.TabIndex = 24;
			this.label13.Text = "RelationKey";
			// 
			// ReferenceReversePropertyname
			// 
			this.ReferenceReversePropertyname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferenceReversePropertyname.FormattingEnabled = true;
			this.ReferenceReversePropertyname.Location = new System.Drawing.Point(104, 168);
			this.ReferenceReversePropertyname.Name = "ReferenceReversePropertyname";
			this.ReferenceReversePropertyname.Size = new System.Drawing.Size(223, 21);
			this.ReferenceReversePropertyname.TabIndex = 22;
			this.ReferenceReversePropertyname.TextChanged += new System.EventHandler(this.ReferenceReversePropertyname_TextChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(16, 168);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(72, 13);
			this.label10.TabIndex = 21;
			this.label10.Text = "Propertyname";
			// 
			// ReferenceReverseColumnname
			// 
			this.ReferenceReverseColumnname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferenceReverseColumnname.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ReferenceReverseColumnname.FormattingEnabled = true;
			this.ReferenceReverseColumnname.Location = new System.Drawing.Point(104, 144);
			this.ReferenceReverseColumnname.Name = "ReferenceReverseColumnname";
			this.ReferenceReverseColumnname.Size = new System.Drawing.Size(223, 21);
			this.ReferenceReverseColumnname.TabIndex = 20;
			this.ReferenceReverseColumnname.TextChanged += new System.EventHandler(this.ReferenceReverseColumnname_TextChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(16, 144);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(84, 13);
			this.label9.TabIndex = 19;
			this.label9.Text = "Reverse column";
			// 
			// ReferencePropertyname
			// 
			this.ReferencePropertyname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferencePropertyname.FormattingEnabled = true;
			this.ReferencePropertyname.Location = new System.Drawing.Point(104, 40);
			this.ReferencePropertyname.Name = "ReferencePropertyname";
			this.ReferencePropertyname.Size = new System.Drawing.Size(223, 21);
			this.ReferencePropertyname.TabIndex = 18;
			this.ReferencePropertyname.SelectedIndexChanged += new System.EventHandler(this.ReferencePropertyname_SelectedIndexChanged);
			this.ReferencePropertyname.TextChanged += new System.EventHandler(this.ReferencePropertyname_TextChanged);
			// 
			// ReferenceReverseTablename
			// 
			this.ReferenceReverseTablename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferenceReverseTablename.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ReferenceReverseTablename.FormattingEnabled = true;
			this.ReferenceReverseTablename.Location = new System.Drawing.Point(104, 120);
			this.ReferenceReverseTablename.Name = "ReferenceReverseTablename";
			this.ReferenceReverseTablename.Size = new System.Drawing.Size(223, 21);
			this.ReferenceReverseTablename.TabIndex = 17;
			this.ReferenceReverseTablename.SelectedIndexChanged += new System.EventHandler(this.ReferenceReverseTablename_SelectedIndexChanged);
			this.ReferenceReverseTablename.TextChanged += new System.EventHandler(this.ReferenceReverseTablename_TextChanged);
			// 
			// ReferenceColumnname
			// 
			this.ReferenceColumnname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.ReferenceColumnname.Location = new System.Drawing.Point(104, 16);
			this.ReferenceColumnname.Name = "ReferenceColumnname";
			this.ReferenceColumnname.Size = new System.Drawing.Size(223, 20);
			this.ReferenceColumnname.TabIndex = 16;
			this.ReferenceColumnname.TextChanged += new System.EventHandler(this.ReferenceColumnname_TextChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(16, 40);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 13);
			this.label6.TabIndex = 15;
			this.label6.Text = "Propertyname";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 120);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(73, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "Reverse table";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(16, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(68, 13);
			this.label8.TabIndex = 13;
			this.label8.Text = "Columnname";
			// 
			// TableProperties
			// 
			this.TableProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TableProperties.Controls.Add(this.TableTablename);
			this.TableProperties.Controls.Add(this.label1);
			this.TableProperties.Location = new System.Drawing.Point(8, 408);
			this.TableProperties.Name = "TableProperties";
			this.TableProperties.Size = new System.Drawing.Size(335, 48);
			this.TableProperties.TabIndex = 1;
			this.TableProperties.TabStop = false;
			this.TableProperties.Text = "Table properties";
			// 
			// TableTablename
			// 
			this.TableTablename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.TableTablename.Location = new System.Drawing.Point(88, 16);
			this.TableTablename.Name = "TableTablename";
			this.TableTablename.Size = new System.Drawing.Size(239, 20);
			this.TableTablename.TabIndex = 1;
			this.TableTablename.TextChanged += new System.EventHandler(this.TableTablename_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name";
			// 
			// FieldProperties
			// 
			this.FieldProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FieldProperties.Controls.Add(this.FieldDefaultValue);
			this.FieldProperties.Controls.Add(this.label12);
			this.FieldProperties.Controls.Add(this.FieldPropertyname);
			this.FieldProperties.Controls.Add(this.FieldFieldname);
			this.FieldProperties.Controls.Add(this.FieldDatatype);
			this.FieldProperties.Controls.Add(this.FieldColumnname);
			this.FieldProperties.Controls.Add(this.label5);
			this.FieldProperties.Controls.Add(this.FieldExcludeSelect);
			this.FieldProperties.Controls.Add(this.FieldExcludeUpdate);
			this.FieldProperties.Controls.Add(this.FieldAutogenerate);
			this.FieldProperties.Controls.Add(this.label4);
			this.FieldProperties.Controls.Add(this.label3);
			this.FieldProperties.Controls.Add(this.label2);
			this.FieldProperties.Location = new System.Drawing.Point(8, 8);
			this.FieldProperties.Name = "FieldProperties";
			this.FieldProperties.Size = new System.Drawing.Size(335, 192);
			this.FieldProperties.TabIndex = 1;
			this.FieldProperties.TabStop = false;
			this.FieldProperties.Text = "Field properties";
			// 
			// FieldDefaultValue
			// 
			this.FieldDefaultValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FieldDefaultValue.Location = new System.Drawing.Point(96, 112);
			this.FieldDefaultValue.Name = "FieldDefaultValue";
			this.FieldDefaultValue.Size = new System.Drawing.Size(216, 20);
			this.FieldDefaultValue.TabIndex = 14;
			this.FieldDefaultValue.TextChanged += new System.EventHandler(this.FieldDefaultValue_TextChanged);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(16, 112);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(70, 13);
			this.label12.TabIndex = 13;
			this.label12.Text = "Default value";
			// 
			// FieldPropertyname
			// 
			this.FieldPropertyname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FieldPropertyname.FormattingEnabled = true;
			this.FieldPropertyname.Location = new System.Drawing.Point(96, 64);
			this.FieldPropertyname.Name = "FieldPropertyname";
			this.FieldPropertyname.Size = new System.Drawing.Size(231, 21);
			this.FieldPropertyname.TabIndex = 12;
			this.FieldPropertyname.TextChanged += new System.EventHandler(this.FieldPropertyname_TextChanged);
			// 
			// FieldFieldname
			// 
			this.FieldFieldname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FieldFieldname.FormattingEnabled = true;
			this.FieldFieldname.Location = new System.Drawing.Point(96, 40);
			this.FieldFieldname.Name = "FieldFieldname";
			this.FieldFieldname.Size = new System.Drawing.Size(231, 21);
			this.FieldFieldname.TabIndex = 11;
			this.FieldFieldname.TextChanged += new System.EventHandler(this.FieldFieldname_TextChanged);
			// 
			// FieldDatatype
			// 
			this.FieldDatatype.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FieldDatatype.FormattingEnabled = true;
			this.FieldDatatype.Location = new System.Drawing.Point(96, 88);
			this.FieldDatatype.Name = "FieldDatatype";
			this.FieldDatatype.Size = new System.Drawing.Size(231, 21);
			this.FieldDatatype.TabIndex = 10;
			this.FieldDatatype.TextChanged += new System.EventHandler(this.FieldDatatype_TextChanged);
			// 
			// FieldColumnname
			// 
			this.FieldColumnname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.FieldColumnname.Location = new System.Drawing.Point(96, 16);
			this.FieldColumnname.Name = "FieldColumnname";
			this.FieldColumnname.Size = new System.Drawing.Size(231, 20);
			this.FieldColumnname.TabIndex = 7;
			this.FieldColumnname.TextChanged += new System.EventHandler(this.FieldColumnname_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(16, 88);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Datatype";
			// 
			// FieldExcludeSelect
			// 
			this.FieldExcludeSelect.AutoSize = true;
			this.FieldExcludeSelect.Location = new System.Drawing.Point(16, 168);
			this.FieldExcludeSelect.Name = "FieldExcludeSelect";
			this.FieldExcludeSelect.Size = new System.Drawing.Size(131, 17);
			this.FieldExcludeSelect.TabIndex = 5;
			this.FieldExcludeSelect.Text = "Exclude from SELECT";
			this.FieldExcludeSelect.UseVisualStyleBackColor = true;
			this.FieldExcludeSelect.CheckedChanged += new System.EventHandler(this.FieldExcludeSelect_CheckedChanged);
			// 
			// FieldExcludeUpdate
			// 
			this.FieldExcludeUpdate.AutoSize = true;
			this.FieldExcludeUpdate.Location = new System.Drawing.Point(16, 152);
			this.FieldExcludeUpdate.Name = "FieldExcludeUpdate";
			this.FieldExcludeUpdate.Size = new System.Drawing.Size(134, 17);
			this.FieldExcludeUpdate.TabIndex = 4;
			this.FieldExcludeUpdate.Text = "Exclude from UPDATE";
			this.FieldExcludeUpdate.UseVisualStyleBackColor = true;
			this.FieldExcludeUpdate.CheckedChanged += new System.EventHandler(this.FieldExcludeUpdate_CheckedChanged);
			// 
			// FieldAutogenerate
			// 
			this.FieldAutogenerate.AutoSize = true;
			this.FieldAutogenerate.Location = new System.Drawing.Point(16, 136);
			this.FieldAutogenerate.Name = "FieldAutogenerate";
			this.FieldAutogenerate.Size = new System.Drawing.Size(213, 17);
			this.FieldAutogenerate.TabIndex = 3;
			this.FieldAutogenerate.Text = "AutoGenerated / Exclude from INSERT";
			this.FieldAutogenerate.UseVisualStyleBackColor = true;
			this.FieldAutogenerate.CheckedChanged += new System.EventHandler(this.FieldAutogenerate_CheckedChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(16, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Propertyname";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(55, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Fieldname";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Columnname";
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// DataClassCustomizer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(553, 564);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.panel1);
			this.Name = "DataClassCustomizer";
			this.Text = "DataClassCustomizer";
			this.panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.IgnoredFieldProperties.ResumeLayout(false);
			this.IgnoredFieldProperties.PerformLayout();
			this.ReferenceProperties.ResumeLayout(false);
			this.ReferenceProperties.PerformLayout();
			this.TableProperties.ResumeLayout(false);
			this.TableProperties.PerformLayout();
			this.FieldProperties.ResumeLayout(false);
			this.FieldProperties.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.GroupBox TableProperties;
        private System.Windows.Forms.GroupBox FieldProperties;
        private System.Windows.Forms.GroupBox ReferenceProperties;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton RemoveButton;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
		private System.Windows.Forms.ToolStripMenuItem addReferenceToolStripMenuItem;
        private System.Windows.Forms.TextBox TableTablename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox FieldExcludeSelect;
        private System.Windows.Forms.CheckBox FieldExcludeUpdate;
        private System.Windows.Forms.CheckBox FieldAutogenerate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ReferencePropertyname;
        private System.Windows.Forms.ComboBox ReferenceReverseTablename;
        private System.Windows.Forms.TextBox ReferenceColumnname;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox FieldPropertyname;
        private System.Windows.Forms.ComboBox FieldFieldname;
        private System.Windows.Forms.ComboBox FieldDatatype;
		private System.Windows.Forms.TextBox FieldColumnname;
        private System.Windows.Forms.ComboBox ReferenceReversePropertyname;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox ReferenceReverseColumnname;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.GroupBox IgnoredFieldProperties;
        private System.Windows.Forms.TextBox IgnoredFieldName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox FieldDefaultValue;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button GenerateRelationKey;
        private System.Windows.Forms.TextBox ReferenceRelationKey;
        private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox ReferenceType;
		private System.Windows.Forms.Label label14;

    }
}