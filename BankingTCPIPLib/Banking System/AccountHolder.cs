using System;
using System.Text.Json.Serialization;
using BankingTCPIPLib.Banking_System.Miscellaneous;

namespace BankingTCPIPLib.Banking_System
{
    [Serializable]
    public class AccountHolder : Human, IEquatable<AccountHolder>
    {
        [JsonInclude] public BankAccount BankAccount { get; init; }
        public string Password { get; init; }

        public bool Equals(AccountHolder other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            return other.BankAccount.UniqueId == BankAccount.UniqueId;
        }

        public AccountHolder()
        {
            
        }
        public AccountHolder Copy()
        {
            var transactionAcc = new BankAccount();
            transactionAcc.AddMoneyAmountToAccount(BankAccount.GetMoneyAmount());
            return new AccountHolder {BankAccount = transactionAcc};
        }
    }
}