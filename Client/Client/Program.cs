using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static int BufferSize = 1024;
        public static int PortNumber = 9999;
        public static IPAddress AddressIP = IPAddress.Parse("127.0.0.1");

        public static void Main()
        {
            Start();
            Console.Read();
        }

        static void Start()
        {
            try
            {
                TcpClient client = new TcpClient();

                // 1. connect
                Console.WriteLine("Waiting to connect to server...");
                client.Connect(AddressIP, PortNumber);
                Stream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                string local = ((IPEndPoint)client.Client.LocalEndPoint).ToString();
                string remote = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
                Console.WriteLine($"Client started on {local}");
                Console.WriteLine($"Connected to {remote}");

                while (client.Connected)
                {
                    // 2. send
                    Console.Write("Enter your message: ");
                    string strSend = Console.ReadLine().TrimEnd('\0');
                    writer.WriteLineAsync(strSend);

                    if (strSend.ToLower().StartsWith("break"))
                        break;

                    // 3. receive
                    string strRead = reader.ReadLine();
                    Console.WriteLine($"Received: {strRead.TrimEnd('\0')}");
                }

                // 4. Close    
                Console.WriteLine($"Disconnected to {remote}");
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Start();
            }
        }
    }
}
