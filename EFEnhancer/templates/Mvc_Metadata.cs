using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace _namespace_.Models
{
    public class _table_Meta
    {
        //[Required]
        //[DataType(DataType.Text)]
        //[MinLength(5)]
        //[MaxLength(5)]
        //[JsonIgnore]
        //[Range]
        //[Remote()]
        //[UIHint("CustomEditorName")]
        [DisplayName("_displayname_")]
        
        public object _column_ { get; set; }

    }

    [MetadataType(typeof(_table_Meta))]
    public partial class _table_ : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ID == null)
            {
                yield return new ValidationResult("Error", new string[] { "Error Detail" });
            }
        }
    }

}