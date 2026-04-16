using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword) : IRequest<bool>, ITransactional;
