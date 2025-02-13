using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Animals
{

    public class Herbo : Animal
    {
        public int LeveofKindness { get; set; }

        public Herbo(int food, int number, string name, bool ishealty, string descrip, int levelkind) : base(food, number, name, ishealty, descrip)
        {
            LeveofKindness = levelkind;
        }
    }
}
