using System;
using System.Threading.Tasks;
using BankingTCPIPLib;

namespace BankingServer
{
    internal static class Program
    {
        private static Task _listenThread;

        private static void Main()
        {
            try
            {
                _listenThread = Task.Factory.StartNew(ServerObject.Listen, TaskCreationOptions.LongRunning);
                _listenThread.Wait();
            }
            catch (Exception ex)
            {
                ServerObject.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}