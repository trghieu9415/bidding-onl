using MongoDB.Bson.Serialization.Attributes;

namespace L3.Infrastructure.Persistence.Mongo;

public record AuctionSearchDocument(
  [property: BsonId] Guid AuctionId,
  Guid CatalogItemId,
  string Name,
  List<Guid> CategoryIds,
  decimal CurrentPrice,
  string AuctionStatus,
  Guid SessionId,
  DateTime StartTime,
  DateTime EndTime,
  string? MainImageUrl
);
