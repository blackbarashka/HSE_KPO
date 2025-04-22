using System.Collections.Generic;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Прокси-репозиторий для кэширования счетов.
    /// </summary>
    public class BankAccountRepositoryProxy : IBankAccountRepository
    {
        private readonly IBankAccountRepository _repository;
        private readonly Dictionary<int, BankAccount> _cache = new();

        public BankAccountRepositoryProxy(IBankAccountRepository repository)
        {
            _repository = repository;
        }

        public void Add(BankAccount account)
        {
            _repository.Add(account);
            _cache[account.Id] = account;
        }

        public void Update(BankAccount account)
        {
            _repository.Update(account);
            _cache[account.Id] = account;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
            _cache.Remove(id);
        }

        public BankAccount GetById(int id)
        {
            if (_cache.ContainsKey(id))
            {
                return _cache[id];
            }

            var account = _repository.GetById(id);
            if (account != null)
            {
                _cache[id] = account;
            }
            return account;
        }

        public IEnumerable<BankAccount> GetAll()
        {
            // Не кэшируем список всех счетов
            return _repository.GetAll();
        }
    }
}
