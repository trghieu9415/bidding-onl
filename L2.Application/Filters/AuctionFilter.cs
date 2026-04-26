using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using FluentValidation;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;

namespace L2.Application.Filters;

public class AuctionFilter : BaseFilter {
  public Guid? SessionId { get; set; }
  public Guid? CatalogItemId { get; set; }
  public AuctionStatus? Status { get; set; }
  public Guid? OwnerId { get; set; }

  [CompareTo(nameof(Auction.CurrentPrice))]
  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public decimal? MinPrice { get; set; }

  [CompareTo(nameof(Auction.CurrentPrice))]
  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public decimal? MaxPrice { get; set; }

  [CompareTo(nameof(Auction.WinningAt))]
  [OperatorComparison(OperatorType.GreaterThanOrEqual)]
  public DateTime? MinWinningAt { get; set; }

  [CompareTo(nameof(Auction.WinningAt))]
  [OperatorComparison(OperatorType.LessThanOrEqual)]
  public DateTime? MaxWinningAt { get; set; }
}

public sealed class AuctionFilterValidator : BaseFilterValidator<AuctionFilter> {
  public AuctionFilterValidator() {
    RuleFor(x => x)
      .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
      .WithMessage("Giá tối thiểu không được lớn hơn giá tối đa.");

    RuleFor(x => x)
      .Must(x => !x.MinWinningAt.HasValue || !x.MaxWinningAt.HasValue || x.MinWinningAt <= x.MaxWinningAt)
      .WithMessage("Thời gian thắng tối thiểu không được lớn hơn thời gian thắng tối đa.");

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
