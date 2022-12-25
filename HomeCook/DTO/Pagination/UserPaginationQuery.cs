using FluentValidation;
using HomeCook.Data.Enums;
using HomeCook.Data.Extensions;
using HomeCook.DTO;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace HomeCook.Data.Models.CustomModels
{
    public class UserPaginationQuery : IValidatableObject
    {
        public string? SearchPhrase { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            int[] allowedPagedSizes = new[] { 5, 10, 15 };
            string[] allowedSortByCollumnNames = { 
                nameof(AppUser.firstName).ToUpper(),
                nameof(AppUser.surname).ToUpper(),
                nameof(AppUser.Email).ToUpper(),
                nameof(AppUser.LastLogin).ToUpper()
            };

            return this.Rules<UserPaginationQuery>(v =>
            {
                v.RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
                v.RuleFor(x => x.PageSize).Custom((value,context) =>
                {
                    if (!allowedPagedSizes.Contains(value))
                    {
                        context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPagedSizes)}]");
                    }
                });
                v.RuleFor(x => x.SortBy).Must(v => string.IsNullOrEmpty(v) || allowedSortByCollumnNames.Contains(v.ToUpper()))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",",allowedSortByCollumnNames)}]");
            })
            .Validate(this)
            .Result();
        }
    }
}
