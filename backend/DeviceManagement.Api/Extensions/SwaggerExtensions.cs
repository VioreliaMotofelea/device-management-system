using DeviceManagement.Api.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace DeviceManagement.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddDeviceManagementSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Device Management API", Version = "v1" });

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "JWT: use header **Authorization: Bearer {token}**",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                });

            options.OperationFilter<AuthorizeOperationFilter>();
        });

        return services;
    }
}

