namespace Domain
{
    /// <summary>
    /// Тип категории.
    /// </summary>
    public enum CategoryType
    {
        Income,
        Expense
    }

    /// <summary>
    /// Категория операции.
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public CategoryType Type { get; set; }
        public string Name { get;  set; }

        internal Category(int id, CategoryType type, string name)
        {
            Id = id;
            Type = type;
            Name = name;
        }

        public void UpdateName(string name)
        {
            // Валидация имени
            Name = name;
        }
    }
}
