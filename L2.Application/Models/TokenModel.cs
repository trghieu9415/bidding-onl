namespace L2.Application.Models;

public record TokenModel {
  public string Token { get; init; } = null!;
  public DateTime ExpiredAt { get; init; }
}
