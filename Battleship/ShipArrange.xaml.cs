using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Battleship
{
    enum Orientation
    {
        VERTICAL,
        HORIZONTAL
    };

    public partial class ShipArrange : UserControl
    {
        private EventHandler __playEventHandler;
        public EventHandler playEventHandler
        {
            get { return __playEventHandler; }
            set { __playEventHandler = value; }
        }

        private Grid[] __playerGrid;
        public Grid[] playerGrid
        {
            get { return __playerGrid; }
        }

        private Fleet __playerFleet;
        public Fleet playerFleet
        {
            get { return __playerFleet; }
        }

        private Orientation __shipOrientation = Orientation.HORIZONTAL;

        private SolidColorBrush __unselectedShipStroke =
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCED7DB"));

        private SolidColorBrush __selectedShipStroke =
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#03A9F4"));

        private int __numShipsPlaced;

        private Path __lastPlacedShip;
        private Path[] __shipPaths;

        public ShipArrange()
        {
            InitializeComponent();
            UIHelper.ReDrawGrid(shipyardGrid);

            __playerGrid = new Grid[] {
                gridA1, gridA2, gridA3, gridA4, gridA5, gridA6, gridA7, gridA8, gridA9, gridA10,
                gridB1, gridB2, gridB3, gridB4, gridB5, gridB6, gridB7, gridB8, gridB9, gridB10,
                gridC1, gridC2, gridC3, gridC4, gridC5, gridC6, gridC7, gridC8, gridC9, gridC10,
                gridD1, gridD2, gridD3, gridD4, gridD5, gridD6, gridD7, gridD8, gridD9, gridD10,
                gridE1, gridE2, gridE3, gridE4, gridE5, gridE6, gridE7, gridE8, gridE9, gridE10,
                gridF1, gridF2, gridF3, gridF4, gridF5, gridF6, gridF7, gridF8, gridF9, gridF10,
                gridG1, gridG2, gridG3, gridG4, gridG5, gridG6, gridG7, gridG8, gridG9, gridG10,
                gridH1, gridH2, gridH3, gridH4, gridH5, gridH6, gridH7, gridH8, gridH9, gridH10,
                gridI1, gridI2, gridI3, gridI4, gridI5, gridI6, gridI7, gridI8, gridI9, gridI10,
                gridJ1, gridJ2, gridJ3, gridJ4, gridJ5, gridJ6, gridJ7, gridJ8, gridJ9, gridJ10
            };

            __shipPaths = new Path[] {
                destroyer,
                cruiser,
                submarine,
                battleship,
                carrier
            };

            __playerFleet = new Fleet();

            reset();
        }


        private void reset()
        {
            horizontalRadioButton.IsChecked = true;
            verticalRadioButton.IsChecked = false;

            foreach (Grid square in __playerGrid)
            {
                square.Tag = "water";
                square.Background = new SolidColorBrush(Colors.White);
            }

            foreach (Path shipPath in __shipPaths)
            {
                shipPath.IsEnabled = true;
                shipPath.Opacity = 100;
                if (shipPath.Stroke != __unselectedShipStroke)
                {
                    shipPath.Stroke = __unselectedShipStroke;
                }
            }
            __numShipsPlaced = 0;
            __lastPlacedShip = null;
        }

        private void ship_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Path shipPath = (Path)sender;
            if (!shipPath.IsEnabled)
            {
                return;
            }
            if (__lastPlacedShip != null)
            {
                __lastPlacedShip.Stroke = __unselectedShipStroke;
            }

            __lastPlacedShip = shipPath;
            shipPath.Stroke = __selectedShipStroke;
        }

        private void gridMouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid square = (Grid)sender;
            int squareIndex = -1;
            
            //Check if ship has been selected
            if (__lastPlacedShip == null)
            {
                MessageBox.Show(
                    "You must choose a ship",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            BaseShip ship = __playerFleet[__lastPlacedShip.Name];

            //Check if square has a ship already in place
            if (!square.Tag.Equals("water"))
            {
                return;
            }

            //Find chosen square. Index should never be -1.
            squareIndex = Array.IndexOf(
                __playerGrid,
                square
            );

            //Check if there is enough space for the ship
            bool canPlaceShip = (
                CanPlaceShipHorizontally(
                    ship.points,
                    squareIndex
                ) ||
                CanPlaceShipVertically(
                    ship.points,
                    squareIndex
                )
            );
            if (!canPlaceShip) return;

            //Set the ship to grid
            if (__shipOrientation.Equals(Orientation.HORIZONTAL))
            {
                PlaceShipHorizontally(
                    ship,
                    squareIndex
                );
            }
            else
            {
                PlaceShipVertically(
                    ship,
                    squareIndex
                );
            }
            __lastPlacedShip.IsEnabled = false;
            __lastPlacedShip.Opacity = 0.5;
            __lastPlacedShip.Stroke = __unselectedShipStroke;
            __lastPlacedShip = null;
            __numShipsPlaced++;
        }

        private void PlaceShipVertically(BaseShip ship, int squareIndex)
        {
            int shipSize = ship.points;
            SolidColorBrush shipColor = ship.color;
            string shipName = ship.name;
            int counter;
            int temp;

            //If two columns
            if (squareIndex + (shipSize * 10) > 100)
            {
                counter = 0;
                temp = 10;
                while ((squareIndex / 10 + counter) % 100 < 10)
                {
                    __playerGrid[squareIndex + counter * 10].Background = shipColor;
                    __playerGrid[squareIndex + counter * 10].Tag = shipName;
                    counter++;
                }
                for (int i = counter; i < shipSize; i++)
                {
                    __playerGrid[squareIndex - temp].Background = shipColor;
                    __playerGrid[squareIndex - temp].Tag = shipName;
                    temp += 10;
                }
            }
            //If one column
            else
            {
                counter = 0;
                for (int i = 0; i < shipSize * 10; i += 10)
                {
                    __playerGrid[squareIndex + i].Background = shipColor;
                    __playerGrid[squareIndex + i].Tag = shipName;
                }
            }
        }

        private void PlaceShipHorizontally(BaseShip ship, int squareIndex)
        {
            int shipSize = ship.points;
            SolidColorBrush shipColor = ship.color;
            string shipName = ship.name;
            int counter;
            int temp;

            if ((squareIndex + shipSize - 1) % 10 < shipSize - 1)
            {
                counter = 0;
                temp = 1;

                while ((squareIndex + counter) % 10 > 1)
                {
                    __playerGrid[squareIndex + counter].Background = shipColor;
                    __playerGrid[squareIndex + counter].Tag = shipName;
                    counter++;
                }
                for (int i = counter; i < shipSize; i++)
                {
                    __playerGrid[squareIndex - temp].Background = shipColor;
                    __playerGrid[squareIndex - temp].Tag = shipName;
                    temp++;
                }
            }
            //If one row
            else
            {
                for (int i = 0; i < shipSize; i++)
                {
                    __playerGrid[squareIndex + i].Background = shipColor;
                    __playerGrid[squareIndex + i].Tag = shipName;
                }
            }
        }

        private bool CanPlaceShipHorizontally(int shipSize, int squareIndex)
        {
            int counter;

            if (__shipOrientation.Equals(Orientation.HORIZONTAL))
            {
                try
                {
                    counter = 1;
                    for (int i = 0; i < shipSize; i++)
                    {
                        if (squareIndex + i <= 99)
                        {
                            if (!__playerGrid[squareIndex + i].Tag.Equals("water"))
                            {
                                throw new IndexOutOfRangeException("Invalid ship placement, not enough space!");
                            }
                        }
                        else
                        {
                            if (!__playerGrid[squareIndex - counter].Tag.Equals("water"))
                            {
                                throw new IndexOutOfRangeException("Invalid ship placement");
                            }
                            counter++;
                        }
                    }
                }
                catch (IndexOutOfRangeException iore)
                {
                    MessageBox.Show(
                        iore.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return false;
                }
                return true;
            }
            return false;
        }
        private bool CanPlaceShipVertically(int shipSize, int squareIndex)
        {
            int counter;

            if (__shipOrientation.Equals(Orientation.VERTICAL))
            {
                try
                {
                    counter = 10;
                    for (int i = 0; i < shipSize * 10; i += 10)
                    {
                        if (squareIndex + i <= 99)
                        {
                            if (!__playerGrid[squareIndex + i].Tag.Equals("water"))
                            {
                                throw new IndexOutOfRangeException("Invalid ship placement!");
                            }
                        }
                        else
                        {
                            if (!__playerGrid[squareIndex - counter].Tag.Equals("water"))
                            {
                                throw new IndexOutOfRangeException("Invalid ship placement! Wrong counter.");
                            }
                            counter += 10;
                        }
                    }
                    if ((squareIndex / 10) + (shipSize * 10) > 100)
                    {
                        throw new IndexOutOfRangeException("Invalid ship placement, not enough space!");
                    }
                }
                catch (IndexOutOfRangeException iore)
                {
                    MessageBox.Show(
                        iore.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return false;
                }
                return true;
            }
            return false;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (__numShipsPlaced != 5)
            {
                return;
            }
            playEventHandler(this,e);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            reset();
        }

        private void btnRandomize_Click(object sender, RoutedEventArgs e)
        {
            reset();
            Random random = new Random();

            int shipSize, index;
            string shipName;
            SolidColorBrush shipColor;

            Orientation orientation;
            bool unavailableIndex = true;
            
            //Iterate over ships in fleet (key, value)
            foreach (KeyValuePair<string, dynamic> entry in __playerFleet)
            {
                //Set size and ship type
                shipSize = entry.Value.points;
                shipName = entry.Key;
                shipColor = entry.Value.color;
                unavailableIndex = true;

                if (random.Next(0, 2) == 0)
                    orientation = Orientation.HORIZONTAL;
                else
                    orientation = Orientation.VERTICAL;

                //Set ships
                if (orientation.Equals(Orientation.HORIZONTAL))
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while ((index + shipSize - 1) % 10 < shipSize - 1)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < shipSize; j++)
                        {
                            if (index + j > 99 || !__playerGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < shipSize; j++)
                    {
                        __playerGrid[index + j].Tag = shipName;
                        __playerGrid[index + j].Background = shipColor;
                    }
                }
                else
                {
                    index = random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while (index / 10 + shipSize * 10 > 100)
                        {
                            index = random.Next(0, 100);
                        }

                        for (int j = 0; j < shipSize * 10; j += 10)
                        {
                            if (index + j > 99 || !__playerGrid[index + j].Tag.Equals("water"))
                            {
                                index = random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < shipSize * 10; j += 10)
                    {
                        __playerGrid[index + j].Tag = shipName;
                        __playerGrid[index + j].Background = shipColor;
                    }
                }

            }
            __numShipsPlaced = 5;
            foreach (var element in __shipPaths)
            {
                element.IsEnabled = false;
                element.Opacity = .5;
                if (element.Stroke != __unselectedShipStroke)
                {
                    element.Stroke = __unselectedShipStroke;
                }

            }
        }

        private void verticalRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            __shipOrientation = Orientation.VERTICAL;
        }

        private void horizontalRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            __shipOrientation = Orientation.HORIZONTAL;
        }
    }
}
