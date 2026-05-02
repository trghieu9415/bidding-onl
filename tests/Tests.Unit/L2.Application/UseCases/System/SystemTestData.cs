using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.ValueObjects;

namespace Tests.Unit.L2.Application.UseCases.System;

internal static class SystemTestData {
  internal static Order CreateOrder(Guid bidderId) {
    return Order.Create(
      bidderId,
      "John Doe",
      "john@example.com",
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Laptop",
      "img.png",
      new Address("John Doe", "0123456789", "123 Street")
    );
  }
}
