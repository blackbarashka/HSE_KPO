using Common.Models;

namespace OrdersService.Services
{
    /// <summary>
    /// Интерфейс сервиса для работы с заказами.
    /// </summary>
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderRequest request);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order> GetOrderAsync(string orderId);
        Task UpdateOrderStatusAsync(string orderId, bool paymentSuccess, string failureReason);
    }
}
