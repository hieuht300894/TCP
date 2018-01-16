using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static int BufferSize = 1024;
        public static int PortNumber = 9999;
        public static IPAddress AddressIP = IPAddress.Parse("127.0.0.1");

        static void Main(string[] args)
        {
            Start();
        }

        async static void Start()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    TcpListener listener = new TcpListener(AddressIP, PortNumber);

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
                catch (Exception ex)
                {
                    Start();
                }

                Console.Read();
            });
        }
    }
}
