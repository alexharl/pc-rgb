using System;
using System.Collections.Generic;

namespace PcRGB.Model.Render
{
    public enum LayerBlendMode
    {
        NORMAL = 1
    }

    public class Layer
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Layer> Layers { get; set; }
        public Position Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<List<Pixel>> Pixels { get; set; }
        public LayerBlendMode BlendMode { get; set; }
        public bool Visible { get; set; }
        public Layer(int width, int height)
        {
            Width = width;
            Height = height;
            Visible = true;
            Position = new Position(0, 0);
            Layers = new List<Layer> { };
            BlendMode = LayerBlendMode.NORMAL;
            InitPixels();
        }

        public void InitPixels()
        {
            Pixels = new List<List<Pixel>>();
            for (var h = 0; h < Height; h++)
            {
                var row = new List<Pixel>();
                Pixels.Add(row);
                for (var w = 0; w < Width; w++)
                {
                    var pixel = new Pixel
                    {
                        Position = new Position
                        {
                            X = w,
                            Y = h
                        },
                        Color = new HSB(0, 0, 0)
                    };
                    row.Add(pixel);
                }
            }
        }

        public void ApplyLayer(Layer layer)
        {
            if (!layer.Visible) return;

            if (Position.X + Width < layer.Position.X)
            {
                // keine überschneidung -> ist links
            }
            else if (layer.Position.X + layer.Width < Position.X)
            {
                // keine überschneidung -> ist rechts
            }
            else if (Position.Y + Height < layer.Position.Y)
            {
                // keine überschneidung -> ist drüber
            }
            else if (layer.Position.Y + layer.Height < Position.Y)
            {
                // keine überschneidung -> ist drunter
            }
            else
            {
                int startX = 0;
                int startY = 0;
                int endX = 0;
                int endY = 0;
                // überschneidung

                if (Position.X < layer.Position.X)
                {
                    // ist links
                    // beginne bei layer.Position.X
                    startX = layer.Position.X;
                }
                else
                {
                    // liegt drin
                    // beginne bei Position.X
                    startX = Position.X;
                }

                if (Position.X + Width > layer.Position.X + layer.Width)
                {
                    // steht rechts über
                    // stop bei layer Breite
                    endX = layer.Position.X + layer.Width;
                }
                else
                {
                    // endet drin
                    // stop bei Breite
                    endX = Position.X + Width;
                }

                if (Position.Y < layer.Position.Y)
                {
                    // ist drüber
                    // beginne bei layer.Position.Y
                    startY = layer.Position.Y;
                }
                else
                {
                    // liegt drin
                    // beginne bei Position.Y
                    startY = Position.Y;
                }

                if (Position.Y + Width > layer.Position.Y + layer.Width)
                {
                    // steht unten über
                    // ende bei layer Höhe
                    endY = layer.Position.Y + layer.Width;
                }
                else
                {
                    // endet drin
                    // stop bei Höhe
                    endY = Position.Y + Width;
                }

                for (int row = startX; row < endX; row++)
                {
                    for (int index = startY; index < endY; index++)
                    {
                        Pixel from = layer.Pixels[row][index];

                        if (!from.Transparent)
                        {
                            Pixels[row][index].Color = new HSB(from.Color.Hue, from.Color.Saturation, from.Color.Brightness);
                        }
                    }
                }
            }
        }

        public virtual void Update()
        {
            foreach (var layer in Layers)
            {
                layer.Update();
            }
        }

        public Layer Render()
        {
            var tmpLayer = new Layer(Width, Height);

            // copy pixels to tmpLayer
            for (int row = 0; row < Width; row++)
            {
                for (int index = 0; index < Height; index++)
                {
                    Pixel from = Pixels[row][index];
                    Pixel to = tmpLayer.Pixels[row][index];
                    to.Color = new HSB(from.Color.Hue, from.Color.Saturation, from.Color.Brightness);
                    to.Transparent = from.Transparent;
                }
            }

            foreach (var layer in Layers)
            {
                tmpLayer.ApplyLayer(layer.Render());
            }

            return tmpLayer;
        }
    }
}