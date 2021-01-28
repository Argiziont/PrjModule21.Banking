using System;
using BankingTCPIPLib.Banking_System.Interfaces;
using BankingTCPIPLib.Banking_System.Miscellaneous;

namespace BankingTCPIPLib.Banking_System.BankingOperations
{
    public class RemovalOperation : IAccountMoneyChange
    {
        /// <summary>
        ///     Removes value of money from account
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
                    Description = "Account isn't active couldn't provide transaction", Sender = account,
                    MoneyAmount = money
                });
                throw new OperationCanceledException("Account isn't Active");
            }

            var transactionTmp = account.Copy();
            try
            {
                transactionTmp.BankAccount.WithdrawMoneyAmountToAccount(money);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e);
                throw;
            }

            account.BankAccount.WithdrawMoneyAmountToAccount(money);
            account.BankAccount.AddOperation(new BankingTransaction
                {Description = $"Removing {money} from Account", Sender = account, MoneyAmount = money});
        }
    }
}