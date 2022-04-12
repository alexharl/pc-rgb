using System.Collections.Generic;

namespace PcRGB.Model.Cofig
{
    public class PixelPositionConfig
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class ComponentConfig
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public IEnumerable<PixelPositionConfig> PixelPositions { get; set; }
    }
    public class RendererConfig
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public IEnumerable<ComponentConfig> Components { get; set; }
    }
}