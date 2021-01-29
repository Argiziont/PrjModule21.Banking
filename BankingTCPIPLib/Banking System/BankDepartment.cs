using System;
using System.Collections.Generic;
using System.Linq;
using BankingTCPIPLib.Banking_System.Miscellaneous;

namespace BankingTCPIPLib.Banking_System
{
    [Serializable]
    public class BankDepartment
    {
        private Random _rng = new();
        public List<AccountHolder> Accounts { get; private set; } = new();

        /// <summary>
        ///     Registers this account in banking system
        /// </summary>
        /// <param name="account">Account for register</param>
        /// <param name="id"></param>
        public void RegisterAccount(AccountHolder account, string id)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            account.BankAccount.RegisterBankAccount(id);
            Accounts.Add(account);
        }

        /// <summary>
        ///     Disables this account in banking system
        /// </summary>
        /// <param name="account">Account for disabling</param>
        public void DisableAccount(AccountHolder account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (!account.BankAccount.IsAccountActive)
                throw new OperationCanceledException("This account is already disabled");

            account.BankAccount.DisableAccount();
        }

        /// <summary>
        ///     Activates this account in banking system if it was disabled
        /// </summary>
        /// <param name="account">Account for activation</param>
        public void ActivateAccount(AccountHolder account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (!Accounts.Contains(account))
                throw new OperationCanceledException("This account isn't registered yet");

            account.BankAccount.ActivateAccount();
        }

        //public List<BankingTransaction> GetAllAccountsHistory()
        //{
        //    return Accounts.SelectMany(account => account.BankAccount.OperationHistory).ToList();
        //}
    }
}