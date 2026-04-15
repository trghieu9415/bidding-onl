using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.RequestPassword;

public record RequestPasswordCommand(string Email) : IRequest<Unit>, ITransactional;
