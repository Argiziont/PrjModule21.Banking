using System;
using System.Net.Sockets;

namespace BankingTCPIPLib
{
    public class ClientListener
    {
        internal readonly TcpClient TcpClient;
        internal string Id { get; }
        internal NetworkStream Stream => TcpClient.GetStream();

        internal ClientListener(TcpClient tcpTcpClient)
        {
            Id = Guid.NewGuid().ToString();
            TcpClient = tcpTcpClient;
        }

        /// <summary>
        ///     Close connection for this client
        /// </summary>
        internal void Close()
        {
            Stream?.Close();
        }
    }
}