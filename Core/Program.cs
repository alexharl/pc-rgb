using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using core.Model.Graphics;
using core.Model.Layers;
using core.Model.Extensions;
using core.Model.Cofig;
using core.Model.Serial;
using core.Effects;


namespace core
{
    class Program
    {
        static Renderer _renderer { get; set; }
        static FastLEDStream _stream { get; set; }

        static void InitializeFastLEDStream()
        {
            var portName = Environment.GetEnvironmentVariable("FastLEDStream__ComPortName");

            if (string.IsNullOrWhiteSpace(portName))
            {
                Console.WriteLine("[FastLEDStream] Port not specified");
            }
            else
            {
                try
                {
                    var portBaud = Environment.GetEnvironmentVariable("FastLEDStream__ComPortBaudrate");
                    if (string.IsNullOrWhiteSpace(portBaud)) portBaud = "115200";

                    _stream = new FastLEDStream(portName, Int32.Parse(portBaud));
                    _stream.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[FastLEDStream] Failed to connect to port {e.Message}");
                }
            }
        }

        static async Task InitializeRenderer()
        {
            List<LEDLayer> ledLayers = new List<LEDLayer>();

            var path = Environment.GetEnvironmentVariable("Core__ControllerConfig");
            if (!string.IsNullOrWhiteSpace(path))
            {
                RendererConfig config = RendererConfig.Load(path);

                if (config.Controllers?.Count() > 0)
                {
                    ledLayers.AddRange(config.Controllers.Select(controller =>
                    {
                        var ledLayer = new LEDLayer(controller.Name, (byte)controller.Id, controller.X, controller.Y, controller.Width, controller.Height);
                        ledLayer.PixelPositions = controller.PixelPositions.Select(p => new Point(p.X, p.Y)).ToList();
                        return ledLayer;
                    }).ToArray());
                }

                _renderer = new Renderer(config.Name, config.Width, config.Height, (layer) =>
                {
                    foreach (var ledLayer in ledLayers)
                    {
                        var pixelBuffer = new List<byte>().AddPixels(ledLayer.PixelsFrom(layer));
                        _stream.SetController(ledLayer.HardwareId, pixelBuffer);
                    }

                    _stream.Show();
                });

                var movingRainbowEffect = new MovingRainbowEffect(0, 0, _renderer.Rect.Size.Width, _renderer.Rect.Size.Height);
                movingRainbowEffect.Activate();
                _renderer.Layers.Add(movingRainbowEffect);

                await _renderer.Animate();
            }
        }

        static void Main(string[] args)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            InitializeFastLEDStream();

            var thread = new Thread(() =>
            {
                _ = InitializeRenderer();
            });
            thread.Start();

            manualResetEvent.WaitOne();
        }

    }
}
