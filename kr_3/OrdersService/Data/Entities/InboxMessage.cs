namespace OrdersService.Data.Entities
{
    /// <summary>
    /// Представляет сообщение Inbox для получения сообщений из Outbox
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
