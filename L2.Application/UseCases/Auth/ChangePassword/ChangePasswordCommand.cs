using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auth.ChangePassword;

public record ChangePasswordCommand(string OldPassword, string NewPassword) : IRequest<Unit>, ITransactional;
