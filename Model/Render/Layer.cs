using System;
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
        public List<Pixel> Pixels { get; set; }

        public LayerBlendMode BlendMode { get; set; } = LayerBlendMode.NORMAL;
        public List<Layer> Layers { get; set; } = new List<Layer>();

        public bool Visible { get; set; } = true;

        public Layer(string name, int width, int height)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
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

        public void Clear()
        {
            SetColor(new HSB(0, 0, 0));
        }

        public void SetColor(HSB color)
        {
            Each((index, row) =>
            {
                PixelAt(row, index).Color = HSB.Copy(color);
            });
        }

        public static Layer From(Layer layer)
        {
            var newLayer = new Layer(layer.Name, layer.Size.Width, layer.Size.Height);
            layer.Each((index, row) =>
            {
                Pixel from = layer.PixelAt(row, index);
                Pixel to = newLayer.PixelAt(row, index);
                to.Color.CopyFrom(from.Color);
            });
            return newLayer;
        }

        public Pixel PixelAt(int x, int y)
        {
            var index = (y * Size.Height) + x;
            return Pixels[index];
        }

        public void InitPixels()
        {
            Pixels = new List<Pixel>();
            for (var idx = 0; idx < Size.Width * Size.Height; idx++)
            {
                var colIdx = idx % Size.Width;
                var rowIdx = Math.Floor((float)(idx / Size.Height));
                Pixels.Add(new Pixel
                {
                    Position = new Point((int)rowIdx, colIdx),
                    Color = new HSB(0, 0, 0)
                });
            }
        }

        public void Apply(Layer layer)
        {
            if (!layer.Visible) return;

            Intersection(layer)?.Each((index, row) =>
            {
                Pixel from = layer.PixelAt(row, index);
                PixelAt(row, index).Apply(from);
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