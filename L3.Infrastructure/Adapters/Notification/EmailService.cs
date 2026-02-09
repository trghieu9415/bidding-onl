using L2.Application.Ports.Notification;
using Microsoft.Extensions.Logging;

namespace L3.Infrastructure.Adapters.Notification;

public class EmailService(ILogger<EmailService> logger) : IEmailService {
  public async Task SendResetPasswordEmailAsync(string email, string token, CancellationToken ct = default) {
    var resetLink = $"https://frontend.com/reset-password?token={token}&email={email}";

    logger.LogInformation("Sending reset password email to {Email}", email);
    logger.LogInformation("Reset Link: {Link}", resetLink);

    await Task.CompletedTask;
  }
}
