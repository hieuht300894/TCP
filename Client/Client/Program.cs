using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static int BufferSize = 1024;
        public static IPEndPoint ServerHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
        public static IPEndPoint ClientHost = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001);
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
                client = new TcpClient(ClientHost);

                Console.WriteLine("Waiting to connect to server...");
                client.Connect(ServerHost);

                Stream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                string local = ((IPEndPoint)client.Client.LocalEndPoint).ToString();
                string remote = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
                Console.WriteLine($"Client started on {local}");
                Console.WriteLine($"Connected to {remote}");

                writer.WriteLine(clsGeneral.fKey.Ping.ToString());

                while (PingNetwork(ServerHost))
                {
                    string strRead = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(strRead))
                    {
                        strRead = strRead.Trim('\0');
                        Console.WriteLine($"Received: {strRead}");
                    }

                    writer.WriteLine(clsGeneral.fKey.Ping.ToString());
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
        static bool PingNetwork(IPEndPoint iP)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(iP.Address, 1000, new byte[32], new PingOptions(128, true));
            if (reply == null)
                return false;
            if (reply.Status == IPStatus.Success)
                return true;
            return false;
        }
    }
}
