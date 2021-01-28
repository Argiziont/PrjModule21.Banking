using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BankingTCPIPLib.Banking_System;
using BankingTCPIPLib.Banking_System.BankingOperations;
using Newtonsoft.Json;

namespace BankingTCPIPLib
{
    public class ServerObject
    {
        private static TcpListener _tcpListener;
        private readonly List<ClientObject> _clients = new();

        private readonly List<AccountHolder> _users= new();
        private readonly BankDepartment _bankDepartment = new();

        private static string  _localDb = @"Users.bin";

        /// <summary>
        ///     Add new connection
        /// </summary>
        /// <param name="clientObject">Client to work with</param>
        public void AddConnection(ClientObject clientObject)
        {
            _clients.Add(clientObject);
        }

        /// <summary>
        ///     Delete client from connection
        /// </summary>
        /// <param name="id">Client ID</param>
        public void RemoveConnection(string id)
        {
            var client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                _clients.Remove(client);
        }

        /// <summary>
        ///     Server listener which checks if someone send message
        /// </summary>
        public void Listen()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 8888);
                _tcpListener.Start();
                Console.WriteLine("Server is running. Waiting for connections...");

                ReadUsers();

                while (true)
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();

                    var clientObject = new ClientObject(tcpClient, this);
                    var clientThread = new Thread(clientObject.Process);
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void RegisterNewUser(string account, string id)
        {
            if (!File.Exists(_localDb))
                File.Create(_localDb);

            File.AppendAllLines(_localDb,new []{ account });
            _bankDepartment.RegisterAccount(JsonConvert.DeserializeObject<AccountHolder>(account), id);
        }
        public bool LoginUser(string name,string password, string id)
        {
            if (!File.Exists(_localDb))
                File.Create(_localDb);

            if (_users.FindIndex(a => a.Name == name && a.Password == password) == -1)
                return false;

            _bankDepartment.RegisterAccount(_users.Find(a => a.Name == name && a.Password == password),id);
            return true;
        }

        public bool OpenAccount(string id)
        {
            try
            {
                var account = _users.Find(a => a.BankAccount.UniqueId == id);
                _bankDepartment.ActivateAccount(account);
                WriteUsers();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool MakeDeposit(int amount, string id)
        {
            try
            {
                DepositOperation deposit = new();
                var account = _users.Find(a => a.BankAccount.UniqueId == id);
                deposit.WithdrawalAccrueMoney(account, amount);
                WriteUsers();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool WithdrawalFromAccount(int amount, string id)
        {
            try
            {
                RemovalOperation removal = new();
                var account = _users.Find(a => a.BankAccount.UniqueId == id);
                removal.WithdrawalAccrueMoney(account, amount);
                WriteUsers();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public decimal GetBalance(string id)
        {
            var account = _users.Find(a => a.BankAccount.UniqueId == id);
            return account?.BankAccount.MoneyAmount ?? throw new OperationCanceledException("Couldn't get money from account");
        }

        /// <summary>
        ///     Close connection for each client end stop server
        /// </summary>
        public void Disconnect()
        {
            _tcpListener.Stop();

            WriteUsers();

            foreach (var t in _clients)
                t.Close();

            Environment.Exit(0);
        }

        private void ReadUsers()
        {
            var lines= File.ReadAllLines(_localDb, Encoding.UTF8);
            foreach (var user in lines)
            {
                _users.Add(JsonConvert.DeserializeObject<AccountHolder>(user));
            }
        }

        private void WriteUsers()
        {
            var serializedUsers = _users.Select(JsonConvert.SerializeObject).ToArray();
            File.WriteAllLines(_localDb,serializedUsers);
        }
    }
}