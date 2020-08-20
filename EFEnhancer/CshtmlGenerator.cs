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
    class CshtmlGenerator
    {
        List<Table> Tables { get; set; }
        Table Table { get; set; }
        string Namespace { get; set; }
        public CshtmlGenerator(string _namespace, List<Table> tables, Table table)
        {
            this.Tables = tables;
            this.Table = table;
            this.Namespace = _namespace;
        }

        public string GenerateCode(string templateName)
        {
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName));
            var template_name = templateName.Split('_')[0];
            template = template
                    .Replace("_namespace_", Namespace)
                    .Replace("_table_", Table.Name)
                    .Replace("_theadColumns_", GetTheadColumns())
                    .Replace("_tbodyColumns_", GetTbodyColumns())
                    .Replace("_detailFields_", GetFormDetail(template_name))
                    .Replace("_editFields_", GetFormFields(template_name))
                    .Replace("_newFields_", GetFormFields(template_name))
                    .Replace("_displaycolumn_", Table.DisplayColumn.Name)
                    .Replace("_tabitems_", GetTabItems(template_name))
                    .Replace("_tabpanes_", GetTabPanes(template_name))
                    .Replace("_pkname_", Table.Columns[0].Name)
                    ;

            return template;
        }

        private string GetTabPanes(string templateName)
        {
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_TabPane.cshtml"));
            template = template.Replace("_table_", Table.Name);

            var tabs = new List<string>();
            foreach (var d in Table.Dependents)
            {
                tabs.Add(template.Replace("_childtable_", d.Name)
                    .Replace("_pkname_", Table.Columns[0].Name)
                    .Replace("_childfkcolname_", d.ForeignKeys.First(x => x.Value == Table).Key.Name)
                    );
            }

            return string.Join("\r\n", tabs);
        }

        private string GetTabItems(string templateName)
        {
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_TabItem.cshtml"));
            template = template.Replace("_table_", Table.Name);

            var tabs = new List<string>();
            foreach (var d in Table.Dependents)
            {
                tabs.Add(template.Replace("_childtable_", d.Name));
            }

            return string.Join("\r\n", tabs);
        }

        string GetTheadColumns()
        {
            var ths = Table.PrimitiveColumns.Where(x => x.Name != "ID").Select(x => string.Format("<th>@Html.DisplayNameFor(m => m.{0})</th>", x.Name)).ToList();
            return string.Join("\r\n\t\t\t\t\t", ths);
        }

        string GetTbodyColumns()
        {
            var template = "<td>@Html.DisplayFor(modelItem => item.{0})</td>";
            var tds = new List<string>();
            foreach(var c in Table.Columns.Where(x => x.Name != "ID"))
            {
                if (c.IsSimpleType() || c.IsForeignKey)
                {
                    var name = c.Name;
                    if (c.IsForeignKey)
                    {

                        name = c.ReferenceTable.Name + "." + c.ReferenceTable.DisplayColumn.Name;
                    }

                    tds.Add(string.Format(template, name));
                }
            }
            
            return string.Join("\r\n\t\t\t\t\t\t", tds);
        }

        string GetFormFields(string templateName)
        {
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_FormField.cshtml"));
            var ddlTemplate = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_FormLookupField.cshtml"));
            var fields = new List<string>();

            var primitiveColumns = Table.PrimitiveColumns;
            
            foreach(var c in Table.Columns)
            {
                if(c.IsForeignKey) //FK, get as a dropdownlist 
                {
                    var referenceTable = c.ReferenceTable;
                    var columnTemplate = ddlTemplate.Replace("_column_", c.Name).Replace("_table_", referenceTable.Name).Replace("\r\n", "\r\n\t\t");
                    fields.Add(columnTemplate);
                }
                else if(primitiveColumns.Contains(c)) //primitive 
                {
                    if(c.Type == typeof(Guid)) //guid
                    {
                        var columnTemplate = string.Format("@Html.HiddenFor(m => m.{0})", c.Name);
                        fields.Add(columnTemplate);
                    }
                    else //primitive
                    {
                        var columnTemplate = template.Replace("_column_", c.Name).Replace("\r\n", "\r\n\t\t");
                        fields.Add(columnTemplate);
                    }
                    
                }

            }

            return string.Join("\r\n\r\n\t\t", fields);
        }

        string GetFormDetail(string templateName)
        {
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_FormDetail.cshtml"));

            var fields = new List<string>();
            foreach (var c in Table.Columns)
            {                
                if (c.IsSimpleType() || c.IsForeignKey)
                {
                    var name = c.Name;
                    if (c.IsForeignKey)
                    {
                        name = c.ReferenceTable.Name + "." + c.ReferenceTable.DisplayColumn.Name;                        
                    }
                    fields.Add(template.Replace("_column_", c.Name).Replace("_refcolumn_", name).Replace("\r\n", "\r\n\t\t"));
                }
            }

            return string.Join("\r\n\r\n\t\t", fields);
        }
    }
}
