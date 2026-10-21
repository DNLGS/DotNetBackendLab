using DotNetFoundations.Database;
using DotNetFoundations.Middlewares;
using DotNetFoundations.Repository;
using DotNetFoundations.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ModelsRepository>(sp => new ModelsRepository(sp.GetRequiredService<DatabaseContext>()));
builder.Services.AddScoped<DatabaseService>(sp => new DatabaseService(sp.GetRequiredService<DatabaseContext>()));
builder.Services.AddScoped<ClientsService>(sp => new ClientsService(sp.GetRequiredService<ModelsRepository>(), sp.GetRequiredService<DatabaseService>()));
builder.Services.AddScoped<ProductsService>(sp => new ProductsService(sp.GetRequiredService<ModelsRepository>(), sp.GetRequiredService<DatabaseService>()));
builder.Services.AddScoped<ItemsService>(sp => new ItemsService(sp.GetRequiredService<ModelsRepository>(), sp.GetRequiredService<DatabaseService>()));
builder.Services.AddScoped<OrderService>(sp => new OrderService(sp.GetRequiredService<ModelsRepository>(), sp.GetRequiredService<DatabaseService>()));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
