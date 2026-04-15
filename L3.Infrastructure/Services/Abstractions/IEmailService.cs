namespace L3.Infrastructure.Services.Abstractions;

public interface IEmailService {
  Task SendOrderConfirmationEmailAsync(string email, string orderId, CancellationToken ct = default);
  Task SendResetPasswordEmailAsync(string email, string token, CancellationToken ct = default);
}
