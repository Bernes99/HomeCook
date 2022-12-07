using FluentValidation;
using HomeCook.Data.Extensions;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO.Recipe
{
    public class AddRecipeProductDto : IValidatableObject
    {
        public string ProductId { get; set; }
        public float Amount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Rules<AddRecipeProductDto>(v =>
            {
                v.RuleFor(x => x.ProductId).NotEmpty();
                v.RuleFor(x => x.Amount).NotEmpty().GreaterThan(0f);
            })
            .Validate(this)
            .Result();
        }
    }
}
