using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using HSE.AntiPlagiat.FileAnalysisService;
using System.Reflection;


    public class AnalysisControllerTests
    {
    /// <summary>
    /// Тест для метода AnalyzeFile контроллера AnalysisController, который проверяет, что метод возвращает OkResult при успешном анализе файла.
    /// </summary>
    [Fact]
        public void AnalyzeFile_ReturnsOkResult()
        { // Arrange var textAnalysisMock = new Mock<ITextAnalysisService>(); var simMock = new Mock<ISimilarityService>();
            textAnalysisMock
                .Setup(s => s.AnalyzeText(It.IsAny<string>()))
                .Returns(new Models.TextAnalysisResult { Paragraphs = 2, Words = 5, Characters = 20 });

            var controller = new AnalysisController(textAnalysisMock.Object, simMock.Object);

            // Act
            var result = controller.AnalyzeFile("some-file-id").Result;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
