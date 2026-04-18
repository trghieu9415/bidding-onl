using L2.Application.Abstractions;
using L2.Application.Repositories;
using L2.Application.Repositories.Read;
using L3.Infrastructure.Persistence;
using L3.Infrastructure.Persistence.Repositories;
using L3.Infrastructure.Persistence.Repositories.Read;
using L3.Infrastructure.Seeding;
using L3.Infrastructure.Seeding.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace L3.Infrastructure.Extensions;

public static class PersistenceExtensions {
  public static IServiceCollection AddPostgresPersistence(this IServiceCollection services, IConfiguration config) {
    // Connection & DbContext
    var connectionString = config.GetConnectionString("DefaultConnection");
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    dataSourceBuilder.EnableDynamicJson();

    var dataSource = dataSourceBuilder.Build();
    services.AddSingleton(dataSource);
    services.AddDbContext<AppDbContext>((_, options) => { options.UseNpgsql(dataSource); });

    services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());

    // Repositories
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    // Read Repositories
    services.AddScoped(typeof(IReadRepository<,>), typeof(EfReadRepository<,>));
    services.AddScoped<IAuctionReadRepository, AuctionReadRepository>();
    services.AddScoped<IOrderReadRepository, OrderReadRepository>();
    services.AddScoped<IPaymentReadRepository, PaymentReadRepository>();
    services.AddScoped<ISessionReadRepository, SessionReadRepository>();

    // Seeders
    services.AddScoped<DbInitializer>();
    services.AddScoped<ISeeder, UserSeeder>();
    services.AddScoped<ISeeder, AdminSeeder>();
    services.AddScoped<ISeeder, CategorySeeder>();
    services.AddScoped<ISeeder, CatalogItemSeeder>();
    services.AddScoped<ISeeder, AuctionSessionSeeder>();

    return services;
  }
}
