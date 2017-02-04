using System.Windows.Media;

namespace Battleship.Ship
{
    class Battleship : BaseShip
    {
        public Battleship()
        {
            _points = 4;
            _name = "Battleship";
            _color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00e600"));
        }
    }
}