using System;

namespace PcRGB.Model.Render
{
    public class Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2()
        {
            X = 0;
            Y = 0;
        }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Vector2 pos)
        {
            return Vector2.Distance(this, pos);
        }

        public static double Distance(Vector2 pos1, Vector2 pos2)
        {
            return Math.Sqrt(Math.Pow((pos2.X - pos1.X), 2) + Math.Pow((pos2.Y - pos1.Y), 2));
        }
    }
}