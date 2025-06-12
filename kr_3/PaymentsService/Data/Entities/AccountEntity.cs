namespace PaymentsService.Data.Entities
{
    /// <summary>
    /// Сущность аккаунта пользователя.
    /// </summary>
    public class AccountEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Навигационное свойство
        public ICollection<TransactionEntity> Transactions { get; set; }
    }
}
