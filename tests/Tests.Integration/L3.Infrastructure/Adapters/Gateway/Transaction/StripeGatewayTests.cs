using System.Text.Json;
using L3.Infrastructure.Adapters.Gateway.Transaction;
using L3.Infrastructure.Options;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Adapters.Gateway.Transaction;

public class StripeGatewayTests {
  [Fact]
  public void ToClientPayload_and_ToWebhookPayload_map_expected_properties() {
    var gateway = new StripeGateway(new StripeSettings {
      SecretKey = "sk_test",
      EndpointSecret = "whsec_test",
      Currency = "usd",
      SuccessUrl = "https://frontend.test/success",
      CancelUrl = "https://frontend.test/cancel",
      Retry = 1
    });

    using var clientDocument = JsonDocument.Parse("""{ "session_id": "cs_test_123" }""");
    using var webhookDocument = JsonDocument.Parse("""
      {
        "raw_body": "{}",
        "stripe_signature": "sig_test"
      }
      """);

    var clientPayload = Assert.IsType<StripeClientPayload>(gateway.ToClientPayload(clientDocument.RootElement));
    var webhookPayload = Assert.IsType<StripeWebhookPayload>(gateway.ToWebhookPayload(webhookDocument.RootElement));

    Assert.Equal("cs_test_123", clientPayload.SessionId);
    Assert.Equal("{}", webhookPayload.RawBody);
    Assert.Equal("sig_test", webhookPayload.StripeSignature);
  }
}
