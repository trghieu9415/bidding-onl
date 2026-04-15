using L3.Infrastructure.Options;
using L3.Infrastructure.Services.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace L3.Infrastructure.Services;

public class EmailService(
  EmailOptions options,
  ILogger<EmailService> logger
) : IEmailService {
  public async Task SendOrderConfirmationEmailAsync(string email, string orderId, CancellationToken ct = default) {
    var subject = $"[Movie Online] Xác nhận đơn hàng #{orderId}";

    var placeholders = new Dictionary<string, string> {
      { "OrderId", orderId }
    };

    var body = await GetEmailTemplateAsync("OrderConfirmation.html", placeholders);
    await SendEmailAsync(email, subject, body, ct);
  }

  public async Task SendResetPasswordEmailAsync(string email, string token, CancellationToken ct = default) {
    const string subject = "[Movie Online] Khôi phục mật khẩu của bạn";

    var resetLink = $"{options.FrontEndUrl}?token={token}&email={email}";

    var placeholders = new Dictionary<string, string> {
      { "ResetLink", resetLink }
    };

    var body = await GetEmailTemplateAsync("ResetPassword.html", placeholders);
    await SendEmailAsync(email, subject, body, ct);
  }

  // NOTE: ========== [Helpers để đọc Template] ==========
  private static async Task<string> GetEmailTemplateAsync(string fileName, Dictionary<string, string> placeholders) {
    var path = Path.Combine(AppContext.BaseDirectory, "Templates", "Email", fileName);

    if (!File.Exists(path)) {
      throw new FileNotFoundException($"Không tìm thấy template email: {fileName}");
    }

    var template = await File.ReadAllTextAsync(path);
    return placeholders.Aggregate(template, (current, item) => current.Replace($"{{{{{item.Key}}}}}", item.Value));
  }

  private async Task SendEmailAsync(string email, string subject, string htmlBody, CancellationToken ct = default) {
    try {
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(options.FromName, options.FromAddress));
      message.To.Add(MailboxAddress.Parse(email));
      message.Subject = subject;

      message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

      using var client = new SmtpClient();
      client.ServerCertificateValidationCallback = (_, _, _, _) => true;

      await client.ConnectAsync(options.Host, options.Port,
        options.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto, ct);

      if (!string.IsNullOrEmpty(options.Username)) {
        await client.AuthenticateAsync(options.Username, options.Password, ct);
      }

      await client.SendAsync(message, ct);
      await client.DisconnectAsync(true, ct);

      logger.LogInformation("Gửi email thành công tới: {Email}", email);
    } catch (Exception ex) {
      logger.LogError(ex, "Lỗi nghiêm trọng khi gửi email tới {Email}", email);
    }
  }
}
