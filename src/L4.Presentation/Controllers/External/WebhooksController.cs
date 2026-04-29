using System.Text.Json;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.UseCases.Transactions.Commands.ProcessPayment;
using Microsoft.AspNetCore.Mvc;

namespace L4.Presentation.Controllers.External;

public class WebhooksController : ExternalController {
  [HttpPost("stripe")]
  public async Task<IActionResult> StripeWebhook(CancellationToken ct) {
    using var reader = new StreamReader(Request.Body);
    var rawBody = await reader.ReadToEndAsync(ct);

    var payloadObj = new {
      raw_body = rawBody,
      stripe_signature = Request.Headers["Stripe-Signature"].ToString()
    };

    var jsonElement = JsonSerializer.SerializeToElement(payloadObj);
    var request = new ProcessPaymentRequest(PaymentMethod.Stripe, jsonElement);

    var isSucceed = await Mediator.Send(new ProcessPaymentCommand(request), ct);
    return isSucceed ? Ok() : BadRequest();
  }

  [HttpPost("paypal")]
  public async Task<IActionResult> PaypalWebhook(CancellationToken ct) {
    using var reader = new StreamReader(Request.Body);
    var rawBody = await reader.ReadToEndAsync(ct);

    var payloadObj = new {
      auth_algo = Request.Headers["PAYPAL-AUTH-ALGO"].ToString(),
      cert_url = Request.Headers["PAYPAL-CERT-URL"].ToString(),
      transmission_id = Request.Headers["PAYPAL-TRANSMISSION-ID"].ToString(),
      transmission_sig = Request.Headers["PAYPAL-TRANSMISSION-SIG"].ToString(),
      transmission_time = Request.Headers["PAYPAL-TRANSMISSION-TIME"].ToString(),
      raw_body = rawBody
    };

    var jsonElement = JsonSerializer.SerializeToElement(payloadObj);
    var request = new ProcessPaymentRequest(PaymentMethod.Paypal, jsonElement);

    var isSucceed = await Mediator.Send(new ProcessPaymentCommand(request), ct);
    return isSucceed ? Ok() : BadRequest();
  }
}
