using System;
using System.Diagnostics;
using Domain;
using Facade;
using HSEBank.ImportExport;
using ImportExport;

public static class MenuHandlers
{
    public static void ShowAnalyticsMenu(FinancialFacade facade)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("==== Аналитика ====");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Подсчет разницы доходов и расходов за период");
            Console.WriteLine("2. Группировка доходов и расходов по категориям");
            Console.WriteLine("3. Другая аналитика");
            Console.WriteLine("4. Назад");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    CalculateIncomeExpenseDifference(facade);
                    break;
                case "2":
                    GroupIncomesExpensesByCategory(facade);
                    break;
                case "3":
                    // Другие аналитические функции
                    Console.WriteLine("Функция пока не реализована. Нажмите Enter, чтобы вернуться...");
                    Console.ReadLine();
                    break;
                case "4":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
            }
        }
    }
    /// <summary>
    /// Метод для отображения меню импорта и экспорта данных.
    /// </summary>
    /// <param name="facade"></param>
    public static void ShowImportExportMenu(FinancialFacade facade)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();

            Console.WriteLine("==== Импорт и экспорт данных ====");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Экспорт данных");
            Console.WriteLine("2. Импорт данных");
            Console.WriteLine("3. Назад");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ExportData(facade);
                    break;
                case "2":
                    ImportData(facade);
                    break;
                case "3":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
            }
        }
    }
    /// <summary>
    /// Метод для отображения меню управления данными.
    /// </summary>
    /// <param name="facade"></param>
    public static void ShowDataManagementMenu(FinancialFacade facade)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("==== Управление данными ====");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Пересчет баланса при несоответствиях");
            Console.WriteLine("2. Создать банковский аккаунт.");
            Console.WriteLine("3. Создать категорию.");
            Console.WriteLine("4. Создать операцию.");
            Console.WriteLine("5. Редактировать операцию.");
            Console.WriteLine("6. Удалить операцию.");
            Console.WriteLine("7. Редактировать банковский счет.");
            Console.WriteLine("8. Удалить банковский счет.");
            Console.WriteLine("9. Редактировать категорию.");
            Console.WriteLine("10. Удалить категорию.");
            Console.WriteLine("11. Назад");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RecalculateBalances(facade);
                    break;
                case "2":
                    string accountName = InputHelpers.ReadString("Введите название банковского аккаунта: ");
                    decimal accountBalance = InputHelpers.ReadDecimal("Введите начальный баланс: ");
                    facade.CreateBankAccount(accountName, accountBalance);
                    Console.WriteLine("Банковский аккаунт успешно создан. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
                case "3":
                    CategoryType categoryType = InputHelpers.ReadEnum<CategoryType>("Введите тип категории (Income/Expense): ");
                    string categoryName = InputHelpers.ReadString("Введите название категории: ");
                    facade.CreateCategory(categoryType, categoryName);
                    Console.WriteLine("Категория успешно создана. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
                case "4":
                    DateTime operationDate = InputHelpers.ReadDateTime("Введите дату операции (yyyy-MM-dd): ");
                    OperationType operationType = InputHelpers.ReadEnum<OperationType>("Введите тип операции (Income/Expense): ");
                    decimal operationAmount = InputHelpers.ReadDecimal("Введите сумму операции: ");
                    int accountId = InputHelpers.ReadInt("Введите ID банковского аккаунта: ");
                    string operationDescription = InputHelpers.ReadString("Введите описание операции: ");
                    int categoryId = InputHelpers.ReadInt("Введите ID категории: ");
                    facade.CreateOperation(operationDate, operationType, operationAmount, accountId, operationDescription, categoryId);
                    Console.WriteLine("Операция успешно создана. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
                case "5":
                    EditOperation(facade);
                    break;
                case "6":
                    DeleteOperation(facade);
                    break;
                case "7":
                    EditBankAccount(facade);
                    break;
                case "8":
                    DeleteBankAccount(facade);
                    break;

                case "9":
                    EditCategory(facade);
                    break;
                case "10":
                    DeleteCategory(facade);
                    break;
                case "11":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
            }
        }
    }

    public static void ShowStatistics(FinancialFacade facade)
    {
        bool back = false;
        while (!back)
        {
            Console.Clear();
            Console.WriteLine("==== Статистика ====");
            Console.WriteLine("1. Показать все операции");
            Console.WriteLine("2. Показать все банковские счета.");
            Console.WriteLine("3. Показать все категории.");
            Console.WriteLine("4. Назад");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ShowAllOperations(facade);
                    break;
                case "2":
                    ShowAllBankAccounts(facade);
                    break;
                case "3":
                    ShowCategories(facade);
                    break;
                case "4":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
            }

        }
        

    }
    /// <summary>
    /// Подсчет разницы доходов и расходов за период.
    /// </summary>
    /// <param name="facade"></param>
    private static void CalculateIncomeExpenseDifference(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Подсчет разницы доходов и расходов ====");
        DateTime startDate = InputHelpers.ReadDateTime("Введите дату начала периода (yyyy-MM-dd): ");
        DateTime endDate = InputHelpers.ReadDateTime("Введите дату окончания периода (yyyy-MM-dd): ");

        Stopwatch stopwatch = Stopwatch.StartNew();

        decimal totalIncome = facade.GetTotalIncomeForPeriod(startDate, endDate);
        decimal totalExpense = facade.GetTotalExpenseForPeriod(startDate, endDate);
        decimal difference = totalIncome - totalExpense;

        stopwatch.Stop();

        Console.WriteLine($"\nОбщий доход: {totalIncome:C}");
        Console.WriteLine($"Общие расходы: {totalExpense:C}");
        Console.WriteLine($"Разница: {difference:C}");
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");

        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }
    /// <summary>
    /// Группировка доходов и расходов по категориям.
    /// </summary>
    /// <param name="facade"></param>
    private static void GroupIncomesExpensesByCategory(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Группировка по категориям ====");

        Stopwatch stopwatch = Stopwatch.StartNew();

        var groupedData = facade.GetOperationsGroupedByCategory();

        stopwatch.Stop();

        foreach (var group in groupedData)
        {
            Console.WriteLine($"\nКатегория: {group.Key.Name} ({group.Key.Type})");
            foreach (var operation in group.Value)
            {
                Console.WriteLine($"- {operation.Date.ToShortDateString()}: {operation.Amount:C} ({operation.Description})");
            }
        }

        Console.WriteLine($"\nВремя выполнения: {stopwatch.ElapsedMilliseconds} мс");

        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }
    /// <summary>
    /// Экспорт данных.
    /// </summary>
    /// <param name="facade"></param>
    private static void ExportData(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Экспорт данных ====");
        Console.WriteLine("Выберите формат:");
        Console.WriteLine("1. CSV");
        Console.WriteLine("2. JSON");
        Console.WriteLine("3. YAML");
        Console.Write("Ваш выбор: ");

        string formatChoice = Console.ReadLine();
        DataExporter exporter = null;
        string filePath = "";

        switch (formatChoice)
        {
            case "1":
                exporter = new CsvDataExporter(facade);
                filePath = "export.csv";
                break;
            case "2":
                exporter = new JsonDataExporter(facade);
                filePath = "export.json";
                break;
            case "3":
                exporter = new YamlDataExporter(facade);
                filePath = "export.yaml";
                break;
            default:
                Console.WriteLine("Неверный выбор. Нажмите Enter, чтобы вернуться...");
                Console.ReadLine();
                return;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        exporter.Export(filePath);

        stopwatch.Stop();

        Console.WriteLine($"\nДанные успешно экспортированы в файл {filePath}.");
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }
    /// <summary>
    /// Импорт данных.
    /// </summary>
    /// <param name="facade"></param>
    private static void ImportData(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Импорт данных ====");
        Console.WriteLine("Выберите формат:");
        Console.WriteLine("1. CSV");
        Console.WriteLine("2. JSON");
        Console.WriteLine("3. YAML");
        Console.Write("Ваш выбор: ");

        string formatChoice = Console.ReadLine();
        DataImporter importer = null;

        Console.Write("Введите путь к файлу: ");
        string filePath = Console.ReadLine();

        switch (formatChoice)
        {
            case "1":
                importer = new CsvDataImporter(facade);
                break;
            case "2":
                importer = new CsvDataImporter(facade);
                break;
            case "3":
                importer = new CsvDataImporter(facade);
                break;
            default:
                Console.WriteLine("Неверный выбор. Нажмите Enter, чтобы вернуться...");
                Console.ReadLine();
                return;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        importer.Import(filePath);

        stopwatch.Stop();

        Console.WriteLine($"\nДанные успешно импортированы из файла {filePath}.");
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }
    /// <summary>
    /// Пересчет баланса.
    /// </summary>
    /// <param name="facade"></param>
    private static void RecalculateBalances(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Пересчет баланса ====");

        Stopwatch stopwatch = Stopwatch.StartNew();

        facade.RecalculateBalances();

        stopwatch.Stop();

        Console.WriteLine("\nБалансы успешно пересчитаны.");
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }
    public static void ShowAllOperations(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Все операции ====");
        var operations = facade.GetOperations();
        foreach (var operation in operations)
        {
            Console.WriteLine($"{operation.Date.ToShortDateString()} {operation.Type} {operation.Amount:C} {operation.Description}");
        }
        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }

    //Метод для вывода всез банковских счетов.
    public static void ShowAllBankAccounts(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Все банковские счета ====");
        var accounts = facade.GetBankAccounts();
        foreach (var account in accounts)
        {
            Console.WriteLine($"{account.Id} {account.Name} {account.Balance:C}");
        }
        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }

    //Метод для удаления операции.
    public static void DeleteOperation(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Удаление операции ====");
        int operationId = InputHelpers.ReadInt("Введите ID операции: ");
        facade.DeleteOperation(operationId);
        Console.WriteLine("Операция успешно удалена. Нажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }
    //Метод для удаления банковского счета.
    public static void DeleteBankAccount(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Удаление банковского счета ====");
        int accountId = InputHelpers.ReadInt("Введите ID банковского счета: ");
        facade.DeleteBankAccount(accountId);
        Console.WriteLine("Банковский счет успешно удален. Нажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }
    //Метод для удаления категории.
    public static void DeleteCategory(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Удаление категории ====");
        int categoryId = InputHelpers.ReadInt("Введите ID категории: ");
        facade.DeleteCategory(categoryId);
        Console.WriteLine("Категория успешно удалена. Нажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }
    //Метод для редактирования операции.
    public static void EditOperation(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Редактирование операции ====");
        int operationId = InputHelpers.ReadInt("Введите ID операции: ");
        DateTime operationDate = InputHelpers.ReadDateTime("Введите дату операции (yyyy-MM-dd): ");
        OperationType operationType = InputHelpers.ReadEnum<OperationType>("Введите тип операции (Income/Expense): ");
        decimal operationAmount = InputHelpers.ReadDecimal("Введите сумму операции: ");
        int accountId = InputHelpers.ReadInt("Введите ID банковского аккаунта: ");
        string operationDescription = InputHelpers.ReadString("Введите описание операции: ");
        int categoryId = InputHelpers.ReadInt("Введите ID категории: ");
        
        facade.UpdateOperation(new Operation(operationId, operationType, accountId, operationAmount, operationDate, operationDescription, categoryId));
        Console.WriteLine("Операция успешно отредактирована. Нажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }
    //Метод для редактирования банковского счета.
    public static void EditBankAccount(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Редактирование банковского счета ====");
        int accountId = InputHelpers.ReadInt("Введите ID банковского счета: ");
        string accountName = InputHelpers.ReadString("Введите название банковского аккаунта: ");
        decimal accountBalance = InputHelpers.ReadDecimal("Введите начальный баланс: ");
        facade.UpdateBankAccount(new BankAccount(accountId, accountName, accountBalance));
        Console.WriteLine("Банковский счет успешно отредактирован. Нажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }
    //Метод для редактирования категории.
    public static void EditCategory(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Редактирование категории ====");
        int categoryId = InputHelpers.ReadInt("Введите ID категории: ");
        CategoryType categoryType = InputHelpers.ReadEnum<CategoryType>("Введите тип категории (Income/Expense): ");
        string categoryName = InputHelpers.ReadString("Введите название категории: ");
        facade.UpdateCategory(new Category(categoryId, categoryType, categoryName));
        Console.WriteLine("Категория успешно отредактирована. Нажмите Enter, чтобы продолжить...");
        Console.ReadLine();
    }
    //Метод для отображения категорий.
    public static void ShowCategories(FinancialFacade facade)
    {
        Console.Clear();
        Console.WriteLine("==== Категории ====");
        var categories = facade.GetCategories();
        foreach (var category in categories)
        {
            Console.WriteLine($"{category.Id} {category.Type} {category.Name}");
        }
        Console.WriteLine("\nНажмите Enter, чтобы вернуться...");
        Console.ReadLine();
    }
}
