using System;
using System.Collections.Generic;
using System.Drawing;
using PcRGB.Model.Extensions;

namespace PcRGB.Model.Render
{
    public delegate void PointEachDelegate(int x, int y);
    public delegate void PixelEachDelegate(Pixel pixel);

    public enum LayerBlendMode
    {
        NORMAL = 1
    }

    public class Layer
    {
        public Rectangle Rect { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Pixel> Pixels { get; set; }

        public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.NORMAL;
        public List<Layer> Layers { get; set; } = new List<Layer>();

        public bool Visible { get; set; } = true;

        public Layer(string name, int x, int y, int width, int height)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Rect = new Rectangle(new Point(x, y), new Size(width, height));
            InitPixels();
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Clear()
        {
            SetColor(new HSB(0, 0, 0));
        }

        public void SetColor(HSB color)
        {
            Pixels.ForEach(pixel => pixel.Color = HSB.Copy(color));
        }

        public Pixel PixelAt(int x, int y)
        {
            if (x < 0 || x > Rect.Width) return null; // x out of bounds
            if (y < 0 || y > Rect.Height) return null; // y out of bounds

            var index = (y * Rect.Size.Height) + x;
            if (index < 0 || index >= Pixels.Count) return null; // pixel index out of bounds

            return Pixels[index];
        }

        public void InitPixels()
        {
            Pixels = new List<Pixel>();
            Rect.Each((x, y) => Pixels.Add(new Pixel(x, y)));
        }

        public void Apply(Layer layer)
        {
            if (!layer.Visible) return;

            var intersection = Rectangle.Intersect(Rect, layer.Rect);
            intersection.Each((x, y) =>
            {
                Pixel from = layer.PixelAt(x, y);
                Pixel to = PixelAt(x + intersection.X, y + intersection.Y);
                if (from != null && to != null)
                {
                    to.Apply(from);
                }
            });
        }

        public virtual void Update()
        {
            foreach (var layer in Layers)
                layer.Update();
        }

        public Layer Render()
        {
            foreach (var layer in Layers)
                Apply(layer.Render());

            return this;
        }
    }
}