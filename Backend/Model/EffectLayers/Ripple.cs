using System.Drawing;
using System.Numerics;
using PcRGB.Model.Extensions;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class Ripple : EffectLayer
    {
        Point Center = new Point(12, 10);

        float Radius = 0;
        float MaxRadius = 16;
        float Direction = 1;
        bool OutwardsOnly = true;

        public Ripple(int x, int y, int width, int height) : base("Ripple", x, y, width, height) { }

        void UpdatePixels()
        {
            Pixels.ForEach(pixel =>
            {
                float distanceToCenter = Vector2.Distance(new Vector2(pixel.Position.X, pixel.Position.Y), new Vector2(Center.X, Center.Y));
                pixel.Color = new HSB(0, 0, 0, 0);

                if (distanceToCenter <= Radius + 2 && distanceToCenter >= Radius - 2)
                {
                    pixel.Color.Brightness = HSB.MapToValue(distanceToCenter, 0, Radius);
                    pixel.Color.Alpha = 1;
                }
            });
        }
        public void UpdateRadius()
        {
            switch (Direction)
            {
                case 1:
                    if (Radius++ >= MaxRadius)
                    {
                        if (OutwardsOnly)
                        {
                            Radius = 0;
                        }
                        else
                        {
                            Radius = MaxRadius;
                            Direction = 0;
                        }
                    }
                    break;
                default:
                    if (Radius-- <= 0)
                    {
                        Radius = 0;
                        Direction = 1;
                    }
                    break;
            }
        }

        public override void Update()
        {
            if (!Active) return;

            UpdateRadius();
            UpdatePixels();
        }
    }
}