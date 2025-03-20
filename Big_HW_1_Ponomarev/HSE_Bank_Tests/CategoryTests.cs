using HSE_Bank.Domain.Entities;
using HSE_Bank.Infrastructure.Fabrics;

namespace HSE_Bank_Tests
{
    public class CategoryTests
    {
        private readonly DomainObjectFactory _factory = new();

        [Fact]
        public void CreateCategory_ShouldCreateValidCategory()
        {
            var category = _factory.CreateCategory(1, "Salary", TypeCategory.Income);
            Assert.Equal(1, category.Id);
            Assert.Equal("Salary", category.Name);
            Assert.Equal(TypeCategory.Income, category.Type);
        }

        [Fact]
        public void CreateCategory_ShouldThrowException_WhenNameIsNull()
        {
            Assert.Throws<ArgumentException>(() => _factory.CreateCategory(1, null, TypeCategory.Income));
        }
    }
}