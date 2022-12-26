using FluentValidation;
using HomeCook.Data.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO.Product
{
    public class AddUserProductDto : IValidatableObject
    {
        public string ProductId { get; set; }
        [JsonIgnore]
        public long ProductInternalId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public float? Amount { get; set; }
        public bool IsOnShoppingList { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Rules<AddUserProductDto>(v =>
            {
                //v.RuleFor(x => x.ProductInternalId).Null();
            })
            .Validate(this)
            .Result();
        }
    }
}
