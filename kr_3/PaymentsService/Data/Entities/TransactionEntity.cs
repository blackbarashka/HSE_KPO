using PaymentsService.Data.Entities;

namespace PaymentsService.Data.Entities
{
    /// <summary>
    /// Перечисление типов транзакций.
    /// </summary>
    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }
    /// <summary>
    /// Сущность транзакции.
    /// </summary>
    public class TransactionEntity
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string ReferenceId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public AccountEntity Account { get; set; }
    }
}
