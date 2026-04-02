using DeviceManagement.Api.Middleware;
using DeviceManagement.Infrastructure.DependencyInjection;
using DeviceManagement.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDeviceManagementInfrastructure(connectionString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseGlobalExceptionHandler();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
}

app.Run();
