using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Battleship
{

    public partial class AIPlay : BasePlay
    {
        private Random __random = new Random();
        private List<int> __hitList = new List<int>();

        public AIPlay(string playerName, Grid[] playerGrid, Fleet playerFleet): base(playerName, playerGrid, playerFleet)
        {
            _enemyName = "Computer";
            enemyNameLabel.Content = _enemyName;
            _isMyTurn = true;
            setupCompGrid();
            CheckTurn();
        }

        private void setupCompGrid()
        {
            int shipSize, index;
            string shipName;
            Orientation orientation;
            bool unavailableIndex = true;

            foreach (KeyValuePair<string, dynamic> entry in _enemyFleet)
            {
                //Set size and ship type
                shipSize = entry.Value.points;
                shipName = entry.Key;
                unavailableIndex = true;

                if (__random.Next(0, 2) == 0)
                    orientation = Orientation.HORIZONTAL;
                else
                    orientation = Orientation.VERTICAL;

                //Set ships
                if (orientation.Equals(Orientation.HORIZONTAL))
                {
                    index = __random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while ((index + shipSize - 1) % 10 < shipSize - 1)
                        {
                            index = __random.Next(0, 100);
                        }

                        for (int j = 0; j < shipSize; j++)
                        {
                            if (index + j > 99 || !_enemyGrid[index + j].Tag.Equals("water"))
                            {
                                index = __random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < shipSize; j++)
                    {
                        _enemyGrid[index + j].Tag = shipName;
                    }
                }
                else
                {
                    index = __random.Next(0, 100);
                    while (unavailableIndex == true)
                    {
                        unavailableIndex = false;

                        while (index / 10 + shipSize * 10 > 100)
                        {
                            index = __random.Next(0, 100);
                        }

                        for (int j = 0; j < shipSize * 10; j += 10)
                        {
                            if (index + j > 99 || !_enemyGrid[index + j].Tag.Equals("water"))
                            {
                                index = __random.Next(0, 100);
                                unavailableIndex = true;
                                break;
                            }
                        }
                    }
                    for (int j = 0; j < shipSize * 10; j += 10)
                    {
                        _enemyGrid[index + j].Tag = shipName;
                    }
                }
            }
        }

        protected override void CheckTurn()
        {
            if (_isMyTurn && !_isFinished)
            {
                turnWait.Visibility = Visibility.Collapsed;
            }
            else
            {
                turnWait.Visibility = Visibility.Visible;
                compTurn();
            }
        }

        private void compTurn()
        {
            intelligentMoves();
            _isMyTurn = true;
            CheckResult();
            CheckTurn();
        }

        private void intelligentMoves()
        {
            // If there are no squares to hit
            if (__hitList.Count == 0)
            {
                hunterMode();
            }
            // assumes there is a ship found
            else {
                killerMode();
            }
        }

        private void hunterMode()
        {
            int position;
            do
            {
                position = __random.Next(100);
            } while ((_playerGrid[position].Tag.Equals("miss")) || (_playerGrid[position].Tag.Equals("hit")));
            fireAtLocation(position);
        }
        private void killerMode()
        {
            int position;
            // Prepare to fight at a random grid of the hitList
            do
            {
                position = __random.Next(__hitList.Count);
            } while (_playerGrid[__hitList[position]].Tag.Equals("miss") || _playerGrid[__hitList[position]].Tag.Equals("hit"));

            fireAtLocation(__hitList[position]);
        }

        private void fireAtLocation(int position)
        {
            string tag = _playerGrid[position].Tag.ToString();

            //If the position contains one of the ships (therefore, not water, missed shot, or already hit ship)
            if (!tag.Equals("water"))
            {
                // If this grid is in the hitList, remove it
                if (__hitList != null && __hitList.Contains(position))
                    __hitList.Remove(position);

                // If ship is hit mark it down
                _playerFleet[tag].points--;

                // Mark the grid as hit
                _playerGrid[position].Tag = "hit";
                _playerGrid[position].Background = _colorMapping["hit"];

                // If a ship is destroyed clear the hitList to return to Hunter Mode
                if (_playerFleet["destroyer"].points == 0  ||
                    _playerFleet["cruiser"].points == 0    ||
                    _playerFleet["submarine"].points == 0  ||
                    _playerFleet["battleship"].points == 0 ||
                    _playerFleet["carrier"].points == 0)
                {
                    __hitList.Clear();
                }
                // If a ship is not destroyed add adjacent grids to hitList
                else
                {
                    // Computer hit a ship, add the adjacent grids to hitList
                    // If the position is on the left side
                    if (position % 10 == 0)
                        __hitList.Add(position + 1);
                    // If the position is on the  right side
                    else if (position % 10 == 9)
                        __hitList.Add(position - 1);
                    // Is the position is not on the left or right
                    else
                    {
                        __hitList.Add(position + 1);
                        __hitList.Add(position - 1);
                    }
                    // If the position is on the top
                    if (position < 10)
                        __hitList.Add(position + 10);
                    // If the position is on the bottom
                    else if (position > 89)
                        __hitList.Add(position - 10);
                    // If the position is not on the top or bottom
                    else
                    {
                        __hitList.Add(position + 10);
                        __hitList.Add(position - 10);
                    }

                    // The following code should improve the AI's options by removing squares that are likely to be misses
                    try
                    {
                        __hitList.Remove(position - 11);
                    }
                    catch (Exception e) { }
                    try
                    {
                        __hitList.Remove(position - 9);
                    }
                    catch (Exception e) { }
                    try
                    {
                        __hitList.Remove(position + 9);
                    }
                    catch (Exception e) { }
                    try
                    {
                        __hitList.Remove(position + 11);
                    }
                    catch (Exception e) { }
                }
            }
            else
            {
                _playerGrid[position].Tag = "miss";
                _playerGrid[position].Background = new SolidColorBrush(Colors.LightGray);
            }
        }
    }
}
