using System;
using System.Windows;
using System.Windows.Controls;

namespace Battleship
{

    public enum Mode { Single, Multi }

    public partial class Setup : UserControl
    {
        private Mode __mode;
        public Mode mode
        {
            get { return __mode; }
        } 

        private string __name;
        public string name
        {
            get { return __name; }
        }

        private EventHandler __singleEventHandler;
        public EventHandler singleEventHandler
        {
            get { return __singleEventHandler; }
            set { __singleEventHandler = value; }
        }

        private EventHandler __multiEventHandler;
        public  EventHandler multiEventHandler
        {
            get { return __multiEventHandler; }
            set { __multiEventHandler = value; }
        }


        public Setup()
        {
            InitializeComponent();
        }

        private void singleButton_Click(object sender, RoutedEventArgs e)
        {
            __name = nameTextBox.Text;
            if (name == "")
            {
                MessageBox.Show(
                    "You must enter a name",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            else
            {
                __mode = Mode.Single;
                __singleEventHandler(this, e);
            }
        }

        private void multiButton_Click(object sender, RoutedEventArgs e)
        {
            __name = nameTextBox.Text;
            if (name == "")
            {
                MessageBox.Show(
                    "You must enter a name",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            else
            {
                __mode = Mode.Multi;
                __multiEventHandler(this, e);
            }
        }
    }
}
