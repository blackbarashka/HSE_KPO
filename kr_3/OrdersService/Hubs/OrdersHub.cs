using Common.Models;
using Microsoft.AspNetCore.SignalR;


namespace OrdersService.Hubs
{
    /// <summary>
    /// Интерфейс для клиента, который будет получать обновления о заказах.
    /// </summary>
    public interface IOrdersClient
    {
        Task OrderStatusChanged(Order order);
    }
    /// <summary>
    /// Хаб SignalR для управления подписками на обновления заказов.
    /// </summary>
    public class OrdersHub : Hub<IOrdersClient>
    {
        private readonly ILogger<OrdersHub> _logger;
        /// <summary>
        /// Конструктор хаба для управления подписками на обновления заказов.
        /// </summary>
        /// <param name="logger"></param>
        public OrdersHub(ILogger<OrdersHub> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Подписка клиента на обновления заказов для конкретного пользователя.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task SubscribeToOrderUpdates(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            _logger.LogInformation($"Client {Context.ConnectionId} subscribed to user {userId} order updates");
        }
        /// <summary>
        /// Отписка клиента от обновлений заказов для конкретного пользователя.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client {Context.ConnectionId} disconnected");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
