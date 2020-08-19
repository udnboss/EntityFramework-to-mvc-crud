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
        public List<Column> Columns { get; set; }
        public Dictionary<Column, Table> ForeignKeys { get; set; }

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
                    return Name.Any(char.IsUpper) ? Regex.Replace(Name, "([a-z])([A-Z])", "$1 $2") : Name;
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
        }
    }
}
