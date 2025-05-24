// Program.cs
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
// Добавляем сервисы в контейнер зависимостей.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/keys")) // общий путь к ключам
    .SetApplicationName("AntiPlagiatWeb");

var app = builder.Build();
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
// Добавляем поддержку HTTP.
app.Urls.Add("http://*:80");

app.Run();
