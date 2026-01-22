using L2.Application.Abstractions;

namespace L2.Application.UseCases.Auth.Admin.Login;

public record LoginCommand(string Email, string Password) : ICommand<LoginResult>;