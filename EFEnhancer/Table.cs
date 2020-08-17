using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFEnhancer
{
    public class Table
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public List<Column> Columns { get; set; }

        public Table()
        {
            Columns = new List<Column>();
        }
      
        private string GetLookups(List<Table> otherTables)
        {
            var lookups = new Dictionary<Table.Column, Table>();
            foreach(var c in Columns)
            {
                foreach(var t in otherTables)
                {
                    if(c.Type == t.Type)
                    {
                        //foreign key detected
                        lookups.Add(c,t);
                    }
                }
            }
            var lookupStrs = new List<string>();
            foreach(var lookup in lookups)
            {
                var c = lookup.Key;
                var t = lookup.Value;
                var pk = t.Columns[0];
                var textCol = t.Columns[1].Name == "Name" ? t.Columns[1] : t.Columns[0];
                var tmp = string.Format(@"{{""{0}"", db.{1}.Select(x => new {{ id = x.{2}, text = x.{3} }}).ToList() }}", c.Name, t.Name, pk.Name, textCol.Name);
                lookupStrs.Add(tmp);
            }

            return string.Join(",\n\t\t\t\t", lookupStrs);
        }

        public string GetCode(List<Table> otherTables, string _namespace = "WorkflowWeb", string _controller = "Comment")
        {
            var pktype = Columns[0].Type.Name;
            var lookups = GetLookups(otherTables);

            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\Vue.cs"));

            template = template
                            .Replace("_namespace_", _namespace)
                            .Replace("_controller_", _controller)
                            .Replace("_table_", Name)
                            .Replace("_pktype_", pktype)
                            .Replace("_lookups_", lookups);

            return template;
        }
        public class Column
        {
            public Type Type { get; set; }
            public string Name { get; set; }
        }
    }
}
