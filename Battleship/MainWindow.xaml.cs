using System;
using System.Windows;
using System.Windows.Controls;

namespace Battleship
{

    public partial class MainWindow : Window
    {
        private Grid __grid = new Grid();

        private Setup __setup;
        private MultiSetup __multi;
        private ShipArrange __shipArrange;
        private AIPlay __aiPlay;
        private NetPlay __netPlay;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }
      
        private void SetWindowDimensions(int width, int height) {
            MinWidth = width;
            MinHeight = height;
            Width = width;
            Height = height;
        }

        private void InitializeGame()
        {
            Content = __grid;

            __grid.Children.Clear();
            SetWindowDimensions(340, 260);

            __setup = new Setup();
            __grid.Children.Add(__setup);

            __setup.singleEventHandler += new EventHandler(shipSetup);
            __setup.multiEventHandler += new EventHandler(multiSetup);
        }

        private void multiSetup(object sender, EventArgs e)
        {
            //Close setup
            __grid.Children.Clear();
            SetWindowDimensions(340, 300);

            __multi = new MultiSetup();

            __grid.Children.Add(__multi);

            __multi.playEventHandler += new EventHandler(shipSetup);
        }

        private void shipSetup(object sender, EventArgs e)
        {
            //Close setup
            __grid.Children.Clear();
            SetWindowDimensions(460, 500);

            //Initialize ship placement phase
            __shipArrange = new ShipArrange();

            //Add ship placement grid
            __grid.Children.Add(__shipArrange);

            __shipArrange.playEventHandler += new EventHandler(playGame);
        }

        private void playGame(object sender, EventArgs e)
        {
            __grid.Children.Clear();
            SetWindowDimensions(675, 400);

            if (__setup.mode == Mode.Single)
            {
                __aiPlay = new AIPlay(
                    __setup.name,
                    __shipArrange.playerGrid,
                    __shipArrange.playerFleet
                );
                __grid.Children.Add(__aiPlay);
            }
            else
            {
                __netPlay = new NetPlay(
                    __setup.name,
                    __shipArrange.playerGrid,
                    __shipArrange.playerFleet,
                    __multi.isServer,
                    __multi.connection
                );
                __grid.Children.Add(__netPlay);
            }
        }
    }
}
