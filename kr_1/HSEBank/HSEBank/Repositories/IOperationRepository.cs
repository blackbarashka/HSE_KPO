using System;
using System.Collections.Generic;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Репозиторий операций.
    /// </summary>
    public interface IOperationRepository
    {
        IEnumerable<Operation> GetAll();
        Operation GetById(int id);
        void Add(Operation operation);
        void Update(Operation operation);
        void Delete(int id);

        // Дополнительный метод для получения операций за период
        IEnumerable<Operation> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}
