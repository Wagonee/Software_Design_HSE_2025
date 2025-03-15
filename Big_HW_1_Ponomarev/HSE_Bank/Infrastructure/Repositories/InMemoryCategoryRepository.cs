using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Repositories;

namespace HSE_Bank.Infrastructure.Repositories;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = new List<Category>();

    public void AddCategory(Category category)
    {
        _categories.Add(category);
    }

    public void DeleteCategory(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category != null)
        {
            _categories.Remove(category);
        }
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _categories;
    }

    public Category? GetCategoryById(int id)
    {
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    public void UpdateCategory(Category category)
    {
        int index = _categories.IndexOf(category);
        if (index != -1)
        {
            _categories[index] = category;
        }
    }
}