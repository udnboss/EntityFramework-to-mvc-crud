using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkflowWeb.Business
{
    public class BusinessResult<T>
    {
        public State Status { get; set; }
        public string Message { get; set; }
        public int RecordsAffected { get; set; }
        public T Data { get; set; }
    }
}