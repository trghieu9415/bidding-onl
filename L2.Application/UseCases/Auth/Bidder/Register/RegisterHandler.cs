using L2.Application.Models;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Auth.Bidder.Register;

public class RegisterHandler(IAuthService authService) : IRequestHandler<RegisterCommand, RegisterResult> {
  public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken ct) {
    var user = new User {
      Email = request.Email,
      FullName = request.FullName,
      PhoneNumber = request.PhoneNumber
    };

    var tokens = await authService.RegisterAsync(user, request.Password, ct);
    return new RegisterResult(tokens);
  }
}