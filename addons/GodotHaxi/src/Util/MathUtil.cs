using System;

namespace GodotHaxi;

public class MathUtil
{
    public static double GetDistance2D(double x, double y, double x2, double y2)
    {
        double deltaX = x2 - x;
        double deltaY = y2 - y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public static double GetDistance2DInt(double x, double y, double x2, double y2) => (int)Math.Floor(GetDistance2D(x, y, x2, y2));
}