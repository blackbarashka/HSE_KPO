namespace PaymentsService.Data.Entities
{
    /// <summary>
    /// Сущность сообщения Outbox для отправки уведомлений
    /// </summary>
    public class OutboxMessage
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public string MessageType { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
