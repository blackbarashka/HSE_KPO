using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Настройка HttpClient для API
builder.Services.AddHttpClient("ApiGateway", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiGatewayUrl"] ?? "http://api-gateway");
    client.DefaultRequestHeaders.Add("X-User-Id", "user-123"); // Для тестирования
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.Urls.Add("http://*:80");
app.Run();
