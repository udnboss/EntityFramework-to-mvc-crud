using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFEnhancer
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            syntaxEditor1.Document.Language = cSharpSyntaxLanguage1;
            syntaxEditor2.Document.Language = xmlSyntaxLanguage1;
        }
        List<Table> dbTables;
        private void button1_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();

            var tables = new List<Table>();
            var props = typeof(COMMENTSEntities).GetProperties();
            foreach(var p in props)
            {
                if(p.PropertyType.Name.StartsWith("DbSet"))
                {
                    var type = p.PropertyType.GenericTypeArguments[0];
                    var table = new Table { Type = type, Name = type.Name };
                    tables.Add(table);

                    var subprops = type.GetProperties();
                    foreach(var sp in subprops)
                    {
                        var column = new Table.Column { Name = sp.Name, Type = sp.PropertyType };
                        table.Columns.Add(column);
                    }
                }
            }

            foreach(var t in tables)
            {
                var tableNode = new TreeNode(t.Name) { Tag = t };
                treeView1.Nodes.Add(tableNode);

                foreach(var c in t.Columns)
                {
                    var columnNode = new TreeNode(c.Name) { Tag = c };
                    tableNode.Nodes.Add(columnNode);
                }
            }

            dbTables = tables;


        }

        private void syntaxEditor1_ViewVerticalScroll(object sender, ActiproSoftware.SyntaxEditor.EditorViewEventArgs e)
        {
            //MessageBox.Show(e.View.FirstVisibleDisplayLineIndex.ToString());
            var line = e.View.FirstVisibleDisplayLineIndex;
            syntaxEditor2.Views[0].FirstVisibleDisplayLineIndex = line;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = e.Node.Tag;

            syntaxEditor1.Text = (e.Node.Tag as Table).GetCode(dbTables);
        }
    }
}
