using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WorkflowWeb.Models;

namespace WorkflowWeb.Business
{
    public enum Operation
    {
        Select, Insert, Update, Delete
    }

    public enum State
    {
        Success,
        AccessDenied,
        NoRecordsAffected,
        NoRecordFound,
        Error
    }

    public class BaseBusiness<T> : IBusiness<T>
    {
        protected string user;
        protected IMSEntities db;

        public BaseBusiness()
        {
            db = new IMSEntities();
        }

        public BaseBusiness(DbContext db, string user)
        {
            this.db = (IMSEntities)db;
            this.user = user;
        }

        public void SetDB(IMSEntities db)
        {
            this.db = db;
        }

        public virtual bool CheckAuthorization(object m, Operation o, string user)
        {
            return true;
        }

        public virtual IQueryable<T> GetIQueryable()
        {
            return db.Set(typeof(T)).AsQueryable().Cast<T>();
        }

        public virtual IQueryable<T> GetIQueryable(Expression<Func<T, bool>> where)
        {
            return GetIQueryable().Where(where).AsQueryable();
        }

        public virtual BusinessResult<T> Get(object id)
        {               
            var m = (T)db.Set(typeof(T)).Find(id);

            if(m == null)
            {
                return new BusinessResult<T> { Status = State.NoRecordFound, Data = m, RecordsAffected = 0 };
            }

            var o = Operation.Select;
            if (CheckAuthorization(m, o, user))
            {
                return new BusinessResult<T> { Status = State.Success, Data = m, RecordsAffected = 1 };
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> Edit(object id)
        {
            var o = Operation.Update;

            var result = Get(id);
            if (result.Status == State.Success)
            {                
                if (CheckAuthorization(result.Data, o, user))
                {
                    return result;
                }
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> New(T f)
        {
            var o = Operation.Insert;
            if (CheckAuthorization(f, o, user))
            {
                return new BusinessResult<T> { Status = State.Success, Data = f, RecordsAffected = 0 };
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> Insert(T m)
        {
            var result = New(m);
            if(result.Status == State.Success)
            {
                db.Set(typeof(T)).Add(m);
                result = Commit(m, Operation.Insert);
            }

            return result;
        }

        public virtual BusinessResult<T> Update(T m)
        {
            var o = Operation.Update;
            if (CheckAuthorization(m, o, user))
            {
                db.Entry(m).State = EntityState.Modified;
                var result = Commit(m, Operation.Update);
                return result;
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> Delete(T m)
        {
            var o = Operation.Delete;
            if (CheckAuthorization(m, o, user))
            {
                //db.Set(typeof(T)).Remove(m);
                var em = db.Entry(m);
                em.State = EntityState.Deleted;
                var result = Commit(m, Operation.Delete);
                return result;
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> Commit(T m, Operation o)
        {
            try
            {
                var affected = db.SaveChanges();
                var status = affected > 0 ? State.Success : State.NoRecordsAffected;
                if(status == State.Success)
                {
                    Log(m, o);
                }
                return new BusinessResult<T> { Status = status, RecordsAffected = affected, Message = "" };
            }
            catch (Exception e)
            {
                return new BusinessResult<T> { Status = State.Error, RecordsAffected = 0, Message = e.Message + (e.InnerException != null ? "; " + e.InnerException.Message : "") };
            }
        }

        public virtual BusinessResult<T> AccessDenied<T>(Operation o)
        {
            return new BusinessResult<T> { Status = State.AccessDenied, RecordsAffected = 0, Message = string.Format("Unauthorized to perform {0} operation", o) };
        }

        public virtual BusinessResult<List<T>> GetList(T filter)
        {
            var o = Operation.Select;

            if (CheckAuthorization(filter, o, user))
            {
                try
                {
                    var data = GetIQueryable(filter).ToList();
                    return new BusinessResult<List<T>> { Status = State.Success, RecordsAffected = data.Count, Data = data };
                }

                catch (Exception e)
                {
                    return new BusinessResult<List<T>> { Status = State.Error, RecordsAffected = 0, Message = e.Message + (e.InnerException != null ? "; " + e.InnerException.Message : "") };
                }
            }

            return AccessDenied<List<T>>(o);
        }

        private IQueryable<T> GetIQueryable(T filter)
        {
            var data = GetIQueryable();

            //if (filter != null)
            //{
            //    if (filter.ID != null && filter.ID.ToString() != "00000000-0000-0000-0000-000000000000") data = data.Where(x => x.ID == filter.ID);
            //    if (filter.Name != null && filter.Name.ToString() != "00000000-0000-0000-0000-000000000000") data = data.Where(x => x.Name == filter.Name);
            //}

            if(filter != null)
            {
                //try reflection
                var props = typeof(T).GetProperties();
                foreach(var p in props.Where(p => p.PropertyType.IsPrimitive))
                {
                    //build expression
                    ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
                    Expression whereProperty = Expression.Property(parameter, p);
                    Expression otherProperty = Expression.Constant(p.GetValue(filter), p.PropertyType);
                    Expression condition = Expression.Equal(whereProperty, otherProperty);
                    Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

                    data = data.Where(lambda);
                }
            }
            
            return data;
        }

        private void Log(T m, Operation o)
        {
            //Log to your database here, the variable user has the username.
            //if(o == Operation.Delete)
            //    Debug.Assert(true);
        }
    }
}