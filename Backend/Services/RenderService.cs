using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using PcRGB.Model.EffectLayers;
using PcRGB.Model.Render;
using PcRGB.Model.Cofig;
using PcRGB.Hubs;

namespace PcRGB.Services
{
    public class RenderService : BackgroundService
    {
        public Renderer Renderer;
        private readonly IHubContext<CanvasHub> _hubContext;

        public RenderService(IHubContext<CanvasHub> hubContext)
        {
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

            SerialConnect();

            return Task.CompletedTask;
        }

        public void LoadConfig(string path)
        {
            RendererConfig config = RendererConfig.Load(path);
            Renderer = Renderer.FromConfig(config, OnRendered);
        }

        public bool SerialConnect()
        {
            var portName = Environment.GetEnvironmentVariable("PCRGB__ComPortName");
            var portBaud = Environment.GetEnvironmentVariable("PCRGB__ComPortBaudrate");

            if (string.IsNullOrWhiteSpace(portName))
            {
                Console.WriteLine("[SerialService] Port not specified");
            }
            else
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(portBaud)) portBaud = "115200";

                    Renderer?.SerialConnect(portName, Int32.Parse(portBaud));

                    return Renderer.SerialOpen;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[RenderService] Failed to connect to port {e.Message}");
                }
            }

            return false;
        }

        private void OnRendered(Layer layer)
        {
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

            var rippleEffect = new Ripple(0, 0, Renderer.Rect.Size.Width, Renderer.Rect.Size.Height);
            rippleEffect.Activate();
            rippleEffect.Visible = false;
            Renderer.Layers.Add(rippleEffect);

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