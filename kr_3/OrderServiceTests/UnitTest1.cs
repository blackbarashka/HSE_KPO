using Common.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrdersService.Data;
using OrdersService.Data.Entities;
using OrdersService.Hubs;
using OrdersService.Services;
using FluentAssertions;

namespace OrdersService.Tests.Services
{
    /// <summary>
    /// Тесты для сервиса управления заказами.
    /// </summary>
    public class OrderServiceTests
    {
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly Mock<IHubContext<OrdersHub, IOrdersClient>> _ordersHubMock;
        private readonly DbContextOptions<OrdersDbContext> _dbContextOptions;
        /// <summary>
        /// Конструктор для инициализации тестов сервиса управления заказами.
        /// </summary>
        public OrderServiceTests()
        {
            _loggerMock = new Mock<ILogger<OrderService>>();
            _ordersHubMock = new Mock<IHubContext<OrdersHub, IOrdersClient>>();
            _dbContextOptions = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(databaseName: $"OrdersDb_{Guid.NewGuid()}")
                .Options;

            // Настройка мока для IOrdersClient
            var ordersClientMock = new Mock<IOrdersClient>();
            ordersClientMock.Setup(x => x.OrderStatusChanged(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            // Настройка мока для IHubClients<IOrdersClient>
            var hubClientsMock = new Mock<IHubClients<IOrdersClient>>();
            hubClientsMock.Setup(x => x.User(It.IsAny<string>()))
                .Returns(ordersClientMock.Object);
            hubClientsMock.Setup(x => x.All)
                .Returns(ordersClientMock.Object);

            // Настройка мока для IHubContext<OrdersHub, IOrdersClient>
            _ordersHubMock.Setup(x => x.Clients)
                .Returns(hubClientsMock.Object);
        }

        /// <summary>
        /// Тест для успешного создания заказа.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOrderAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var orderEntity = new OrderEntity
            {
                Id = "test-order-id",
                UserId = "test-user",
                Amount = 100,
                Description = "Test Order",
                Status = OrderStatus.New,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            dbContext.Orders.Add(orderEntity);
            await dbContext.SaveChangesAsync();

            var orderService = new OrderService(dbContext, _loggerMock.Object, _ordersHubMock.Object);

            // Act
            var result = await orderService.GetOrderAsync("test-order-id");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(orderEntity.Id);
            result.UserId.Should().Be(orderEntity.UserId);
        }
        /// <summary>
        /// Тест для получения заказа, который не существует.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserOrdersAsync_ShouldReturnUserOrders()
        {
            // Arrange
            using var dbContext = new OrdersDbContext(_dbContextOptions);
            var userId = "test-user";
            var orders = new[]
            {
                new OrderEntity
                {
                    Id = "order-1",
                    UserId = userId,
                    Amount = 100,
                    Description = "Test Order 1",
                    Status = OrderStatus.New,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OrderEntity
                {
                    Id = "order-2",
                    UserId = userId,
                    Amount = 200,
                    Description = "Test Order 2",
                    Status = OrderStatus.Processing,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new OrderEntity
                {
                    Id = "order-3",
                    UserId = "another-user",
                    Amount = 300,
                    Description = "Test Order 3",
                    Status = OrderStatus.New,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            dbContext.Orders.AddRange(orders);
            await dbContext.SaveChangesAsync();

            var orderService = new OrderService(dbContext, _loggerMock.Object, _ordersHubMock.Object);

            // Act
            var result = await orderService.GetUserOrdersAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Select(o => o.Id).Should().Contain(new[] { "order-1", "order-2" });
            result.Select(o => o.UserId).Should().AllBe(userId);
        }

        
    }
}
