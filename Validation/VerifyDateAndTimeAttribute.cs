using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TimelyTastes.Validation.VerifyDateAndTimeAttribute
{
    public class VerifyDateAndTimeAttribute : Attribute, IModelValidator
    {
        public bool IsRequired = true;
        public string ErrorMessage { get; set; } = "Please ensure the date and time are correct";

        private readonly string _otherProperty;

        public VerifyDateAndTimeAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {

            var thisValue = context.Model as DateTime?;

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

            var otherValue = otherProperty.GetValue(context.Container) as DateTime?;

            if (!otherValue.HasValue)
                return Enumerable.Empty<ModelValidationResult>();


            if (thisValue.Value > otherValue.Value)
            {
                return new[]
                {
                    new ModelValidationResult("", "The end time cannot be before the start time")
                };
            }

            var minutes = (otherValue.Value - thisValue.Value).TotalMinutes;

            if (minutes < 30)
            {
                return new[]
                {
                   new ModelValidationResult("", "The pick up window must be at least 30 minutes")
               };
            }




            return Enumerable.Empty<ModelValidationResult>();
        }
    }
}