using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Репозиторий категорий.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories;
        private int _currentId;

        public CategoryRepository()
        {
            _categories = new List<Category>();
            _currentId = 0;
        }

        private int GenerateId()
        {
            _currentId++;
            return _currentId;
        }

        public IEnumerable<Category> GetAll()
        {
            return _categories;
        }

        public Category GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Category category)
        {
            category.Id = GenerateId(); // Присваиваем новый ID
            _categories.Add(category);
        }

        public void Update(Category category)
        {
            var existingCategory = GetById(category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.Type = category.Type;
            }
        }

        public void Delete(int id)
        {
            var category = GetById(id);
            if (category != null)
            {
                _categories.Remove(category);
            }
        }
    }
}
