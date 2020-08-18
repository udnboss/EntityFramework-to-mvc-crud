using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
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

            var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\"), "*.cs");
            var data = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
            comboBox1.DataSource = data;

            db = new COMMENTSEntities();

            syntaxEditor1.Document.Language = cSharpSyntaxLanguage1;
            syntaxEditor2.Document.Language = xmlSyntaxLanguage1;
        }
        List<Table> dbTables;
        DbContext db;
        private void button1_Click(object sender, EventArgs e)
        {

            var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\"), "*.cshtml").Select(x => Path.GetFileName(x));

            treeView1.Nodes.Clear();
            var parser = new EfParser(db);
            var tables = parser.GetTables();
            
            //build tree
            foreach(var t in tables)
            {
                var tableNode = new TreeNode(t.Name) { Tag = t };
                treeView1.Nodes.Add(tableNode);
                TreeNode columnsFolder = tableNode.Nodes.Add("Columns");
                foreach(var c in t.Columns)
                {
                    var columnNode = new TreeNode(c.Name) { Tag = c };
                    columnsFolder.Nodes.Add(columnNode);
                }

                TreeNode templatesFolder = tableNode.Nodes.Add("Templates");
                foreach (var file in files)
                {
                    var fileNode = new TreeNode(file) { Tag = file };
                    templatesFolder.Nodes.Add(fileNode);
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
            if(e.Node.Tag is string)
            {
                var _namespace = "WorkflowWeb";
                var table = (e.Node.Parent.Parent.Tag as Table);
                var cshtmlGen = new CshtmlGenerator(_namespace, dbTables, table);
                var csGen = new CsGenerator(_namespace, dbTables, table);
                var templateName = e.Node.Tag.ToString();
                var codeTemplate = templateName.Split('_')[0] + ".cs";
                syntaxEditor1.Text = csGen.GenerateCode(codeTemplate);
                syntaxEditor2.Text = cshtmlGen.GenerateCode(templateName);
            }

        }
    }
}
