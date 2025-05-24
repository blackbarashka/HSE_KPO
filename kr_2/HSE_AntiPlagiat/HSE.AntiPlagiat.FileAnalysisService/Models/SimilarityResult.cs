/// <summary>
/// Результат сравнения файлов, содержащий информацию о проценте схожести и идентификаторах файлов.
/// </summary>
public class SimilarityResult
{
    public Guid Id { get; set; }
    public Guid OriginalFileId { get; set; }
    public Guid ComparedFileId { get; set; }
    public double SimilarityPercentage { get; set; }
    public DateTime ComparisonDate { get; set; }
}