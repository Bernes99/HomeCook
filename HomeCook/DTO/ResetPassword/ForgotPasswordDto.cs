using FluentValidation;
using HomeCook.Data.Extensions;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO.ResetPassword
{
    public class ForgotPasswordDto : IValidatableObject
    {
        public string Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return this.Rules<ForgotPasswordDto>(v =>
            {
                v.RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
                v.RuleFor(x => x.Email).EmailAddress().WithMessage("Incorrect email address");
            })
            .Validate(this)
            .Result();
        }
    }
}
