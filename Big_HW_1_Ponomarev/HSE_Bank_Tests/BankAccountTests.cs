using HSE_Bank.Infrastructure.Fabrics;

namespace HSE_Bank_Tests
{
    public class BankAccountTests
    {
        private readonly DomainObjectFactory _factory = new();

        [Fact]
        public void CreateBankAccount_ShouldCreateValidAccount()
        {
            var account = _factory.CreateBankAccount("Test", 100, 1);
            Assert.Equal("Test", account.Name);
            Assert.Equal(100, account.Balance);
            Assert.Equal(1, account.Id);
        }

        [Fact]
        public void CreateBankAccount_ShouldThrowException_WhenBalanceIsNegative()
        {
            Assert.Throws<ArgumentException>(() => _factory.CreateBankAccount("Test", -100, 1));
        }

        [Fact]
        public void CreateBankAccount_ShouldThrowException_WhenNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => _factory.CreateBankAccount("", 100, 1));
        }
    }
}