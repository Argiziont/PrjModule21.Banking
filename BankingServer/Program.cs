using System;
using System.Threading;
using BankingTCPIPLib;

namespace BankingServer
{
    internal static class Program
    {
        private static Thread _listenThread;

        private static void Main()
        {
            try
            {
                _listenThread = new Thread(ServerObject.Listen);
                _listenThread.Start();
            }
            catch (Exception ex)
            {
                ServerObject.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}