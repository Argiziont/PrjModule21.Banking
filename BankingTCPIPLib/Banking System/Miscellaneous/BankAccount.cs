using System;
using System.Collections.Generic;

namespace BankingTCPIPLib.Banking_System.Miscellaneous
{
    [Serializable]
    public class BankAccount
    {
        public BankAccount()
        {
        }

        public BankAccount(decimal moneyAmount, string uniqueId, List<BankingTransaction> operationHistory,
            bool isAccountActive)
        {
            MoneyAmount = moneyAmount;
            UniqueId = uniqueId;
            // OperationHistory = operationHistory;
            IsAccountActive = isAccountActive;
        }

        public decimal MoneyAmount { get; private set; }

        public string UniqueId { get; private set; }

        //public List<BankingTransaction> OperationHistory { get; private set; }
        public bool IsAccountActive { get; private set; }

        /// <summary>
        ///     Adds given amount of money to this account
        /// </summary>
        /// <param name="amount">Amount of money</param>
        internal void AddMoneyAmountToAccount(decimal amount)
        {
            MoneyAmount += amount;
        }

        /// <summary>
        ///     Removes given amount of money from this account
        /// </summary>
        /// <param name="amount">Amount of money</param>
        internal void WithdrawMoneyAmountToAccount(decimal amount)
        {
            if (MoneyAmount < amount)
                throw new OperationCanceledException("There no such money for this operation on Account");
            MoneyAmount -= amount;
        }

        /// <summary>
        ///     Registers this account in banking system
        /// </summary>
        /// <param name="id">Unique id in bank</param>
        internal void RegisterBankAccount(string id)
        {
            UniqueId = id;
            IsAccountActive = true;
        }

        /// <summary>
        ///     Disables account
        /// </summary>
        internal void DisableAccount()
        {
            IsAccountActive = false;
        }

        /// <summary>
        ///     Activates account if it was disabled
        /// </summary>
        internal void ActivateAccount()
        {
            IsAccountActive = true;
        }

        /// <summary>
        ///     Adds operation to history of this account
        /// </summary>
        internal void AddOperation(BankingTransaction transaction)
        {
            //OperationHistory ??= new List<BankingTransaction>();

            //OperationHistory.Add(transaction);
        }

        ///// <summary>
        /////     Returns operations of this account
        ///// </summary>
        //public List<BankingTransaction> GetOperationHistory()
        //{
        //    return OperationHistory;
        //}

        /// <summary>
        ///     Returns money amount on this account
        /// </summary>
        public decimal GetMoneyAmount()
        {
            return MoneyAmount;
        }
    }
}