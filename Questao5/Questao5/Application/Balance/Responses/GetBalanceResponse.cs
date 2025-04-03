namespace Questao5.Application.Balance;

public class GetBalanceByIdResponse
{
    public int AccountNumber { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string ResponseDateTime { get; set; } = string.Empty;
}