using L2.Application.Abstractions;
using L2.Application.Ports.Repositories;
using L3.Infrastructure.Adapters.Repositories;
using L3.Infrastructure.Persistence;
using L3.Infrastructure.Seeding;
using L3.Infrastructure.Seeding.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

    services.AddSingleton(dataSourceBuilder.Build());
    services.AddDbContext<AppDbContext>((serviceProvider, options) => {
      var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();
      options.UseNpgsql(dataSource, builder =>
        builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
      );
      options.ConfigureWarnings(warning =>
        warning.Ignore(RelationalEventId.PendingModelChangesWarning)
      );
    });

    services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<AppDbContext>());

    // Repositories
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>));

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
