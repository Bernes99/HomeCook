using HomeCook.Data.Enums;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace HomeCook.DTO.Pagination
{
    public class ProductPaginationQuery : IValidatableObject
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            int[] allowedPagedSizes = new[] { 5, 10, 15 };
            string[] allowedSortByCollumnNames = {
                nameof(HomeCook.Data.Models.Product.Name).ToUpper(),
                nameof(HomeCook.Data.Models.Product.Calories).ToUpper(),
                nameof(HomeCook.Data.Models.Product.UnitType).ToUpper(),
            };

            return this.Rules<ProductPaginationQuery>(v =>
            {
                v.RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
                v.RuleFor(x => x.PageSize).Custom((value, context) =>
                {
                    if (!allowedPagedSizes.Contains(value))
                    {
                        context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPagedSizes)}]");
                    }
                });
                v.RuleFor(x => x.SortBy).Must(v => string.IsNullOrEmpty(v) || allowedSortByCollumnNames.Contains(v.ToUpper()))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByCollumnNames)}]");
            })
            .Validate(this)
            .Result();
        }
    }
}
