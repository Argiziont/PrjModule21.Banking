using System;
using BankingTCPIPLib.Banking_System.Interfaces;
using BankingTCPIPLib.Banking_System.Miscellaneous;

namespace BankingTCPIPLib.Banking_System.BankingOperations
{
    public class InterAccountTransaction : IAccountToAccountTransaction
    {
        /// <summary>
        ///     Safely transmits money between account
        /// </summary>
        /// <param name="depositHolder">Sink account</param>
        /// <param name="creditHolder">Source account</param>
        /// <param name="amount">Money amount</param>
        public void MakeInterAccountTransaction(AccountHolder depositHolder, AccountHolder creditHolder, decimal amount)
        {
            if (depositHolder == null) throw new ArgumentNullException(nameof(depositHolder));
            if (creditHolder == null) throw new ArgumentNullException(nameof(creditHolder));
            if (!depositHolder.BankAccount.IsAccountActive)
            {
                depositHolder.BankAccount.AddOperation(new BankingTransaction
                {
                    Description = "Deposit account isn't active couldn't provide transaction", Getter = depositHolder,
                    MoneyAmount = amount
                });
                throw new OperationCanceledException("Deposit account isn't Active");
            }

            if (!creditHolder.BankAccount.IsAccountActive)
            {
                creditHolder.BankAccount.AddOperation(new BankingTransaction
                {
                    Description = "Credit account isn't active couldn't provide transaction", Getter = creditHolder,
                    MoneyAmount = amount
                });
                throw new OperationCanceledException("Credit account isn't Active");
            }

            var transactionDepositHolder = depositHolder.Copy();
            var transactionCreditHolder = creditHolder.Copy();

            transactionDepositHolder.BankAccount.AddMoneyAmountToAccount(amount);
            transactionCreditHolder.BankAccount.WithdrawMoneyAmountToAccount(amount);

            depositHolder.BankAccount.AddMoneyAmountToAccount(amount);
            creditHolder.BankAccount.WithdrawMoneyAmountToAccount(amount);

            depositHolder.BankAccount.AddOperation(new BankingTransaction
            {
                Description =
                    $"Transfer {amount} to this account from account with id \"{creditHolder.BankAccount.UniqueId}\"",
                Getter = depositHolder,
                MoneyAmount = amount, Sender = creditHolder
            });

            creditHolder.BankAccount.AddOperation(new BankingTransaction
            {
                Description = $"Transfer {amount} to account with id \"{depositHolder.BankAccount.UniqueId}\"",
                Getter = depositHolder,
                MoneyAmount = amount,
                Sender = creditHolder
            });
        }
    }
}