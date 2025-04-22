namespace Domain
{
    /// <summary>
    /// Банковский счет.
    /// </summary>
    public class BankAccount
    {
        public int Id { get; set; }
        public string Name { get;  set; }
        public decimal Balance { get;  set; }

        internal BankAccount(int id, string name, decimal balance)
        {
            Id = id;
            Name = name;
            Balance = balance;
        }

        public void UpdateName(string name)
        {
            // Валидация имени
            Name = name;
        }

        public void UpdateBalance(decimal amount)
        {
            Balance += amount;
        }
    }
}
