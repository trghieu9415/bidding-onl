namespace L1.Core.Domain.Transaction.ValueObjects;

public record Address(
  string ReceiverName,
  string PhoneNumber,
  string ShippingAddress
);
