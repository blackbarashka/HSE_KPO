using ZooHSE.Intarface;

namespace ZooHSE
{
    /// <summary>
    /// Класс вещь, реализующий интерфейс IInventory. 
    /// </summary>
    public class Thing : IInventory // Класс не абстрактый, так как он используется для создания объектов, при добавлении нового предмета.
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public Thing(int number, string name)
        {

            Number = number;
            Name = name;
        }
    }
}
