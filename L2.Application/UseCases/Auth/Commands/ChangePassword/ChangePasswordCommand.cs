using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Data) : IRequest<bool>, ITransactional;

public record ChangePasswordRequest(string OldPassword, string NewPassword);
