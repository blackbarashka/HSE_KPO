/// <summary>
/// Результат анализа текста, содержащий информацию о количестве параграфов, слов и символов,
/// </summary>
public class TextAnalysisResult
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public int ParagraphCount { get; set; }
    public int WordCount { get; set; }
    public int CharacterCount { get; set; }
    public string? WordCloudUrl { get; set; }
    public DateTime AnalysisDate { get; set; }
}
