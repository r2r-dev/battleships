using System;
using System.Windows.Media;

namespace Battleship
{
    abstract class BaseShip
    {
        protected int _points;
        protected String _name;
        protected SolidColorBrush _color;

        public int points
        {
            get { return _points; }
            set { _points = value; }
        }

        public String name
        {
           get { return _name; }
        }

        public SolidColorBrush color
        {
            get { return _color; }
        }
    }
}
