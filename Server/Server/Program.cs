using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static int BufferSize = 1024;
        public static int PortNumber = 9999;
        public static IPAddress AddressIP = IPAddress.Parse("127.0.0.1");
        public static Dictionary<string, TcpClient> dClients = new Dictionary<string, TcpClient>();
        public static TcpListener listener = new TcpListener(AddressIP, PortNumber);

        static void Main(string[] args)
        {
            Start2();
            Console.Read();
        }

        static void Start1()
        {
            try
            {
                // 1. listen
                listener.Start();
                Console.WriteLine($"Server started on {((IPEndPoint)listener.LocalEndpoint).ToString()}");
                Console.WriteLine("Waiting for a connection...");

                Socket socket = listener.AcceptSocket();
                Stream stream = new NetworkStream(socket);
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                Console.WriteLine($"Connection received from {((IPEndPoint)socket.RemoteEndPoint).ToString()}");

                while (true)
                {
                    // 2. receive
                    string strReceive = reader.ReadLine().TrimEnd('\0');
                    Console.WriteLine($"Received: {strReceive}");

                    if (strReceive.ToLower().StartsWith("break"))
                        break;

                    // 3. send
                    string strSend = $"Hello {strReceive}";
                    writer.WriteLine(strSend);
                    Console.WriteLine($"Sended: {strSend}");
                }

                // 4. close
                Console.WriteLine($"Disconnection received from {((IPEndPoint)socket.RemoteEndPoint).ToString()}");
                stream.Close();
                socket.Close();
                listener.Stop();
            }
            catch { }
        }

        static void Start2()
        {
            try
            {
                listener.Start();
                Console.WriteLine($"Server started on {((IPEndPoint)listener.LocalEndpoint).ToString()}");
                Console.WriteLine("Waiting for connections...");

                start:
                TcpClient client = listener.AcceptTcpClient();
                dClients.Add(((IPEndPoint)client.Client.RemoteEndPoint).ToString(), client);
                ThreadPool.QueueUserWorkItem(CreateAConnection, client);
                goto start;
            }
            catch { }
        }

        static void CreateAConnection(object obj)
        {
            try
            {
                TcpClient client = (TcpClient)obj;
                string remote = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();

                Stream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                Console.WriteLine($"Connection received from {remote}");

                while (client.Connected)
                {
                    // 2. receive
                    string strReceive = reader.ReadLineAsync().Result;
                    Console.WriteLine($"Received from {remote}: {strReceive.TrimEnd('\0')}");

                    if (strReceive.ToLower().StartsWith("break"))
                        break;

                    // 3. send
                    string strSend = $"Hello {strReceive}";
                    Console.WriteLine($"Sended to {remote}: {strSend}");
                    writer.WriteLineAsync(strSend);
                }

                // 4. close
                Console.WriteLine($"Disconnection received from {remote}");
                stream.Close();
                client.Close();
            }
            catch { }
            finally { RemoveDisconnections(); }

        }

        static void RemoveDisconnections()
        {
            for (int i = dClients.Count - 1; i >= 0; i--)
            {
                var dClient = dClients.ToList()[i];
                if (!dClient.Value.Connected)
                    dClients.Remove(dClient.Key);

                if (dClients.Count == 0)
                {
                    Console.WriteLine("No connection...");
                    listener.Server.Shutdown(SocketShutdown.Both);
                    listener.Stop();
                }
            }
        }
    }
}
