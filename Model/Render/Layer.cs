using System.Collections.Generic;
using System.Drawing;

namespace PcRGB.Model.Render
{
    public enum LayerBlendMode
    {
        NORMAL = 1
    }

    public class Layer : Rect
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<List<Pixel>> Pixels { get; set; }

        public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.NORMAL;
        public List<Layer> Layers { get; set; } = new List<Layer>();

        public bool Visible { get; set; } = true;

        public Layer(int width, int height)
        {
            Size = new Size(width, height);
            Position = new Point(0, 0);
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

        public void SetColor(HSB color)
        {
            Each((index, row) =>
            {
                Pixels[row][index].Color = HSB.Copy(color);
            });
        }

        public static Layer From(Layer layer)
        {
            var newLayer = new Layer(layer.Size.Width, layer.Size.Height);
            layer.Each((index, row) =>
            {
                Pixel from = layer.Pixels[row][index];
                Pixel to = newLayer.Pixels[row][index];
                to.Color.CopyFrom(from.Color);
            });
            return newLayer;
        }

        public void InitPixels()
        {
            Pixels = new List<List<Pixel>>();
            for (var w = 0; w < Size.Width; w++)
            {
                var row = new List<Pixel>();
                Pixels.Add(row);
                for (var h = 0; h < Size.Height; h++)
                {
                    row.Add(new Pixel
                    {
                        Position = new Point(w, h),
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