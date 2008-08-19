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

        private List<TypeConfiguration.MappedClass> m_classes;
        private List<TypeConfiguration.IgnoredClass> m_ignoredclasses;
        private bool m_isUpdating = false;

        private DataClassCustomizer()
        {
            InitializeComponent();
        }

        public DataClassCustomizer(List<TypeConfiguration.MappedClass> classes, List<TypeConfiguration.IgnoredClass> ignoredclasses)
            : this()
        {
            m_classes = classes;
            m_ignoredclasses = ignoredclasses;

            foreach (TypeConfiguration.MappedClass mc in m_classes)
            {
                TreeNode table = new TreeNode(mc.TableName, IMAGE_TABLE, IMAGE_TABLE);
                table.Tag = mc;

                foreach (TypeConfiguration.MappedField mf in mc.Columns.Values)
                {
                    TreeNode field = new TreeNode(mf.ColumnName, IMAGE_FIELD, IMAGE_FIELD);
                    field.Tag = mf;
                    table.Nodes.Add(field);
                }

                foreach (TypeConfiguration.ReferenceField rf in mc.ReferenceColumns.Values)
                {
                    TreeNode field = new TreeNode(rf.PropertyName, IMAGE_REFERENCE, IMAGE_REFERENCE);
                    field.Tag = rf;
                    table.Nodes.Add(field);
                }

                foreach (TypeConfiguration.IgnoredField i in mc.IgnoredFields.Values)
                {
                    TreeNode field = new TreeNode(i.Fieldname, IMAGE_IGNOREDFIELD, IMAGE_IGNOREDFIELD);
                    field.Tag = i;
                    table.Nodes.Add(field);
                }

                treeView.Nodes.Add(table);
            }

            foreach (TypeConfiguration.IgnoredClass ic in m_ignoredclasses)
            {
                TreeNode table = new TreeNode(ic.Tablename, IMAGE_IGNOREDTABLE, IMAGE_IGNOREDTABLE);
                table.Tag = ic;
                treeView.Nodes.Add(table);
            }


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

                foreach (TypeConfiguration.MappedClass mc in m_classes)
                    ReferenceReverseTablename.Items.Add(mc.TableName);
            }
            finally
            {
                m_isUpdating = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

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
                if (e.Node.Tag as TypeConfiguration.IgnoredClass != null)
                {
                    TypeConfiguration.IgnoredClass ic = e.Node.Tag as TypeConfiguration.IgnoredClass;
                    m = TableProperties;
                    TableTablename.Text = ic.Tablename;
                }
                else if (e.Node.Tag as TypeConfiguration.IgnoredField != null)
                {
                    TypeConfiguration.IgnoredField i = e.Node.Tag as TypeConfiguration.IgnoredField;
                    m = IgnoredFieldProperties;
                    IgnoredFieldName.Text = i.Fieldname;
                }
                else if (e.Node.Tag as TypeConfiguration.MappedField != null)
                {
                    TypeConfiguration.MappedField mf = e.Node.Tag as TypeConfiguration.MappedField;
                    m = FieldProperties;
                    FieldColumnname.Text = mf.ColumnName;
                    FieldFieldname.Text = mf.FieldName;
                    FieldPropertyname.Text = mf.PropertyName;
                    FieldDatatype.Text = mf.DataTypeName;
                    FieldAutogenerate.Checked = mf.IgnoreWithInsert || mf.IsAutoGenerated;
                    FieldExcludeSelect.Checked = mf.IgnoreWithSelect;
                    FieldExcludeUpdate.Checked = mf.IgnoreWithUpdate;
                    try
                    {
                        FieldDefaultValue.Text = mf.DefaultValue == null ? "" : mf.DefaultValue.ToString();
                    }
                    catch
                    {
                        FieldDefaultValue.Text = "";
                    }
                }
                else if (e.Node.Tag as TypeConfiguration.ReferenceField != null)
                {
                    TypeConfiguration.ReferenceField rf = e.Node.Tag as TypeConfiguration.ReferenceField;
                    m = ReferenceProperties;
                    ReferenceColumnname.Text = rf.Column;
                    ReferencePropertyname.Text = rf.PropertyName;
                    ReferenceIsCollection.Checked = rf.IsCollection;
                    ReferenceReverseTablename.Text = rf.ReverseTablename;
                    ReferenceReverseColumnname.Text = rf.ReverseColumn;
                    ReferenceReversePropertyname.Text = rf.ReversePropertyName;
                    ReferenceRelationKey.Text = rf.RelationKey;
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

            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            treeView.SelectedNode.Text = FieldColumnname.Text;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).ColumnName = FieldColumnname.Text;
        }

        private void FieldFieldname_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).FieldName = FieldFieldname.Text;
        }

        private void FieldPropertyname_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).PropertyName = FieldPropertyname.Text;

        }

        private void FieldDatatype_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).DataTypeName = FieldColumnname.Text;
        }

        private void FieldAutogenerate_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).IsAutoGenerated = FieldAutogenerate.Checked;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).IgnoreWithInsert = FieldAutogenerate.Checked;
        }

        private void FieldExcludeUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).IgnoreWithUpdate = FieldExcludeUpdate.Checked;
        }

        private void FieldExcludeSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).IgnoreWithSelect = FieldExcludeSelect.Checked;
        }

        private void ReferenceColumnname_TextChanged(object sender, EventArgs e)
        {
            ReferenceReversePropertyname.Items.Clear();
            if (treeView.SelectedNode != null && treeView.SelectedNode.Parent != null && treeView.SelectedNode.Parent.Tag as TypeConfiguration.MappedClass != null)
                ReferenceReversePropertyname.Items.Add((treeView.SelectedNode.Parent.Tag as TypeConfiguration.MappedClass).TableName);

            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField == null)
                return;
            treeView.SelectedNode.Text = ReferenceColumnname.Text;
            (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField).Column = ReferenceColumnname.Text;
        }

        private void ReferencePropertyname_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField).PropertyName = ReferencePropertyname.Text;
        }

        private void ReferenceIsCollection_CheckedChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField).IsCollection = ReferenceIsCollection.Checked;
        }

        private void ReferenceReverseTablename_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField).ReverseTablename = ReferenceReverseTablename.Text;
        }

        private void ReferenceReverseColumnname_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField).ReverseColumn = ReferenceReverseColumnname.Text;
        }

        private void ReferenceReversePropertyname_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField).ReversePropertyName = ReferenceReversePropertyname.Text;
        }

        private void TableTablename_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.IgnoredClass == null)
                return;
            treeView.SelectedNode.Text = TableTablename.Text;
            (treeView.SelectedNode.Tag as TypeConfiguration.IgnoredClass).Tablename = TableTablename.Text;
        }

        private void IgnoredFieldName_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.IgnoredField == null)
                return;
            treeView.SelectedNode.Text = IgnoredFieldName.Text;
            (treeView.SelectedNode.Tag as TypeConfiguration.IgnoredField).Fieldname = IgnoredFieldName.Text;

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

            if (table.Tag as TypeConfiguration.MappedClass == null)
            {
                MessageBox.Show(this, "Please select a table where the reference will be inserted", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                TypeConfiguration.ReferenceField rf = TypeConfiguration.CreateReferenceField();
                rf.RelationKey = Guid.NewGuid().ToString();
                (table.Tag as TypeConfiguration.MappedClass).ReferenceColumns.Add(Guid.NewGuid().ToString(), rf);
                TreeNode field = new TreeNode(rf.PropertyName, IMAGE_REFERENCE, IMAGE_REFERENCE);
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

            if (table.Tag as TypeConfiguration.MappedClass == null)
            {
                MessageBox.Show(this, "Please select a table where the ignored field be inserted", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                TypeConfiguration.IgnoredField rf = new TypeConfiguration.IgnoredField();
                (table.Tag as TypeConfiguration.MappedClass).IgnoredFields.Add(Guid.NewGuid().ToString(), rf);
                TreeNode field = new TreeNode(rf.Fieldname, IMAGE_IGNOREDFIELD, IMAGE_IGNOREDFIELD);
                field.Tag = rf;
                table.Nodes.Add(field);
                treeView.SelectedNode = field;
                field.EnsureVisible();
            }

        }

        private void addIgnoredTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TypeConfiguration.IgnoredClass ic = new TypeConfiguration.IgnoredClass();
            TreeNode field = new TreeNode(ic.Tablename, IMAGE_IGNOREDTABLE, IMAGE_IGNOREDTABLE);
            field.Tag = ic;
            treeView.Nodes.Add(field);
            treeView.SelectedNode = field;
            field.EnsureVisible();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null)
            {
                MessageBox.Show(this, "Please select an item to remove", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (treeView.SelectedNode.Tag as TypeConfiguration.MappedClass != null)
            {
                TypeConfiguration.IgnoredClass ic = new TypeConfiguration.IgnoredClass();
                ic.Tablename = (treeView.SelectedNode.Tag as TypeConfiguration.MappedClass).TableName;
                treeView.SelectedNode.Remove();

                TreeNode field = new TreeNode(ic.Tablename, IMAGE_IGNOREDTABLE, IMAGE_IGNOREDTABLE);
                field.Tag = ic;
                treeView.Nodes.Add(field);
                treeView.SelectedNode = field;
                field.EnsureVisible();
            }
            else if (treeView.SelectedNode.Tag as TypeConfiguration.MappedField != null)
            {
                TypeConfiguration.MappedClass mc = treeView.SelectedNode.Parent.Tag as TypeConfiguration.MappedClass;
                TypeConfiguration.IgnoredField rf = new TypeConfiguration.IgnoredField();
                rf.Fieldname = (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).ColumnName;
                mc.IgnoredFields.Add(rf.Fieldname, rf);

                foreach(string key in mc.Columns.Keys)
                    if (mc.Columns[key] == treeView.SelectedNode.Tag)
                    {
                        mc.Columns.Remove(key);
                        break;
                    }

                TreeNode table = treeView.SelectedNode.Parent;
                treeView.SelectedNode.Remove();

                TreeNode field = new TreeNode(rf.Fieldname, IMAGE_IGNOREDFIELD, IMAGE_IGNOREDFIELD);
                field.Tag = rf;
                table.Nodes.Add(field);
                treeView.SelectedNode = field;
                field.EnsureVisible();
            }
            else if (treeView.SelectedNode.Tag as TypeConfiguration.IgnoredClass != null)
            {
                treeView.SelectedNode.Remove();
            }
            else if (treeView.SelectedNode.Tag as TypeConfiguration.IgnoredField != null)
            {
                TypeConfiguration.MappedClass mc = treeView.SelectedNode.Parent.Tag as TypeConfiguration.MappedClass;
                foreach (string key in mc.IgnoredFields.Keys)
                    if (mc.IgnoredFields[key] == treeView.SelectedNode.Tag)
                    {
                        mc.IgnoredFields.Remove(key);
                        break;
                    }

                treeView.SelectedNode.Remove();
            }
            else if (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField != null)
            {
                TypeConfiguration.MappedClass mc = treeView.SelectedNode.Parent.Tag as TypeConfiguration.MappedClass;
                foreach (string key in mc.ReferenceColumns.Keys)
                    if (mc.ReferenceColumns[key] == treeView.SelectedNode.Tag)
                    {
                        mc.ReferenceColumns.Remove(key);
                        break;
                    }

                treeView.SelectedNode.Remove();
            }
        }

        private void ReferenceReverseTablename_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReferenceReverseColumnname.Items.Clear();
            ReferencePropertyname.Items.Clear();

            foreach(TypeConfiguration.MappedClass mc in m_classes)
                if (mc.TableName == ReferenceReverseTablename.Text)
                {
                    ReferencePropertyname.Items.Add(mc.TableName);
                    foreach (string s in mc.Columns.Keys)
                        ReferenceReverseColumnname.Items.Add(s);
                    break;
                }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            m_classes = new List<TypeConfiguration.MappedClass>();
            m_ignoredclasses = new List<TypeConfiguration.IgnoredClass>();

            foreach (TreeNode n in treeView.Nodes)
                if (n.Tag as TypeConfiguration.MappedClass != null)
                    m_classes.Add(n.Tag as TypeConfiguration.MappedClass);
                else if (n.Tag as TypeConfiguration.IgnoredClass != null)
                    m_ignoredclasses.Add(n.Tag as TypeConfiguration.IgnoredClass);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public List<TypeConfiguration.MappedClass> Tables { get { return m_classes; } }
        public List<TypeConfiguration.IgnoredClass> Ignored { get { return m_ignoredclasses; } }

        private void FieldDefaultValue_TextChanged(object sender, EventArgs e)
        {
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.MappedField == null)
                return;
            if ((treeView.SelectedNode.Tag as TypeConfiguration.MappedField).DataType == typeof(string))
                (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).DefaultValue = FieldDefaultValue.Text;
            else if (FieldDefaultValue.Text.Trim().Length == 0)
                (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).DefaultValue = null;
            else
                try
                {
                    (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).DefaultValue = Convert.ChangeType(FieldDefaultValue.Text, (treeView.SelectedNode.Tag as TypeConfiguration.MappedField).DataType);
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
            if (m_isUpdating || treeView.SelectedNode == null || treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField == null)
                return;
            (treeView.SelectedNode.Tag as TypeConfiguration.ReferenceField).RelationKey = ReferenceRelationKey.Text;
        }

    }
}
