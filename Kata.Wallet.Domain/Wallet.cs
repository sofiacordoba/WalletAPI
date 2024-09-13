namespace Kata.Wallet.Domain;

public class Wallet
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public string UserDocument { get; set; }
    public string UserName { get; set; }
    public string Currency { get; set; }
    public List<Transaction>? IncomingTransactions { get; set; }
    public List<Transaction>? OutgoingTransactions { get; set; }
}

