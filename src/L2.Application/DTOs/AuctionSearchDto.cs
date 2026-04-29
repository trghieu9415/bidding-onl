namespace L2.Application.DTOs;

public record AuctionSearchDto {
  public Guid AuctionId { get; init; }
  public Guid CatalogItemId { get; init; }
  public string Name { get; init; } = string.Empty;
  public decimal CurrentPrice { get; init; }
  public string AuctionStatus { get; init; } = string.Empty;
  public Guid SessionId { get; init; }
  public DateTime StartTime { get; init; }
  public DateTime EndTime { get; init; }
  public string? MainImageUrl { get; init; }
}
