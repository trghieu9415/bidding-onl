using L2.Application.Models;

namespace L2.Application.UseCases.Auth.Admin.Login;

public record LoginResult(AuthTokens Tokens);