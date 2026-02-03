using L2.Application.Abstractions;

namespace L2.Application.UseCases.Auth.Admin.RefreshAccess;

public record RefreshAccessCommand(string RefreshToken) : ICommand<RefreshAccessResult>;
