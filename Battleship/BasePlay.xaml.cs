using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Battleship
{

    public abstract partial class BasePlay : UserControl 
    {
        protected Dictionary<string, SolidColorBrush> _colorMapping = new Dictionary<string, SolidColorBrush>{
            {"water", new SolidColorBrush(Colors.White) },
            {"hit", new SolidColorBrush(Colors.Red) },
            {"miss", new SolidColorBrush(Colors.LightGray) },
            {"destroyer", (SolidColorBrush)(new BrushConverter().ConvertFrom("#88cc00")) },
            {"cruiser", (SolidColorBrush)(new BrushConverter().ConvertFrom("#33cc33")) },
            {"submarine", (SolidColorBrush)(new BrushConverter().ConvertFrom("#00e64d")) },
            {"battleship", (SolidColorBrush)(new BrushConverter().ConvertFrom("#00cc00")) },
            {"carrier", (SolidColorBrush)(new BrushConverter().ConvertFrom("#00e600")) }
        };

        protected delegate void ShotEvent(int row, int column, string hitName, string squareName);
        protected ShotEvent _shotEvent;

        protected string _lastShot;

        protected bool _isMyTurn = false;
        protected bool _isFinished = false;

        protected Grid[] _playerGrid;
        protected Fleet _playerFleet;

        protected Grid[] _enemyGrid;
        protected Fleet _enemyFleet;
        protected string _enemyName;


        protected abstract void CheckTurn();

        public BasePlay(string playerName, Grid[] playerGrid, Fleet playerFleet)
        {
            _playerFleet = playerFleet;
            _enemyFleet = new Fleet();

            InitializeComponent();
            initiateSetup(playerGrid);

            playerNameLabel.Content = playerName;
        }

        private void initiateSetup(Grid[] playerGrid)
        {
            _enemyGrid = new Grid[100];
            CompGrid.Children.CopyTo(
                _enemyGrid,
                0
            );
            for (int i = 0; i < 100; i++)
            {
                _enemyGrid[i].Tag = "water";
            }

            //Set player grid
            _playerGrid = new Grid[100];
            PlayerGrid.Children.CopyTo(
                _playerGrid,
                0
            );

            for (int i = 0; i < 100; i++)
            {
                _playerGrid[i].Background = playerGrid[i].Background;
                _playerGrid[i].Tag = playerGrid[i].Tag.ToString();
            }
        }

        protected virtual void gridMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set sender to square chosen
            Grid square = (Grid)sender;
            string squareName = square.Tag.ToString();

            string shotName;

            int column = Grid.GetColumn(square);
            int row = Grid.GetRow(square);

            //Check if player turn yet
            if (!_isMyTurn || _isFinished)
            {
                return;
            }

            switch (squareName)
            {
                case "water":
                    shotName = "miss";
                    break;
                case "miss":
                case "hit":
                    return;
                default:
                    shotName = "hit";
                    _enemyFleet[squareName].points--;
                    break;
            }

            square.Tag = shotName;
            square.Background = _colorMapping[shotName];

            
            if (_shotEvent != null) _shotEvent(
                row,
                column,
                shotName,
                squareName
            );

            _isMyTurn = false;
            CheckResult();
            CheckTurn();
        }

        protected void CheckResult()
        {
            Action<Fleet, String, String> checkPlayer = (
                Fleet fleet,
                String sunkMessage,
                String resultMessage
            ) => {
                int sunkShips = 0;
                foreach (KeyValuePair<string, dynamic> entry in fleet)
                {
                    if (entry.Value.points == 0)
                    {
                        entry.Value.points = -1;
                        MessageBox.Show(
                            String.Format(
                                "{0} {1}!",
                                sunkMessage,
                                entry.Value.name
                            )
                        );
                        sunkShips++;
                    }
                    else if (entry.Value.points == -1)
                    {
                        sunkShips++;
                    }
                }
                if (sunkShips == fleet.Count)
                {
                    turnWait.Visibility = Visibility.Collapsed;
                    disableGrids();
                    MessageBox.Show(resultMessage);
                    Environment.Exit(0);
                }
            };

            checkPlayer(
                _enemyFleet,
                String.Format("You sunk {0}", _enemyName),
                "You won!"
            );
            checkPlayer(
                _playerFleet,
                String.Format("{0} sunk your", _enemyName),
                "You lost!"
            );
        }

        private void disableGrids()
        {
            foreach (Grid element in _enemyGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = _colorMapping["miss"];
                }
                else if (!element.Tag.Equals("hit") && !element.Tag.Equals("miss"))
                {
                    element.Background = new SolidColorBrush(Colors.LightGreen);
                }
                element.IsEnabled = false;
            }

            foreach (Grid element in _playerGrid)
            {
                if (element.Tag.Equals("water"))
                {
                    element.Background = _colorMapping["miss"];
                }
                element.IsEnabled = false;
            }
        }
    }
}
