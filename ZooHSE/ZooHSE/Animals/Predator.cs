namespace ZooHSE.Animals
{
    /// <summary>
    /// Класс хищник, наследуется от класса Animal. Класс используется для создания объектов хищников, при добавлении нового животного.
    /// </summary>
    public class Predator : Animal
    {
        public Predator(int food, int number, string name, bool ishealty, string descrip) : base(food, number, name, ishealty, descrip) { }
    }
}
