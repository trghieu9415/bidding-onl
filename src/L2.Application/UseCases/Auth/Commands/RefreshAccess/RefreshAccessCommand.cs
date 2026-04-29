using L2.Application.Abstractions;
using L2.Application.Models;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.RefreshAccess;

public record RefreshAccessCommand(string RefreshToken) : IRequest<RefreshAccessResult>, ITransactional;

public record RefreshAccessResult(AuthTokens Tokens);
