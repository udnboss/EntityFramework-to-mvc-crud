using ActiproSoftware.SyntaxEditor.Addons.DotNet.Ast;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFEnhancer
{
    class EfParser
    {
        public DbContext DB;
        public EfParser(DbContext db)
        {
            this.DB = db;
        }
        public List<Table> GetTables()
        {
            var tables = new List<Table>();
            var props = DB.GetType().GetProperties();
            foreach (var p in props)
            {
                if (p.PropertyType.Name.StartsWith("DbSet")) //tables
                {
                    var type = p.PropertyType.GenericTypeArguments[0];
                    var table = new Table { Type = type, Name = type.Name };
                    tables.Add(table);

                    var subprops = type.GetProperties();
                    foreach (var sp in subprops)
                    {
                        if (!sp.PropertyType.Name.StartsWith("System.Collections.Generic.ICollection")) //skip collections
                        {
                            var column = new Table.Column { Name = sp.Name, Type = sp.PropertyType };
                            table.Columns.Add(column);
                        }
                    }
                }
            }

            foreach(var t in tables)
            {
                var fks = GetForeignKeyProperties(t.Type);
                foreach (var fk in fks)
                {
                    var col = t.Columns.First(x => x.Name == fk.Key);
                    var refTable = tables.First(x => x.Name == fk.Value.Item1);

                    col.ReferenceTable = refTable;
                    col.NavigationProperty = t.Columns.First(x => x.Name == fk.Value.Item2);
                    t.ForeignKeys.Add(col, refTable);
                }
            }
            

            return tables;
        }
        private Dictionary<string, Tuple<string, string>> GetForeignKeyProperties(Type DBType)
        {
            EntityType table = GetTableEntityType(DBType);
            Dictionary<string, Tuple<string, string>> foreignKeys = new Dictionary<string, Tuple<string, string>>();

            foreach (NavigationProperty np in table.NavigationProperties)
            {
                var association = (np.ToEndMember.DeclaringType as AssociationType);
                var constraint = association.ReferentialConstraints.FirstOrDefault();

                //((table.NavigationProperties[2].ToEndMember.DeclaringType) as AssociationType).ReferentialConstraints[0].ToProperties[0].Name

                //if (constraint != null && constraint.ToRole.GetEntityType() == table)
                //    foreignKeys.Add(np.Name, constraint.ToProperties.First().Name);

                if (constraint != null && constraint.ToRole.GetEntityType() == table)
                {
                    var key = constraint.ToProperties[0].Name;
                    if(!foreignKeys.ContainsKey(key))
                    {
                        foreignKeys.Add(constraint.ToProperties[0].Name,new Tuple<string, string>(constraint.FromRole.Name, np.Name));
                    }
                }
                    
            }

            return foreignKeys;
        }
        private EntityType GetTableEntityType(Type DBType)
        {
            ObjectContext objContext = ((IObjectContextAdapter)DB).ObjectContext;
            MetadataWorkspace workspace = objContext.MetadataWorkspace;
            EntityType table = workspace.GetEdmSpaceType((StructuralType)workspace.GetItem<EntityType>(DBType.FullName, DataSpace.OSpace)) as EntityType;
            return table;
        }
    }
}
