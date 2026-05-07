using Application.Interface;
using Application.Interface.Services;
using Application.Service;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//локал
if (builder.Environment.IsDevelopment())
{

    var localConnection = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=ltybc1977";
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(localConnection));
}
//докер
else
{
    
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}


var assembly = Assembly.GetExecutingAssembly();

try
{
    foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
    {
        foreach (var interfac in type.GetInterfaces())
        {
            if (interfac.IsGenericType && interfac.GetGenericTypeDefinition() == typeof(IHandler<>))
            {
                builder.Services.AddScoped(interfac, type);
            }
        }
    }
}catch(ReflectionTypeLoadException ex)
{
    Console.WriteLine(ex);

    foreach (var loaderException in ex.LoaderExceptions)
    {
        Console.WriteLine("----");
        Console.WriteLine(loaderException);
    }
}

builder.Services.AddHttpClient<IRoutingService, RoutingService>(client =>
    client.Timeout = TimeSpan.FromSeconds(30)
);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHostedService<ProcessWorker>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>();
builder.Services.AddScoped<IRoutingService, RoutingService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuditService, ManualErrorFixAuditService>();
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
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

try
{
    app.MapControllers();
}
catch (ReflectionTypeLoadException ex)
{
    Console.WriteLine(ex);

    foreach (var e in ex.LoaderExceptions)
    {
        Console.WriteLine("==============");
        Console.WriteLine(e.Message);
    }
}

app.Run();
