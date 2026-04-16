using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken, bool AllDevices) : IRequest<bool>, ITransactional;
