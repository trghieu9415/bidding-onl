using System.Net;

namespace Tests.Integration.TestSupport;

internal sealed class RecordingHttpMessageHandler : HttpMessageHandler {
  private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responses = new();

  public List<HttpRequestMessage> Requests { get; } = [];

  public void Enqueue(HttpStatusCode statusCode, string body) {
    _responses.Enqueue(_ => new HttpResponseMessage(statusCode) {
      Content = new StringContent(body)
    });
  }

  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
    Requests.Add(request);

    if (_responses.Count == 0) {
      throw new InvalidOperationException("No HTTP response was queued for the request.");
    }

    var response = _responses.Dequeue()(request);
    response.RequestMessage = request;
    return Task.FromResult(response);
  }
}
