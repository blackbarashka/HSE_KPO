using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Animals
{
    /// <summary>
    /// Класс тигр, наследуется от хищника.
    /// </summary>
    public class Tiger : Predator
    {
        //Добавил новое свойство Taillength (длина хвоста).
        public double Taillength { get; set; }
        public Tiger(int food, int number, string name, bool ishealty, string discrip, double taillength) : base(food, number, name, ishealty, "Тигр")
        {
            Taillength = taillength;
        }
    }
}
