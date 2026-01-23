using System.Text.Json;
using System.Text.Json.Serialization;
using L0.API.Extensions;
using L0.API.Middlewares;
using L2.Application;
using L2.Application.Behaviors;
using L3.Infrastructure;
using L3.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// --- Web API & JSON Configuration ---
builder.Services
  .AddControllers()
  .ConfigureApiBehaviorOptions(options => {
    options.SuppressModelStateInvalidFilter = true;
  })
  .AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
  });

// --- Routing Configuration ---
builder.Services.AddRouting(options => {
  options.LowercaseUrls = true;
  options.LowercaseQueryStrings = true;
});

// --- Swagger Documentation ---
builder.Services.AddSwaggerDocument();

// --- Application Layer (MediatR, AutoMapper, Validation) ---
var applicationAssembly = typeof(IApplication).Assembly;

builder.Services.AddMediatR(cfg => {
  cfg.RegisterServicesFromAssembly(applicationAssembly);
  cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
  cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

builder.Services.AddAutoMapper(config => {}, applicationAssembly);

// --- Infrastructure & Third-Party Layers ---
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

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

// --- Error Handling & Security ---
app.UseMiddleware<GlobalExceptionHandler>();
app.UseHttpsRedirection();

// --- Swagger UI ---
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Dashboard API v2");
  });
}

// --- Authentication & Authorization ---
// app.UseAuthentication();
// app.UseAuthorization();

// --- Endpoints ---
app.MapControllers();

// --- Allow Static Files ---
app.UseStaticFiles();

app.Run();
