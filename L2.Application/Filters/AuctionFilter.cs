using AutoFilterer.Attributes;
using AutoFilterer.Enums;
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
