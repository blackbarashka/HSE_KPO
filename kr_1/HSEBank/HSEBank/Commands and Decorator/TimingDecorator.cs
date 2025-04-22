using System;
using System.Diagnostics;

namespace Commands
{
    /// <summary>
    /// Декоратор для измерения времени выполнения команды.
    /// </summary>
    public class TimingDecorator : ICommand
    {
        private readonly ICommand _command;

        public TimingDecorator(ICommand command)
        {
            _command = command;
        }

        public void Execute()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _command.Execute();
            stopwatch.Stop();
            Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
        }
    }
}
