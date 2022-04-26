using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PcRGB.Model.Cofig
{
    public class PixelPositionConfig
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class ControllerConfig
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public byte Id { get; set; }
        public IEnumerable<PixelPositionConfig> PixelPositions { get; set; }
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