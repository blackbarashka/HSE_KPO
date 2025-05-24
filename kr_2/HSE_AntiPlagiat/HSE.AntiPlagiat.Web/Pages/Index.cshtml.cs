// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty]
    public IFormFile Upload { get; set; }

    public List<FileViewModel> Files { get; set; } = new List<FileViewModel>();

    public IndexModel(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<IndexModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        await LoadFilesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Upload == null || Upload.Length == 0)
        {
            ModelState.AddModelError("Upload", "Пожалуйста, выберите файл");
            return Page();
        }

        if (!Upload.FileName.EndsWith(".txt"))
        {
            ModelState.AddModelError("Upload", "Поддерживаются только файлы .txt");
            return Page();
        }

        var apiGatewayUrl = _configuration["ApiGateway:BaseUrl"];
        var client = _httpClientFactory.CreateClient();

        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = Upload.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", Upload.FileName);

            var response = await client.PostAsync($"{apiGatewayUrl}/api/files", content);
            response.EnsureSuccessStatusCode();

            var fileInfo = await response.Content.ReadFromJsonAsync<FileViewModel>();

            // Запускаем анализ файла
            await client.PostAsync($"{apiGatewayUrl}/api/analysis/{fileInfo.Id}", null);

            return RedirectToPage("./FileDetails", new { id = fileInfo.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            ModelState.AddModelError("Upload", $"Ошибка при загрузке файла: {ex.Message}");
            await LoadFilesAsync();
            return Page();
        }
    }

    private async Task LoadFilesAsync()
    {
        var apiGatewayUrl = _configuration["ApiGateway:BaseUrl"];
        var client = _httpClientFactory.CreateClient();

        try
        {
            var response = await client.GetAsync($"{apiGatewayUrl}/api/files");
            if (response.IsSuccessStatusCode)
            {
                Files = await response.Content.ReadFromJsonAsync<List<FileViewModel>>() ?? new List<FileViewModel>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading files");
        }
    }

    public class FileViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public long Size { get; set; }
    }
}
