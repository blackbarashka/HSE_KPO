using ApiGateway.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway для интернет-магазина"
    });

    // Security схема для заголовка X-User-Id
    c.AddSecurityDefinition("UserId", new OpenApiSecurityScheme
    {
        Description = "ID пользователя для тестирования",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-User-Id",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "UserId" }
            },
            new string[] {}
        }
    });

    // Описания API для Orders
    c.AddOperation("/api/orders", OperationType.Post, new OpenApiOperation
    {
        Summary = "Создать новый заказ",
        Description = "Создает новый заказ для пользователя",
        OperationId = "createOrder",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Orders" } },
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string> { "userId", "amount", "description" },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["userId"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("user-123") },
                            ["amount"] = new OpenApiSchema { Type = "number", Format = "decimal", Example = new OpenApiDouble(100) },
                            ["description"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("Тестовый заказ") }
                        }
                    }
                }
            }
        },
        Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Заказ успешно создан" },
            ["400"] = new OpenApiResponse { Description = "Некорректные данные запроса" },
            ["401"] = new OpenApiResponse { Description = "Отсутствует заголовок X-User-Id" }
        }
    });

    // Описания API для получения списка заказов пользователя.
    // GET /api/orders/user/{userId}
    c.AddOperation("/api/orders/user/{userId}", OperationType.Get, new OpenApiOperation
    {
        Summary = "Получить список заказов пользователя",
        Description = "Возвращает список всех заказов указанного пользователя",
        OperationId = "getUserOrders",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Orders" } },
        Parameters = new List<OpenApiParameter>
        {
            new OpenApiParameter
            {
                Name = "userId",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" },
                Example = new OpenApiString("user-123")
            }
        },
        Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Список заказов" },
            ["401"] = new OpenApiResponse { Description = "Отсутствует заголовок X-User-Id" }
        }
    });
    // Описания API для получения информации о заказе по ID.
    // GET /api/orders/{orderId}
    c.AddOperation("/api/orders/{orderId}", OperationType.Get, new OpenApiOperation
    {
        Summary = "Получить информацию о заказе",
        Description = "Возвращает детальную информацию о заказе по его ID",
        OperationId = "getOrder",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Orders" } },
        Parameters = new List<OpenApiParameter>
        {
            new OpenApiParameter
            {
                Name = "orderId",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            }
        },
        Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Информация о заказе" },
            ["404"] = new OpenApiResponse { Description = "Заказ не найден" },
            ["401"] = new OpenApiResponse { Description = "Отсутствует заголовок X-User-Id" }
        }
    });

    // Описания API для Accounts.
    // POST /api/accounts
    c.AddOperation("/api/accounts", OperationType.Post, new OpenApiOperation
    {
        Summary = "Создать новый счет",
        Description = "Создает новый счет для пользователя",
        OperationId = "createAccount",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Accounts" } },
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string> { "userId" },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["userId"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("user-123") }
                        }
                    }
                }
            }
        },
        Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Счет успешно создан" },
            ["400"] = new OpenApiResponse { Description = "Счет для пользователя уже существует" },
            ["401"] = new OpenApiResponse { Description = "Отсутствует заголовок X-User-Id" }
        }
    });

    // Оperation для получения списка счетов пользователя.
    // PUT /api/accounts/topup
    c.AddOperation("/api/accounts/topup", OperationType.Put, new OpenApiOperation
    {
        Summary = "Пополнить счет",
        Description = "Пополняет счет пользователя на указанную сумму",
        OperationId = "topUpAccount",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Accounts" } },
        RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Required = new HashSet<string> { "userId", "amount" },
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["userId"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("user-123") },
                            ["amount"] = new OpenApiSchema { Type = "number", Format = "decimal", Example = new OpenApiDouble(100) }
                        }
                    }
                }
            }
        },
        Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Счет успешно пополнен" },
            ["400"] = new OpenApiResponse { Description = "Ошибка при пополнении счета" },
            ["401"] = new OpenApiResponse { Description = "Отсутствует заголовок X-User-Id" }
        }
    });

    
    // GET /api/accounts/{userId}
    c.AddOperation("/api/accounts/{userId}", OperationType.Get, new OpenApiOperation
    {
        Summary = "Получить информацию о счете пользователя",
        Description = "Возвращает информацию о счете указанного пользователя",
        OperationId = "getAccount",
        Tags = new List<OpenApiTag> { new OpenApiTag { Name = "Accounts" } },
        Parameters = new List<OpenApiParameter>
        {
            new OpenApiParameter
            {
                Name = "userId",
                In = ParameterLocation.Path,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" },
                Example = new OpenApiString("user-123")
            }
        },
        Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse { Description = "Информация о счете" },
            ["404"] = new OpenApiResponse { Description = "Счет не найден" },
            ["401"] = new OpenApiResponse { Description = "Отсутствует заголовок X-User-Id" }
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    try
    {
        var commonXmlFile = "Common.xml";
        var commonXmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, commonXmlFile);
        if (System.IO.File.Exists(commonXmlPath))
        {
            c.IncludeXmlComments(commonXmlPath);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Не удалось загрузить XML-комментарии: {ex.Message}");
    }
});

// Добавляем YARP для API Gateway
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transforms =>
    {
        // Добавляем middleware для передачи user ID из заголовка во все запросы
        transforms.AddRequestTransform(async context =>
        {
            if (context.HttpContext.Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                context.ProxyRequest.Headers.Add("X-User-Id", userId.ToString());
            }
        });
    });

// CORS для Frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Настройка конвейера HTTP-запросов
app.UseCors();
app.UseHttpsRedirection();

// Middleware для проверки X-User-Id до UseSwagger
app.Use(async (context, next) =>
{
    // Проверяем, является ли запрос связанным со Swagger
    bool isSwaggerRequest =
        context.Request.Path.StartsWithSegments("/swagger") ||
        context.Request.Path.Value?.Contains("swagger.json") == true ||
        context.Request.Path.Value?.Contains("-swagger") == true;

    // Пропускаем запросы Swagger и WebSocket
    if (isSwaggerRequest || context.Request.Path.StartsWithSegments("/ordershub"))
    {
        await next();
        return;
    }

    // Для всех других запросов требуем X-User-Id
    if (!context.Request.Headers.ContainsKey("X-User-Id"))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { error = "X-User-Id header is required" });
        return;
    }

    await next();
});

// Конфигурация Swagger
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
    });
}

// Маршрутизация
app.UseAuthorization();
app.MapControllers();
app.MapReverseProxy();

// Указание порта
app.Urls.Add("http://*:80");
app.Run();
