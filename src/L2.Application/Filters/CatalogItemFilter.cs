using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using FluentValidation;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;

// Bro điều chỉnh namespace entity cho đúng nhé

namespace L2.Application.Filters;

public class CatalogItemFilter : BaseFilter {
  public Guid? OwnerId { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Name { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? Description { get; set; }

  public ItemStatus? Status { get; set; }
  public ItemCondition? Condition { get; set; }

  [CompareTo(nameof(CatalogItem.StartingPrice))]
  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public decimal? MinStartingPrice { get; set; }

  [CompareTo(nameof(CatalogItem.StartingPrice))]
  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public decimal? MaxStartingPrice { get; set; }

  public Guid? CategoryIds { get; set; }
}

public sealed class CatalogItemFilterValidator : BaseFilterValidator<CatalogItemFilter> {
  public CatalogItemFilterValidator() {
    RuleFor(x => x.Name)
      .MaximumLength(200)
      .WithMessage("Tên sản phẩm không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.Name));

    RuleFor(x => x.Description)
      .MaximumLength(1000)
      .WithMessage("Mô tả sản phẩm không được vượt quá 1000 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.Description));

    RuleFor(x => x)
      .Must(x => !x.MinStartingPrice.HasValue || !x.MaxStartingPrice.HasValue ||
                 x.MinStartingPrice <= x.MaxStartingPrice)
      .WithMessage("Giá khởi điểm tối thiểu không được lớn hơn giá khởi điểm tối đa.");

    RuleFor(x => x.MinStartingPrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá khởi điểm tối thiểu không được nhỏ hơn 0.")
      .When(x => x.MinStartingPrice.HasValue);

    RuleFor(x => x.MaxStartingPrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá khởi điểm tối đa không được nhỏ hơn 0.")
      .When(x => x.MaxStartingPrice.HasValue);
  }
}
