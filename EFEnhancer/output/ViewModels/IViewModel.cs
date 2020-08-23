using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WorkflowWeb.ViewModels
{
    public interface IViewModel<T>
    {
        BaseViewModel<T> FromModel<M>(M m, bool convertSubs);
        T ToModel(bool convertSubs);
        string ToRouteFilter();
        IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}