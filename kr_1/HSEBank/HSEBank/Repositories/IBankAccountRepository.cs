using System.Collections.Generic;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Репозиторий банковских счетов.
    /// </summary>
    public interface IBankAccountRepository
    {
        void Add(BankAccount account);
        void Update(BankAccount account);
        void Delete(int id);
        BankAccount GetById(int id);
        IEnumerable<BankAccount> GetAll();
    }
}
