using FluentValidation;
using HomeCook.Data.Extensions;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO.Product
{
    public class CategoryDto : IValidatableObject
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Rules<CategoryDto>(v =>
            {
                v.RuleFor(x => x.Name).NotEmpty();
            })
            .Validate(this)
            .Result();
        }
    }
}
