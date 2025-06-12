using Common.Models;

namespace OrdersService.Data.Entities
{
    /// <summary>
    /// Представляет сущность заказа в базе данных
    /// </summary>
    public class OrderEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
