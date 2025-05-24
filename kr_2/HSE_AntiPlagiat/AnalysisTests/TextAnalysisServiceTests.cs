using Xunit;
using HSE.AntiPlagiat.FileAnalysisService;
namespace HSE.AntiPlagiat.FileAnalysisService.Tests.Services
{
    /// <summary>
    /// ����� ��� ������� ������� ������.
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
            string text = "����� ����.\n\n����� ���.";

            // Act
            var result = _service.AnalyzeText(text);

            // Assert
            Assert.Equal(2, result.Paragraphs);
        }
        /// <summary>
        /// ���� ��� ������ AnalyzeText, ������� ��������� ������������ �������� ���� � �������� � ������.
        /// </summary>
        [Fact]
        public void AnalyzeText_ShouldReturnCorrectWordAndCharCount()
        {
            // Arrange
            string text = "�����1 �����2.";

            // Act
            var result = _service.AnalyzeText(text);

            // Assert
            Assert.Equal(2, result.Words);
            Assert.True(result.Characters > 0);
        }
    }
}
