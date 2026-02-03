using L2.Application.Models;

namespace L2.Application.UseCases.Auth.Bidder.RefreshAccess;

public record RefreshAccessResult(AuthTokens Tokens);
