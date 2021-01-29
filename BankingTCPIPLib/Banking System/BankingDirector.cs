using BankingTCPIPLib.Banking_System.BankingOperations;
using BankingTCPIPLib.Banking_System.Miscellaneous;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BankingTCPIPLib.Banking_System
{
    public class BankingDirector
    {
        private BankDepartment _bankDepartment = new();
        private const string LocalDb = @"Users.bin";

        public void RegisterNewUser(string account, string id)
        {
            if (!File.Exists(LocalDb))
                File.Create(LocalDb);

            if (_bankDepartment.Accounts.Count == 0)
                LoadDb();

            var userAccount = JsonConvert.DeserializeObject<AccountHolder>(account);
            if (_bankDepartment.Accounts.Find(a => a.Name == userAccount.Name && a.Surname == userAccount.Surname) !=
                null)
                throw new OperationCanceledException("User with this name and surname already exist");
            
            _bankDepartment.RegisterAccount(userAccount, id);

            WriteDb();
        }
        public void LoginUser(string name, string password, string id)
        {
            if (!File.Exists(LocalDb))
                File.Create(LocalDb);
            if (_bankDepartment.Accounts.Count == 0)
                LoadDb();
            var account = _bankDepartment.Accounts.Find(a => a.Name == name && a.Password == password);
            if (account == null) throw new OperationCanceledException($"User with id {id} doesn't exist, wrong password or name!");
            account.BankAccount.RegisterBankAccount(id);
            WriteDb();
            Console.WriteLine($"User with id: {account.BankAccount.UniqueId} logged in account");
        }
        public void OpenAccount(string id)
        {
            LoadDb();
            var account = _bankDepartment.Accounts.Find(a => a.BankAccount.UniqueId == id);
            if (account == null) throw new OperationCanceledException("User doesn't exist");
                
            _bankDepartment.ActivateAccount(account);
            WriteDb();
            Console.WriteLine($"User with id: {account.BankAccount.UniqueId} opened his account");
        }
        public void MakeDeposit(decimal amount, string id)
        {
            LoadDb();
            DepositOperation deposit = new();
                
            var account = _bankDepartment.Accounts.Find(a => a.BankAccount.UniqueId == id);
            if (account == null) throw new OperationCanceledException("User doesn't exist");

            deposit.WithdrawalAccrueMoney(account, amount);
            WriteDb();
            Console.WriteLine($"User with id: {account.BankAccount.UniqueId} made deposit, money amount: {amount}");
        }
        public void WithdrawalFromAccount(decimal amount, string id)
        {
            LoadDb();
            RemovalOperation removal = new();
                
            var account = _bankDepartment.Accounts.Find(a => a.BankAccount.UniqueId == id);
            if (account == null) throw new OperationCanceledException("User doesn't exist");
                
            removal.WithdrawalAccrueMoney(account, amount);
            WriteDb();
            Console.WriteLine($"User with id: {account.BankAccount.UniqueId} withdrawal money fromm account, money amount: {amount}");
        }
        public decimal GetBalance(string id)
        {
            LoadDb();
            var account = _bankDepartment.Accounts.Find(a => a.BankAccount.UniqueId == id);
            if (account == null) throw new OperationCanceledException("User doesn't exist");
            
            Console.WriteLine($"User with id: {account.BankAccount.UniqueId} checked account balance");
            
            return account.BankAccount.GetMoneyAmount();
        }

        private void LoadDb()
        {
            var lines = File.ReadAllLines(LocalDb, Encoding.UTF8);
            _bankDepartment = new BankDepartment();
            
            foreach (var user in lines)
            {
                var result = JObject.Parse(user);
                var items = result["BankAccount"]?.Children().ToArray();

                var bankAccount = new BankAccount(
                    Convert.ToDecimal(items?[0].First),
                    items?[1].First?.ToString(),
                    null,
                    Convert.ToBoolean(items?[2].First)
                );
                var deserializedUser = JsonConvert.DeserializeObject<AccountHolder>(user);
                deserializedUser.BankAccount = bankAccount;

                _bankDepartment.Accounts.Add(deserializedUser);
            }
        }

        private void WriteDb()
        {
            var serializedUsers = _bankDepartment.Accounts.Select(JsonConvert.SerializeObject).ToArray();
            File.WriteAllLines(LocalDb, serializedUsers);
        }

    }
}