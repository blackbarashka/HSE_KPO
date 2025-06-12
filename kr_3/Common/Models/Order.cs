using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    /// <summary>
    /// Модель заказа в интернет-магазине
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Уникальный идентификатор заказа
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Идентификатор пользователя, который создал заказ
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Сумма заказа    
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Описание заказа
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderStatus Status { get; set; }
        /// <summary>
        /// Дата и время создания заказа    
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Дата и время последнего обновления заказа
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
    /// <summary>
    /// Статусы заказа
    /// </summary>
    public enum OrderStatus
    {
        New,       // Сразу после создания
        Processing, // В обработке
        Finished,  // Оплата успешна
        Cancelled  // Оплата не удалась
    }

    /// <summary>
    /// Запрос на создание нового заказа
    /// </summary>
    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
