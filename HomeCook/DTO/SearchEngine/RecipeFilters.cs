using FluentValidation;
using HomeCook.Data.Extensions;
using HomeCook.DTO.Recipe;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO.SearchEngine
{
    public class RecipeFilters : IValidatableObject
    {
        public string[]? CategoryNames { get; set; }
        public string[]? Products { get; set; }
        public string? DateCreatedUtc { get; set; }
        public float Difficulty { get; set; }
        public float Rating { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Rules<RecipeFilters>(v =>
            {
                v.RuleFor(x => x.Difficulty).GreaterThanOrEqualTo(0f);
                v.RuleFor(x => x.Rating).GreaterThanOrEqualTo(0f);
            })
            .Validate(this)
            .Result();
        }
    }
}
