using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Entities;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class UpdateCategoryCommand : ICommands
{
    private readonly ICategoryFacade _categoryFacade;
    private readonly Category _category;

    public UpdateCategoryCommand(ICategoryFacade categoryFacade, Category category)
    {
        _categoryFacade = categoryFacade;
        _category = category;
    }

    public void Execute()
    {
        _categoryFacade.UpdateCategory(_category);
    }
}