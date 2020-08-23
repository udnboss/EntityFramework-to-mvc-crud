using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkflowWeb.ViewModels
{
    public class BaseViewModel<T> : IViewModel<T>
    {
        public BaseViewModel()
        {
            
        }

        public BaseViewModel(T m, bool convertSubs = false)
        {
            
        }

        public virtual BaseViewModel<T> FromModel<M>(M m, bool convertSubs)
        {
            throw new NotImplementedException();
        }

        public virtual T ToModel(bool convertSubs)
        {
            throw new NotImplementedException();
        }

        public virtual string ToRouteFilter()
        {
            var route_filter = JsonConvert.SerializeObject(this);
            var bytes = System.Text.Encoding.ASCII.GetBytes(route_filter);
            route_filter = Convert.ToBase64String(bytes);
            return route_filter;
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.GetType().GetProperty("ID").GetValue(this) == null)
            {
                yield return new ValidationResult("Error", new string[] { "Error Detail" });
            }
        }

        protected virtual bool IsUniqueList(List<object> items)
        {
            //unique check for TIMS_ProjectPackage related properties            
            var nonNullItems = items.Where(x => x != null);
            var uniqueItemsCount = nonNullItems.Distinct().Count();
            if (nonNullItems.Count() != uniqueItemsCount)
            {
                return false;
            }

            return true;
        }
    }
}