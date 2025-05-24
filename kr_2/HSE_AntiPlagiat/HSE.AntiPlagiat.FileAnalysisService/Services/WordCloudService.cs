
using System.Text.RegularExpressions;
/// <summary>
/// Сервис для генерации облаков слов на основе текста.
/// </summary>
public class WordCloudService : IWordCloudService
{
    private readonly HttpClient _httpClient;
    /// <summary>
    /// Инициализирует новый экземпляр сервиса для генерации облаков слов.
    /// </summary>
    /// <param name="httpClient"></param>
    public WordCloudService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    /// <summary>
    /// Генерирует облако слов на основе переданного текста и возвращает URL изображения облака слов.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public async Task<string> GenerateWordCloudAsync(string text)
    {
        try
        {
            // Фильтруем текст, удаляя часто встречающиеся слова
            var filteredWords = FilterCommonWords(text);

            // Формируем запрос к API облака слов
            var requestData = new
            {
                format = "png",
                width = 800,
                height = 400,
                fontScale = 15,
                text = filteredWords
            };

            var response = await _httpClient.PostAsJsonAsync("https://quickchart.io/wordcloud", requestData);

            if (!response.IsSuccessStatusCode)
                return null;

            // Получаем URL изображения из ответа
            var result = await response.Content.ReadFromJsonAsync<WordCloudResponse>();
            return result?.Url;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Фильтрует текст, удаляя часто встречающиеся слова и короткие слова.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private string FilterCommonWords(string text)
    {
        

        var commonWords = new HashSet<string> { "а", "в", "и", "на", "с", "по", "для", "что", "к", "у", "за", "из", "под", "над", "от", "о", "при", "через" };

        var words = System.Text.RegularExpressions.Regex.Matches(text.ToLower(), @"\b[\w\d]+\b")
            .Cast<Match>()
            .Select(m => m.Value)
            .Where(word => !commonWords.Contains(word) && word.Length > 2)
            .ToList();

        return string.Join(" ", words);
    }
    /// <summary>
    /// Класс для представления ответа API облака слов. 
    /// </summary>
    private class WordCloudResponse
    {
        public string Url { get; set; }
    }
}
