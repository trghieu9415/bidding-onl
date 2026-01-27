using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Events;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.Ports.Repositories;
using L2.Application.UseCases.Bidding.System.EndSession;
using L2.Application.UseCases.Bidding.System.StartSession;
using L3.Infrastructure.Persistence;
using L3.Infrastructure.Persistence.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace L3.Worker.Consumers.Bidding.Events;

public class SessionPublishedConsumer(
  IMessageScheduler scheduler,
  MongoDbContext mongoContext,
  IRepository<Auction> auctionRepo,
  IRepository<CatalogItem> itemRepo,
  IRepository<Category> categoryRepo,
  IRepository<AuctionSession> sessionRepo,
  ILogger<SessionPublishedConsumer> logger
) : IConsumer<SessionPublishedEvent> {
  public async Task Consume(ConsumeContext<SessionPublishedEvent> context) {
    var msg = context.Message;

    try {
      await ScheduleSessionTransitions(msg);
      await SyncSearchReadModel(msg);
    } catch (Exception ex) {
      logger.LogError(ex, "Lỗi khi xử lý SessionPublishedEvent cho Session: {SessionId}", msg.SessionId);
      throw;
    }
  }

  private async Task ScheduleSessionTransitions(SessionPublishedEvent msg) {
    await scheduler.SchedulePublish(msg.StartTime, new StartSessionCommand(msg.Id));
    await scheduler.SchedulePublish(msg.EndTime, new EndSessionCommand(msg.Id));
  }

  private async Task SyncSearchReadModel(SessionPublishedEvent msg) {
    var session = await sessionRepo.GetByIdAsync(msg.SessionId)
                  ?? throw new InvalidOperationException($"Không tìm thấy Session {msg.SessionId}");

    var auctions = await auctionRepo.GetByKeysAsync(session.AuctionIds.ToList());
    var collection = mongoContext.GetCollection<AuctionSearchDocument>(DocumentKeys.AuctionSearch);

    var categoryCache = new Dictionary<Guid, Category>();

    foreach (var auction in auctions) {
      var item = await itemRepo.GetByIdAsync(auction.CatalogItemId);
      if (item == null) {
        logger.LogWarning("Không tìm thấy vật phẩm {ItemId} cho cuộc đấu giá {AuctionId}", auction.CatalogItemId,
          auction.Id);
        continue;
      }

      var ancestors = await BuildCategoryHierarchy(item.CategoryIds.ToList(), categoryCache);

      var doc = new AuctionSearchDocument(
        auction.Id,
        item.Id,
        item.Name,
        ancestors,
        auction.CurrentPrice,
        auction.Status.ToString(),
        session.Id,
        session.TimeFrame.StartTime,
        session.TimeFrame.EndTime,
        item.Images.MainImageUrl
      );

      await collection.ReplaceOneAsync(
        x => x.AuctionId == auction.Id,
        doc,
        new ReplaceOptions { IsUpsert = true }
      );
    }

    logger.LogInformation("Đã đồng bộ {Count} đấu giá vào Search DB cho Session {SessionId}", auctions.Count,
      session.Id);
  }

  private async Task<List<Guid>> BuildCategoryHierarchy(List<Guid> leafIds, Dictionary<Guid, Category> cache) {
    if (leafIds.Count == 0) {
      return [];
    }

    var result = new HashSet<Guid>(leafIds);
    foreach (var id in leafIds) {
      var currentId = id;
      while (currentId != Guid.Empty) {
        if (!cache.TryGetValue(currentId, out var cat)) {
          cat = await categoryRepo.GetByIdAsync(currentId);
          if (cat != null) {
            cache[currentId] = cat;
          }
        }

        if (cat?.ParentId != null) {
          result.Add(cat.ParentId.Value);
          currentId = cat.ParentId.Value;
        } else {
          break;
        }
      }
    }

    return result.ToList();
  }
}
