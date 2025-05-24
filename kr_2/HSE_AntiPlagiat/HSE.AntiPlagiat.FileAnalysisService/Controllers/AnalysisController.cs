// Controllers/AnalysisController.cs
using Microsoft.AspNetCore.Mvc;
/// <summary>
/// Контроллер для анализа текстов и проверки на плагиат.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly ITextAnalysisService _analysisService;
    private readonly ISimilarityService _similarityService;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    /// <summary>
    /// Инициализирует новый экземпляр контроллера для анализа текстов и проверки на плагиат.
    /// </summary>
    /// <param name="analysisService"></param>
    /// <param name="similarityService"></param>
    /// <param name="httpClient"></param>
    /// <param name="configuration"></param>
    public AnalysisController(
        ITextAnalysisService analysisService,
        ISimilarityService similarityService,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _analysisService = analysisService;
        _similarityService = similarityService;
        _httpClient = httpClient;
        _configuration = configuration;
    }
    /// <summary>
    /// Запускает анализ файла по его идентификатору. Если файл уже был проанализирован, возвращает существующий результат.
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [HttpPost("{fileId}")]
    public async Task<IActionResult> AnalyzeFile(Guid fileId)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Проверяем, существует ли уже анализ
            var existingResult = await _analysisService.GetAnalysisResultAsync(fileId);
            if (existingResult != null)
            {
                return Ok(existingResult);
            }

            // Получаем файл из сервиса хранения
            var fileStorageServiceUrl = _configuration["Services:FileStorageService"];
            var response = await _httpClient.GetAsync($"{fileStorageServiceUrl}/api/files/{fileId}");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error retrieving file from storage service");
            }

            var content = await response.Content.ReadAsStringAsync();

            // Остальной код метода
            Console.WriteLine($"Analysis stage 1 took {sw.ElapsedMilliseconds}ms");
            // Анализируем текст
            var analysisResult = await _analysisService.AnalyzeTextAsync(fileId, content);
            Console.WriteLine($"Analysis stage 2 took {sw.ElapsedMilliseconds}ms");
          

            var similarFiles = await _similarityService.FindSimilarFilesAsync(fileId, content);
            Console.WriteLine($"Similarity check took {sw.ElapsedMilliseconds}ms");

            // Возвращаем результат анализа вместе со списком похожих файлов
            var result = new
            {
                Analysis = analysisResult,
                PlagiarismResults = similarFiles
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error after {sw.ElapsedMilliseconds}ms: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
    /// <summary>
    /// Получает результаты анализа текста по идентификатору файла. Если анализ еще не выполнен, возвращает 404 Not Found.
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetAnalysisResult(Guid fileId)
    {
        var analysisResult = await _analysisService.GetAnalysisResultAsync(fileId);
        if (analysisResult == null) return NotFound();

        return Ok(analysisResult);
    }


    /// <summary>
    /// Сравнивает два файла по их содержимому и возвращает результат сравнения.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("compare")]
    public async Task<IActionResult> CompareFiles(CompareFilesRequest request)
    {
        try
        {
            if (request.OriginalFileId == request.ComparedFileId)
            {
                return BadRequest("Cannot compare a file with itself");
            }

            var fileStorageServiceUrl = _configuration["Services:FileStorageService"];

            // Получаем оригинальный файл
            var originalResponse = await _httpClient.GetAsync($"{fileStorageServiceUrl}/api/files/{request.OriginalFileId}");
            if (!originalResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)originalResponse.StatusCode, "Error retrieving original file");
            }
            var originalContent = await originalResponse.Content.ReadAsStringAsync();

            // Получаем файл для сравнения
            var comparedResponse = await _httpClient.GetAsync($"{fileStorageServiceUrl}/api/files/{request.ComparedFileId}");
            if (!comparedResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)comparedResponse.StatusCode, "Error retrieving compared file");
            }
            var comparedContent = await comparedResponse.Content.ReadAsStringAsync();

            // Сравниваем файлы
            var result = await _similarityService.CompareTwoFilesAsync(
                request.OriginalFileId, request.ComparedFileId, originalContent, comparedContent);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
/// <summary>
/// Запрос для сравнения двух файлов.
/// </summary>
public class CompareFilesRequest
{
    public Guid OriginalFileId { get; set; }
    public Guid ComparedFileId { get; set; }
}
