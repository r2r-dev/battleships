using System.Windows.Media;

namespace Battleship.Ship
{
    class Destroyer : BaseShip
    {
        public Destroyer()
        {
            _points = 2;
            _name = "Destroyer";
            _color = (SolidColorBrush)(new BrushConverter().ConvertFrom("#88cc00"));
        }
    }
}