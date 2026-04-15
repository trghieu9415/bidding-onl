using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Register;

public record RegisterCommand(string Email, string FullName, string Password, string? PhoneNumber)
  : IRequest<RegisterResult>, ITransactional;
public record RegisterResult(AuthTokens Tokens);
