namespace L2.Application.Models;

public record TokenModel {
  public required string Token { get; init; }
  public DateTime ExpiredAt { get; init; }
}

public record AuthTokens(TokenModel Access, TokenModel Refresh);
