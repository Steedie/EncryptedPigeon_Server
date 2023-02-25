using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace EncryptedPigeon_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Encrypted Pigion Server!");

            int recv;
            byte[] data = new byte[1024];

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 904);

            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            newSocket.Bind(endpoint);

            Console.WriteLine("Waiting for a client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 904);
            EndPoint tmpRemote = (EndPoint)sender;

            while (true)
            {
                recv = newSocket.ReceiveFrom(data, ref tmpRemote);
                Console.WriteLine(DecryptedData(data, recv));
            }

            
            string welcome = "Welcome to my server!";
            data = Encoding.ASCII.GetBytes(welcome);

            if (newSocket.Connected)
            {
                newSocket.Send(data);
            }

            while (true)
            {
                if (!newSocket.Connected)
                {
                    Console.WriteLine("Client disconnected");
                    break;
                }

                data = new byte[1024];
                recv = newSocket.ReceiveFrom(data, ref tmpRemote);

                if (recv == 0)
                    break;

                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            }

            newSocket.Close();
        }

        static string DecryptedData(byte[] data, int recv)
        {
            string decryptedString = "";
            int key = 11;
            int shift = 0;

            for (int i = 0; i < recv; i++)
            {
                int unicode = data[i] - (key + shift);

                if (unicode < 0)
                    unicode = 0;

                char character = Convert.ToChar(unicode);
                decryptedString += character;

                shift += 3;
                if (shift > 15)
                    shift = 0;
            }
            return decryptedString;
        }
    }
}