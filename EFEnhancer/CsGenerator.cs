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
            var pkname = Table.Columns[0].Name;
            var lookups = this.GetLookups();
            var defaults = this.GetDefaults();
            var include = this.GetInclude();
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName));

            template = template
                            .Replace("_namespace_", Namespace)
                            .Replace("_controller_", Table.Name)
                            .Replace("_table_", Table.Name)
                            .Replace("_include_", include)
                            .Replace("_pktype_", pktype)
                            .Replace("_setnewguid_", pktype == "Guid" ? "m.ID = Guid.NewGuid(); " : "")
                            .Replace("_pkname_", pkname)
                            .Replace("_lookups_", lookups)
                            .Replace("_defaults_", defaults);

            return template;
        }

        private string GetInclude()
        {
            var template = ".Include(x => x.{0})";
            var includes = Table.Columns.Where(c => c.NavigationProperty != null).Select(c => string.Format(template, c.NavigationProperty.Name));
            return string.Join("\r\n\t\t\t\t", includes);
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

            return string.Join(",\r\n\t\t\t\t", lookupStrs);
        }        

        private string GetDefaults()
        {
            var defaults = new List<string>();
            foreach(var c in Table.PrimitiveColumns)
            {
                if(c.Type == typeof(DateTime))
                {
                    var d = string.Format("{0} = DateTime.Now", c.Name);
                    defaults.Add(d);
                }
            }

            return string.Join(",\r\n\t\t\t\t", defaults);
        }
    }
}
