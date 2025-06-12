using Common.Messages;
using Common.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Data.Entities;
using OrdersService.Hubs;

using System.Text.Json;

namespace OrdersService.Services
{
    /// <summary>
    /// Сервис для управления заказами.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly OrdersDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;
        private readonly IHubContext<OrdersHub, IOrdersClient> _ordersHub;
        /// <summary>
        /// Конструктор сервиса для управления заказами.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="logger"></param>
        /// <param name="ordersHub"></param>
        public OrderService(
            OrdersDbContext dbContext,
            ILogger<OrderService> logger,
            IHubContext<OrdersHub, IOrdersClient> ordersHub)
        {
            _dbContext = dbContext;
            _logger = logger;
            _ordersHub = ordersHub;
        }
        /// <summary>
        /// Создает новый заказ и инициирует процесс оплаты через Outbox.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new InvalidOperationException("Order amount must be positive");
            }

            var orderId = Guid.NewGuid().ToString();
            var now = DateTime.UtcNow;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Создаем заказ
                var orderEntity = new OrderEntity
                {
                    Id = orderId,
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Description = request.Description,
                    Status = OrderStatus.New,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _dbContext.Orders.Add(orderEntity);

                // Создаем сообщение в outbox для запуска оплаты
                var paymentCommand = new ProcessPaymentCommand
                {
                    OrderId = orderId,
                    UserId = request.UserId,
                    Amount = request.Amount
                };

                _dbContext.OutboxMessages.Add(new OutboxMessage
                {
                    MessageId = $"order-{orderId}",
                    MessageType = nameof(ProcessPaymentCommand),
                    Content = JsonSerializer.Serialize(paymentCommand),
                    CreatedAt = now
                });

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // Возвращаем созданный заказ
                return new Order
                {
                    Id = orderEntity.Id,
                    UserId = orderEntity.UserId,
                    Amount = orderEntity.Amount,
                    Description = orderEntity.Description,
                    Status = orderEntity.Status,
                    CreatedAt = orderEntity.CreatedAt,
                    UpdatedAt = orderEntity.UpdatedAt
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        /// <summary>
        /// Получает список заказов пользователя, отсортированных по дате создания.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            var orderEntities = await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orderEntities.Select(o => new Order
            {
                Id = o.Id,
                UserId = o.UserId,
                Amount = o.Amount,
                Description = o.Description,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            });
        }
        /// <summary>
        /// Получает заказ по идентификатору.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Order> GetOrderAsync(string orderId)
        {
            var orderEntity = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (orderEntity == null)
            {
                return null;
            }

            return new Order
            {
                Id = orderEntity.Id,
                UserId = orderEntity.UserId,
                Amount = orderEntity.Amount,
                Description = orderEntity.Description,
                Status = orderEntity.Status,
                CreatedAt = orderEntity.CreatedAt,
                UpdatedAt = orderEntity.UpdatedAt
            };
        }
        /// <summary>
        /// Обновляет статус заказа после обработки платежа.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="paymentSuccess"></param>
        /// <param name="failureReason"></param>
        /// <returns></returns>
        public async Task UpdateOrderStatusAsync(string orderId, bool paymentSuccess, string failureReason)
        {
            // Реализация идемпотентного потребителя
            var existingInbox = await _dbContext.InboxMessages
                .FirstOrDefaultAsync(m => m.MessageId == $"payment-{orderId}");

            if (existingInbox?.ProcessedAt != null)
            {
                // Уже обработали это сообщение
                return;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var order = await _dbContext.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    _logger.LogWarning($"Order {orderId} not found for status update");
                    return;
                }

                // Обновляем статус заказа
                order.Status = paymentSuccess ? OrderStatus.Finished : OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                // Сохраняем идемпотентный inbox
                if (existingInbox == null)
                {
                    _dbContext.InboxMessages.Add(new InboxMessage
                    {
                        MessageId = $"payment-{orderId}",
                        MessageType = "PaymentProcessed",
                        Content = JsonSerializer.Serialize(new { OrderId = orderId, Success = paymentSuccess }),
                        ReceivedAt = DateTime.UtcNow,
                        ProcessedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    existingInbox.ProcessedAt = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                await _ordersHub.Clients.Group(order.UserId).OrderStatusChanged(new Order
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Amount = order.Amount,
                    Description = order.Description,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {orderId} status");
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
