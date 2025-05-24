using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSE.AntiPlagiat.FileAnalysisService;

namespace HSE.AntiPlagiat.FileAnalysisService.Tests.Services
{
    public class SimilarityServiceTests
    {
        /// <summary>
        /// Тест для метода CompareTwoFilesAsync, который проверяет, что результат сравнения сохраняется в базе данных.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CompareTwoFilesAsync_WhenCalled_SavesResultToDb()
        { // Arrange var mockDbContext = new Mock<AnalysisDbContext>(); // Можно настроить mock-объект DbSet<SimilarityResult> и его поведение, если нужно
            var mockHttpClient = new Mock<System.Net.Http.HttpClient>();
            var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();

            var service = new SimilarityService(
                mockDbContext.Object,
                mockHttpClient.Object,
                mockConfig.Object);

            var originalId = Guid.NewGuid();
            var comparedId = Guid.NewGuid();

            // Act
            var result = await service.CompareTwoFilesAsync(
                originalId,
                comparedId,
                "Some sample text",
                "Some text that is somewhat similar");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(originalId, result.OriginalFileId);
            Assert.Equal(comparedId, result.ComparedFileId);
            Assert.InRange(result.SimilarityPercentage, 0.0, 100.0);
            mockDbContext.Verify(m => m.SimilarityResults.Add(It.IsAny<SimilarityResult>()), Times.Once);
            mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}