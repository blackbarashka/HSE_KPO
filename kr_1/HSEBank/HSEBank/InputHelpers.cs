using System;
/// <summary>
/// Класс с методами для ввода данных с консоли. Проверка на корректность ввода.
/// </summary>
public static class InputHelpers
{
    /// <summary>
    /// Метод для ввода целого числа с консоли.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result))
                return result;

            Console.WriteLine("Неверный формат. Попробуйте снова.\n");
        }
    }

    /// <summary>
    /// Метод для ввода дробного числа с консоли.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal result))
                return result;

            Console.WriteLine("Неверный формат суммы. Попробуйте снова.\n");
        }
    }

    /// <summary>
    /// Метод для ввода даты с консоли.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static DateTime ReadDateTime(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime result))
                return result;

            Console.WriteLine("Неверный формат даты. Попробуйте снова (yyyy-MM-dd).\n");
        }
    }

    /// <summary>
    /// Метод для ввода перечисления с консоли.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static T ReadEnum<T>(string prompt) where T : struct
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (Enum.TryParse(input, out T value) && Enum.IsDefined(typeof(T), value))
                return value;

            Console.WriteLine("Неверный вариант. Повторите ввод.\n");
        }
    }
    /// <summary>
    /// Метод для ввода строки с консоли.
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public static string ReadString(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
}
