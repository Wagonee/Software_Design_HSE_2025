using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;
using HSE_Bank.Domain.Entities;

namespace HSE_Bank.Application.Commands;

public class CreateCategoryCommand : ICommands
{
    private readonly ICategoryFacade _categoryFacade;
    private readonly string _categoryName;
    private readonly TypeCategory _categoryType;

    public CreateCategoryCommand(ICategoryFacade categoryFacade, string categoryName, TypeCategory categoryType)
    {
        _categoryFacade = categoryFacade;
        _categoryName = categoryName;
        _categoryType = categoryType;
    }

    public void Execute()
    {
        _categoryFacade.CreateCategory(_categoryName, _categoryType);
    }
}