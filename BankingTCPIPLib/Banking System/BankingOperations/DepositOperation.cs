using System;
using BankingTCPIPLib.Banking_System.Interfaces;
using BankingTCPIPLib.Banking_System.Miscellaneous;

namespace BankingTCPIPLib.Banking_System.BankingOperations
{
    public class DepositOperation : IAccountMoneyChange
    {
        /// <summary>
        ///     Adds value of money to account
        /// </summary>
        /// <param name="account">Given account</param>
        /// <param name="money">Money amount</param>
        public void WithdrawalAccrueMoney(AccountHolder account, decimal money)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (!account.BankAccount.IsAccountActive)
            {
                account.BankAccount.AddOperation(new BankingTransaction
                {
                    Description = "Account isn't active couldn't provide transaction", Getter = account,
                    MoneyAmount = money
                });
                throw new OperationCanceledException("Account isn't Active");
            }

            account.BankAccount.AddMoneyAmountToAccount(money);
            //account.BankAccount.AddOperation(new BankingTransaction
            //    {Description = $"Adding {money} to Account", Getter = account, MoneyAmount = money});
        }
    }
}