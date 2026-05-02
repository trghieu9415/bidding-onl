using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.ValueObjects;

namespace Tests.Unit.L2.Application.UseCases.Transactions;

internal static class TransactionTestData {
  internal static Address Address => new("John Doe", "0123456789", "123 Street");

  internal static Order CreatePendingOrder(Guid bidderId) {
    return Order.Create(
      bidderId,
      "John Doe",
      "john@example.com",
      Guid.NewGuid(),
      Guid.NewGuid(),
      "Laptop",
      "img.png",
      Address
    );
  }
}
