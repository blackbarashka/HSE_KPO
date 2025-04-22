using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Facade;
using ImportExport;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSEBank.ImportExport
{
    /// <summary>
    /// Экспортер данных в формат YAML.
    /// </summary>
    public class YamlDataExporter : DataExporter
    {
        public YamlDataExporter(FinancialFacade facade) : base(facade)
        {
        }

        public override void Export(string filePath)
        {
            // Получаем данные из фасада
            var exportData = new ExportData
            {
                BankAccounts = _facade.GetBankAccounts(),
                Categories = _facade.GetCategories(),
                Operations = _facade.GetOperations()
            };

            // Используем YamlDotNet для сериализации
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yamlContent = serializer.Serialize(exportData);
            File.WriteAllText(filePath, yamlContent);
        }
    }

    // Класс для удобства сериализации.
    public class ExportData
    {
        public IEnumerable<BankAccount> BankAccounts { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Operation> Operations { get; set; }
    }
}
