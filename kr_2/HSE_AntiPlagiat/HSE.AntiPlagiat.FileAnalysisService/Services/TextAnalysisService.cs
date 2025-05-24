
using Microsoft.EntityFrameworkCore;
/// <summary>
/// Сервис для анализа текста и генерации результатов анализа.
/// </summary>
public class TextAnalysisService : ITextAnalysisService
{
    private readonly AnalysisDbContext _dbContext;
    private readonly IWordCloudService _wordCloudService;
    /// <summary>
    /// Инициализирует новый экземпляр сервиса для анализа текста.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="wordCloudService"></param>
    public TextAnalysisService(AnalysisDbContext dbContext, IWordCloudService wordCloudService)
    {
        _dbContext = dbContext;
        _wordCloudService = wordCloudService;
    }
    /// <summary>
    /// Анализирует текст и возвращает результаты анализа, включая количество абзацев, слов, символов и URL облака слов.
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public async Task<TextAnalysisResult> AnalyzeTextAsync(Guid fileId, string content)
    {
        var existingResult = await GetAnalysisResultAsync(fileId);
        if (existingResult != null) return existingResult;

        var paragraphCount = CountParagraphs(content);
        var wordCount = CountWords(content);
        var characterCount = content.Length;

        var wordCloudUrl = await _wordCloudService.GenerateWordCloudAsync(content);

        var result = new TextAnalysisResult
        {
            Id = Guid.NewGuid(),
            FileId = fileId,
            ParagraphCount = paragraphCount,
            WordCount = wordCount,
            CharacterCount = characterCount,
            WordCloudUrl = wordCloudUrl,
            AnalysisDate = DateTime.UtcNow
        };


        _dbContext.AnalysisResults.Add(result);
        await _dbContext.SaveChangesAsync();

        return result;
    }
    /// <summary>
    /// Получает результаты анализа текста по идентификатору файла.
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<TextAnalysisResult> GetAnalysisResultAsync(Guid fileId)
    {
        return await _dbContext.AnalysisResults.FirstOrDefaultAsync(r => r.FileId == fileId);
    }
    /// <summary>
    /// Считает количество абзацев в тексте на основе разделителей абзацев (два или более переноса строки).
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private int CountParagraphs(string text)
    {
        // Разделители абзацев - два или более переноса строки подряд
        var paragraphs = System.Text.RegularExpressions.Regex.Split(text, @"(\r\n|\n){2,}");
        return paragraphs.Where(p => !string.IsNullOrWhiteSpace(p)).Count();
    }
    /// <summary>
    /// Считает количество слов в тексте на основе последовательностей букв и цифр.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private int CountWords(string text)
    {
        // Слово - последовательность букв и цифр
        var words = System.Text.RegularExpressions.Regex.Matches(text, @"\b[\w\d]+\b");
        return words.Count;
    }
}
