using System;
using System.Collections.Generic;
using System.IO;
using Domain;
using Facade;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ImportExport
{
    /// <summary>
    /// Импортер данных из YAML-файла.
    /// </summary>
    public class YamlDataImporter : DataImporter
    {
        public YamlDataImporter(FinancialFacade facade) : base(facade)
        {
        }

        public override void Import(string filePath)
        {
            // Используем YamlDotNet для десериализации
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yamlContent = File.ReadAllText(filePath);
            var importData = deserializer.Deserialize<ImportData>(yamlContent);

            // Добавляем объекты в репозитории через фасад
            foreach (var bankAccount in importData.BankAccounts)
                facade.AddBankAccount(bankAccount);

            foreach (var category in importData.Categories)
                facade.AddCategory(category);

            foreach (var operation in importData.Operations)
                facade.AddOperation(operation);
        }
    }
}
