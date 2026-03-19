using L1.Core.Domain.Transaction.Enums;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record OrderDto : IdDto {
  public Guid BidderId { get; init; }
  public string BidderName { get; init; } = null!;
  public Guid AuctionId { get; init; }
  public Guid CatalogId { get; init; }
  public string CatalogName { get; init; } = null!;
  public string CatalogImage { get; init; } = null!;
  public OrderStatus Status { get; init; }
  public decimal Price { get; init; }
}
