using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Domain;
using Facade;

namespace ImportExport
{
    /// <summary>
    /// Импортер данных из JSON-файла.
    /// </summary>
    public class JsonDataImporter : DataImporter
    {
        public JsonDataImporter(FinancialFacade facade) : base(facade)
        {
        }

        public override void Import(string filePath)
        {
            // Чтение файла
            string json = File.ReadAllText(filePath);
            // Десериализация
            ImportData importData = JsonSerializer.Deserialize<ImportData>(json);
            // Добавление данных в репозитории через фасад
            foreach (var bankAccount in importData.BankAccounts)
            {
                facade.AddBankAccount(bankAccount);
            }
            foreach (var category in importData.Categories)
            {
                facade.AddCategory(category);
            }
            foreach (var operation in importData.Operations)
            {
                facade.AddOperation(operation);
            }
        }
    }

    // Вспомогательный класс для десериализации
    public class ImportData
    {
        public List<BankAccount> BankAccounts { get; set; }
        public List<Category> Categories { get; set; }
        public List<Operation> Operations { get; set; }
    }
}
