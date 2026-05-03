using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.ValueObjects;
using L2.Application.DTOs.Base;

namespace L2.Application.DTOs;

public record OrderDto : IdDto<Order> {
  public Guid BidderId { get; init; }
  public required string BidderName { get; init; }
  public Guid AuctionId { get; init; }
  public Guid CatalogId { get; init; }
  public required string CatalogName { get; init; }
  public required string CatalogImage { get; init; }
  public OrderStatus Status { get; init; }
  public decimal Price { get; init; }
  public required Address Address { get; init; }
}
