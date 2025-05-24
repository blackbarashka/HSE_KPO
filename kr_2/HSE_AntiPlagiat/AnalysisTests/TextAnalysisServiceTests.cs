using Xunit;
using HSE.AntiPlagiat.FileAnalysisService;
namespace HSE.AntiPlagiat.FileAnalysisService.Tests.Services
{
    /// <summary>
    /// Тесты для сервиса анализа текста.
    /// </summary>
    public class TextAnalysisServiceTests
    {
        private readonly TextAnalysisService _service;
        public TextAnalysisServiceTests()
        {
            _service = new TextAnalysisService();
        }

        [Fact]
        public void AnalyzeText_ShouldReturnCorrectParagraphCount()
        {
            // Arrange
            string text = "Абзац один.\n\nАбзац два.";

            // Act
            var result = _service.AnalyzeText(text);

            // Assert
            Assert.Equal(2, result.Paragraphs);
        }
        /// <summary>
        /// Тест для метода AnalyzeText, который проверяет правильность подсчета слов и символов в тексте.
        /// </summary>
        [Fact]
        public void AnalyzeText_ShouldReturnCorrectWordAndCharCount()
        {
            // Arrange
            string text = "Слово1 слово2.";

            // Act
            var result = _service.AnalyzeText(text);

            // Assert
            Assert.Equal(2, result.Words);
            Assert.True(result.Characters > 0);
        }
    }
}
