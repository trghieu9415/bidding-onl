using AutoFilterer.Attributes;
using AutoFilterer.Enums;
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
