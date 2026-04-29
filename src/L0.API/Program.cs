using L0.API.ExceptionHandler;
using L0.API.Extensions;
using L0.API.Hubs;
using L3.Infrastructure;
using L3.Infrastructure.Seeding;
using L3.Worker;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// --- Infrastructure ---
builder.Configuration.AddJsonFile("secrets.json", true, true);
builder.Services.AddInfrastructure(builder.Configuration);

// --- Worker ---
builder.Services.AddWorker(builder.Configuration);

// --- Global Exception Handler ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Presentation Extension ---
builder.AddSerilogCustom();
builder.Services
  .AddJwtAuthentication(builder.Configuration)
  .AddWebApiDefaults()
  .AddSwaggerDocument()
  .AddSignalRAdapters(builder.Configuration)
  .AddAppRateLimiter(builder.Configuration);

// --- Http Context ---
builder.Services.AddHttpContextAccessor();


// =========================================================================
// || -_-_-_-_-_-_-_-_-_-_-_-_-_-_ APP BUILD _-_-_-_-_-_-_-_-_-_-_-_-_-_- ||
// =========================================================================
var app = builder.Build();

// --- CLI Flag: Seeding Data ---
if (args.Contains("--seeding")) {
  using var scope = app.Services.CreateScope();
  try {
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.SeedAsync();
  } catch (Exception ex) {
    Console.WriteLine($"[-] Lỗi trong quá trình Seeding: {ex.Message}");
  }

  return;
}

// --- Custom Middlewares ---
app.UseHttpsRedirection();

// --- Swagger UI ---
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Dashboard API");
    c.SwaggerEndpoint("/swagger/v3/swagger.json", "External API");
    c.DocExpansion(DocExpansion.None);
  });
}

// --- Exception Handler ---
app.UseExceptionHandler();

// --- Allow Static Files ---
app.UseStaticFiles();

// --- Routing ---
app.UseRouting();

// --- CORS ---
// TODO: In production, restrict CORS to known frontend domains using .WithOrigins(...).
app.UseCors(options => options
  .AllowAnyMethod()
  .AllowAnyHeader()
  .SetIsOriginAllowed(_ => true)
  .AllowCredentials());

// --- Rate Limit ---
app.UseRateLimiter();

// --- Auth ---
app.UseAuthentication();
app.UseAuthorization();

// --- Endpoints ---
app.MapControllers();
app.MapHub<AuctionHub>("/hubs/auction");
app.MapHub<UserHub>("/hubs/notification");

app.Run();
