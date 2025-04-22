using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Domain;
using Facade;
using ImportExport;

namespace HSEBank.ImportExport
{
    /// <summary>
    /// Экспортер данных в формат JSON.
    /// </summary>
    public class JsonDataExporter : DataExporter
    {
        public JsonDataExporter(FinancialFacade facade) : base(facade)
        {
        }

        public override void Export(string filePath)
        {
            // Формируем объект, чтобы сохранить совместимость с JsonDataImporter
            var exportData = new ImportData
            {
                BankAccounts = _facade.GetBankAccounts().ToList(),
                Categories = _facade.GetCategories().ToList(),
                Operations = _facade.GetOperations().ToList()
            };

            // Настройки для удобного чтения JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Сериализация и запись в файл
            string json = JsonSerializer.Serialize(exportData, options);
            File.WriteAllText(filePath, json);
        }
    }

    // Класс для совместимости с импортом
    public class ImportData
    {
        public List<BankAccount> BankAccounts { get; set; }
        public List<Category> Categories { get; set; }
        public List<Operation> Operations { get; set; }
    }
}
