namespace L3.Infrastructure.Persistence.Outbox;

public class OutboxMessage {
  public Guid Id { get; set; }
  public string Type { get; set; } = null!;
  public string Content { get; set; } = null!;
  public DateTime OccurredOnUtc { get; set; }
  public DateTime? ProcessedOnUtc { get; set; }
  public string? Error { get; set; }
}
