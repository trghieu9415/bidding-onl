namespace L2.Application.Ports.Gateways;

public interface IEmailService {
  Task SendResetPasswordEmailAsync(string email, string token, CancellationToken ct = default);
}
