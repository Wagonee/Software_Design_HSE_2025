using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Domain.Interfaces.Facades;

public interface ICategoryFacade
{
    Category? GetCategoryById(int id);
    IEnumerable<Category> GetAllCategories();
    Category CreateCategory(string name, TypeCategory type);
    void UpdateCategory(Category category);
    void DeleteCategory(int id);
}