using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace L0.API.Extensions;

public static class SwaggerExtension {
  public static IServiceCollection AddSwaggerDocument(this IServiceCollection services) {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c => {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bidding App - User API", Version = "v1" });
      c.SwaggerDoc("v2", new OpenApiInfo { Title = "Bidding App - Dashboard API", Version = "v2" });

      c.DescribeAllParametersInCamelCase();
      c.DocInclusionPredicate((docName, apiDesc) =>
        string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase)
      );

      c.TagActionsBy(api => [api.ActionDescriptor.RouteValues["controller"]]);

      var jwtScheme = new OpenApiSecurityScheme {
        Name = "Authorization",
        Description = "Nhập Access Token của bạn: `Bearer {token}`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
          Id = JwtBearerDefaults.AuthenticationScheme
        }
      };

      c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
      c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { jwtScheme, Array.Empty<string>() }
      });
    });


    return services;
  }
}
