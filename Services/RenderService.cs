using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PcRGB.Model.EffectLayers;
using PcRGB.Model.Render;
using Microsoft.Extensions.Hosting;

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
        public RenderService(SerialService serialService)
        {
            _serialService = serialService;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("[RenderService] ExecuteAsync");
            CreateCanvas();
            return Task.CompletedTask;
        }

        private void CreateCanvas()
        {
            Canvas = new Layer(20, 20);
            Components.Add(new Component
            {
                Id = 1,
                Name = "Ram 1",
                PixelPositions = new List<Vector2>{
                    new Vector2(5,18),
                    new Vector2(5,16),
                    new Vector2(5,14),
                    new Vector2(5,12),
                    new Vector2(5,10),
                    new Vector2(5,8),
                    new Vector2(5,6),
                    new Vector2(5,4)
                }
            });
            Components.Add(new Component
            {
                Id = 2,
                Name = "Ram 2",
                PixelPositions = new List<Vector2>{
                    new Vector2(4,18),
                    new Vector2(4,16),
                    new Vector2(4,14),
                    new Vector2(4,12),
                    new Vector2(4,10),
                    new Vector2(4,8),
                    new Vector2(4,6),
                    new Vector2(4,4)
                }
            });
            Components.Add(new Component
            {
                Id = 3,
                Name = "Reservoire",
                PixelPositions = new List<Vector2>{
                    new Vector2(0,3),
                    new Vector2(0,5),
                    new Vector2(0,7),
                    new Vector2(0,10),
                    new Vector2(0,13),
                    new Vector2(0,15)
                }
            });
            Components.Add(new Component
            {
                Id = 4,
                Name = "SSD",
                PixelPositions = new List<Vector2>{
                    new Vector2(15,3),
                    new Vector2(14,3),
                    new Vector2(13,3),
                    new Vector2(12,3),
                    new Vector2(11,3),
                    new Vector2(10,3)
                }
            });
            Components.Add(new Component
            {
                Id = 5,
                Name = "CPU",
                PixelPositions = new List<Vector2>{
                    new Vector2(9,11),
                    new Vector2(9,10),
                    new Vector2(9,9),
                    new Vector2(9,8),

                    new Vector2(10,7),
                    new Vector2(11,7),
                    new Vector2(12,7),
                    new Vector2(13,7),
                    new Vector2(14,7),

                    new Vector2(15,8),
                    new Vector2(15,9),
                    new Vector2(15,10),
                    new Vector2(15,11),
                    new Vector2(15,12),

                    new Vector2(14,13),
                    new Vector2(13,13),
                    new Vector2(12,13),
                    new Vector2(11,13),
                    new Vector2(10,13),

                    new Vector2(9,12)
                }
            });

            var movingRainbowEffect = new MovingRainbowEffect(Canvas.Size.X, Canvas.Size.Y);
            movingRainbowEffect.Running = true;
            Canvas.Layers.Add(movingRainbowEffect);

            var scanningLinesEffect = new ScanningLinesEffect(Canvas.Size.X, Canvas.Size.Y);
            scanningLinesEffect.Running = true;
            Canvas.Layers.Add(scanningLinesEffect);

            Canvas.Update();
        }

        public Layer Render()
        {
            return Canvas.Render();
        }

        public Layer Update()
        {
            Canvas.Update();
            var layer = Render();

            var buffer = new List<byte>();
            foreach (var component in Components)
            {
                buffer.AddRange(component.BufferFrom(layer));
            }
            _serialService.Write(buffer);

            return layer;
        }
    }
}