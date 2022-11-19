using FluentValidation;
using HomeCook.Data.Extensions;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO
{
    public class UserDto : IValidatableObject
    {
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? Surname { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            return this.Rules<UserDto>(v =>
            {
                v.RuleFor(x => x.Id).NotEmpty().WithMessage("[[[Id is required]]]");
            })
            .Validate(this)
            .Result();
        }
    }
}
