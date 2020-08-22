using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using _namespace_.Models;

namespace _namespace_.Business
{
    public class _table_Business : BaseBusiness<_table_>
    {
        public _table_Business() { }
        public _table_Business(DbContext db, string user) : base(db, user) { }
        public override BusinessResult<List<_table_>> GetList(_table_ filter)
        {
            var o = Operation.Select;

            if (CheckAuthorization(filter, o, user))
            {
                try
                {
                    var data = GetIQueryable(filter).ToList();
                    return new BusinessResult<List<_table_>> { Status = State.Success, RecordsAffected = data.Count, Data = data };
                }

                catch (Exception e)
                {
                    return new BusinessResult<List<_table_>> { Status = State.Error, RecordsAffected = 0, Message = e.Message + (e.InnerException != null ? "; " + e.InnerException.Message : "") };
                }
            }

            return AccessDenied<List<_table_>>(o);
        }

        private IQueryable<_table_> GetIQueryable(_table_ filter)
        {
            var data = GetIQueryable();

            if (filter != null)
            {
                _filterconditions_
            }

            return data;
        }
    }

}