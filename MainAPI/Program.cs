using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Application.Service;
using Infrastructure.Persistance;
using Infrastructure.Persistance.configuration;
using Infrastructure.Persistance.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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


var assemblies = AppDomain.CurrentDomain.GetAssemblies();
Console.WriteLine("Assemblies for scanning [ total: {0} ]",assemblies.Length);
try
{
    foreach(var assembly in assemblies)
    {

        Console.WriteLine("Assembly [ number {0} ]", assembly.FullName);
        foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                foreach (var interf in type.GetInterfaces().Where(intf => intf.IsGenericType &&
                intf.GetGenericTypeDefinition() == typeof(IHandler<>))
              )
                {
                    builder.Services.AddScoped(interf, type);
                    Console.WriteLine($"Registered: {interf.Name} -> {type.Name}");
                }
            }
        foreach(var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            foreach (var interf in type.GetInterfaces().Where(intf => intf == typeof(IFraudRule)))
            {
                builder.Services.AddScoped(interf, type);
                Console.WriteLine($"Registered: {interf.Name} -> {type.Name}");
            }
        }
       
    }
} catch(ReflectionTypeLoadException ex)
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
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuditService, ManualErrorFixAuditService>();
builder.Services.AddScoped<IAntiFraudCheckRepository, AntiFraudCheckRepository>();
builder.Services.AddScoped<IAntiFraudCheckService, AntiFraudCheckService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAntiFraudTrackingService, AntiFraudTrackingService>();
builder.Services.AddHostedService<ProcessWorker>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoutingService, RoutingService>();
builder.Services.AddScoped<IAuditRepository, ErrorAuditRepository>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];

builder.Services.AddAuthentication(options => {

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
        });
});
var app = builder.Build();

app.UseCors("AllowFrontend");
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
