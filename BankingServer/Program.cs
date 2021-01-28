using System;
using System.Threading;
using BankingTCPIPLib;

namespace BankingServer
{
    internal static class Program
    {
        private static ServerObject _server;
        private static Thread _listenThread;

        private static void Main()
        {
            try
            {
                _server = new ServerObject();
                _listenThread = new Thread(_server.Listen);
                _listenThread.Start();
            }
            catch (Exception ex)
            {
                _server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
