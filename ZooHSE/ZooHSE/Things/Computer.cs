using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Things
{
    /// <summary>
    /// Класс компьютера, наследуемый от класса вещи.
    /// </summary>
    public class Computer : Thing
    {
        public Computer(int number, string name) : base(number, name)
        {
        }
    }
}
