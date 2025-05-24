using Microsoft.EntityFrameworkCore;
/// <summary>
/// Сервис для сравнения файлов на основе их содержимого.
/// </summary>
public class FileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public DateTime UploadDate { get; set; }
}
/// <summary>
/// Сервис для сравнения файлов на основе их содержимого.   
/// </summary>
public class SimilarityService : ISimilarityService
{
    private readonly AnalysisDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    /// <summary>
    /// Инициализирует новый экземпляр сервиса для сравнения файлов.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="httpClient"></param>
    /// <param name="configuration"></param>
    public SimilarityService(AnalysisDbContext dbContext, HttpClient httpClient, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _configuration = configuration;
    }
    /// <summary>
    /// Сравнивает два файла по их содержимому и сохраняет результат сравнения в базе данных.
    /// </summary>
    /// <param name="originalFileId"></param>
    /// <param name="comparedFileId"></param>
    /// <param name="originalContent"></param>
    /// <param name="comparedContent"></param>
    /// <returns></returns>
    public async Task<SimilarityResult> CompareTwoFilesAsync(Guid originalFileId, Guid comparedFileId, string originalContent, string comparedContent)
    {
        var existingResult = await _dbContext.SimilarityResults.FirstOrDefaultAsync(
            r => r.OriginalFileId == originalFileId && r.ComparedFileId == comparedFileId);

        if (existingResult != null) return existingResult;

        var similarity = CalculateSimilarity(originalContent, comparedContent);

        var result = new SimilarityResult
        {
            Id = Guid.NewGuid(),
            OriginalFileId = originalFileId,
            ComparedFileId = comparedFileId,
            SimilarityPercentage = similarity,
            ComparisonDate = DateTime.UtcNow
        };

        _dbContext.SimilarityResults.Add(result);
        await _dbContext.SaveChangesAsync();

        return result;
    }
    /// <summary>
    /// Ищет файлы, схожие с заданным файлом, на основе его содержимого и заданного порога схожести.
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="content"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public async Task<IEnumerable<SimilarityResult>> FindSimilarFilesAsync(Guid fileId, string content, double threshold = 50.0)
    {
        var fileStorageServiceUrl = _configuration["Services:FileStorageService"];
        var results = new List<SimilarityResult>();

        // Получаем список всех файлов
        var response = await _httpClient.GetAsync($"{fileStorageServiceUrl}/api/files");
        if (!response.IsSuccessStatusCode) return results;

        var files = await response.Content.ReadFromJsonAsync<List<FileDto>>();

        foreach (var file in files)
        {
            if (file.Id == fileId) continue;

            // Получаем содержимое файла
            var fileResponse = await _httpClient.GetAsync($"{fileStorageServiceUrl}/api/files/{file.Id}");
            if (!fileResponse.IsSuccessStatusCode) continue;

            var fileContent = await fileResponse.Content.ReadAsStringAsync();

            // Сравниваем файлы
            var similarity = CalculateSimilarity(content, fileContent);

            if (similarity >= threshold)
            {
                results.Add(new SimilarityResult
                {
                    Id = Guid.NewGuid(),
                    OriginalFileId = fileId,
                    ComparedFileId = file.Id,
                    SimilarityPercentage = similarity,
                    ComparisonDate = DateTime.UtcNow
                });
            }
        }

        // Сохраняем результаты в БД
        _dbContext.SimilarityResults.AddRange(results);
        await _dbContext.SaveChangesAsync();

        return results;
    }

    /// <summary>
    /// Вычисляет процент схожести между двумя текстами на основе алгоритма Левенштейна.
    /// </summary>
    /// <param name="text1"></param>
    /// <param name="text2"></param>
    /// <returns></returns>
    private double CalculateSimilarity(string text1, string text2)
    {
        // Используем алгоритм Левенштейна для определения схожести текстов
        int levenshteinDistance = LevenshteinDistance(text1, text2);
        double maxLength = Math.Max(text1.Length, text2.Length);

        return (1.0 - levenshteinDistance / maxLength) * 100.0;
    }
    /// <summary>
    /// Вычисляет расстояние Левенштейна между двумя строками.
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    private int LevenshteinDistance(string s1, string s2)
    {
        var distances = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
        {
            distances[i, 0] = i;
        }

        for (int j = 0; j <= s2.Length; j++)
        {
            distances[0, j] = j;
        }

        for (int i = 1; i <= s1.Length; i++)
        {
            for (int j = 1; j <= s2.Length; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;

                distances[i, j] = Math.Min(
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost);
            }
        }

        return distances[s1.Length, s2.Length];
    }

}
