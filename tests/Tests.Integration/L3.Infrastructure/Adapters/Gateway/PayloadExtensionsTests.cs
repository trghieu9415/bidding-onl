using System.Text.Json;
using L3.Infrastructure.Adapters.Gateway;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Adapters.Gateway;

public class PayloadExtensionsTests {
  [Fact]
  public void ExtractProperties_reads_requested_values_from_json_element() {
    using var document = JsonDocument.Parse("""
      { "token": "tok-1", "payerId": "payer-1" }
      """);

    var result = document.RootElement.ExtractProperties("token", "payerId");

    Assert.Equal("tok-1", result["token"]);
    Assert.Equal("payer-1", result["payerId"]);
  }

  [Fact]
  public void ExtractProperties_throws_when_required_property_is_missing() {
    using var document = JsonDocument.Parse("""
      { "token": "tok-1" }
      """);

    var ex = Assert.Throws<ArgumentException>(() => document.RootElement.ExtractProperties("token", "payerId"));

    Assert.Contains("payerId", ex.Message);
  }

  [Fact]
  public void ExtractProperties_from_string_throws_for_blank_json() {
    Assert.Throws<ArgumentException>(() => PayloadExtensions.ExtractProperties(" ", "token"));
  }
}
