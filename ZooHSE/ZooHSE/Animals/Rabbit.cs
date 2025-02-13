using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Animals
{
    public class Rabbit : Herbo
    {
        // Добавил новое свойство Earlength (длина ушей).
        public double Earlength { get; set; }
        public Rabbit(int food, int number, string name, int levelkind, bool ishealty, string discrip, double earlength) : base(food, number, name, ishealty, "Кролик", levelkind)
        {
            Earlength = earlength;
        }
    }
}
