// Messages/PaymentMessages.cs
namespace Common.Messages
{
    /// <summary>
    /// Класс для представления команды на обработку платежа.
    /// </summary>
    public class ProcessPaymentCommand
    {
        /// <summary>
        /// Идентификатор заказа, для которого требуется обработать платеж.
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// Идентификатор пользователя, инициировавшего платеж.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Сумма платежа, которую необходимо обработать.
        /// </summary>
        public decimal Amount { get; set; }
    }
    /// <summary>
    /// Класс для представления события об успешной обработке платежа.
    /// </summary>
    public class PaymentProcessedEvent
    {
        public string OrderId { get; set; }
        public bool IsSuccess { get; set; }
        public string FailureReason { get; set; }
    }
}
