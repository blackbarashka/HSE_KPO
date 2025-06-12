using System;
using System.Threading.Tasks;
using Common.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentsService.Data;
using PaymentsService.Data.Entities;
using PaymentsService.Services;
using Xunit;

namespace PaymentsService.Tests.Services
{
    /// <summary>
    /// Тесты для сервиса управления счетами пользователей
    /// </summary>
    public class AccountServiceTests
    {
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly DbContextOptions<PaymentsDbContext> _dbContextOptions;

        public AccountServiceTests()
        {
            _loggerMock = new Mock<ILogger<AccountService>>();
            _dbContextOptions = new DbContextOptionsBuilder<PaymentsDbContext>()
                .UseInMemoryDatabase(databaseName: $"PaymentsDb_{Guid.NewGuid()}")
                .Options;
        }
        /// <summary>
        /// Тест для успешного создания счета
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateAccountAsync_ShouldCreateAccount_WhenUserHasNoAccount()
        {
            // Arrange
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = new AccountService(dbContext, _loggerMock.Object);
            var userId = "test-user";

            // Act
            var result = await accountService.CreateAccountAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Balance.Should().Be(0);

            // Verify account was saved to DB
            var savedAccount = await dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            savedAccount.Should().NotBeNull();
        }
        /// <summary>
        /// Тест для обработки ошибки при создании счета, если счет уже существует
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateAccountAsync_ShouldThrow_WhenUserAlreadyHasAccount()
        {
            // Arrange
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var userId = "test-user";
            dbContext.Accounts.Add(new AccountEntity
            {
                UserId = userId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();

            var accountService = new AccountService(dbContext, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => accountService.CreateAccountAsync(userId));
        }

        /// <summary>
        /// Тест для успешного пополнения счета
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TopUpAccountAsync_ShouldThrow_WhenAccountDoesNotExist()
        {
            // Arrange
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = new AccountService(dbContext, _loggerMock.Object);
            var userId = "non-existent-user";
            var amount = 100m;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => accountService.TopUpAccountAsync(userId, amount));
        }
        /// <summary>
        /// Тест для успешного пополнения счета
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TopUpAccountAsync_ShouldThrow_WhenAmountIsNegative()
        {
            // Arrange
            using var dbContext = new PaymentsDbContext(_dbContextOptions);
            var accountService = new AccountService(dbContext, _loggerMock.Object);
            var userId = "test-user";
            var amount = -100m;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => accountService.TopUpAccountAsync(userId, amount));
        }
    }
}
