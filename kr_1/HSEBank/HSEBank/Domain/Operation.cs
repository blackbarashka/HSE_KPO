using System;

namespace Domain
{
    public enum OperationType
    {
        Income,
        Expense
    }
    /// <summary>
    /// Класс операция.
    /// </summary>
    public class Operation
    {
        public int Id { get; set; }
        public OperationType Type { get; set; }
        public int BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }

        public Operation(int id, OperationType type, int bankAccountId, decimal amount, DateTime date, string description, int categoryId)
        {
            Id = id;
            Type = type;
            BankAccountId = bankAccountId;
            Amount = amount;
            Date = date;
            Description = description;
            CategoryId = categoryId;
        }
    }
}
