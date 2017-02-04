using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Battleship
{
    public partial class NetPlay: BasePlay
    {
        private bool __isServer;
        private SocketManagement __connection;
        private int[] __playerBoard = Enumerable.Repeat(0, 100).ToArray();
        private int[] __enemyBoard = Enumerable.Repeat(0, 100).ToArray();

        private List<string> __boardMapping = new List<string>() {
            "water",
            "hit",
            "miss",
            "destroyer",
            "cruiser",
            "submarine",
            "battleship",
            "carrier"
        };


        public NetPlay(string playerName, Grid[] playerGrid, Fleet playerFleet, bool isServer, SocketManagement connection): base(playerName, playerGrid, playerFleet)
        {
            __connection = connection;
            __isServer = isServer;
            _isMyTurn = isServer;

            string tag;
            for (int i = 0; i < 100; i++)
            {
                tag = playerGrid[i].Tag.ToString();
                __playerBoard[i] = __boardMapping.FindIndex(
                    item => item == tag
                );
            }

            Task.Factory.StartNew(() =>
            {
                if (__isServer)
                {
                    __enemyBoard = __connection.getBoard();
                    __connection.sendBoard(__playerBoard);

                    _enemyName = __connection.getShot();
                    __connection.sendShot(playerName);
                }
                else
                {
                    __connection.sendBoard(__playerBoard);
                    __enemyBoard = __connection.getBoard();

                    __connection.sendShot(playerName);
                    _enemyName = __connection.getShot();
                }

                Dispatch(SetEnemyName);
                Dispatch(ResetEnemyGrid);
                Dispatch(CheckTurn);
            });
        }

        private void SetEnemyName()
        {
            enemyNameLabel.Content = _enemyName;
        }

        protected override void gridMouseDown(object sender, MouseButtonEventArgs e)
        {
            _shotEvent = (row, column, shotName, squareName) =>
            {
                __enemyBoard[row * 10 + column] = __boardMapping.FindIndex(
                    item => item == shotName
                );
                __connection.sendShot(squareName);
                __connection.sendBoard(__enemyBoard);
            };
            base.gridMouseDown(sender, e);
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
                GetDataFromOthers();
            }
            ResetPlayerGrid();
        }

        private void ResetPlayerGrid()
        {
            for (int i = 0; i < 100; i++)
            {
                _playerGrid[i].Tag = __boardMapping[__playerBoard[i]];
                _playerGrid[i].Background = _colorMapping[_playerGrid[i].Tag.ToString()];
            }
        }

        private void ResetEnemyGrid()
        {
            for (int i = 0; i < 100; i++)
            {
                _enemyGrid[i].Tag = __boardMapping[__enemyBoard[i]];
            }
        }

        private void GetDataFromOthers()
        {
            Task.Factory.StartNew(() => {
                _lastShot = __connection.getShot();
                if (_playerFleet.ContainsKey(_lastShot))
                {
                    _playerFleet[_lastShot].points--;
                }
                __playerBoard = __connection.getBoard();
                _isMyTurn = true;
                Dispatch(CheckResult);
                Dispatch(CheckTurn);
            });
        }
        private void Dispatch(Action function)
        {
            if (Dispatcher.CheckAccess()) function();
            else Dispatcher.BeginInvoke(function);
        }
    }
}
