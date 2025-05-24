/// <summary>
/// Сервис для анализа текста и генерации результатов анализа.
/// </summary>
public interface ITextAnalysisService
{
    Task<TextAnalysisResult> AnalyzeTextAsync(Guid fileId, string content);
    Task<TextAnalysisResult> GetAnalysisResultAsync(Guid fileId);
}
