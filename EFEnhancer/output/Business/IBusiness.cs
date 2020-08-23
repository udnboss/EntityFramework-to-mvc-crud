using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WorkflowWeb.Models;

namespace WorkflowWeb.Business
{
    public interface IBusiness<T>
    {
        bool CheckAuthorization(object m, Operation o, string user);

        IQueryable<T> GetIQueryable();

        IQueryable<T> GetIQueryable(Expression<Func<T, bool>> where);

        BusinessResult<T> Get(object id);

        BusinessResult<T> New(T m);
        BusinessResult<T> Edit(object id);
        BusinessResult<T> Insert(T m);

        BusinessResult<T> Update(T m);

        BusinessResult<T> Delete(T m);

        BusinessResult<T> Commit(T t, Operation o);

        BusinessResult<T> AccessDenied<T>(Operation o);
        BusinessResult<List<T>> GetList(T t);
    }
}