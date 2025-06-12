using Common.Models;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Services;

namespace OrdersService.Controllers
{
    /// <summary>
    /// Контроллер для управления заказами в интернет-магазине
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        /// <summary>
        /// Конструктор контроллера заказов
        /// </summary>
        /// <param name="orderService"></param>
        /// <param name="logger"></param>
        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }
        /// <summary>
        /// Создание нового заказа
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            try
            {
                _logger.LogInformation($"Попытка создать заказ: UserId={request.UserId}, Amount={request.Amount}");

                var order = await _orderService.CreateOrderAsync(request);

                // Преобразуем в анонимный объект для контроля над форматом
                var result = new
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Amount = order.Amount,
                    Description = order.Description,
                    Status = order.Status.ToString(),
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                };

                _logger.LogInformation($"Заказ успешно создан: Id={order.Id}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при создании заказа: {ex.Message}");
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }


        /// <summary>
        /// Получение списка заказов пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            try
            {
                var orders = await _orderService.GetUserOrdersAsync(userId);
                _logger.LogInformation($"Найдено {orders.Count()} заказов для пользователя {userId}");
                return Ok(orders.Select(o => new
                {
                    o.Id,
                    o.UserId,
                    o.Amount,
                    o.Description,
                    Status = o.Status.ToString(),
                    o.CreatedAt,
                    o.UpdatedAt
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении заказов пользователя {userId}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получение заказа по ID
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderAsync(orderId);
                if (order == null)
                {
                    return NotFound("Order not found");
                }
                var result = new
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    Amount = order.Amount,
                    Description = order.Description,
                    Status = order.Status.ToString(),
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка при получении заказа {orderId}: {ex.Message}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

    }
}
