using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooHSE
{
    /// <summary>
    /// Класс ветеринарной клиники.
    /// </summary>
    public class VetClinic
    {
        /// <summary>
        /// Проверка здоровья животного, пользователь сам вводит наличие здоровья у животного.
        /// </summary>
        /// <param name="animal"></param>
        /// <returns></returns>
        public bool CheckHealth(Animal animal) => animal.IsHealthy;
    }
}
