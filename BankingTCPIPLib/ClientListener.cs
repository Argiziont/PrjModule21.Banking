using System;
using System.Net.Sockets;

namespace BankingTCPIPLib
{
    public class ClientListener
    {
        private readonly TcpClient _tcpClient;

        internal ClientListener(TcpClient tcpTcpClient)
        {
            Id = Guid.NewGuid().ToString();
            _tcpClient = tcpTcpClient;
        }

        internal string Id { get; }
        internal NetworkStream Stream => _tcpClient.GetStream();

        /// <summary>
        ///     Close connection for this client
        /// </summary>
        internal void Close()
        {
            Stream?.Close();
        }
    }
}