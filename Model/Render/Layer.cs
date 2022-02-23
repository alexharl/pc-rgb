using System;
using System.Collections.Generic;

namespace PcRGB.Model.Render
{
    public enum LayerBlendMode
    {
        NORMAL = 1
    }

    public class Layer : Rect
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Layer> Layers { get; set; } = new List<Layer>();
        public List<List<Pixel>> Pixels { get; set; }
        public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.NORMAL;
        public bool Visible { get; set; } = true;

        public Layer(int width, int height)
        {
            Size = new Vector2(width, height);
            Position = new Vector2(0, 0);
            InitPixels();
        }

        public static Layer From(Layer layer)
        {
            var newLayer = new Layer(layer.Size.X, layer.Size.Y);
            layer.Each((index, row) =>
            {
                Pixel from = layer.Pixels[row][index];
                Pixel to = newLayer.Pixels[row][index];
                to.Color = new HSB(from.Color.Hue, from.Color.Saturation, from.Color.Brightness);
                to.Transparent = from.Transparent;
            });
            return newLayer;
        }

        public void InitPixels()
        {
            Pixels = new List<List<Pixel>>();
            for (var w = 0; w < Size.X; w++)
            {
                var row = new List<Pixel>();
                Pixels.Add(row);
                for (var h = 0; h < Size.Y; h++)
                {
                    row.Add(new Pixel
                    {
                        Position = new Vector2(w, h),
                        Color = new HSB(0, 0, 0)
                    });
                }
            }
        }

        public void Apply(Layer layer)
        {
            if (!layer.Visible) return;

            Intersection(layer)?.Each((index, row) =>
            {
                Pixel from = layer.Pixels[row][index];
                Pixels[row][index].Apply(from);
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