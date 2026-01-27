using System.Text;
using L2.Application.Ports.Identity;
using L2.Application.Ports.Notification;
using L2.Application.Ports.Repositories;
using L2.Application.Ports.Search;
using L2.Application.Ports.Security;
using L2.Application.Ports.Storage;
using L3.Infrastructure.Adapters.Identity;
using L3.Infrastructure.Adapters.Notification;
using L3.Infrastructure.Adapters.Repositories;
using L3.Infrastructure.Adapters.Search;
using L3.Infrastructure.Adapters.Security;
using L3.Infrastructure.Adapters.Storage;
using L3.Infrastructure.Identity;
using L3.Infrastructure.Persistence;
using L3.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Sieve.Services;

namespace L3.Infrastructure;

public static class InfrastructureConfig {
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {
    services
      .AddPersistence(config)
      .AddMongo()
      .AddIdentityConfig()
      .AddRepositories()
      .AddAuthStrategy(config)
      .AddThirdPartyServices(config)
      .AddSeeders();

    return services;
  }

  // NOTE: ========== [Cơ sở dữ liệu] ==========
  private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config) {
    var connectionString = config.GetConnectionString("DefaultConnection");

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    dataSourceBuilder.EnableDynamicJson();
    var dataSource = dataSourceBuilder.Build();

    services.AddDbContext<AppDbContext>(options =>
      options.UseNpgsql(dataSource,
        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

    services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
    services.AddScoped<DbInitializer>();

    return services;
  }

  // NOTE: ========== [Cơ sở dữ liệu tìm kiếm] ==========
  private static IServiceCollection AddMongo(this IServiceCollection services) {
    services.AddSingleton<MongoDbContext>();
    services.AddScoped<IAuctionSearchService, MongoAuctionSearchService>();
    return services;
  }

  // NOTE: ========== [Identity User] ==========
  private static IServiceCollection AddIdentityConfig(this IServiceCollection services) {
    services.AddIdentityCore<AppUser>(options => {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
      })
      .AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultTokenProviders();

    return services;
  }

  // NOTE: ========== [Repositories] ==========
  private static IServiceCollection AddRepositories(this IServiceCollection services) {
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(EfReadRepository<>));

    return services;
  }

  // NOTE: ========== [Xác thực] ==========
  private static IServiceCollection AddAuthStrategy(this IServiceCollection services, IConfiguration config) {
    services.AddScoped<IJwtService, JwtService>();
    services.AddScoped<IAuthentication, AuthenticationService>();
    services.AddScoped<ICurrentUser, CurrentUser>();
    services.AddScoped<IUserService, UserService>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = config["Jwt:Issuer"],
          ValidAudience = config["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!))
        };

        options.Events = new JwtBearerEvents {
          OnMessageReceived = context => {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) {
              context.Token = accessToken;
            }

            return Task.CompletedTask;
          }
        };
      });

    return services;
  }

  // NOTE: ========== [Dịch vụ ngoài] ==========
  private static IServiceCollection AddThirdPartyServices(this IServiceCollection services, IConfiguration config) {
    services.AddScoped<ISieveProcessor, AppSieveProcessor>();
    services.AddScoped<IEmailService, EmailService>();
    services.AddScoped<IBinaryStorage, LocalBinaryStorage>();

    services.AddStackExchangeRedisCache(options => {
      options.Configuration = config["Redis:Configuration"] ?? "localhost:6379";
      options.InstanceName = config["Redis:InstanceName"] ?? "Bidding_";
    });
    services.AddScoped<ICacheStorage, RedisCacheStorage>();

    return services;
  }

  // NOTE: ========== [Dữ liệu mẫu] ==========
  private static IServiceCollection AddSeeders(this IServiceCollection services) {
    services.AddScoped<ISeeder, UserSeeder>();
    services.AddScoped<ISeeder, CategorySeeder>();
    services.AddScoped<ISeeder, CatalogItemSeeder>();
    services.AddScoped<ISeeder, AuctionSessionSeeder>();

    return services;
  }
}
