using L2.Application.Ports.Identity;
using L2.Application.Ports.Security;
using L3.Infrastructure.Adapters.Identity;
using L3.Infrastructure.Adapters.Security;
using L3.Infrastructure.Identity;
using L3.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Infrastructure.Extensions;

public static class IdentityExtensions {
  public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services) {
    // Identity Core Configuration
    services.AddIdentityCore<AppUser>(options => {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
      })
      .AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultTokenProviders();

    // Auth Services Implementation
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IUserService, UserService>();

    return services;
  }
}
