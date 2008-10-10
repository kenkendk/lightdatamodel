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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.LightDatamodel;

namespace DataClassFileBuilder
{

    public partial class DataClassCustomizer : Form
    {
        private const int IMAGE_TABLE = 0;
        private const int IMAGE_VIEW = 1;
        private const int IMAGE_IGNOREDTABLE = 2;
        private const int IMAGE_FIELD = 3;
        private const int IMAGE_REFERENCE = 4;
        private const int IMAGE_IGNOREDFIELD = 5;

		private List<ConfigurationContainer.Table> m_classes;
        //private List<TypeConfiguration.IgnoredClass> m_ignoredclasses;
        private bool m_isUpdating = false;

        private DataClassCustomizer()
        {
            InitializeComponent();
        }

        //public DataClassCustomizer(List<TypeConfiguration.MappedClass> classes, List<TypeConfiguration.IgnoredClass> ignoredclasses) : this()
		public DataClassCustomizer(ConfigurationContainer.Table[] classes)
			: this()
        {
            m_classes = new List<ConfigurationContainer.Table>(classes);
            //m_ignoredclasses = ignoredclasses;

			foreach (ConfigurationContainer.Table mc in m_classes)
            {
                TreeNode table = new TreeNode(mc.Name, IMAGE_TABLE, IMAGE_TABLE);
                table.Tag = mc;

				foreach (ConfigurationContainer.Column mf in mc.Columns)
                {
                    TreeNode field = new TreeNode(mf.Name, IMAGE_FIELD, IMAGE_FIELD);
                    field.Tag = mf;
                    table.Nodes.Add(field);
                }

				foreach (ConfigurationContainer.Relation rf in mc.Relations)
                {
                    TreeNode field = new TreeNode(rf.Propertyname, IMAGE_REFERENCE, IMAGE_REFERENCE);
                    field.Tag = rf;
                    table.Nodes.Add(field);
                }

				//foreach (TypeConfiguration.IgnoredField i in mc.IgnoredFields.Values)
				//{
				//    TreeNode field = new TreeNode(i.Fieldname, IMAGE_IGNOREDFIELD, IMAGE_IGNOREDFIELD);
				//    field.Tag = i;
				//    table.Nodes.Add(field);
				//}

                treeView.Nodes.Add(table);
            }

			//foreach (TypeConfiguration.IgnoredClass ic in m_ignoredclasses)
			//{
			//    TreeNode table = new TreeNode(ic.Tablename, IMAGE_IGNOREDTABLE, IMAGE_IGNOREDTABLE);
			//    table.Tag = ic;
			//    treeView.Nodes.Add(table);
			//}


            foreach (Control c in splitContainer1.Panel2.Controls)
                c.Visible = false;
            foreach (Control c in splitContainer1.Panel2.Controls)
                c.Dock = DockStyle.Fill;

            try
            {
                m_isUpdating = true;

                FieldDatatype.Items.Add(typeof(string));
                FieldDatatype.Items.Add(typeof(bool));
                FieldDatatype.Items.Add(typeof(int));
                FieldDatatype.Items.Add(typeof(float));
                FieldDatatype.Items.Add(typeof(double));
                FieldDatatype.Items.Add(typeof(long));
                FieldDatatype.Items.Add(typeof(DateTime));

				foreach (ConfigurationContainer.Table mc in m_classes)
                    ReferenceReverseTablename.Items.Add(mc.Name);
            }
            finally
            {
                m_isUpdating = false;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RemoveButton.Enabled = treeView.SelectedNode != null;

            if (e == null || e.Node == null || e.Node.Tag == null)
                return;

            try
            {
                m_isUpdating = true;
                Control m = null;
				if (e.Node.Tag as ConfigurationContainer.Table != null)
				{
					ConfigurationContainer.Table ic = e.Node.Tag as ConfigurationContainer.Table;
					m = TableProperties;
					TableTablename.Text = ic.Name;
					TableIgnoreCheck.Checked = ic.Ignore;
				}
				//else if (e.Node.Tag as TypeConfiguration.IgnoredField != null)
				//{
				//    TypeConfiguration.IgnoredField i = e.Node.Tag as TypeConfiguration.IgnoredField;
				//    m = IgnoredFieldProperties;
				//    IgnoredFieldName.Text = i.Fieldname;
				//}
                //else if (e.Node.Tag as TypeConfiguration.MappedField != null)
				if (e.Node.Tag as ConfigurationContainer.Column != null)
                {
					ConfigurationContainer.Column mf = e.Node.Tag as ConfigurationContainer.Column;
                    m = FieldProperties;
                    FieldColumnname.Text = mf.Name;
                    FieldFieldname.Text = mf.Fieldname;
					FieldPropertyname.Text = mf.Fieldname;
					FieldDatatype.Text = mf.Typename;
                    FieldAutogenerate.Checked = mf.IgnoreWithInsert || mf.Autonumber;
                    FieldExcludeSelect.Checked = mf.IgnoreWithSelect;
                    FieldExcludeUpdate.Checked = mf.IgnoreWithUpdate;
                    try
                    {
                        FieldDefaultValue.Text = mf.Default == null ? "" : mf.Default.ToString();
                    }
                    catch
                    {
                        FieldDefaultValue.Text = "";
                    }
                }
				else if (e.Node.Tag as ConfigurationContainer.Relation != null)
                {
					ConfigurationContainer.Relation rf = e.Node.Tag as ConfigurationContainer.Relation;
                    m = ReferenceProperties;
                    ReferenceColumnname.Text = rf.Databasefield;
                    ReferencePropertyname.Text = rf.Propertyname;
					ReferenceType.Text = rf.Type.ToString();
                    ReferenceReverseTablename.Text = rf.ReverseTablename;
                    ReferenceReverseColumnname.Text = rf.ReverseDatabasefield;
                    ReferenceReversePropertyname.Text = rf.ReversePropertyname;
                    ReferenceRelationKey.Text = rf.Name;
                }

                foreach (Control c in splitContainer1.Panel2.Controls)
                    c.Visible = c == m;
            }
            finally
            {
                m_isUpdating = false;
            }
        }

        private void FieldColumnname_TextChanged(object sender, EventArgs e)
        {
			FieldFieldname.Items.Clear();
			FieldFieldname.Items.Add("m_" + FieldColumnname.Text);
			FieldPropertyname.Items.Clear();
			FieldPropertyname.Items.Add(FieldColumnname.Text);

			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			treeView.SelectedNode.Text = FieldColumnname.Text;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Name = FieldColumnname.Text;
        }

        private void FieldFieldname_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Fieldname = FieldFieldname.Text;
        }

        private void FieldPropertyname_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Fieldname = FieldPropertyname.Text;

        }

        private void FieldDatatype_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Typename = FieldDatatype.Text;
        }

        private void FieldAutogenerate_CheckedChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Autonumber = FieldAutogenerate.Checked;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).IgnoreWithInsert = FieldAutogenerate.Checked;
        }

        private void FieldExcludeUpdate_CheckedChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).IgnoreWithUpdate = FieldExcludeUpdate.Checked;
        }

        private void FieldExcludeSelect_CheckedChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).IgnoreWithSelect = FieldExcludeSelect.Checked;
        }

		private void FieldIndexCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Index = FieldIndexCheck.Checked;
		}

		private void TableIgnoreCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Table == null)
				return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Table).Ignore = TableIgnoreCheck.Checked;
		}

        private void ReferenceColumnname_TextChanged(object sender, EventArgs e)
        {
            ReferenceReversePropertyname.Items.Clear();
			if (treeView.SelectedNode != null && treeView.SelectedNode.Parent != null && treeView.SelectedNode.Parent.Tag as ConfigurationContainer.Table != null)
				ReferenceReversePropertyname.Items.Add((treeView.SelectedNode.Parent.Tag as ConfigurationContainer.Table).Name);

			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Relation == null)
                return;
            treeView.SelectedNode.Text = ReferenceColumnname.Text;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Relation).Databasefield = ReferenceColumnname.Text;
        }

        private void ReferencePropertyname_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Relation == null)
                return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Relation).Propertyname = ReferencePropertyname.Text;
        }

		private void ReferenceType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Relation == null)
				return;
			ConfigurationContainer.Relation.RelationType tmp = ConfigurationContainer.Relation.RelationType.OneToOne;
			try
			{
				tmp = (ConfigurationContainer.Relation.RelationType)Enum.Parse(typeof(ConfigurationContainer.Relation.RelationType), ReferenceType.Text);
			}
			catch
			{
				return;
			}
			(treeView.SelectedNode.Tag as ConfigurationContainer.Relation).Type = tmp;
		}

        private void ReferenceReverseTablename_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Relation == null)
                return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Relation).ReverseTablename = ReferenceReverseTablename.Text;
        }

        private void ReferenceReverseColumnname_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Relation == null)
                return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Relation).ReverseDatabasefield = ReferenceReverseColumnname.Text;
        }

        private void ReferenceReversePropertyname_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Relation == null)
                return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Relation).ReversePropertyname = ReferenceReversePropertyname.Text;
        }

        private void TableTablename_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null)
			    return;
			treeView.SelectedNode.Text = TableTablename.Text;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Table).Name = TableTablename.Text;
        }

        private void IgnoredFieldName_TextChanged(object sender, EventArgs e)
        {
			//if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.IgnoredField == null)
			//    return;
			//treeView.SelectedNode.Text = IgnoredFieldName.Text;
			//(treeView.SelectedNode.Tag as TypeConfiguration.IgnoredField).Fieldname = IgnoredFieldName.Text;

        }

        private void addReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show(this, "Please select a table where the reference will be inserted", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode table = treeView.SelectedNode;
            if (table.Parent != null)
                table = table.Parent;

			if (table.Tag as ConfigurationContainer.Table == null)
            {
                MessageBox.Show(this, "Please select a table where the reference will be inserted", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
				ConfigurationContainer.Relation rf = new ConfigurationContainer.Relation();
                rf.Name = Guid.NewGuid().ToString();
				rf.Tablename = (table.Tag as ConfigurationContainer.Table).Name;
				(table.Tag as ConfigurationContainer.Table).Relations.Add(rf);
                TreeNode field = new TreeNode(rf.Propertyname, IMAGE_REFERENCE, IMAGE_REFERENCE);
                field.Tag = rf;
                table.Nodes.Add(field);
                treeView.SelectedNode = field;
                field.EnsureVisible();
            }
        }

        private void addIgnoredFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show(this, "Please select a table where the ignored field will be inserted", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode table = treeView.SelectedNode;
            if (table.Parent != null)
                table = table.Parent;

			if (table.Tag as ConfigurationContainer.Table == null)
            {
                MessageBox.Show(this, "Please select a table where the ignored field be inserted", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
				//TypeConfiguration.IgnoredField rf = new TypeConfiguration.IgnoredField();
				//(table.Tag as TypeConfiguration.MappedClass).IgnoredFields.Add(Guid.NewGuid().ToString(), rf);
				//TreeNode field = new TreeNode(rf.Fieldname, IMAGE_IGNOREDFIELD, IMAGE_IGNOREDFIELD);
				//field.Tag = rf;
				//table.Nodes.Add(field);
				//treeView.SelectedNode = field;
				//field.EnsureVisible();
            }

        }

        private void addIgnoredTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
			//TypeConfiguration.IgnoredClass ic = new TypeConfiguration.IgnoredClass();
			//TreeNode field = new TreeNode(ic.Tablename, IMAGE_IGNOREDTABLE, IMAGE_IGNOREDTABLE);
			//field.Tag = ic;
			//treeView.Nodes.Add(field);
			//treeView.SelectedNode = field;
			//field.EnsureVisible();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show(this, "Please select an item to remove", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

			if (treeView.SelectedNode.Tag as ConfigurationContainer.Table != null)
            {
				//TypeConfiguration.IgnoredClass ic = new TypeConfiguration.IgnoredClass();
				//ic.Tablename = (treeView.SelectedNode.Tag as TypeConfiguration.MappedClass).Tablename;
				//treeView.SelectedNode.Remove();

				//TreeNode field = new TreeNode(ic.Tablename, IMAGE_IGNOREDTABLE, IMAGE_IGNOREDTABLE);
				//field.Tag = ic;
				//treeView.Nodes.Add(field);
				//treeView.SelectedNode = field;
				//field.EnsureVisible();
            }
			else if (treeView.SelectedNode.Tag as ConfigurationContainer.Column != null)
            {
				ConfigurationContainer.Table mc = treeView.SelectedNode.Parent.Tag as ConfigurationContainer.Table;
				//TypeConfiguration.IgnoredField rf = new TypeConfiguration.IgnoredField();
				//rf.Fieldname = (treeView.SelectedNode.Tag as DatabaseDiscover.Column).Name;
				//mc.IgnoredFields.Add(rf.Fieldname, rf);

				//foreach (DatabaseDiscover.Column col in mc.Columns)
				//    if (mc.MappedFields[key] == treeView.SelectedNode.Tag)
				//    {
				//        mc.MappedFields.Remove(key);
				//        break;
				//    }

				//TreeNode table = treeView.SelectedNode.Parent;
				//treeView.SelectedNode.Remove();

				//TreeNode field = new TreeNode(rf.Fieldname, IMAGE_IGNOREDFIELD, IMAGE_IGNOREDFIELD);
				//field.Tag = rf;
				//table.Nodes.Add(field);
				//treeView.SelectedNode = field;
				//field.EnsureVisible();
            }
			//else if (treeView.SelectedNode.Tag as TypeConfiguration.IgnoredClass != null)
			//{
			//    treeView.SelectedNode.Remove();
			//}
			//else if (treeView.SelectedNode.Tag as TypeConfiguration.IgnoredField != null)
			//{
			//    TypeConfiguration.MappedClass mc = treeView.SelectedNode.Parent.Tag as TypeConfiguration.MappedClass;
			//    foreach (string key in mc.IgnoredFields.Keys)
			//        if (mc.IgnoredFields[key] == treeView.SelectedNode.Tag)
			//        {
			//            mc.IgnoredFields.Remove(key);
			//            break;
			//        }

			//    treeView.SelectedNode.Remove();
			//}
			else if (treeView.SelectedNode.Tag as ConfigurationContainer.Relation != null)
            {
				ConfigurationContainer.Table mc = treeView.SelectedNode.Parent.Tag as ConfigurationContainer.Table;
				foreach (ConfigurationContainer.Relation r in mc.Relations)
                    if (r == treeView.SelectedNode.Tag)
                    {
                        mc.Relations.Remove(r);
                        break;
                    }

                treeView.SelectedNode.Remove();
            }
        }

        private void ReferenceReverseTablename_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReferenceReverseColumnname.Items.Clear();
            ReferencePropertyname.Items.Clear();

			foreach (ConfigurationContainer.Table mc in m_classes)
                if (mc.Name == ReferenceReverseTablename.Text)
                {
                    ReferencePropertyname.Items.Add(mc.Name);
                    foreach (ConfigurationContainer.Column col in mc.Columns)
                        ReferenceReverseColumnname.Items.Add(col.Name);
                    break;
                }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
			m_classes = new List<ConfigurationContainer.Table>();
			//m_ignoredclasses = new List<TypeConfiguration.IgnoredClass>();

			foreach (TreeNode n in treeView.Nodes)
				if (n.Tag as ConfigurationContainer.Table != null)
					m_classes.Add(n.Tag as ConfigurationContainer.Table);
				//else if (n.Tag as TypeConfiguration.IgnoredClass != null)
				//    m_ignoredclasses.Add(n.Tag as TypeConfiguration.IgnoredClass);

			this.DialogResult = DialogResult.OK;
			this.Close();
        }

		public ConfigurationContainer.Table[] Tables { get { return m_classes.ToArray(); } }
        //public List<TypeConfiguration.IgnoredClass> Ignored { get { return m_ignoredclasses; } }

        private void FieldDefaultValue_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Column == null)
                return;
			if ((treeView.SelectedNode.Tag as ConfigurationContainer.Column).Typename == "System.String")
				(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Default = FieldDefaultValue.Text;
            else if (FieldDefaultValue.Text.Trim().Length == 0)
				(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Default = null;
            else
                try
                {
					(treeView.SelectedNode.Tag as ConfigurationContainer.Column).Default = Convert.ChangeType(FieldDefaultValue.Text, (treeView.SelectedNode.Tag as ConfigurationContainer.Column).GetFieldType());
                    errorProvider1.SetError(FieldDefaultValue, null);
                }
                catch (Exception ex)
                {
                    errorProvider1.SetError(FieldDefaultValue, "Invalid value: " + ex.Message);
                }
        }

        private void GenerateRelationKey_Click(object sender, EventArgs e)
        {
            ReferenceRelationKey.Text = Guid.NewGuid().ToString();
        }

        private void ReferencePropertyname_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ReferenceRelationKey_TextChanged(object sender, EventArgs e)
        {
			if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as ConfigurationContainer.Relation == null)
                return;
			(treeView.SelectedNode.Tag as ConfigurationContainer.Relation).Name = ReferenceRelationKey.Text;
        }

    }
}
