using L2.Application.Abstractions;

namespace L2.Application.UseCases.Auth.Bidder.Register;

public record RegisterCommand(string Email, string FullName, string Password, string? PhoneNumber)
  : ICommand<RegisterResult>;