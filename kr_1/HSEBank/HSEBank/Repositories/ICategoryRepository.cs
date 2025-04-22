using System.Collections.Generic;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Репозиторий категорий.
    /// </summary>
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Category GetById(int id);
        void Add(Category category);
        void Update(Category category);
        void Delete(int id);
    }
}
