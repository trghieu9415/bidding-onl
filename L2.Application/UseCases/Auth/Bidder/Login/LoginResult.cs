using L2.Application.Models;

namespace L2.Application.UseCases.Auth.Bidder.Login;

public record LoginResult(AuthTokens Tokens);