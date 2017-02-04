using System;
using System.Collections.Generic;
using Battleship.Ship;

namespace Battleship
{
    public class Fleet: Dictionary<string, dynamic>
    {
        public Fleet(): base() {
            Add(
                "battleship",
                new Ship.Battleship()
            );
            Add(
                "cruiser",
                new Ship.Cruiser()
            );
            Add(
                "carrier",
                new Ship.Carrier()
            );
            Add(
                "destroyer",
                new Ship.Destroyer()
            );
            Add(
                "submarine",
                new Ship.Submarine()
            );
        }
    }
}
