namespace L2.Application.Ports.Notification;

public interface IEmailService {
  Task SendResetPasswordEmailAsync(string email, string token, CancellationToken ct = default);
}