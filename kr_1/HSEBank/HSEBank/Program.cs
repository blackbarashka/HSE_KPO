//Musaev Umakhan.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Commands;
using Facade;
using ImportExport;
using Repositories;
using Domain;
using HSEBank.ImportExport;

class Program
{
    static void Main(string[] args)
    {
        // Инициализация репозиториев
        IBankAccountRepository bankAccountRepository = new BankAccountRepositoryProxy(new BankAccountRepository());
        ICategoryRepository categoryRepository = new CategoryRepositoryProxy(new CategoryRepository());
        IOperationRepository operationRepository = new OperationRepositoryProxy(new OperationRepository());

        // Инициализация фасада
        FinancialFacade facade = new FinancialFacade(bankAccountRepository, categoryRepository, operationRepository);

        // Главное меню
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("==== Приложение 'Учет финансов' ====");
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Аналитика");
            Console.WriteLine("2. Импорт и экспорт данных");
            Console.WriteLine("3. Управление данными");
            Console.WriteLine("4. Статистика");
            Console.WriteLine("5. Выход");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    MenuHandlers.ShowAnalyticsMenu(facade);
                    break;
                case "2":
                    MenuHandlers.ShowImportExportMenu(facade);
                    break;
                case "3":
                    MenuHandlers.ShowDataManagementMenu(facade);
                    break;
                case "4":
                    MenuHandlers.ShowStatistics(facade);
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Нажмите Enter, чтобы продолжить...");
                    Console.ReadLine();
                    break;
            }
        }
    }
}
