using FluentAssertions;
using L1.Core.Domain.Transaction.ValueObjects;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.ValueObjects;

public class TransactionValueObjectTests {
  [Fact]
  public void Address_RecordStoresReceiverPhoneAndShippingAddress() {
    // Arrange
    const string receiverName = "John Doe";
    const string phoneNumber = "0123456789";
    const string shippingAddress = "123 Auction Street";

    // Act
    var address = new Address(receiverName, phoneNumber, shippingAddress);

    // Assert
    address.ReceiverName.Should().Be("John Doe");
    address.PhoneNumber.Should().Be("0123456789");
    address.ShippingAddress.Should().Be("123 Auction Street");
  }

  [Fact]
  public void Address_RecordUsesValueEquality() {
    // Arrange
    var left = new Address("John Doe", "0123456789", "123 Auction Street");
    var right = new Address("John Doe", "0123456789", "123 Auction Street");

    // Act
    var isEqual = left.Equals(right);

    // Assert
    isEqual.Should().BeTrue();
    left.Should().Be(right);
  }
}
