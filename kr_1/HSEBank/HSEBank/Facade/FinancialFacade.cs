using Domain;
using Repositories;


namespace Facade
{
    /// <summary>
    /// Фасад для работы с финансовыми данными. Содержит методы для работы со счетами, категориями и операциями.  
    /// </summary>
    public class FinancialFacade
    {
        // Репозитории для работы с данными.
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOperationRepository _operationRepository;
        private int _currentId;

        public FinancialFacade(IBankAccountRepository bankAccountRepository, ICategoryRepository categoryRepository, IOperationRepository operationRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _categoryRepository = categoryRepository;
            _operationRepository = operationRepository;
        }

        /// <summary>
        /// Создание банковского счета.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="balance"></param>
        public void CreateBankAccount(string name, decimal balance)
        {
            var id = GenerateId(); // Метод генерации ID
            var account = Factories.BankAccountFactory.Create(id, name, balance);
            _bankAccountRepository.Add(account);
        }
        /// <summary>
        /// Генерация ID для новых объектов.
        /// </summary>
        /// <returns></returns>
        private int GenerateId()
        {
            _currentId++;
            return _currentId;
        }
        /// <summary>
        /// Создание категории.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public void CreateCategory(CategoryType type, string name)
        {
            var id = GenerateId(); // Метод генерации ID
            var category = Factories.CategoryFactory.Create(id, type, name);
            _categoryRepository.Add(category);
        }
        /// <summary>
        /// Создание операции. 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        /// <param name="accountId"></param>
        /// <param name="des"></param>
        /// <param name="categoryId"></param>
        public void CreateOperation(DateTime date, OperationType type, decimal amount, int accountId, string des, int categoryId)
        {
            var id = GenerateId(); // Метод генерации ID
            var operation = Factories.OperationFactory.Create(id, type, accountId, amount, date,"", categoryId);
            _operationRepository.Add(operation);
        }



        /// <summary>
        /// Получение общего дохода за период. 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public decimal GetTotalIncomeForPeriod(DateTime startDate, DateTime endDate)
        {
            var incomes = _operationRepository.GetAll()
                .Where(o => o.Type == OperationType.Income && o.Date >= startDate && o.Date <= endDate);
            return incomes.Sum(o => o.Amount);
        }

        /// <summary>
        /// Получение общего расхода за период.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public decimal GetTotalExpenseForPeriod(DateTime startDate, DateTime endDate)
        {
            var expenses = _operationRepository.GetAll()
                .Where(o => o.Type == OperationType.Expense && o.Date >= startDate && o.Date <= endDate);
            return expenses.Sum(o => o.Amount);
        }

        /// <summary>
        /// Получение операций, сгруппированных по категориям.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Category, List<Operation>> GetOperationsGroupedByCategory()
        {
            var operations = _operationRepository.GetAll();
            var categories = _categoryRepository.GetAll().ToDictionary(c => c.Id, c => c);

            var grouped = operations.GroupBy(o => o.CategoryId)
                .ToDictionary(
                    g => categories.ContainsKey(g.Key) ? categories[g.Key] : new Category(0, CategoryType.Expense, "Неизвестная категория"),
                    g => g.ToList()
                );

            return grouped;
        }

        /// <summary>
        /// Пересчет балансов счетов.
        /// </summary>
        public void RecalculateBalances()
        {
            var accounts = _bankAccountRepository.GetAll();
            foreach (var account in accounts)
            {
                var operations = _operationRepository.GetAll()
                    .Where(o => o.BankAccountId == account.Id);

                decimal newBalance = operations.Sum(o =>
                    o.Type == OperationType.Income ? o.Amount : -o.Amount
                );

                account.UpdateBalance(newBalance - account.Balance);
                _bankAccountRepository.Update(account);
            }
        }

        /// <summary>
        /// Получение всех банковских счетов.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BankAccount> GetBankAccounts()
        {
            return _bankAccountRepository.GetAll();
        }
        /// <summary>
        /// Получение всех категорий.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Category> GetCategories()
        {
            return _categoryRepository.GetAll();
        }
        /// <summary>
        /// Получение всех операций.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Operation> GetOperations()
        {
            return _operationRepository.GetAll();
        }
        /// <summary>
        /// Добавление банковского счета.   
        /// </summary>
        /// <param name="account"></param>
        public void AddBankAccount(BankAccount account)
        {
            _bankAccountRepository.Add(account);
        }
        /// <summary>
        /// Добавление категории.
        /// </summary>
        /// <param name="category"></param>
        public void AddCategory(Category category)
        {
            _categoryRepository.Add(category);
        }
        /// <summary>
        /// Добавление операции.
        /// </summary>
        /// <param name="operation"></param>
        public void AddOperation(Operation operation)
        {
            _operationRepository.Add(operation);
        }

        //Метод для удаления операции.
        public void DeleteOperation(int id)
        {
            _operationRepository.Delete(id);
        }
        //Метод для удаления категории.
        public void DeleteCategory(int id)
        {
            _categoryRepository.Delete(id);
        }
        //Метод для удаления счета.
        public void DeleteBankAccount(int id)
        {
            _bankAccountRepository.Delete(id);
        }
        //Метод для редактирования операции.
        public void UpdateOperation(Operation operation)
        {
            _operationRepository.Update(operation);
        }
        //Метод для редактирования категории.
        public void UpdateCategory(Category category)
        {
            _categoryRepository.Update(category);
        }
        //Метод для редактирования счета.
        public void UpdateBankAccount(BankAccount account)
        {
            _bankAccountRepository.Update(account);
        }
    }
}

