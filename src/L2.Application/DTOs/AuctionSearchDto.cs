namespace L2.Application.DTOs;

public record AuctionSearchDto {
  public Guid AuctionId { get; init; }
  public Guid CatalogItemId { get; init; }
  public required string Name { get; init; }
  public decimal CurrentPrice { get; init; }
  public required string AuctionStatus { get; init; }
  public Guid SessionId { get; init; }
  public DateTime StartTime { get; init; }
  public DateTime EndTime { get; init; }
  public string? MainImageUrl { get; init; }
}
