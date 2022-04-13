using System.Drawing;
namespace PcRGB.Model.Render
{
    public delegate void PointEachDelegate(int x, int y);

    public class Rect
    {
        public Rectangle R {get; set;}
        public Point Position { get; set; }
        public Size Size { get; set; }

        public void Each(PointEachDelegate point)
        {
            for (int x = (int)Position.X; x < Position.X + Size.Width; x++)
            {
                for (int y = (int)Position.Y; y < Position.Y + Size.Height; y++)
                {
                    point(x, y);
                }
            }
        }

        public Rect Intersection(Rect rect)
        {
            if (Position.X + Size.Width < rect.Position.X)
            {
                // keine überschneidung -> endet links
            }
            else if (rect.Position.X + rect.Size.Width < Position.X)
            {
                // keine überschneidung -> beginnt rechts
            }
            else if (Position.Y + Size.Height < rect.Position.Y)
            {
                // keine überschneidung -> endet drüber
            }
            else if (rect.Position.Y + rect.Size.Height < Position.Y)
            {
                // keine überschneidung -> beginnt drunter
            }
            else
            {
                int startX =
                    Position.X < rect.Position.X
                    ? rect.Position.X
                    : Position.X;

                int startY =
                    Position.Y < rect.Position.Y
                    ? rect.Position.Y
                    : Position.Y;

                int endX =
                    Position.X + Size.Width > rect.Position.X + rect.Size.Width
                    ? rect.Position.X + rect.Size.Width
                    : Position.X + Size.Width;

                int endY =
                    Position.Y + Size.Height > rect.Position.Y + rect.Size.Height
                    ? rect.Position.Y + rect.Size.Height
                    : Position.Y + Size.Height;

                return new Rect
                {
                    Position = new Point(startX, startY),
                    Size = new Size(endX - startX, endY - startY),
                };
            }
            return null;
        }
    }
}