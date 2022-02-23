namespace PcRGB.Model.Render
{
    public class Pixel
    {
        public bool Transparent { get; set; }
        public Vector2 Position { get; set; }
        public HSB Color { get; set; }

        public void Apply(Pixel pixel)
        {
            if (pixel.Transparent) return;

            Color = new HSB(pixel.Color.Hue, pixel.Color.Saturation, pixel.Color.Brightness);
        }
    }

}