using ActiproSoftware.SyntaxEditor.Addons.DotNet.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EFEnhancer
{
    public class Table
    {
        public Type Type { get; set; }
        public string Name { get; set; }

        public string DisplayName
        {
            get
            {
                var dn = Name.Any(char.IsUpper) ? Regex.Replace(Name, "([a-z])([A-Z])", "$1 $2") : Name;
                if(dn.Contains('_'))
                {
                    dn = dn.Substring(dn.IndexOf('_') + 1);
                }
                return dn;
            }
        }
        public List<Column> Columns { get; set; }
        public Dictionary<Column, Table> ForeignKeys { get; set; }

        public List<Table> Dependents { get; set; }
        public Column DisplayColumn
        {
            get
            {
                return Columns.FirstOrDefault(x => x.Name == "Name" || x.Name == "Title" || x.Type == typeof(String)) ?? Columns.First();
            }
        }
        public List<Column> PrimitiveColumns
        {
            get
            {
                var primitive = Columns
                        .Where(x => x.IsSimpleType())
                        .ToList();

                return primitive;
            }
        }
        public Table()
        {
            Columns = new List<Column>();
            ForeignKeys = new Dictionary<Column, Table>();
            Dependents = new List<Table>();
        }

        public List<TableParentRelation> Parents { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public class TableParentRelation
        {
            public Table Table { get; set; }
            public List<TableParentRelation> Parents { get; set; }

            public TableParentRelation() 
            {
                Parents = new List<TableParentRelation>();
            }
        }

        public class Column
        {
            public Type Type { get; set; }
            public string Name { get; set; }

            public bool IsPrimaryKey { get { return Name == "ID"; } }      
            
            public bool IsForeignKey { get { return ReferenceTable != null; } }

            public bool IsCollection
            {
                get
                {
                    return Type.IsInterface;
                }
            }

            public Type ActualType
            {
                get
                {
                    return IsCollection || Type.IsGenericType ? this.Type.GenericTypeArguments[0] : this.Type;
                }
            }

            public Table ReferenceTable { get; set; }
            public Column NavigationProperty { get; set; }
            public string DisplayName
            {
                get
                {
                    var d = Name.Any(char.IsUpper) ? Regex.Replace(Name, "([a-z])([A-Z])", "$1 $2") : Name;

                    if (d.EndsWith(" ID"))
                        d = d.Substring(0, d.Length - 3);

                    return d;
                }
            }

            public bool IsSimpleType(Type type = null)
            {
                if(type == null)
                    type = this.Type;

                return
                    type.IsPrimitive ||
                    new Type[] {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
                    }.Contains(type) ||
                    type.IsEnum ||
                    Convert.GetTypeCode(type) != TypeCode.Object ||
                    (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                    ;               
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
