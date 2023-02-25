using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace EncryptedPigeon_Server
{
    internal class Program
    {
        static int key = 1;

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

                string recievedString = Encoding.ASCII.GetString(data, 0, recv);
                string decryptedString = Decrypt(recievedString, key);
                Console.WriteLine($"[{tmpRemote}] {decryptedString}");
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

        static string Decrypt(string input, int key)
        {
            int shifter = 0;
            string decryptedInput = "";

            foreach (char c in input)
            {
                int unicodeValue = Convert.ToInt32(c) - (key + shifter);
                char output = Convert.ToChar(unicodeValue);
                decryptedInput += output;

                shifter++;
                if (shifter > 1)
                    shifter = 0;
            }

            return decryptedInput;
        }
    }
}