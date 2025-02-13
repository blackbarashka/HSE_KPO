using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using ZooHSE.Animals;

namespace ZooHSE
{


    public class Zoo
    {
        //Список животных.
        private readonly List<Animal> _animals = new();
        //Список предметов.
        private readonly List<Thing> _inventory = new();

        /// <summary>
        /// Добавление животного в зоопарк. Проверка на здоровье животного. 
        /// </summary>
        /// <param name="animal"></param>
        /// <param name="vetClinic"></param>
        public void AddAnimal(Animal animal, VetClinic vetClinic)
        {
            if (vetClinic.CheckHealth(animal))
            {

                _animals.Add(animal);
            }
            else
            {
                Console.WriteLine($"Животное {animal.Name} не может быть добавлено из-за проблем с здоровьем.");
            }
        }

        /// <summary>
        /// Добавление предмета в инвентарь.
        /// </summary>
        /// <param name="item"></param>
        public void AddInventoryItem(Thing item)
        {
            _inventory.Add(item);
        }

        /// <summary>
        /// Вывод потребляемой еды в день.
        /// </summary>
        public void PrintFoodRequirements()
        {
            int totalFood = _animals.Sum(a => a.Food);
            Console.WriteLine($"Количество употребляемой пищи: {totalFood} кг/день");
        }

        /// <summary>
        /// Вывод животных, которые могут быть добавлены в контактный центр.
        /// </summary>
        public void PrintContactZooAnimals()
        {
            var friendlyAnimals = _animals.OfType<Herbo>().Where(h => h.LeveofKindness > 5);
            foreach (var animal in friendlyAnimals)
            {
                Console.WriteLine($"{animal.Description} по имени {animal.Name} (Иденфикатор: {animal.Number}) может быть добавлено в контактный центр.");
            }
        }

        /// <summary>
        /// Вывод списка инвертаря в зоопарке.
        /// </summary>
        public void PrintInventory()
        {
            Console.WriteLine();
            foreach (var animal in _animals)
            {
                Console.WriteLine($"{animal.Description} по имени {animal.Name}, идинфикатор: {animal.Number}");
            }
            foreach (var item in _inventory)
            {
                Console.WriteLine($"{item.Name}, иденфикатор: {item.Number}");
            }
        }
        public List<Animal> GetAnimals() => _animals;
        public List<Thing> GetInventory() => _inventory;
    }
}
