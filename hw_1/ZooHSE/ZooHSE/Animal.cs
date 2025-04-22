using ZooHSE.Intarface;

namespace ZooHSE
{
    /// <summary>
    /// Абстрактный класс животное, реализующий интерфейсы IAlive и IInventory.
    /// </summary>
    public abstract class Animal : IAlive, IInventory
    {
        public int Food { get; set; }
        // Иденфикатор.
        public int Number { get; set; }

        // Для проверки здоровое\не здоровое животное в вет. клинике.
        public bool IsHealthy { get; set; }

        // Имя животного.
        public string Name { get; set; } 

        // Само животное (объязьяна, тигр, кролик и тд).
        public string Description { get; set; }
        public Animal(int food, int number, string name, bool ishealty, string descrip) { 
            Food = food;
            Number = number;
            Name = name;
            IsHealthy = ishealty;
            Description = descrip;
        }
    }
}
