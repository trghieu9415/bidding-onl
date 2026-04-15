using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Login;

public record LoginCommand(string Email, string Password, UserRole Role)
  : IRequest<LoginResult>, ITransactional;

public record LoginResult(AuthTokens Tokens);
