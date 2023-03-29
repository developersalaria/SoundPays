using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace SoundpaysAdd.Core.Validations
{
    public class ModelClientValidationMinMaxRule : ModelClientValidationRule
    {
        public ModelClientValidationMinMaxRule(string errorMessage, string property, string comparewith)
        {
            ErrorMessage = errorMessage;
            ValidationType = "minmax";
            ValidationParameters.Add("property", property);
            ValidationParameters.Add("comparewith", comparewith);
        }
    }
}
