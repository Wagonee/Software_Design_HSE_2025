using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Interfaces.Repositories;
using HSE_Bank.Infrastructure.Fabrics;

namespace HSE_Bank.Application.Facades;

public class CategoryFacade : ICategoryFacade
{
    private readonly DomainObjectFactory _factory;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryFacade(DomainObjectFactory factory, ICategoryRepository categoryRepository)
    {
        _factory = factory;
        _categoryRepository = categoryRepository;
    }

    public Category? GetCategoryById(int id)
    {
        return _categoryRepository.GetCategoryById(id);
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _categoryRepository.GetAllCategories();
    }

    public Category CreateCategory(string name, TypeCategory type)
    {
        int id = new Random().Next();
        var category = _factory.CreateCategory(id, name, type);
        _categoryRepository.AddCategory(category);
        return category;
    }

    public void UpdateCategory(Category category)
    {
        _categoryRepository.UpdateCategory(category);
    }

    public void DeleteCategory(int id)
    {
        _categoryRepository.DeleteCategory(id);
    }
}