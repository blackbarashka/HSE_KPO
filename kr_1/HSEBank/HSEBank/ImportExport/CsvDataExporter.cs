using System.IO;
using System.Linq;
using Domain;
using Facade;

namespace ImportExport
{
    /// <summary>
    /// Экспортер данных в формат CSV.
    /// </summary>
    public class CsvDataExporter : DataExporter
    {
        public CsvDataExporter(FinancialFacade facade) : base(facade)
        {
        }

        public override void Export(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Экспорт счетов
                writer.WriteLine("Id;Name;Balance");
                foreach (var account in _facade.GetBankAccounts())
                {

                    writer.WriteLine($"{account.Id};{account.Name};{account.Balance}");
                }

                // Экспорт категорий
                writer.WriteLine("\nCategories");
                writer.WriteLine("Id;Type;Name");
                foreach (var category in _facade.GetCategories())
                {
                    writer.WriteLine($"{category.Id};{category.Type};{category.Name}");
                }

                // Экспорт операций
                writer.WriteLine("\nOperations");
                writer.WriteLine("Id;Type,BankAccountId;Amount;Date;Description;CategoryId");
                foreach (var operation in _facade.GetOperations())
                {
                    writer.WriteLine($"{operation.Id},{operation.Type},{operation.BankAccountId},{operation.Amount},{operation.Date},{operation.Description},{operation.CategoryId}");
                }
            }
        }
    }
}
