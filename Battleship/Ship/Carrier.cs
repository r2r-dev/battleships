using System.Windows.Media;

namespace Battleship.Ship
{
    class Carrier : BaseShip
    {
        public Carrier()
        {
            _points = 5;
            _name = "Carrier";
            _color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00cc00"));
        }
    }
}