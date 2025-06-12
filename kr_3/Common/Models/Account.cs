using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    /// <summary>
    /// Модель счета пользователя в сервисе платежей
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Уникальный идентификатор счета
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Баланс счета пользователя
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Дата и время создания счета пользователя
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Дата и время последнего обновления счета пользователя
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Запрос на создание нового счета пользователя
    /// </summary>
    public class CreateAccountRequest
    {
        /// <summary>
        /// Идентификатор пользователя, для которого создается счет
        /// </summary>
        public string UserId { get; set; }
    }

    /// <summary>
    /// Запрос на пополнение счета пользователя
    /// </summary>
    public class TopUpAccountRequest
    {
        /// <summary>
        /// Идентификатор пользователя, чей счет пополняется
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Сумма пополнения счета
        /// </summary>
        public decimal Amount { get; set; }
    }


}
