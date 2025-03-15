using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Category? GetCategoryById(int id);
    IEnumerable<Category> GetAllCategories();
    void AddCategory(Category category);
    void UpdateCategory(Category category);
    void DeleteCategory(int id);
}