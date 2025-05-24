// Pages/FileDetails.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class FileDetailsModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileDetailsModel> _logger;

    public FileViewModel File { get; set; }
    public AnalysisResultViewModel AnalysisResult { get; set; }
    public List<PlagiarismResultViewModel> PlagiarismResults { get; set; } = new List<PlagiarismResultViewModel>();

    public FileDetailsModel(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<FileDetailsModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var apiGatewayUrl = _configuration["ApiGateway:BaseUrl"];
        var client = _httpClientFactory.CreateClient();

        try
        {
            // Получаем информацию о файле
            var fileResponse = await client.GetAsync($"{apiGatewayUrl}/api/files/info/{id}");
            if (!fileResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            File = await fileResponse.Content.ReadFromJsonAsync<FileViewModel>();

            // Получаем результаты анализа
            var analysisResponse = await client.GetAsync($"{apiGatewayUrl}/api/analysis/{id}");
            if (analysisResponse.IsSuccessStatusCode)
            {
                AnalysisResult = await analysisResponse.Content.ReadFromJsonAsync<AnalysisResultViewModel>();

                // Если анализа еще нет, запускаем его
                if (AnalysisResult == null)
                {
                    await client.PostAsync($"{apiGatewayUrl}/api/analysis/{id}", null);
                    TempData["Message"] = "Анализ файла запущен. Обновите страницу через несколько секунд.";
                }
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading file details");
            TempData["ErrorMessage"] = $"Ошибка при загрузке данных: {ex.Message}";
            return RedirectToPage("./Index");
        }
    }

    public class FileViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public long Size { get; set; }
    }

    public class AnalysisResultViewModel
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public string WordCloudUrl { get; set; }
        public DateTime AnalysisDate { get; set; }
        public List<PlagiarismResultViewModel> PlagiarismResults { get; set; }
    }

    public class PlagiarismResultViewModel
    {
        public Guid Id { get; set; }
        public Guid OriginalFileId { get; set; }
        public Guid ComparedFileId { get; set; }
        public double SimilarityPercentage { get; set; }
        public DateTime ComparisonDate { get; set; }
        public string ComparedFileName { get; set; }
    }
}
