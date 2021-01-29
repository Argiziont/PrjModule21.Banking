using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BankingTCPIPLib.Banking_System;
using BankingTCPIPLib.Banking_System.Miscellaneous;

namespace BankingTCPIPLib
{
    public static class ServerObject
    {
        private static TcpListener _tcpListener;
        private static readonly List<ClientListener> Clients = new();
        private static readonly BankingDirector BankingDirector = new();

        /// <summary>
        ///     Add new connection
        /// </summary>
        /// <param name="clientListener">Client to work with</param>
        private static void AddConnection(ClientListener clientListener)
        {
            Clients.Add(clientListener);
        }

        /// <summary>
        ///     Delete client from connection
        /// </summary>
        /// <param name="id">Client ID</param>
        private static void RemoveConnection(string id)
        {
            var client = Clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                Clients.Remove(client);
        }

        /// <summary>
        ///     Server listener which checks if someone send message
        /// </summary>
        public static void Listen()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 8888);
                _tcpListener.Start();

                Console.WriteLine("Server is running. Waiting for connections...");

                while (true)
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();

                    var clientObject = new ClientListener(tcpClient);
                    AddConnection(clientObject);

                    var clientThread =
                        new Thread(() =>
                            ProcessClientMessages(clientObject.Id)); //Start processing client thread
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        ///     Close connection for each client end stop server
        /// </summary>
        public static void Disconnect()
        {
            _tcpListener.Stop();

            foreach (var t in Clients)
                t.Close();

            Environment.Exit(0);
        }

        private static void RegisterNewUser(string account, string id)
        {
            BankingDirector.RegisterNewUser(account, id);
        }

        private static void LoginUser(string name, string password, string id)
        {
            BankingDirector.LoginUser(name, password, id);
        }

        private static void CreateBankingResponse(BankingResponse response, string id, decimal amount = 0)
        {
            switch (response)
            {
                case BankingResponse.OpenAccount:
                    try
                    {
                        BankingDirector.OpenAccount(id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SendMessageToStream("Error" + "  " + e.Message, id);
                        throw;
                    }


                    break;
                case BankingResponse.MakeDeposit:
                    try
                    {
                        BankingDirector.MakeDeposit(amount, id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SendMessageToStream("Error" + "  " + e.Message, id);
                        throw;
                    }

                    break;
                case BankingResponse.WithdrawalFromAccount:
                    try
                    {
                        BankingDirector.WithdrawalFromAccount(amount, id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SendMessageToStream("Error" + "  " + e.Message, id);
                        throw;
                    }

                    break;
                case BankingResponse.GetBalance:
                    try
                    {
                        SendMessageToStream(BankingDirector.GetBalance(id).ToString(CultureInfo.CurrentCulture), id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SendMessageToStream("Error" + "  " + e.Message, id);
                        throw;
                    }

                    break;
                default:
                    var exception =
                        new ArgumentOutOfRangeException(nameof(response), response, "There is no such operation");
                    Console.WriteLine(exception.Message);
                    SendMessageToStream("Error" + "  " + exception.Message, id);
                    throw exception;
            }
        }

        private static void SendMessageToStream(string message, string id)
        {
            var data = Encoding.Unicode.GetBytes(message);

            Clients.Find(c => c.Id == id)?
                .Stream.Write(data, 0, data.Length);
        }

        /// <summary>
        ///     Gets message from network stream
        /// </summary>
        /// <returns>String representation of byte array</returns>
        private static string GetMessageFromStream(NetworkStream stream)
        {
            var data = new byte[64];
            var builder = new StringBuilder();
            do
            {
                var bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (stream.DataAvailable);

            return builder.ToString();
        }

        /// <summary>
        ///     Main client function, sends message to server
        /// </summary>
        private static void ProcessClientMessages(string id)
        {
            try
            {
                var stream =
                    Clients.Find(c => c.Id == id)?.Stream;
                var message = GetMessageFromStream(stream);

                #region Client login/register

                var splitMessage = message.Split(' ');
                if (splitMessage[0] == "register")
                    try
                    {
                        RegisterNewUser(splitMessage[1], id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SendMessageToStream("Error" + "  " + e.Message, id);
                    }

                if (splitMessage[0] == "enter")
                {
                    var password = splitMessage[2];
                    try
                    {
                        LoginUser(splitMessage[1], password, id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        SendMessageToStream("Error" + "  " + e.Message, id);
                    }
                }

                #endregion

                while (true)
                    try
                    {
                        splitMessage = GetMessageFromStream(stream).Split(' ');

                        CreateBankingResponse((BankingResponse) Convert.ToInt32(splitMessage[0]), id,
                            splitMessage.Length > 1 ? Convert.ToDecimal(splitMessage[1]) : 0);
                    }
                    catch
                    {
                        // ignored
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                RemoveConnection(id);
                Clients.Find(c => c.Id == id)?.Stream.Close();
            }
        }
    }
}