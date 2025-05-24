/// <summary>
/// Сервис для генерации облаков слов на основе текста.
/// </summary>
public interface IWordCloudService
{
    Task<string> GenerateWordCloudAsync(string text);
}
