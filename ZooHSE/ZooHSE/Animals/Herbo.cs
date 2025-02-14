using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Animals
{
    /// <summary>
    /// Класс травоядное, наследуется от класса Animal. Класс используется для создания объектов хищников, при добавлении нового животного.
    /// </summary>
    public class Herbo : Animal
    {
        public int LeveofKindness { get; set; }

        public Herbo(int food, int number, string name, bool ishealty, string descrip, int levelkind) : base(food, number, name, ishealty, descrip)
        {
            LeveofKindness = levelkind;
        }
    }
}
