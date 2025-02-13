using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Animals
{
    class Monkey : Herbo
    {
        //Добавил дополнительное свойство IQ.
        public int IQ { get; set; }
        public Monkey(int food, int number, string name, bool ishealty, string descrip, int levelkind, int iq) : base(food, number, name, ishealty, "Объязьяна", levelkind)
        {
            IQ = iq;
        }



    }
}
