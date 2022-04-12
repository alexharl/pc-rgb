using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PcRGB.Model.EffectLayers;
using PcRGB.Model.Render;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using PcRGB.Hubs;
using System.IO;
using Newtonsoft.Json;
using PcRGB.Model.Cofig;

/*
          0   1   2   3   4   5   6   7   8   9   10  11  12  13  14  15  16  17  18  19

0         0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0
1         0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0
2         0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0
3         3   0   0   0   0   0   0   0   0   0   4   4   4   4   4   4   0   0   0   0
4         0   0   0   0   0   1   2   0   0   0   0   0   0   0   0   0   0   0   0   0
5         3   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0
6         0   0   0   0   0   1   2   0   0   0   0   0   0   0   0   0   0   0   0   0
7         3   0   0   0   0   0   0   0   0   0   5   5   5   5   5   0   0   0   0   0
8         0   0   0   0   0   1   2   0   0   5   0   0   0   0   0   5   0   0   0   0
9         3   0   0   0   0   0   0   0   0   5   0   0   0   0   0   5   0   0   0   0
10        0   0   0   0   0   1   2   0   0   5   0   0   0   0   0   5   0   0   0   0
11        3   0   0   0   0   0   0   0   0   5   0   0   0   0   0   5   0   0   0   0
12        0   0   0   0   0   1   2   0   0   5   0   0   0   0   0   5   0   0   0   0
13        3   0   0   0   0   0   0   0   0   0   5   5   5   5   5   0   0   0   0   0
14        0   0   0   0   0   1   2   0   0   0   0   0   0   0   0   0   0   0   0   0
15        0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0
16        0   0   0   0   0   1   2   0   0   0   0   0   0   0   0   0   0   0   0   0
17        0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0
18        0   0   0   0   0   1   2   0   0   0   0   0   0   0   0   0   0   0   0   0
19        0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0   0

*/
namespace PcRGB.Services
{
    public class RenderService : BackgroundService
    {
        public Renderer Renderer;
        public List<Component> Components = new List<Component>();

        private readonly SerialService _serialService;
        private readonly IHubContext<CanvasHub> _hubContext;

        public RenderService(SerialService serialService, IHubContext<CanvasHub> hubContext)
        {
            _serialService = serialService;
            _hubContext = hubContext;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[RenderService] ExecuteAsync");

            var configPath = Environment.GetEnvironmentVariable("PCRGB__ComponentsConfig");
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                LoadConfig(configPath);
            }

            CreateLayers();

            Renderer?.Update();

            return Task.CompletedTask;
        }

        public void LoadConfig(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                RendererConfig config = JsonConvert.DeserializeObject<RendererConfig>(json);

                Renderer = new Renderer(config.Name, config.Width, config.Height, layer =>
                {
                    var buffer = new List<byte>();
                    foreach (var component in Components)
                    {
                        buffer.AddRange(component.BufferFrom(layer));
                    }
                    _serialService.Write(buffer);
                    _hubContext.Clients.All.SendAsync("layer", Renderer.Pixels);
                });

                if (config.Components?.Count() > 0)
                {
                    Components.AddRange(config.Components.Select(c => Component.FromConfig(c)));
                }
            }
        }

        private void CreateLayers()
        {
            var movingRainbowEffect = new MovingRainbowEffect(Renderer.Size.Width, Renderer.Size.Height);
            movingRainbowEffect.Activate();
            Renderer.Layers.Add(movingRainbowEffect);

            var scanningLinesEffect = new ScanningLinesEffect(Renderer.Size.Width, Renderer.Size.Height);
            scanningLinesEffect.Activate();
            Renderer.Layers.Add(scanningLinesEffect);

            var diffusePointEffect = new DiffusePointEffect(Renderer.Size.Width, Renderer.Size.Height);
            diffusePointEffect.Activate();
            Renderer.Layers.Add(diffusePointEffect);

            var drawLayerEffect = new DrawLayerEffect(Renderer.Size.Width, Renderer.Size.Height);
            drawLayerEffect.Activate();
            Renderer.Layers.Add(drawLayerEffect);
        }

        public Renderer SetLayerVisiblility(string layerId, bool visible)
        {
            var layer = Renderer.Layers.Where(layer => layer.Id == layerId).FirstOrDefault();
            if (layer != null)
            {
                layer.Visible = visible;
            }
            return Renderer;
        }

        public Renderer SetPixel(string layerId, int x, int y)
        {
            var layer = Renderer.Layers.Where(layer => layer.Id == layerId).FirstOrDefault();
            if (layer != null)
            {
                var pixel = layer.PixelAt(x, y);
                if (pixel != null)
                {
                    pixel.Color = new HSB(0, 0, 255, 1);
                }
            }

            return Renderer;
        }

        public Layer Render()
        {
            return Renderer.Render();
        }
    }
}