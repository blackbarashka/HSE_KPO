using System;
using System.Collections.Generic;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Прокси-репозиторий операций.
    /// </summary>
    public class OperationRepositoryProxy : IOperationRepository
    {
        private readonly IOperationRepository _repository;

        public OperationRepositoryProxy(IOperationRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Operation> GetAll()
        {
            // Дополнительная логика (например, логирование) может быть добавлена здесь
            return _repository.GetAll();
        }

        public Operation GetById(int id)
        {
            return _repository.GetById(id);
        }
        /// <summary>
        /// Добавление операции.
        /// </summary>
        /// <param name="operation"></param>
        public void Add(Operation operation)
        {
            _repository.Add(operation);
        }
        /// <summary>
        /// Обновление операции.
        /// </summary>
        /// <param name="operation"></param>
        public void Update(Operation operation)
        {
            _repository.Update(operation);
        }
        /// <summary>
        /// Удаление операции.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            _repository.Delete(id);
        }
        /// <summary>
        /// Получение операций за период.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<Operation> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _repository.GetByDateRange(startDate, endDate);
        }
    }
}
