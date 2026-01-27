using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace L3.Infrastructure.Persistence;

public class MongoDbContext {
  private readonly IMongoDatabase _database;

  public MongoDbContext(IConfiguration config) {
    var client = new MongoClient(config.GetConnectionString("MongoConnection"));
    _database = client.GetDatabase(config["Mongo:DatabaseName"]);
  }

  public IMongoCollection<T> GetCollection<T>(string name) {
    return _database.GetCollection<T>(name);
  }
}
