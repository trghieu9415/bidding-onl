using L2.Application.Models;

namespace L2.Application.UseCases.Auth.Admin.RefreshAccess;

public record RefreshAccessResult(AuthTokens Tokens);
