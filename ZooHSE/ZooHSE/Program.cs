using Microsoft.Extensions.DependencyInjection;
using ZooHSE.Animals;
using ZooHSE.Things;
namespace ZooHSE
{
    class Program
    {
        static void Main(string[] args)
        {
            // Идентификатор животного.
            int number = 1;
            // Создание сервис-контейнера.
            var serviceProvider = new ServiceCollection()
                .AddSingleton<VetClinic, VetClinic>()
                .AddSingleton<Zoo>()
                .BuildServiceProvider();

            var zoo = serviceProvider.GetService<Zoo>();
            var vetClinic = serviceProvider.GetService<VetClinic>();

            if (zoo == null || vetClinic == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to initialize Zoo or VetClinic.");
                Console.ResetColor();
                return;
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("============================================");
                Console.WriteLine("                М Е Н Ю                     ");
                Console.WriteLine("============================================");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nВыберите дейсвтвие:");
                Console.WriteLine("1. Добавить животное.");
                Console.WriteLine("2. Добавить предмет.");
                Console.WriteLine("3. Вывод количества употребляемой пищи.");
                Console.WriteLine("4. Вывод контактных животных.");
                Console.WriteLine("5. Вывод инвентаризационного списка.");
                Console.WriteLine("6. Выход.");
                Console.ResetColor();

                Console.Write("\nВаш выбор: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("\nВведите имя животного: ");
                        var name = Console.ReadLine();

                        int food = GetValidInt("Введите количество потребляемой пищи (кг/д): ");
                        bool isHealthy = GetValidBool("Животное здорово? (yes/no): ");
                        bool isHerbivore = GetValidBool("Животное травоядное? (yes/no): ");

                        if (isHerbivore)
                        {
                            int kindness = GetValidInt("Введите уровень доброты (1-10): ");

                            if (kindness < 1 || kindness > 10)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Неверный ввод. Введите пожалуйста корректное значение.");
                                Console.ResetColor();
                                break;
                            }
                            string choiceTwo;
                            do
                            {
                                Console.WriteLine("\nВыберите животное:");
                                Console.WriteLine("1. Кролик");
                                Console.WriteLine("2. Обезьяна");
                                Console.WriteLine("3. Добавить другое животное");
                            } while ((choiceTwo = Console.ReadLine()) != "1" && choiceTwo != "2" && choiceTwo != "3");

                            if (choiceTwo == "1")
                            {
                                int earLength = GetValidInt("Введите длину ушей: ");
                                zoo.AddAnimal(new Rabbit(food, number, name, kindness, isHealthy, "Кролик", earLength), vetClinic);
                            }
                            else if (choiceTwo == "2")
                            {
                                int iq = GetValidInt("Введите IQ: ");
                                zoo.AddAnimal(new Monkey(food, number, name, isHealthy, "Обезьяна", kindness, iq), vetClinic);
                            }
                            else
                            {
                                Console.Write("Введите название нового животного: ");
                                var title = Console.ReadLine();
                                zoo.AddAnimal(new Herbo(food, number, name, isHealthy, title, kindness), vetClinic);
                            }
                        }
                        else
                        {
                            string choiceTwo;
                            do
                            {
                                Console.WriteLine("\nВыберите животное:");
                                Console.WriteLine("1. Тигр");
                                Console.WriteLine("2. Волк");
                                Console.WriteLine("3. Добавить другое животное");

                            } while ((choiceTwo = Console.ReadLine()) != "1" && choiceTwo != "2" && choiceTwo != "3");

                            if (choiceTwo == "1")
                            {
                                int tailLength = GetValidInt("Введите длину хвоста: ");
                                zoo.AddAnimal(new Tiger(food, number, name, isHealthy, "Тигр", tailLength), vetClinic);
                            }
                            else if (choiceTwo == "2")
                            {
                                zoo.AddAnimal(new Wolf(food, number, name, isHealthy, "Волк"), vetClinic);
                            }
                            else
                            {
                                Console.Write("Введите название нового животного: ");
                                var title = Console.ReadLine();
                                zoo.AddAnimal(new Predator(food, number, name, isHealthy, title), vetClinic);
                            }
                        }
                        if (isHealthy)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Иденфикатор животного: {number}");
                            Console.ResetColor();
                            ++number;
                        }

                        break;

                    case "2":
                        string choicethhee;
                        do
                        {
                            Console.WriteLine("\nВыберите предмет:");
                            Console.WriteLine("1. Стол.");
                            Console.WriteLine("2. Компьютер.");
                            Console.WriteLine("3. Добавить другой предмет.");
                        } while ((choicethhee = Console.ReadLine()) != "1" && choicethhee != "2" && choicethhee != "3");

                        if (choicethhee == "1")
                        {
                            zoo.AddInventoryItem(new Table(number, "Стол"));
                        }
                        else if (choicethhee == "2")
                        {
                            zoo.AddInventoryItem(new Computer(number, "Компьютер"));
                        }
                        else
                        {
                            Console.Write("Введите название нового предмета: ");
                            var title = Console.ReadLine();
                            zoo.AddInventoryItem(new Thing(number, title));
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Иденфикатор предмета: {number}");
                        Console.ResetColor();
                        ++number;
                        break;

                    case "3":
                        zoo.PrintFoodRequirements();
                        break;

                    case "4":
                        zoo.PrintContactZooAnimals();
                        break;

                    case "5":
                        zoo.PrintInventory();
                        break;

                    case "6":
                        return;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неверный ввод. Введите пожалуйста корректное значение.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        /// <summary>
        /// Получение корректного целого числа.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static int GetValidInt(string message)
        {
            int result;
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out result))
                    return result;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неверный ввод. Введите пожалуйста корректное значение.");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Получение корректного булевого значения.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static bool GetValidBool(string message)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine()?.ToLower();
                if (input == "yes") return true;
                if (input == "no") return false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Неверный ввод. Введите 'yes' или 'no'.");
                Console.ResetColor();
            }
        }
    }
}
