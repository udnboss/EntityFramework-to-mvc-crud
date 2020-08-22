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
            var properties = this.GetProperties();
            var copy = this.GetCopyProperties();
            var tomodel = this.GetToModelProperties();
            var filterconditions = this.GetFilterConditions();

            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName));

            template = template
                            .Replace("_namespace_", Namespace)
                            .Replace("_controller_", Table.Name)
                            .Replace("_table_", Table.Name)
                            .Replace("_include_", include)
                            .Replace("_pktype_", pktype)
                            .Replace("_nullablepktype_", pktype == "String" ? pktype : pktype + "?")
                            .Replace("_setnewguid_", pktype == "Guid" ? "m.ID = Guid.NewGuid(); " : "")
                            .Replace("_pkname_", pkname)
                            .Replace("_lookups_", lookups)
                            .Replace("_defaults_", defaults)
                            .Replace("_properties_", properties)
                            .Replace("_copyproperties_", copy)
                            .Replace("_tomodelproperties_", tomodel)
                            .Replace("_filterconditions_", filterconditions)
                            .Replace("_primitivecols_", string.Join(", ", Table.PrimitiveColumns.Select(x => x.Name)))
                            ;

            return template;
        }

        private string GetFilterConditions()
        {
            var filterconditions = Table.PrimitiveColumns.Select(x => string.Format("if (filter.{0} != null && filter.{0}.ToString() != \"00000000-0000-0000-0000-000000000000\") data = data.Where(x => x.{0} == filter.{0});", x.Name)).ToList();
            return string.Join("\r\n\t\t\t\t\t", filterconditions);
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

        private string GetProperties()
        {
            var tableNames = Tables.Select(x => x.Name).ToList();

            var props = new List<string>();
            foreach (var c in Table.Columns)
            {
                var lines = new List<string>();
                if(c.Name == "ID")
                {
                    lines.Add(string.Format("[Required(AllowEmptyStrings = false, ErrorMessage = \"{0} is required.\")]", c.DisplayName));
                }
                else if (c.Name == "Name" || c.Name == "Title")
                {
                    lines.Add(string.Format("[Required(AllowEmptyStrings = false, ErrorMessage = \"{0} is required.\")]", c.DisplayName));
                }

                lines.Add(string.Format("[DisplayName(\"{0}\")]", c.DisplayName));

                var typename = c.ActualType.Name;
                if(tableNames.Contains(typename)) //this is an entity reference?
                {
                    typename = string.Format("{0}ViewModel", typename);
                }
                               
                if(c.IsCollection)
                {
                    lines.Add(string.Format("public List<{0}> {1} {{ get; set; }}", typename, c.Name));
                }
                else
                {
                    lines.Add(string.Format("public {0} {1} {{ get; set; }}", typename + (c.Type.Name.StartsWith("Nullable") ? "?" : ""), c.Name));
                }

                lines.Add("");

                props.Add(string.Join("\r\n\t\t", lines));
            }

            return string.Join("\r\n\t\t", props);
        }

        private string GetCopyProperties()
        {
            var tableNames = Tables.Select(x => x.Name).ToList();

            var props = new List<string>();
            foreach (var c in Table.Columns)
            {
                var typename = c.ActualType.Name;
                if (tableNames.Contains(typename)) //this is an entity reference?
                {
                    typename = string.Format("{0}ViewModel", typename);

                    if (c.IsCollection)
                    {
                        props.Add(string.Format("this.{0} = convertSubs && m.{0} != null ? m.{0}.Select(x => new {1}(x)).ToList() : null;", c.Name, typename));
                    }
                    else
                    {
                        props.Add(string.Format("this.{0} = convertSubs ? new {1}(m.{0}) : null;", c.Name, typename));
                    }
                }
                else
                {
                    if (c.IsCollection)
                    {
                        props.Add(string.Format("this.{0} = m.{0}.ToList();", c.Name));
                    }
                    else
                    {
                        props.Add(string.Format("this.{0} = m.{0};", c.Name));
                    }
                }
                
               
            }

            return string.Join("\r\n\t\t\t\t", props);
        }

        private string GetToModelProperties()
        {
            var tableNames = Tables.Select(x => x.Name).ToList();

            var props = new List<string>();
            foreach (var c in Table.Columns)
            {
                var typename = c.ActualType.Name;
                if (tableNames.Contains(typename)) //this is an entity reference?
                {
                    if (c.IsCollection)
                    {
                        props.Add(string.Format("m.{0} = convertSubs && this.{0} != null  ? this.{0}.Select(x => x.ToModel()).ToList() : null;", c.Name, typename));
                    }
                    else
                    {
                        props.Add(string.Format("m.{0} = convertSubs && this.{0} != null ?  this.{0}.ToModel() : null;", c.Name, typename));
                    }
                }
                else
                {
                    props.Add(string.Format("m.{0} = this.{0};", c.Name));
                }
            }

            return string.Join("\r\n\t\t\t", props);
        }
    }
}
