using Application.Interface;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options =>
    options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")));


var assembly = Assembly.GetExecutingAssembly();

foreach(var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
{
    foreach(var interfac in type.GetInterfaces())
    {
        if(interfac.IsGenericType && interfac.GetGenericTypeDefinition() == typeof(IHandler<>))
        {
            builder.Services.AddScoped(interfac, type);
        }
    }
}
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHostedService<ProcessWorker>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (app.Environment.IsEnvironment("Docker"))
    {
        await dbContext.Database.MigrateAsync();
    }
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
