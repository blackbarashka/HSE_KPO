using Domain;

namespace Factories
{
    /// <summary>
    /// Фабрика для создания банковских счетов.
    /// </summary>
    public static class BankAccountFactory
    {
        public static BankAccount Create(int id, string name, decimal balance)
        {
            return new BankAccount(id, name, balance);
        }
    }
}
