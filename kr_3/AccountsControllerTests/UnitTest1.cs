using System;
using System.Threading.Tasks;
using Common.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentsService.Controllers;
using PaymentsService.Services;
using Xunit;

namespace PaymentsService.Tests.Controllers
{
    /// <summary>
    /// Тесты для контроллера счетов пользователей
    /// </summary>
    public class AccountsControllerTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<ILogger<AccountsController>> _loggerMock;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _loggerMock = new Mock<ILogger<AccountsController>>();
            _controller = new AccountsController(_accountServiceMock.Object, _loggerMock.Object);
        }
        /// <summary>
        /// Тест для успешного создания счета
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateAccount_ShouldReturnOk_WhenAccountCreated()
        {
            // Arrange
            var request = new CreateAccountRequest { UserId = "test-user" };
            var expectedAccount = new Account
            {
                UserId = request.UserId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _accountServiceMock.Setup(x => x.CreateAccountAsync(request.UserId)).ReturnsAsync(expectedAccount);

            // Act
            var result = await _controller.CreateAccount(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedAccount);
        }
        /// <summary>
        /// Тест для обработки ошибки при создании счета, если счет уже существует
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateAccount_ShouldReturnBadRequest_WhenAccountAlreadyExists()
        {
            // Arrange
            var request = new CreateAccountRequest { UserId = "existing-user" };
            _accountServiceMock.Setup(x => x.CreateAccountAsync(request.UserId))
                .ThrowsAsync(new InvalidOperationException("Account already exists"));

            // Act
            var result = await _controller.CreateAccount(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
        /// <summary>
        /// Тест для успешного пополнения счета
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TopUp_ShouldReturnOk_WhenAccountTopUpSuccessful()
        {
            // Arrange
            var request = new TopUpAccountRequest { UserId = "test-user", Amount = 100 };
            var updatedAccount = new Account
            {
                UserId = request.UserId,
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _accountServiceMock.Setup(x => x.TopUpAccountAsync(request.UserId, request.Amount))
                .ReturnsAsync(updatedAccount);

            // Act
            var result = await _controller.TopUp(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(updatedAccount);
        }
        /// <summary>
        /// Тест для обработки ошибки при пополнении счета, если счет не существует
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetBalance_ShouldReturnOk_WhenAccountExists()
        {
            // Arrange
            var userId = "test-user";
            var account = new Account
            {
                UserId = userId,
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _accountServiceMock.Setup(x => x.GetAccountAsync(userId)).ReturnsAsync(account);

            // Act
            var result = await _controller.GetBalance(userId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(account);
        }
        /// <summary>
        /// Тест для обработки ошибки при получении баланса, если счет не существует
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetBalance_ShouldReturnNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            var userId = "non-existent-user";
            _accountServiceMock.Setup(x => x.GetAccountAsync(userId)).ReturnsAsync((Account)null);

            // Act
            var result = await _controller.GetBalance(userId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
