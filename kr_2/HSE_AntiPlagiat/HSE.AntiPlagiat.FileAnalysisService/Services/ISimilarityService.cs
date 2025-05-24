/// <summary>
/// Сервис для сравнения файлов на основе их содержимого.
/// </summary>
public interface ISimilarityService
{
    Task<SimilarityResult> CompareTwoFilesAsync(Guid originalFileId, Guid comparedFileId, string originalContent, string comparedContent);
    Task<IEnumerable<SimilarityResult>> FindSimilarFilesAsync(Guid fileId, string content, double threshold = 90.0);
}
