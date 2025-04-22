using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Репозиторий операций.
    /// </summary>
    public class OperationRepository : IOperationRepository
    {
        private readonly List<Operation> _operations;
        private int _currentId;

        public OperationRepository()
        {
            _operations = new List<Operation>();
            _currentId = 0;
        }

        private int GenerateId()
        {
            _currentId++;
            return _currentId;
        }

        public IEnumerable<Operation> GetAll()
        {

            return _operations;
        }
        /// <summary>
        /// Получение операции по ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Operation GetById(int id)
        {
            return _operations.FirstOrDefault(o => o.Id == id);
        }
        /// <summary>
        /// Добавление операции.
        /// </summary>
        /// <param name="operation"></param>
        public void Add(Operation operation)
        {
            operation.Id = GenerateId(); // Устанавливаем новый уникальный ID
            _operations.Add(operation);
        }
        /// <summary>
        /// Обновление операции.
        /// </summary>
        /// <param name="operation"></param>
        public void Update(Operation operation)
        {
            var existingOperation = GetById(operation.Id);
            if (existingOperation != null)
            {
                existingOperation.Type = operation.Type;
                existingOperation.BankAccountId = operation.BankAccountId;
                existingOperation.Amount = operation.Amount;
                existingOperation.Date = operation.Date;
                existingOperation.Description = operation.Description;
                existingOperation.CategoryId = operation.CategoryId;
            }
        }
        /// <summary>
        /// Удаление операции.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var operation = GetById(id);
            if (operation != null)
            {
                _operations.Remove(operation);
            }
        }
        /// <summary>
        /// Получение операций за период.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<Operation> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _operations.Where(o => o.Date.Date >= startDate.Date && o.Date.Date <= endDate.Date);
        }
    }
}
