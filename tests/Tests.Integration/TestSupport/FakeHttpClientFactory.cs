namespace Tests.Integration.TestSupport;

internal sealed class FakeHttpClientFactory(HttpClient client) : IHttpClientFactory {
  public HttpClient CreateClient(string name) {
    return client;
  }
}
