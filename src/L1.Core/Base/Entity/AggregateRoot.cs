using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using L1.Core.Base.Event;

namespace L1.Core.Base.Entity;

public abstract class AggregateRoot : BaseEntity, IHasDomainEvent {
  private readonly List<DomainEvent> _domainEvents = [];
  [NotMapped] [JsonIgnore] public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  public void AddDomainEvent(DomainEvent domainEvent) {
    _domainEvents.Add(domainEvent);
  }

  public void ClearEvents() {
    _domainEvents.Clear();
  }
}
