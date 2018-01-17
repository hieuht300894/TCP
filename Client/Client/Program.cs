using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static int BufferSize = 1024;
        public static int ServerPortNumber = 9999;
        public static int ClientPortNumber = 4001;
        public static IPAddress ServerIPAddress = IPAddress.Parse("127.0.0.1");
        public static IPAddress ClientIPAddress = IPAddress.Parse("127.0.0.1");
        public static TcpClient client;

        public static void Main()
        {
            Start();
            Console.Read();
        }

        static void Start()
        {
            try
            {
                client = new TcpClient(new IPEndPoint(ClientIPAddress, ClientPortNumber));

                // 1. connect
                Console.WriteLine("Waiting to connect to server...");
                client.Connect(ServerIPAddress, ServerPortNumber);

                Stream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                string local = ((IPEndPoint)client.Client.LocalEndPoint).ToString();
                string remote = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
                Console.WriteLine($"Client started on {local}");
                Console.WriteLine($"Connected to {remote}");

                while (client.Connected)
                {
                    string strRead = reader.ReadLine();
                    Console.WriteLine($"Received: {strRead.TrimEnd('\0')}");

                    if (!string.IsNullOrWhiteSpace(strRead))
                    {
                        Console.WriteLine("Enter your message: ");
                        string strSend = string.Empty;
                        writer.WriteLine(strSend);
                    }
                }

                // 4. Close    
                Console.WriteLine($"Disconnected to {remote}");
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                client.Close();
                Start();
            }
        }
    }
}
