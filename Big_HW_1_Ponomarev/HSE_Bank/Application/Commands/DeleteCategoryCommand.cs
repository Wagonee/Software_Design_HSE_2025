using HSE_Bank.Domain.Interfaces.Commands;
using HSE_Bank.Domain.Interfaces.Facades;

namespace HSE_Bank.Application.Commands;

public class DeleteCategoryCommand : ICommands
{
    private readonly ICategoryFacade _categoryFacade;
    private readonly int _categoryId;

    public DeleteCategoryCommand(ICategoryFacade categoryFacade, int categoryId)
    {
        _categoryFacade = categoryFacade;
        _categoryId = categoryId;
    }

    public void Execute()
    {
        _categoryFacade.DeleteCategory(_categoryId);
    }
}