using System.Collections.Generic;

namespace L1.Core.Base.Event;

public interface IHasDomainEvent {
  IReadOnlyCollection<DomainEvent> DomainEvents { get; }
  public void AddDomainEvent(DomainEvent domainEvent);
  public void ClearEvents();
}
