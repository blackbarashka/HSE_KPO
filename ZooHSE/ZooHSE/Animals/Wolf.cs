using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Animals
{
    public class Wolf : Predator
    {
        public Wolf(int food, int number, string name, bool ishealty, string discrip) : base(food, number, name, ishealty, "Волк")
        {
        }
    }
}
