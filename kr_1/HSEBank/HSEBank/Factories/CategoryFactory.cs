using Domain;

namespace Factories
{
    /// <summary>
    /// Фабрика для создания категорий.
    /// </summary>
    public static class CategoryFactory
    {
        public static Category Create(int id, CategoryType type, string name)
        {
            // Валидация данных
            return new Category(id, type, name);
        }
    }
}
