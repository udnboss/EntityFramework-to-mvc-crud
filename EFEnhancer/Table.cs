﻿using ActiproSoftware.SyntaxEditor.Addons.DotNet.Ast;
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
        public Dictionary<Column, Table> ForeignKeys { get; set; }

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

            public Table ReferenceTable { get; set; }

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
