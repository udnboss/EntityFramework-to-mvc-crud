﻿using System;
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

            template = template
                    .Replace("_namespace_", Namespace)
                    .Replace("_table_", Table.Name)
                    .Replace("_theadColumns_", GetTheadColumns())
                    .Replace("_tbodyColumns", GetTbodyColumns())
                    .Replace("_detailFields_", GetFormDetail(templateName.Split('_')[0]))
                    .Replace("_editFields_", GetFormFields(templateName.Split('_')[0]))
                    .Replace("_newFields_", GetFormFields(templateName.Split('_')[0]));

            return template;
        }

        string GetTheadColumns()
        {
            var ths = Table.PrimitiveColumns.Select(x => string.Format("<th>@Html.DisplayNameFor(m => m.{0})</th>", x.Name)).ToList();
            return string.Join("\r\t\t\t\t\t", ths);
        }

        string GetTbodyColumns()
        {
            var tds = Table.PrimitiveColumns.Select(x => string.Format("<td>@Html.DisplayFor(modelItem => item.{0})</td>", x.Name)).ToList();
            return string.Join("\r\t\t\t\t\t\t", tds);
        }

        string GetFormFields(string templateName)
        {
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_FormField.cshtml"));
            var ddlTemplate = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_FormLookupField.cshtml"));
            var fields = new List<string>();

            var primitiveColumns = Table.PrimitiveColumns;
            
            foreach(var c in Table.Columns)
            {
                if(c.IsForeignKey) //FK
                {
                    var referenceTable = c.ReferenceTable;
                    var columnTemplate = ddlTemplate.Replace("_column_", c.Name).Replace("_table_", referenceTable.Name).Replace("\r\n", "\r\n\t\t");
                    fields.Add(columnTemplate);
                }
                else if(primitiveColumns.Contains(c)) //primitive
                {
                    var columnTemplate = template.Replace("_column_", c.Name).Replace("\r\n", "\r\n\t\t");
                    fields.Add(columnTemplate);
                }
            }

            return string.Join("\r\n\r\n\t\t", fields);
        }

        string GetFormDetail(string templateName)
        {
            var template = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "templates\\" + templateName + "_FormDetail.cshtml"));
            var fields = Table.Columns.Select(c => template.Replace("_column_", c.Name).Replace("\r\n", "\r\n\t\t")).ToList();

            return string.Join("\r\n\r\n\t\t", fields);
        }
    }
}
