using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TimelyTastes.Validation.VerifyImageAttribute
{
    public class VerifyImageAttribute : Attribute, IModelValidator
    {
        private readonly string _otherProperty;

        public VerifyImageAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        public bool IsRequired = true;
        public string ErrorMessage { get; set; } = "Something went wrong loading your image";



        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {

            var existingImage = context.Model as byte[];



            PropertyInfo? otherProperty = context.Container?.GetType().GetProperty(_otherProperty);

            if (otherProperty == null)
            {
                return new[]
                {
                    new ModelValidationResult("", $"Unknown property: {_otherProperty}")
                };
            }


            var file = otherProperty.GetValue(context.Container) as IFormFile;

            if (file != null && file.Length > 0)
            {
                return Enumerable.Empty<ModelValidationResult>();
            }

            if (existingImage != null && existingImage.Length > 0)
            {
                return Enumerable.Empty<ModelValidationResult>();
            }

            return new[]
            {
                new ModelValidationResult(_otherProperty, ErrorMessage)
            };

        }
    }
}