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
    public partial class _table_ViewModel : BaseViewModel<_table_>, IValidatableObject
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

        public override _table_ ToModel(bool convertSubs = false)
        {
            var m = new _table_();

            _tomodelproperties_

            return m;
        }

        public override BaseViewModel<_table_> FromModel<M>(M mo, bool convertSubs)
        {
            var m = mo as _table_;
            if (m != null)
            {
                _copyproperties_
            }

            return this;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            _uniquevalidations_

            return errors.AsEnumerable();
        }
    }

}