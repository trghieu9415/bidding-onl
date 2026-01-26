namespace L2.Application.Ports.Realtime;

public interface IRealtimeService {
  Task PublishAsync(string hubKey, string group, string method, object data, CancellationToken ct = default);
}
