using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SoundpaysAdd.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class MinMaxCheckAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _comparisonProperty;
        private readonly string _propType;
        private readonly string _compareType;
        private readonly bool _allowPastDate;
        /// <summary>
        /// Compare min max value of two properties
        /// </summary>
        /// <param name="comparisonProperty"> Porprty name that will be compared</param>
        /// <param name="propertyType"> data type pf properties for now just int or datetime</param>
        /// <param name="compareType"> less or greater</param>
        public MinMaxCheckAttribute(string comparisonProperty, string propertyType, string compareType = "less", bool allowPastDate = false)
        {
            _comparisonProperty = comparisonProperty;
            _propType = propertyType;
            _compareType = compareType;
            _allowPastDate = allowPastDate;
        }

        //serever side validation
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                throw new ArgumentException($"Property with name {_comparisonProperty} not found");
            if (_propType == "int")
            {
                var currentValue = (int)value;
                var comparisonValue = (int)property.GetValue(validationContext.ObjectInstance);


                if (_compareType == "less")
                {
                    if (currentValue > comparisonValue)
                        return new ValidationResult(ErrorMessage);

                    return ValidationResult.Success;
                }
                else
                {
                    if (currentValue < comparisonValue)
                        return new ValidationResult(ErrorMessage);

                    return ValidationResult.Success;
                }
            }

            else if (_propType == "date")
            {
                var currentValue = (DateTime)value;
                var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);
                if (!_allowPastDate)
                {
                    var idProperty = validationContext.ObjectType.GetProperty("Id");
                    var idPropertyValue = (int)idProperty.GetValue(validationContext.ObjectInstance);
                    if (idPropertyValue == 0 && currentValue.Date < DateTime.UtcNow.Date)
                    {
                        return new ValidationResult("Past date are not allowed");
                    }
                    return ValidationResult.Success;
                }
                else if (_compareType == "less")
                {
                    if (currentValue > comparisonValue)
                        return new ValidationResult(ErrorMessage);

                    return ValidationResult.Success;
                }
                else
                {
                    if (currentValue < comparisonValue)
                        return new ValidationResult(ErrorMessage);

                    return ValidationResult.Success;
                }

            }
            else if (_propType == "decimal")
            {
                var currentValue = (decimal)value;
                var comparisonValue = (decimal)property.GetValue(validationContext.ObjectInstance);


                if (_compareType == "less")
                {
                    if (currentValue > comparisonValue)
                        return new ValidationResult(ErrorMessage);

                    return ValidationResult.Success;
                }
                else
                {
                    if (currentValue < comparisonValue)
                        return new ValidationResult(ErrorMessage);

                    return ValidationResult.Success;
                }
            }
            return ValidationResult.Success;
        }

        #region client side validation
        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            var errorMessage = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            MergeAttribute(context.Attributes, "data-val-minmax", errorMessage);
            MergeAttribute(context.Attributes, "data-val-comparewith", _comparisonProperty);
            MergeAttribute(context.Attributes, "data-val-compare", _compareType);
        }

        private bool MergeAttribute(
            IDictionary<string, string> attributes,
            string key,
            string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }
        #endregion

    }
}
