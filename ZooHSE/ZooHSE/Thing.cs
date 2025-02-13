namespace ZooHSE
{
    public class Thing : IInventory
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public Thing(int number, string name)
        {

            Number = number;
            Name = name;
        }
    }
}
