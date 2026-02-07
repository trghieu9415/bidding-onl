namespace L2.Application.Ports.Gateways;

public interface IEventDispatcher {
  Task DispatchEventsAsync(CancellationToken ct = default);
}
