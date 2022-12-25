using FluentValidation;
using HomeCook.Data.Enums;
using HomeCook.Data.Extensions;
using HomeCook.DTO.Product;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO.Recipe
{
    public class AddRecipeDto : IValidatableObject
    {
        public string Title { get; set; }
        public string Introdution { get; set; }
        public string Text { get; set; }
        public string Portion { get; set; }
        public string AuthorId { get; set; }
        public string PreparingTime { get; set; }
        public float Difficulty { get; set; }
        public List<AddRecipeProductDto> Products { get; set; }
        public List<string> CategoriesIds { get; set; }
        public List<string> Tags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Rules<AddRecipeDto>(v =>
            {
                v.RuleFor(x => x.Title).NotEmpty();
                v.RuleFor(x => x.Introdution).NotEmpty();
                v.RuleFor(x => x.Text).NotEmpty();
                v.RuleFor(x => x.Portion).NotEmpty();
                v.RuleFor(x => x.AuthorId).NotEmpty();
                v.RuleFor(x => x.PreparingTime).NotEmpty();
                v.RuleFor(x => x.Difficulty).NotEmpty().LessThanOrEqualTo(10f).GreaterThanOrEqualTo(0f);
                v.RuleFor(x => x.Products).NotEmpty();
                v.RuleFor(x => x.CategoriesIds).NotEmpty();
            })
            .Validate(this)
            .Result();
        }
    }

    public class FormDataJsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var stringValue = valueProviderResult.FirstValue;
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject(stringValue, bindingContext.ModelType);
                if (result != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}
