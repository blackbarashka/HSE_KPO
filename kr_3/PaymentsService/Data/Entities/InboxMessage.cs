namespace PaymentsService.Data.Entities
{
    /// <summary>
    /// Сущность сообщения Inbox для получения уведомлений
    /// </summary>
    public class InboxMessage
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public string MessageType { get; set; }
        public string Content { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
