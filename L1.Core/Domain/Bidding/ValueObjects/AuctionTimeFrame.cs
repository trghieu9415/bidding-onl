using L1.Core.Base.Exception;

namespace L1.Core.Domain.Bidding.ValueObjects;

public record AuctionTimeFrame(DateTime StartTime, DateTime EndTime) {
  public static void Validate(DateTime startTime, DateTime endTime) {
    if (startTime >= endTime) {
      throw new DomainException("Thời gian bắt đầu phải trước thời gian kết thúc.");
    }

    if (endTime <= DateTime.UtcNow) {
      throw new DomainException("Thời gian kết thúc không được ở trong quá khứ.");
    }
  }
}
