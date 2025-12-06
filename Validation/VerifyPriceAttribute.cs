using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TimelyTastes.Validation.VerifyPriceAttribute
{
    public class VerifyPriceAttribute : Attribute, IModelValidator
    {
        private readonly string _otherProperty;

        public VerifyPriceAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        public bool IsRequired = true;
        public string ErrorMessage { get; set; } = "Please ensure the price is correct";


        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {

            var thisValue = context.Model as decimal?;

            if (!thisValue.HasValue)
                return Enumerable.Empty<ModelValidationResult>();

            PropertyInfo? otherProperty = context.Container?.GetType().GetProperty(_otherProperty);

            if (otherProperty == null)
            {
                return new[]
                {
                    new ModelValidationResult("", $"Unknown property: {_otherProperty}")
                };
            }


            var otherValue = otherProperty.GetValue(context.Container) as decimal?;

            if (!otherValue.HasValue)
                return Enumerable.Empty<ModelValidationResult>();

            if (thisValue >= otherValue)
            {
                return new[]
                {
                 new ModelValidationResult("","The discount price should be less than the original price")
                };
            }

            return Enumerable.Empty<ModelValidationResult>();

        }
    }
}