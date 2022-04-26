using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using PcRGB.Model.EffectLayers;
using PcRGB.Model.Render;
using PcRGB.Model.Cofig;
using PcRGB.Model.Control;
using PcRGB.Hubs;
using PcRGB.Model.Extensions;

namespace PcRGB.Services
{
    public class RenderService : BackgroundService
    {
        public Renderer Renderer;
        public List<Controller> Components = new List<Controller>();

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

            var configPath = Environment.GetEnvironmentVariable("PCRGB__ControllerConfig");
            if (!string.IsNullOrWhiteSpace(configPath))
            {
                LoadConfig(configPath);
            }

            CreateLayers();

            var animateOnStartup = Environment.GetEnvironmentVariable("PCRGB__AnimateOnStartup");
            if (!string.IsNullOrWhiteSpace(animateOnStartup) && animateOnStartup == "true")
            {
                _ = Renderer?.Animate();
            }

            return Task.CompletedTask;
        }

        public void LoadConfig(string path)
        {
            RendererConfig config = RendererConfig.Load(path);
            if (config != null)
            {
                Renderer = new Renderer(config.Name, config.Width, config.Height, OnRendered);

                if (config.Controllers?.Count() > 0)
                {
                    Components.AddRange(config.Controllers.Select(c => Controller.FromConfig(c)).ToArray());
                }
            }
        }
        private void OnRendered(Layer layer)
        {
            // send "SET_CONTROLLER" command with pixel values for each component
            _serialService.Write(Components.BufferFrom(layer));

            // send "SHOW" command to display new data
            _serialService.Write(ControllerCommand.Show().Buffer);

            // notify webClients
            _hubContext.Clients.All.SendAsync("layer", Renderer.Pixels);
        }

        private void CreateLayers()
        {
            if (Renderer == null) return;

            var movingRainbowEffect = new MovingRainbowEffect(0, 0, Renderer.Rect.Size.Width, Renderer.Rect.Size.Height);
            movingRainbowEffect.Activate();
            Renderer.Layers.Add(movingRainbowEffect);

            var scanningLinesEffect = new ScanningLinesEffect(0, 0, Renderer.Rect.Size.Width, Renderer.Rect.Size.Height);
            scanningLinesEffect.Activate();
            scanningLinesEffect.Visible = false;
            Renderer.Layers.Add(scanningLinesEffect);

            var diffusePointEffect = new Ripple(0, 0, Renderer.Rect.Size.Width, Renderer.Rect.Size.Height);
            diffusePointEffect.Activate();
            diffusePointEffect.Visible = false;
            Renderer.Layers.Add(diffusePointEffect);

            var drawLayerEffect = new DrawLayerEffect(3, 2, 5, 5);
            drawLayerEffect.Activate();
            drawLayerEffect.Visible = false;
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