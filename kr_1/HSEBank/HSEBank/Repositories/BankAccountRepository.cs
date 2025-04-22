using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Репозиторий банковских счетов.
    /// </summary>
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly List<BankAccount> _bankAccounts;
        private int _currentId;

        public BankAccountRepository()
        {
            _bankAccounts = new List<BankAccount>();
            _currentId = 0; // Инициализируем ID с нуля
        }

        private int GenerateId()
        {
            _currentId++;
            return _currentId;
        }

        public IEnumerable<BankAccount> GetAll()
        {
            return _bankAccounts;
        }

        public BankAccount GetById(int id)
        {
            return _bankAccounts.FirstOrDefault(b => b.Id == id);
        }
        /// <summary>
        /// Добавление нового банковского счета.
        /// </summary>
        /// <param name="bankAccount"></param>
        public void Add(BankAccount bankAccount)
        {
            bankAccount.Id = GenerateId(); // Генерируем уникальный ID
            _bankAccounts.Add(bankAccount);
        }
        /// <summary>
        /// Обновление информации о банковском счете.
        /// </summary>
        /// <param name="bankAccount"></param>
        public void Update(BankAccount bankAccount)
        {
            var existingAccount = GetById(bankAccount.Id);
            if (existingAccount != null)
            {
                existingAccount.Name = bankAccount.Name;
                existingAccount.Balance = bankAccount.Balance;
            }
        }
        /// <summary>
        /// Удаление банковского счета.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var bankAccount = GetById(id);
            if (bankAccount != null)
            {
                _bankAccounts.Remove(bankAccount);
            }
        }
    }
}
