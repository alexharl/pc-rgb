namespace PcRGB.Model.Render
{
    public class Rect
    {
        public delegate void RowEach(int row);
        public delegate void PointEach(int index, int row);
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public void Each(PointEach point)
        {
            for (int r = Position.X; r < Position.X + Size.X; r++)
            {
                for (int index = Position.Y; index < Position.Y + Size.Y; index++)
                {
                    point(index, r);
                }
            }
        }

        public Rect Intersection(Rect rect)
        {
            if (Position.X + Size.X < rect.Position.X)
            {
                // keine überschneidung -> endet links
            }
            else if (rect.Position.X + rect.Size.X < Position.X)
            {
                // keine überschneidung -> beginnt rechts
            }
            else if (Position.Y + Size.Y < rect.Position.Y)
            {
                // keine überschneidung -> endet drüber
            }
            else if (rect.Position.Y + rect.Size.Y < Position.Y)
            {
                // keine überschneidung -> beginnt drunter
            }
            else
            {
                int startX =
                    Position.X < rect.Position.X
                    ? rect.Position.X                                       // links
                    : Position.X;

                int startY =
                    Position.Y < rect.Position.Y
                    ? rect.Position.Y                                       // drüber
                    : Position.Y;

                int endX =
                    Position.X + Size.X > rect.Position.X + rect.Size.X
                    ? rect.Position.X + rect.Size.X                         // rechts
                    : Position.X + Size.X;

                int endY =
                    Position.Y + Size.Y > rect.Position.Y + rect.Size.Y
                    ? rect.Position.Y + rect.Size.Y                         // drunter
                    : Position.Y + Size.Y;

                return new Rect
                {
                    Position = new Vector2(startX, startY),
                    Size = new Vector2(endX - startX, endY - startY),
                };
            }
            return null;
        }
    }
}