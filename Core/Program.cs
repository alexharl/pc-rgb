using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
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
                    Console.WriteLine("[FastLEDStream] Connected");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[FastLEDStream] Failed to connect to port {e.Message}");
                }
            }
        }

        static async Task InitializeRenderer()
        {
            List<LEDLayer> controllerLayers = new List<LEDLayer>();

            var path = Environment.GetEnvironmentVariable("Core__ControllerConfig");
            if (!string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine($"[Core] Load config: {path}");
                RendererConfig config = RendererConfig.Load(path);

                if (config.Controllers?.Count() > 0)
                {
                    Console.WriteLine($"[Core] Controllers: {config.Controllers.Count()}");
                    controllerLayers.AddRange(config.Controllers.Select(controller =>
                    {
                        var ledLayer = new LEDLayer(controller.Name, controller.X, controller.Y, controller.Width, controller.Height);
                        ledLayer.HardwareId = (byte)controller.HardwareId;
                        ledLayer.PixelPositions = controller.PixelPositions.Select(p => new Point(p.X, p.Y)).ToList();
                        return ledLayer;
                    }).ToArray());
                }

                var canvas = new Layer(config.Name, 0, 0, config.Width, config.Height);

                var movingRainbowEffect = new MovingRainbowEffect(0, 0, canvas.Rect.Size.Width, canvas.Rect.Size.Height);
                movingRainbowEffect.Activate();
                canvas.Layers.Add(movingRainbowEffect);

                var pulseEffect = new PulseEffect(0, 0, canvas.Rect.Size.Width, canvas.Rect.Size.Height);
                pulseEffect.BlendMode = LayerBlendMode.Brightness;
                pulseEffect.Activate();
                canvas.Layers.Add(pulseEffect);

                // var rippleEffect = new RippleEffect(0, 0, canvas.Rect.Size.Width, canvas.Rect.Size.Height);
                // rippleEffect.Activate();
                // canvas.Layers.Add(rippleEffect);

                Console.WriteLine($"[Core] Start...");
                var animationLoop = new Loop(() =>
                {
                    canvas.Clear();
                    canvas.Update();

                    var render = canvas.Render();

                    foreach (var ledLayer in controllerLayers)
                    {
                        var pixelBuffer = new List<byte>().AddPixels(ledLayer.PixelsFrom(render));
                        _stream.SetController(ledLayer.HardwareId, pixelBuffer);
                    }

                    _stream.Show();
                });

                await animationLoop.StartLoop();
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
