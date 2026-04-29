using L1.Core.Domain.Transaction.ValueObjects;
using Xunit;

namespace Tests.Unit.L1.Core.Transaction;

public class TransactionValueObjectTests {
  [Fact]
  public void Address_RecordStoresReceiverPhoneAndShippingAddress() {
    var address = new Address("John Doe", "0123456789", "123 Auction Street");

    Assert.Equal("John Doe", address.ReceiverName);
    Assert.Equal("0123456789", address.PhoneNumber);
    Assert.Equal("123 Auction Street", address.ShippingAddress);
  }

  [Fact]
  public void Address_RecordUsesValueEquality() {
    var left = new Address("John Doe", "0123456789", "123 Auction Street");
    var right = new Address("John Doe", "0123456789", "123 Auction Street");

    Assert.Equal(left, right);
  }
}
