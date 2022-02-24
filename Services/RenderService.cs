using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PcRGB.Model.EffectLayers;
using PcRGB.Model.Render;
using Microsoft.Extensions.Hosting;
using System.Numerics;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using PcRGB.Hubs;

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
        private readonly SerialService _serialService;

        public Layer Canvas;
        public List<Component> Components = new List<Component>();
        private readonly IHubContext<CanvasHub> _hubContext;
        public RenderService(SerialService serialService, IHubContext<CanvasHub> hubContext)
        {
            _serialService = serialService;
            _hubContext = hubContext;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[RenderService] ExecuteAsync");
            CreateCanvas();
            return Task.CompletedTask;
        }

        private async Task CreateCanvas()
        {
            Canvas = new Layer(20, 20);
            Components.Add(new Component
            {
                Id = 1,
                Name = "Ram 1",
                PixelPositions = new List<Point>{
                    new Point(5,18),
                    new Point(5,16),
                    new Point(5,14),
                    new Point(5,12),
                    new Point(5,10),
                    new Point(5,8),
                    new Point(5,6),
                    new Point(5,4)
                }
            });
            Components.Add(new Component
            {
                Id = 2,
                Name = "Ram 2",
                PixelPositions = new List<Point>{
                    new Point(4,18),
                    new Point(4,16),
                    new Point(4,14),
                    new Point(4,12),
                    new Point(4,10),
                    new Point(4,8),
                    new Point(4,6),
                    new Point(4,4)
                }
            });
            Components.Add(new Component
            {
                Id = 3,
                Name = "Reservoire",
                PixelPositions = new List<Point>{
                    new Point(0,3),
                    new Point(0,5),
                    new Point(0,7),
                    new Point(0,10),
                    new Point(0,13),
                    new Point(0,15)
                }
            });
            Components.Add(new Component
            {
                Id = 4,
                Name = "SSD",
                PixelPositions = new List<Point>{
                    new Point(15,3),
                    new Point(14,3),
                    new Point(13,3),
                    new Point(12,3),
                    new Point(11,3),
                    new Point(10,3)
                }
            });
            Components.Add(new Component
            {
                Id = 5,
                Name = "CPU",
                PixelPositions = new List<Point>{
                    new Point(9,11),
                    new Point(9,10),
                    new Point(9,9),
                    new Point(9,8),

                    new Point(10,7),
                    new Point(11,7),
                    new Point(12,7),
                    new Point(13,7),
                    new Point(14,7),

                    new Point(15,8),
                    new Point(15,9),
                    new Point(15,10),
                    new Point(15,11),
                    new Point(15,12),

                    new Point(14,13),
                    new Point(13,13),
                    new Point(12,13),
                    new Point(11,13),
                    new Point(10,13),

                    new Point(9,12)
                }
            });

            var movingRainbowEffect = new MovingRainbowEffect(Canvas.Size.Width, Canvas.Size.Height);
            movingRainbowEffect.Activate();
            Canvas.Layers.Add(movingRainbowEffect);

            // var scanningLinesEffect = new ScanningLinesEffect(Canvas.Size.Width, Canvas.Size.Height);
            // scanningLinesEffect.Activate();
            // Canvas.Layers.Add(scanningLinesEffect);

            var diffusePointEffect = new DiffusePointEffect(Canvas.Size.Width, Canvas.Size.Height);
            diffusePointEffect.Activate();
            Canvas.Layers.Add(diffusePointEffect);

            source = new CancellationTokenSource();
            token = source.Token;

            await Task.Delay(2000);
            Canvas.Update();
        }

        public int FrameTime = 30;
        public CancellationToken token;
        public CancellationTokenSource source;
        public async Task AutoRender()
        {
            if (!token.IsCancellationRequested)
            {
                source.Cancel();
                return;
            }
            source = new CancellationTokenSource();
            token = source.Token;
            while (!token.IsCancellationRequested)
            {
                Update();
                await Task.Delay(FrameTime);
            }
        }

        public Layer SetLayerVisiblility(string layerId, bool visible)
        {
            var layer = Canvas.Layers.Where(layer => layer.Id == layerId).FirstOrDefault();
            if (layer != null)
            {
                layer.Visible = visible;
            }
            return Canvas;
        }

        public Layer Render()
        {
            return Canvas.Render();
        }

        public Layer Update()
        {
            Canvas.SetColor(new HSB(0, 0, 0));
            Canvas.Update();
            var layer = Render();

            var buffer = new List<byte>();
            foreach (var component in Components)
            {
                buffer.AddRange(component.BufferFrom(layer));
            }
            _serialService.Write(buffer);

            var signal = new List<List<Pixel>>();
            for (var r = 0; r < layer.Pixels.Count; r++)
            {
                var row = new List<Pixel>();
                for (var c = 0; c < layer.Pixels[r].Count; c++)
                {
                    row.Add(layer.Pixels[c][r]);
                }
                signal.Add(row);
            }
            Console.WriteLine(signal);
            _hubContext.Clients.All.SendAsync("layer", signal);

            return layer;
        }
    }
}