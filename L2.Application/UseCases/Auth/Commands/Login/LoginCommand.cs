using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Login;

public record LoginCommand(LoginRequest Data, UserRole Role) : IRequest<LoginResult>, ITransactional;

public record LoginRequest(string Email, string Password);

public record LoginResult(AuthTokens Tokens);
