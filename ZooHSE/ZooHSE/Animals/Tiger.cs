using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Animals
{
    public class Tiger : Predator
    {
        //Добавляем новое свойство Taillength (длина хвоста).
        public double Taillength { get; set; }
        public Tiger(int food, int number, string name, bool ishealty, string discrip, double taillength) : base(food, number, name, ishealty, "Тигр")
        {
            Taillength = taillength;
        }
    }
}
