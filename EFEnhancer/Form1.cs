using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            treeView1.AfterSelect += treeView1_AfterSelect;

            var path = @"C:\Users\Ali\Source\Repos\WorkflowWeb\WorkflowWeb";
            BuildTree(new DirectoryInfo(path), treeView1.Nodes);
            //remove unnecessary folders
            string[] valid = "Areas,Controllers,Views".Split(',');
            List<TreeNode> toRemove = new List<TreeNode>();
            foreach(TreeNode n in treeView1.Nodes[0].Nodes)
            {
                if(!valid.Contains(n.Text))
                {
                    toRemove.Add(n);                    
                }
            }

            toRemove.ForEach((n) => { treeView1.Nodes.Remove(n); });

            treeView1.ExpandAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    var di = new DirectoryInfo(fbd.SelectedPath);
                    BuildTree(di, treeView1.Nodes);
                }
            }
        }

        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe)
        {
            TreeNode curNode = addInMe.Add(directoryInfo.Name);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                curNode.Nodes.Add(file.FullName, file.Name);
            }
            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
            {
                BuildTree(subdir, curNode.Nodes);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (e.Node.Name.EndsWith(".cs") || e.Node.Name.EndsWith(".cshtml"))
            {
               
                StreamReader reader = new StreamReader(e.Node.Name);
                if (e.Node.Name.EndsWith(".cs"))
                {
                    syntaxEditor1.Document.Language = cSharpSyntaxLanguage1;
                    syntaxEditor2.Document.Language = syntaxEditor1.Document.Language;
                }
                else
                {
                    syntaxEditor1.Document.Language = xmlSyntaxLanguage1;
                    syntaxEditor2.Document.Language = syntaxEditor1.Document.Language;
                }


                syntaxEditor1.Text = reader.ReadToEnd();
                
                syntaxEditor2.Text = syntaxEditor1.Text;
                reader.Close();
            }
        }

        private void syntaxEditor1_ViewVerticalScroll(object sender, ActiproSoftware.SyntaxEditor.EditorViewEventArgs e)
        {
            //MessageBox.Show(e.View.FirstVisibleDisplayLineIndex.ToString());
            var line = e.View.FirstVisibleDisplayLineIndex;
            syntaxEditor2.Views[0].FirstVisibleDisplayLineIndex = line;
        }
    }
}
