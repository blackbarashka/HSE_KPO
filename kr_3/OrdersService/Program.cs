using Microsoft.EntityFrameworkCore;
using Common.EventBus;
using OrdersService.Data;
using OrdersService.Hubs;
using OrdersService.Messaging;
using OrdersService.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// База данных для заказов
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrdersDatabase")));

// Настройки брокера сообщений
builder.Services.Configure<MessageBrokerSettings>(
    builder.Configuration.GetSection("MessageBroker"));

// Регистрация сервисов
builder.Services.AddScoped<IOrderService, OrderService>();

// Добавление SignalR для уведомлений в реальном времени
builder.Services.AddSignalR();

// Добавление фоновых сервисов для обработки сообщений
builder.Services.AddHostedService<PaymentResponseConsumer>();
builder.Services.AddHostedService<TransactionalOutboxProcessor>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orders API",
        Version = "v1",
        Description = "API сервиса заказов интернет-магазина" 
    });

    c.AddSecurityDefinition("UserId", new OpenApiSecurityScheme
    {
        Description = "ID пользователя (для тестирования)",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-User-Id",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    var requirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "UserId"
                },
                In = ParameterLocation.Header
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(requirement);
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<OrdersDbContext>(); 
        context.Database.EnsureCreated(); // для просто создания базы

        Console.WriteLine("База данных успешно создана/обновлена");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при создании/миграции базы данных");
    }
}

if (app.Environment.IsDevelopment() || true) 
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API v1");
    });
}



app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.MapHub<OrdersHub>("/ordershub");
app.Urls.Add("http://*:80");
app.Run();
