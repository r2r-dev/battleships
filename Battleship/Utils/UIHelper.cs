using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace Battleship
{

    class UIHelper
    {
        public static void ReDrawGrid(Grid grid)
        {
            var T = Type.GetType("System.Windows.Controls.Grid+GridLinesRenderer," +
                    " PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            var GLR = Activator.CreateInstance(T);
            GLR.GetType().GetField(
                "s_oddDashPen",
                BindingFlags.Static | BindingFlags.NonPublic).SetValue(
                    GLR,
                    new Pen(
                        (SolidColorBrush)(new BrushConverter().ConvertFrom("#455A64")),
                        1.0
                   )
                );
            GLR.GetType().GetField(
                "s_evenDashPen",
                BindingFlags.Static | BindingFlags.NonPublic).SetValue(
                GLR,
                new Pen(
                    (SolidColorBrush)(new BrushConverter().ConvertFrom("#455A64")),
                    1.0
                )
            );

            grid.ShowGridLines = true;
        }
    }
}
