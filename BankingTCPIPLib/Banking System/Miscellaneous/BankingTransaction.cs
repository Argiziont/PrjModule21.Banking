using System.Text.Json.Serialization;

namespace BankingTCPIPLib.Banking_System.Miscellaneous
{
    public class BankingTransaction
    {
        [JsonIgnore] public AccountHolder Sender { get; init; }

        [JsonIgnore] public AccountHolder Getter { get; init; }

        public decimal MoneyAmount { get; init; }
        public string Description { get; init; }
    }
}