using FluentValidation;
using HomeCook.Data.Extensions;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.DTO
{
    public class RegisterDto : IValidatableObject
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Email { get; set; }
        public bool RemenberLogin { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            return this.Rules<RegisterDto>(v =>
            {
                v.RuleFor(x => x.Login).NotEmpty().WithMessage("[[[Login is required]]]");
                v.RuleFor(x => x.FirstName).MaximumLength(150).WithMessage("[[[FirstName have to have max 150 characters]]]");
                v.RuleFor(x => x.Surname).MaximumLength(150).WithMessage("[[[LastName have to have max 150 characters]]]");
                v.RuleFor(x => x.Password).NotEmpty().Equal(p => p.PasswordConfirm).WithMessage("[[[Passwords must be the same]]]");
                v.RuleFor(x => x.Password).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})").WithMessage("[[[Incorrect password]]]");
                v.RuleFor(x => x.Email).NotEmpty().WithMessage("[[[Email is required]]]");
                v.RuleFor(x => x.Email).EmailAddress().WithMessage("[[[Incorrect email address]]]");



            })
            .Validate(this)
            .Result();
        }

    }
}
