using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrdersService.Controllers;
using OrdersService.Services;
using Xunit;
using FluentAssertions;

namespace OrdersService.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly Mock<ILogger<OrdersController>> _loggerMock;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _loggerMock = new Mock<ILogger<OrdersController>>();
            _controller = new OrdersController(_orderServiceMock.Object, _loggerMock.Object);
        }
        /// <summary>
        /// Тест для успешного создания заказа
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateOrder_ShouldReturnOk_WhenOrderCreated()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                UserId = "test-user",
                Amount = 100,
                Description = "Test Order"
            };

            var expectedOrder = new Order
            {
                Id = "new-order-id",
                UserId = request.UserId,
                Amount = request.Amount,
                Description = request.Description,
                Status = OrderStatus.New,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _orderServiceMock.Setup(x => x.CreateOrderAsync(request)).ReturnsAsync(expectedOrder);

            // Act
            var result = await _controller.CreateOrder(request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedOrder = okResult.Value.Should().BeAssignableTo<object>().Subject;
            okResult.StatusCode.Should().Be(200);
        }
        /// <summary>
        /// Тест для получения заказов пользователя
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserOrders_ShouldReturnOrders()
        {
            // Arrange
            var userId = "test-user";
            var orders = new List<Order>
            {
                new Order
                {
                    Id = "order-1",
                    UserId = userId,
                    Amount = 100,
                    Description = "Test Order 1",
                    Status = OrderStatus.New,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = "order-2",
                    UserId = userId,
                    Amount = 200,
                    Description = "Test Order 2",
                    Status = OrderStatus.Processing,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _orderServiceMock.Setup(x => x.GetUserOrdersAsync(userId)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetUserOrders(userId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
        }
        /// <summary>
        /// Тест для получения заказа по ID
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOrder_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = "test-order";
            var order = new Order
            {
                Id = orderId,
                UserId = "test-user",
                Amount = 100,
                Description = "Test Order",
                Status = OrderStatus.New,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _orderServiceMock.Setup(x => x.GetOrderAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
        }
        /// <summary>
        /// Тест для обработки ошибки при получении заказа, если заказ не существует
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = "non-existent-order";
            _orderServiceMock.Setup(x => x.GetOrderAsync(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _controller.GetOrder(orderId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
