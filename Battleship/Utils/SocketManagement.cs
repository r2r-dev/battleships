using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.IO;

namespace Battleship
{
    public class SocketManagement
    {
        private IPAddress __ip;
        private int __port;
        private TcpListener __tcpListener;
        private TcpClient __tcpClient;
        private NetworkStream __networkStream;

        public SocketManagement(String ip, int port)
        {
            __ip = IPAddress.Parse(ip);
            __port = port;
        }

        public async Task StartAsServer(Action SuccessCallback, Action FailureCallback)
        {
            try
            {
                __tcpListener = new TcpListener(
                    __ip,
                    __port
                );
                __tcpListener.Start();
                __tcpClient = await __tcpListener.AcceptTcpClientAsync();
                __networkStream = __tcpClient.GetStream();
                SuccessCallback();
            }
            catch (SocketException ex) {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                FailureCallback();
            }
        }

        public async Task StartAsClient(Action SuccessCallback, Action FailureCallback)
        {
            try
            {
                __tcpClient = new TcpClient();
                await __tcpClient.ConnectAsync(
                    __ip,
                    __port
                );
                __networkStream = __tcpClient.GetStream();
                SuccessCallback();
            }
            catch (SocketException ex) {
                MessageBox.Show(
                    ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                FailureCallback();
            }
        }

        public bool sendShot(string shot)
        {
            try
            {
                byte[] bytes = new byte[255];
                bytes = new ASCIIEncoding().GetBytes(shot);
                __networkStream.Write(
                    bytes,
                    0,
                    bytes.Length
                );
            }
            catch (IOException)
            {
                ExitWithDisconnectError();
            }
            return true;
        }

        public string getShot()
        {
            string shot = null;
            try {
                byte[] bytes = new byte[255];
                __networkStream.Read(
                    bytes,
                    0,
                    bytes.Length
                );
                shot = new ASCIIEncoding().GetString(bytes).TrimEnd('\0');
            }
            catch (IOException)
            {
                ExitWithDisconnectError();
            }
            return shot;
        }

        public bool sendBoard(int[] obj)
        {
            try
            {
                string temp = "";
                for (int i = 0; i < 100; i++)
                {
                    temp += obj[i];
                }

                byte[] bytes = new byte[255];
                bytes = new ASCIIEncoding().GetBytes(temp);
                __networkStream.Write(
                    bytes,
                    0,
                    bytes.Length
                );
            }
            catch (IOException) {
                ExitWithDisconnectError();
            }
            return true;
        }

        public int[] getBoard()
        {
            int[] obj = null;
            try {
                byte[] bytes = new byte[255];
                __networkStream.Read(
                    bytes,
                    0,
                    bytes.Length
                );
                string temp = new ASCIIEncoding().GetString(bytes);
                char[] charOfTemp = temp.ToCharArray();
                obj = Enumerable.Repeat(0, 100).ToArray();
                for (int i = 0; i < 100; i++)
                {
                    obj[i] = Int32.Parse("" + charOfTemp[i]);
                }
            } catch (IOException) {
                ExitWithDisconnectError();
            }
            return obj;
        }
        private void ExitWithDisconnectError()
        {
            MessageBox.Show(
                "Second player disconnected",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            Environment.Exit(1);
        }
    }
}
