using L1.Core.Domain.Bidding.Events;
using L2.Application.Ports.Realtime;
using L2.Application.Ports.Realtime.Contracts;
using L3.Infrastructure.Persistence;
using L3.Infrastructure.Persistence.Mongo;
using MassTransit;
using MongoDB.Driver;

namespace L3.Worker.Consumers.Bidding.Events;

public class BidPlacedConsumer(
  IRealtimeService realtimeService,
  MongoDbContext mongoContext
) : IConsumer<BidPlacedEvent> {
  public async Task Consume(ConsumeContext<BidPlacedEvent> context) {
    var msg = context.Message;

    await realtimeService.PublishAsync(
      HubKeys.Bidding,
      msg.AuctionId.ToString(),
      "NewBidReceived",
      new {
        msg.BidderId,
        msg.Amount
      },
      context.CancellationToken
    );

    var collection = mongoContext.GetCollection<AuctionSearchDocument>(DocumentKeys.AuctionSearch);

    var update = Builders<AuctionSearchDocument>.Update.Set(x => x.CurrentPrice, msg.Amount);
    await collection.UpdateOneAsync(x => x.AuctionId == msg.AuctionId, update);
  }
}
