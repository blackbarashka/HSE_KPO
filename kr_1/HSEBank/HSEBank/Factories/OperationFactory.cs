using System;
using Domain;

namespace Factories
{
    /// <summary>
    /// Фабрика для создания операций.
    /// </summary>
    public static class OperationFactory
    {

        public static Operation Create(int id, OperationType type, int bankAccountId, decimal amount, DateTime date, string description, int categoryId)
        {
            // Валидация данных
            return new Operation(id, type, bankAccountId, amount, date, description, categoryId);
        }
    }
}
