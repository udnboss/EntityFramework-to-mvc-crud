using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFEnhancer
{
    class CsGenerator
    {
        List<Table> Tables { get; set; }
        Table Table { get; set; }
        string Namespace { get; set; }
        public CsGenerator( string _namespace, List<Table> tables, Table table)
        {
            this.Tables = tables;
            this.Table = table;
            this.Namespace = _namespace;
        }

        public string GenerateCode(string templateName)
        {
            var pktype = Table.Columns[0].Type.Name;
            var lookups = this.GetLookups();

            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName));

            template = template
                            .Replace("_namespace_", Namespace)
                            .Replace("_controller_", Table.Name)
                            .Replace("_table_", Table.Name)
                            .Replace("_pktype_", pktype)
                            .Replace("_lookups_", lookups);

            return template;
        }

        private string GetLookups()
        {
            var lookupStrs = new List<string>();
            foreach (var lookup in Table.ForeignKeys)
            {
                var c = lookup.Key;
                var t = lookup.Value;
                var pk = t.Columns[0];
                var textCol = t.Columns[1].Name == "Name" ? t.Columns[1] : t.Columns[0];
                var tmp = string.Format(@"{{""{0}"", db.{1}.Select(x => new  SelectListItem {{ Value = x.{2}.ToString(), Text = x.{3}.ToString() }}) }}", c.Name, t.Name, pk.Name, textCol.Name);
                lookupStrs.Add(tmp);
            }

            return string.Join(",\n\t\t\t\t", lookupStrs);
        }        
    }
}
