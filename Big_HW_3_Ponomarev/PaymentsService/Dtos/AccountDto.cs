namespace PaymentsService.Dtos;

public class AccountDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}