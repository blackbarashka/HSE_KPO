using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE.Things
{
    /// <summary>
    /// Класс стола, наследуемый от класса вещи.
    /// </summary>
    public class Table : Thing
    {
        public Table(int number, string name) : base(number, name)
        {
        }
    }
}
