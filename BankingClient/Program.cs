using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BankingTCPIPLib.Banking_System;
using BankingTCPIPLib.Banking_System.Miscellaneous;
using Newtonsoft.Json;

namespace BankingClient
{
    internal static class Program
    {
        private const string Host = "127.0.0.1";
        private const int Port = 8888;

        private static TcpClient _client;
        private static NetworkStream _stream;

        private static void Main()
        {
            try
            {
                //TCP client
                _client = new TcpClient();

                //TCP stream
                _client.Connect(Host, Port);
                _stream = _client.GetStream();

                #region Client login/register

                string output;
                Console.WriteLine("Welcome to the banking system");
                Console.WriteLine("Want to register? (y/n)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("\r");
                    Console.Write("Write name : ");
                    var name = Console.ReadLine();
                    Console.Write("Write surname : ");
                    var lastName = Console.ReadLine();
                    Console.Write("Write password : ");
                    var password = Encryptor.Md5Hash(Console.ReadLine());
                    while (password.Length < 6)
                    {
                        Console.WriteLine("Password is too week needed minimum 6 chars");
                        Console.Write("Write password : ");
                        password = Encryptor.Md5Hash(Console.ReadLine());
                    }

                    var person = new AccountHolder
                        {BankAccount = new BankAccount(), Name = name, Surname = lastName, Password = password};
                    output = "register" + ' ' + JsonConvert.SerializeObject(person);
                }
                else
                {
                    Console.WriteLine("\r");
                    Console.Write("Write name : ");
                    var name = Console.ReadLine();
                    Console.Write("Write password : ");
                    var password = Encryptor.Md5Hash(Console.ReadLine());
                    output = "enter" + ' ' + name + ' ' + password;
                }

                #endregion

                var data = Encoding.Unicode.GetBytes(output);
                _stream.Write(data, 0, data.Length);

                var receiveThread = new Thread(ReceiveMessage);

                receiveThread.Start();
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        private static void SendMessage()
        {
            Console.WriteLine(
                "Enter command: \r\n1. account opening;\r\n2. depositing funds into the account;\r\n3. withdrawal of funds from the account;\r\n4. account balance control. ");

            while (true)
            {
                var message = Console.ReadLine();

                if (message == "2" || message == "3")
                {
                    Console.Write("Write amount of money: ");
                    var money = Console.ReadLine();
                    message += ' ' + money;
                }

                var data = Encoding.Unicode.GetBytes(message ?? throw new InvalidOperationException());
                _stream.Write(data, 0, data.Length);
            }
        }

        private static void ReceiveMessage()
        {
            while (true)
                try
                {
                    var data = new byte[64];
                    var builder = new StringBuilder();
                    do
                    {
                        var bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (_stream.DataAvailable);

                    var message = builder.ToString();
                    if (message.Split("  ")[0] == "Error") throw new Exception(message.Split("  ")[1]);
                    Console.WriteLine(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Connection was suspected!");
                    Console.ReadLine();
                    Disconnect();
                }
        }

        private static void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            Environment.Exit(0);
        }
    }
}