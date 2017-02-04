using System.Windows.Media;

namespace Battleship.Ship
{
    class Submarine : BaseShip
    {
        public Submarine()
        {
            _points = 3;
            _name = "Submarine";
            _color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00e64d"));
        }
    }
}