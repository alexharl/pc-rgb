using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace core.Model.Cofig
{
    public class PositionConfig
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class LayerConfig : PositionConfig
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class ControllerConfig : LayerConfig
    {
        public byte HardwareId { get; set; }
        public IEnumerable<PositionConfig> PixelPositions { get; set; }
    }

    public class RendererConfig
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public IEnumerable<ControllerConfig> Controllers { get; set; }

        public static RendererConfig Load(string path)
        {
            try
            {
                using (StreamReader r = new StreamReader(path))
                    return JsonConvert.DeserializeObject<RendererConfig>(r.ReadToEnd());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RendererConfig] Failed to load config from {path}. ERROR: {ex.Message}");
                return null;
            }
        }
    }
}