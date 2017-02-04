using System.Windows.Media;

namespace Battleship.Ship
{
    class Cruiser : BaseShip
    {
        public Cruiser()
        {
            _points = 3;
            _name = "Cruiser";
            _color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#33cc33"));
        }
    }
}