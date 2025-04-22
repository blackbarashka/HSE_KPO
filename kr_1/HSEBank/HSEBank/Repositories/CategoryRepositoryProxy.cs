using System.Collections.Generic;
using Domain;

namespace Repositories
{
    /// <summary>
    /// Прокси-репозиторий категорий.
    /// </summary>
    public class CategoryRepositoryProxy : ICategoryRepository
    {
        private readonly ICategoryRepository _repository;

        public CategoryRepositoryProxy(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Category> GetAll()
        {

            return _repository.GetAll();
        }

        public Category GetById(int id)
        {
            return _repository.GetById(id);
        }

        public void Add(Category category)
        {
            _repository.Add(category);
        }

        public void Update(Category category)
        {
            _repository.Update(category);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }
}
