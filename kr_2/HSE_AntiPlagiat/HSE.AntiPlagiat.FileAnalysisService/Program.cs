// Program.cs
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Добавляем сервисы в контейнер зависимостей.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


// Регистрация сервисов
builder.Services.AddScoped<ITextAnalysisService, TextAnalysisService>();
builder.Services.AddScoped<ISimilarityService, SimilarityService>();
builder.Services.AddScoped<IWordCloudService, WordCloudService>();
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Urls.Add("http://*:80");

app.Run();
