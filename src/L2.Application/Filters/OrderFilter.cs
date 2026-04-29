using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using FluentValidation;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;

namespace L2.Application.Filters;

public class OrderFilter : BaseFilter {
  public Guid? BidderId { get; set; }
  public Guid? AuctionId { get; set; }
  public Guid? CatalogId { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? BidderName { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? BidderEmail { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  public string? CatalogName { get; set; }

  public OrderStatus? Status { get; set; }

  [CompareTo(nameof(Order.Price))]
  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public decimal? MinPrice { get; set; }

  [CompareTo(nameof(Order.Price))]
  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public decimal? MaxPrice { get; set; }

  [StringFilterOptions(StringFilterOption.Contains)]
  [CompareTo("ShippingAddress")]
  public string? City { get; set; }
}

public sealed class OrderFilterValidator : BaseFilterValidator<OrderFilter> {
  public OrderFilterValidator() {
    RuleFor(x => x.BidderName)
      .MaximumLength(200)
      .WithMessage("Tên người đấu giá không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.BidderName));

    RuleFor(x => x.BidderEmail)
      .EmailAddress()
      .WithMessage("Email người đấu giá không hợp lệ.")
      .When(x => !string.IsNullOrWhiteSpace(x.BidderEmail));

    RuleFor(x => x.CatalogName)
      .MaximumLength(200)
      .WithMessage("Tên sản phẩm không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.CatalogName));

    RuleFor(x => x.City)
      .MaximumLength(200)
      .WithMessage("Tên thành phố không được vượt quá 200 ký tự.")
      .When(x => !string.IsNullOrWhiteSpace(x.City));

    RuleFor(x => x)
      .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
      .WithMessage("Giá tối thiểu không được lớn hơn giá tối đa.");

    RuleFor(x => x.MinPrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá tối thiểu không được nhỏ hơn 0.")
      .When(x => x.MinPrice.HasValue);

    RuleFor(x => x.MaxPrice)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Giá tối đa không được nhỏ hơn 0.")
      .When(x => x.MaxPrice.HasValue);
  }
}
