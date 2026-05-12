using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Catalog.Enums;
using L1.Core.Domain.Transaction.Enums;
using L1.Core.Domain.Transaction.ValueObjects;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Gateway;

namespace Tests.Unit.L2.Application.UseCases;

public static class UseCaseTestData {
  public static AuthTokens CreateAuthTokens() {
    return new AuthTokens(
      new TokenModel { Token = "access-token", ExpiredAt = DateTime.UtcNow.AddMinutes(30) },
      new TokenModel { Token = "refresh-token", ExpiredAt = DateTime.UtcNow.AddDays(7) }
    );
  }

  public static User CreateUser(
    Guid? id = null,
    string fullName = "Nguyen Van A",
    string email = "user@example.com",
    bool isActive = true,
    UserRole role = UserRole.Bidder
  ) {
    return new User {
      Id = id ?? Guid.NewGuid(),
      FullName = fullName,
      Email = email,
      PhoneNumber = "0912345678",
      Url = "https://example.com/profile",
      IsActive = isActive,
      Role = role,
      SecurityStamp = "stamp"
    };
  }

  public static CategoryDto CreateCategoryDto(Guid? id = null, string name = "Điện tử") {
    return new CategoryDto {
      Id = id ?? Guid.NewGuid(),
      Name = name,
      ParentId = null,
      CreatedAt = DateTime.UtcNow
    };
  }

  public static CatalogItemDto CreateCatalogItemDto(
    Guid? id = null,
    Guid? ownerId = null,
    string name = "Laptop"
  ) {
    return new CatalogItemDto {
      Id = id ?? Guid.NewGuid(),
      OwnerId = ownerId ?? Guid.NewGuid(),
      Name = name,
      Description = "Gaming laptop",
      Status = ItemStatus.Pending,
      Condition = ItemCondition.NewSealed,
      StartingPrice = 1000,
      MainImageUrl = "https://example.com/main.jpg",
      SubImageUrls = ["https://example.com/sub.jpg"],
      CategoryIds = [Guid.NewGuid()],
      CreatedAt = DateTime.UtcNow
    };
  }

  public static AuctionDto CreateAuctionDto(Guid? id = null, AuctionStatus status = AuctionStatus.Scheduled) {
    return new AuctionDto {
      Id = id ?? Guid.NewGuid(),
      CatalogItemId = Guid.NewGuid(),
      Status = status,
      CurrentPrice = 1000,
      WinningBidId = null,
      StepPrice = 100,
      ReservePrice = 1500,
      Bids = []
    };
  }

  public static AuctionSessionDto CreateAuctionSessionDto(Guid? id = null, SessionStatus status = SessionStatus.Draft) {
    return new AuctionSessionDto {
      Id = id ?? Guid.NewGuid(),
      Title = "Phiên đấu giá tháng 5",
      Status = status,
      StartTime = DateTime.UtcNow.AddHours(1),
      EndTime = DateTime.UtcNow.AddHours(2),
      AuctionIds = [Guid.NewGuid()]
    };
  }

  public static OrderDto CreateOrderDto(
    Guid? id = null,
    Guid? bidderId = null,
    OrderStatus status = OrderStatus.Pending
  ) {
    return new OrderDto {
      Id = id ?? Guid.NewGuid(),
      BidderId = bidderId ?? Guid.NewGuid(),
      BidderName = "Nguyen Van A",
      AuctionId = Guid.NewGuid(),
      CatalogId = Guid.NewGuid(),
      CatalogName = "Laptop",
      CatalogImage = "https://example.com/laptop.jpg",
      Status = status,
      Price = 1000,
      Address = CreateAddress()
    };
  }

  public static PaymentDto CreatePaymentDto(
    Guid? id = null,
    Guid? orderId = null,
    PaymentStatus status = PaymentStatus.Pending
  ) {
    return new PaymentDto {
      Id = id ?? Guid.NewGuid(),
      OrderId = orderId ?? Guid.NewGuid(),
      Amount = 1000,
      PaymentUrl = "https://example.com/pay",
      TransactionId = "txn_123",
      Method = PaymentMethod.Stripe,
      Status = status
    };
  }

  public static Address CreateAddress() {
    return new Address("Nguyen Van A", "0912345678", "123 Nguyen Trai, Quan 1");
  }
}

public sealed record TestClientPayload : ClientPayload;

public sealed record TestWebhookPayload : WebhookPayload;
