using System;
using System.Net.Sockets;
using System.Text;
using BankingTCPIPLib.Banking_System;
using Newtonsoft.Json;

namespace BankingTCPIPLib
{
    public class ClientObject
    {
        private readonly TcpClient _client;
        private readonly ServerObject _server;

        private string _userName;

        //private string _userPassword;
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            _client = tcpClient;
            _server = serverObject;
            serverObject.AddConnection(this);
        }

        protected internal string Id { get; }
        protected internal NetworkStream Stream { get; private set; }

        /// <summary>
        ///     Main client function, sends message to server
        /// </summary>
        public void Process()
        {
            try
            {
                Stream = _client.GetStream();
                
                var message = GetMessage();
                var splitMessage = message.Split(' ');
                if (splitMessage[0]=="register")
                {
                    var deserializedHolder = JsonConvert.DeserializeObject<AccountHolder>(splitMessage[1]);
                    _userName = deserializedHolder.Name;
                    _server.RegisterNewUser(splitMessage[1], Id);
                }
                else if (splitMessage[0] == "enter")
                {
                    var password = splitMessage[2];
                    _userName = splitMessage[1];
                    if (!_server.LoginUser(_userName, password, Id))
                        throw new OperationCanceledException("Login or password wasn't right");
                }

                Console.WriteLine("Possible operations");
                while (true)
                    try
                    {
                        message = GetMessage();
                        switch (message)
                        {
                            case "1":
                                _server.OpenAccount(Id);
                                break;
                            case "2":
                                Console.WriteLine("Money amount");
                                _server.MakeDeposit(Convert.ToInt32(Console.ReadLine()),Id);
                                break;
                            case "3":
                                Console.WriteLine("Money amount");
                                _server.WithdrawalFromAccount(Convert.ToInt32(Console.ReadLine()), Id);
                                break;
                            case "4":
                                Console.WriteLine(_server.GetBalance(Id));
                                break;
                            default:
                                throw new ArgumentException("There is no such command");
                        }
                    }
                    catch
                    {
                        Console.WriteLine(message);
                        break;
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        /// <summary>
        ///     Gets message from network stream
        /// </summary>
        /// <returns>String representation of byte array</returns>
        private string GetMessage()
        {
            var data = new byte[64];
            var builder = new StringBuilder();
            do
            {
                var bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);

            return builder.ToString();
        }
        private string GetMessageObject()
        {
            var z = Stream;
            //var data = new byte[64];
            //var builder = new StringBuilder();
            //do
            //{
            //    var bytes = Stream.Read(data, 0, data.Length);
            //    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            //} while (Stream.DataAvailable);

            //return builder.ToString();
            return null;
        }

        /// <summary>
        ///     Close connection for this client
        /// </summary>
        protected internal void Close()
        {
            Stream?.Close();
            _client?.Close();
        }
    }
}