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
            try
            {
                TcpClient client = new TcpClient();

                // 1. connect
                client.Connect(AddressIP, PortNumber);
                Stream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                Console.WriteLine($"Client started on {((IPEndPoint)client.Client.LocalEndPoint).ToString()}");
                Console.WriteLine($"Connected to {((IPEndPoint)client.Client.RemoteEndPoint).ToString()}");

                while (true)
                {
                    // 2. send
                    Console.Write("Enter your name: ");
                    string strSend = Console.ReadLine().TrimEnd('\0');
                    writer.WriteLine(strSend);

                    if (strSend.ToLower().StartsWith("break"))
                        break;

                    // 3. receive
                    string strRead = reader.ReadLine().TrimEnd('\0');
                    Console.WriteLine($"Received: {strRead}");
                }

                // 4. Close    
                Console.WriteLine($"Disconnected to {((IPEndPoint)client.Client.RemoteEndPoint).ToString()}");
                stream.Close();
                client.Close();
                Console.Read();
            }

            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex);
                Main();
            }

            Console.Read();
        }
    }
}
