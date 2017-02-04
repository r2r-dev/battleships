using System;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Battleship
{
    public partial class MultiSetup : UserControl
    {
        private EventHandler __playEventHandler;
        public EventHandler playEventHandler
        {
            get { return __playEventHandler; }
            set { __playEventHandler = value; }
        }

        private bool __isServer = false;
        public bool isServer
        {
            get { return __isServer; }
        }

        private SocketManagement __connection;
        public SocketManagement connection
        {
            get { return __connection; }
        }

        public MultiSetup()
        {
            InitializeComponent();
        }

        private bool checkIPandPort(string ip, string port)
        {
            bool isIPCorrect = Regex.IsMatch(
                ip,
                @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"
            );
            bool isPortCorrect = Regex.IsMatch(
                port,
                "^[0-9]{1,6}$"
            );

            //Check the ip and port is in valid format
            if (isIPCorrect && isPortCorrect)
            {
                string[] temp = ip.Split('.');
                foreach (string q in temp)
                {
                    try
                    {
                        if (Int32.Parse(q) > 255) return false;
                    }
                    catch (Exception) { return false; }
                }
                return true;
            }
            return false;
        }

        private void EnableAll()
        {
            joinIpTextBox.IsEnabled = true;
            joinPortTextBox.IsEnabled = true;
            clientButton.IsEnabled = true;
            hostTab.IsEnabled = true;
            clientTab.IsEnabled = true;
        }
        private void DisableAll()
        {
            joinIpTextBox.IsEnabled = false;
            joinPortTextBox.IsEnabled = false;
            clientButton.IsEnabled = false;
            hostTab.IsEnabled = false;
            clientTab.IsEnabled = false;
        }

        private void serverButton_Click(object sender, RoutedEventArgs e)
        {
            DisableAll();
            Action success_callback = () =>
            {
                __playEventHandler(this, e);
            };
            Action failure_callback = () =>
            {
                setupWait.Visibility = Visibility.Collapsed;
                EnableAll();
            };

            if (
                checkIPandPort(
                    hostIpTextBox.Text,
                    hostPortTextBox.Text
                )
            )
            {
                __isServer = true;
                setupWait.Visibility = Visibility.Visible;

                string hostIP = hostIpTextBox.Text;
                int hostPort = Int32.Parse(hostPortTextBox.Text);

                __connection = new SocketManagement(
                    hostIP,
                    hostPort
                );

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                __connection.StartAsServer(
                    success_callback,
                    failure_callback
                );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            else EnableAll();
        }

        private void clientButton_Click(object sender, RoutedEventArgs e)
        {
            DisableAll();
            Action success_callback = () =>
            {
                __playEventHandler(this, e);
            };
            Action failure_callback = () =>
            {
                setupWait.Visibility = Visibility.Collapsed;
                EnableAll();
            };

            if (
                checkIPandPort(
                    joinIpTextBox.Text,
                    joinPortTextBox.Text
                )
            )
            {
                __isServer = false;
                setupWait.Visibility = Visibility.Visible;

                string joinIP = joinIpTextBox.Text;
                int joinPort = Int32.Parse(joinPortTextBox.Text);

                __connection = new SocketManagement(
                    joinIP,
                    joinPort
                );
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                __connection.StartAsClient(
                    success_callback,
                    failure_callback
                );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            else EnableAll();
        }
    }
}