using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using _namespace_.Models;

namespace _namespace_.ViewModels
{
    public class _table_ViewModel : IValidatableObject
    {
        _properties_

        public _table_ViewModel()
        {

        }

        public _table_ViewModel(_table_ m, bool convertSubs = false)
        {
            if (m != null)
            {
                _copyproperties_
            }
        }

        public _table_ ToModel(bool convertSubs = false)
        {
            var m = new _table_();

            _tomodelproperties_

            return m;
        }

        public string ToRouteFilter()
        {
            var route_filter = JsonConvert.SerializeObject(new { _primitivecols_ });
            var bytes = System.Text.Encoding.ASCII.GetBytes(route_filter);
            route_filter = Convert.ToBase64String(bytes);
            return route_filter;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ID == null)
            {
                yield return new ValidationResult("Error", new string[] { "Error Detail" });
            }
        }
    }

}