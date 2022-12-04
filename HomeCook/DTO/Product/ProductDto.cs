using HomeCook.Data.Extensions;
using HomeCook.Data.Models.CustomModels;
using HomeCook.Data.Models;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using HomeCook.Data.Enums;

namespace HomeCook.DTO.Product
{
    public class ProductDto : IValidatableObject
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public long? Calories { get; set; }
        public string CategoryId { get; set; }
        public UnitType UnitType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Rules<ProductDto>(v =>
            {
                v.RuleFor(x => x.UnitType).IsInEnum();
                v.RuleFor(x => x.Name).NotEmpty();
                v.RuleFor(x => x.CategoryId).NotEmpty();
            })
            .Validate(this)
            .Result();
        }
    }

}
