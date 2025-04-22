using System;
using System.Collections.Generic;
using System.IO;
using Domain;
using Facade;

namespace ImportExport
{
    /// <summary>
    /// Импортер данных из CSV-файла.
    /// </summary>
    public class CsvDataImporter : DataImporter
    {
        public CsvDataImporter(FinancialFacade facade) : base(facade)
        {
        }

        public override void Import(string filePath)
        {
            while (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден. Пожалуйста, введите корректный путь к файлу(без кавычек):");
                filePath = Console.ReadLine();
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Проверяем секции по заголовкам
                    if (line == "Id;Name;Balance")
                    {
                        ImportAccounts(reader);
                    }
                    else if (line == "Categories")
                    {
                        // Пропускаем строку "Id;Type;Name"
                        reader.ReadLine();
                        ImportCategories(reader);
                    }
                    else if (line == "Operations")
                    {
                        // Пропускаем строку "Id;Type,BankAccountId;Amount;Date;Description;CategoryId"
                        reader.ReadLine();
                        ImportOperations(reader);
                    }
                }
            }
        }
        /// <summary>
        /// Импорт счетов из CSV-файла.
        /// </summary>
        /// <param name="reader"></param>
        private void ImportAccounts(StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    break;

                string[] parts = line.Split(';');
                var account = new BankAccount(
                    int.Parse(parts[0]),
                    parts[1],
                    decimal.Parse(parts[2])
                );
                facade.AddBankAccount(account);
            }
        }
        /// <summary>
        /// Импорт категорий из CSV-файла.
        /// </summary>
        /// <param name="reader"></param>
        private void ImportCategories(StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    break;

                string[] parts = line.Split(';');
                var category = new Category(
                    int.Parse(parts[0]),
                    Enum.Parse<CategoryType>(parts[1]),
                    parts[2]
                );
                facade.AddCategory(category);
            }
        }
        /// <summary>
        /// Импорт операций из CSV-файла.
        /// </summary>
        /// <param name="reader"></param>
        private void ImportOperations(StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    break;

                // Операции разделены запятыми
                string[] parts = line.Split(',');
                var operation = new Operation(
                    int.Parse(parts[0]),
                    Enum.Parse<OperationType>(parts[1]),
                    int.Parse(parts[2]),
                    decimal.Parse(parts[3]),
                    DateTime.Parse(parts[4]),
                    parts[5],
                    int.Parse(parts[6])
                );
                facade.AddOperation(operation);
            }
        }
    }
}

