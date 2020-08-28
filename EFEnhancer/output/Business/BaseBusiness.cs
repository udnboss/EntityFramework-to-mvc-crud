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

    public enum TriggerType { Create, Update, Delete }
    public enum TriggerTiming { Before, After, InsteadOf }
    public class Trigger<T>
    {
        public int Order { get; set; }
        public TriggerType Type { get; set; }
        public TriggerTiming Timing { get; set; }
        public Func<T, BusinessResult<T>> Function { get; set; }
    }

    public class BaseBusiness<T> : IBusiness<T>
    {
        protected string user;
        protected DbContext db;

        public List<Trigger<T>> Triggers { get; set; }

        public BaseBusiness()
        {
            Triggers = new List<Trigger<T>>();
        }

        public BaseBusiness(DbContext db, string user)
        {
            this.db = db;
            this.user = user;
            Triggers = new List<Trigger<T>>();
        }

        public void SetDB(DbContext db)
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

            if (m == null)
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

        public virtual BusinessResult<T> CanEdit(object id)
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

        public virtual BusinessResult<T> CanNew(T f)
        {
            var o = Operation.Insert;
            if (CheckAuthorization(f, o, user))
            {
                return new BusinessResult<T> { Status = State.Success, Data = f, RecordsAffected = 0 };
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> Create(T m)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var result = CanNew(m);
                if (result.Status == State.Success)
                {
                    //this operation
                    Action<T> create = (model) => { db.Set(typeof(T)).Add(model); };
                    result = PerformAll(m, Operation.Insert, create);
                }

                return result;
            }
        }

        public virtual BusinessResult<T> Update(T m)
        {
            var o = Operation.Update;
            if (CheckAuthorization(m, o, user))
            {
                //this operation
                Action<T> create = (model) => { db.Entry(model).State = EntityState.Modified; };
                return PerformAll(m, o, create);
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> CanDelete(T m)
        {
            var o = Operation.Delete;
            if (CheckAuthorization(m, o, user))
            {
                return new BusinessResult<T> { Status = State.Success, Data = m, RecordsAffected = 0 };
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> Delete(T m)
        {
            var o = Operation.Delete;
            if (CheckAuthorization(m, o, user))
            {
                Action<T> action = (model) => {
                    //db.Set(typeof(T)).Remove(m);
                    db.Entry(model).State = EntityState.Deleted;
                };

                return PerformAll(m, o, action); ;
            }

            return AccessDenied<T>(o);
        }

        public virtual BusinessResult<T> PerformAll(T m, Operation o, Action<T> action)
        {
            if (db.Database.CurrentTransaction != null)
            {
                db.Database.CurrentTransaction.Dispose();
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                var result = new BusinessResult<T>();

                var results = new List<BusinessResult<T>>();
                //before operations
                var beforeTriggers = Triggers.Where(t => t.Type == TriggerType.Create && t.Timing == TriggerTiming.Before);
                foreach (var t in beforeTriggers)
                {
                    var tResult = t.Function.Invoke(m);
                    results.Add(tResult);
                }

                //this operation
                try
                {
                    action.Invoke(m);
                    results.Add(new BusinessResult<T> { Status = State.Success, Data = m });
                }
                catch (Exception ex)
                {
                    results.Add(new BusinessResult<T> { Status = State.Error, Data = m, Message = ex.Message });
                }

                //after operations
                var afterTriggers = Triggers.Where(t => t.Type == TriggerType.Create && t.Timing == TriggerTiming.After);
                foreach (var t in afterTriggers)
                {
                    var tResult = t.Function.Invoke(m);
                    results.Add(tResult);
                }

                var failedResults = results.Where(x => x.Status == State.Error || x.Status == State.AccessDenied).ToList();

                try
                {
                    //try to save
                    try
                    {
                        var affected = db.SaveChanges();
                        var status = affected > 0 ? State.Success : State.NoRecordsAffected;
                        result = new BusinessResult<T> { Status = status, RecordsAffected = affected, Message = "" };
                    }
                    catch (Exception e)
                    {
                        result = new BusinessResult<T> { Status = State.Error, RecordsAffected = 0, Message = e.Message + (e.InnerException != null ? "; " + e.InnerException.Message : "") };
                    }

                    if (failedResults.Count == 0)
                    {
                        //commit if everything is ok
                        transaction.Commit();
                        Log(m, o);
                        result = new BusinessResult<T> { Status = State.Success, Data = m };
                    }
                    else
                    {
                        //else rollback
                        transaction.Rollback();
                        result = new BusinessResult<T>
                        {
                            Status = State.Error,
                            Data = m,
                            Message = string.Join("\r\n", failedResults.Select(x => x.Message))
                        };
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result = new BusinessResult<T>
                    {
                        Status = State.Error,
                        Data = m,
                        Message = ex.Message
                    };
                }


                return result;
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

            if (filter != null)
            {
                //try reflection
                var props = typeof(T).GetProperties();
                foreach (var p in props.Where(p => p.PropertyType.IsPrimitive))
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