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

        public Canvas canvas;

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
            canvas = new Canvas(20, 20);
            canvas.Components.Add(new Component
            {
                Id = 1,
                Name = "Ram 1",
                Pixels = new List<Pixel>{
                    canvas.Pixels[18][5],
                    canvas.Pixels[16][5],
                    canvas.Pixels[14][5],
                    canvas.Pixels[12][5],
                    canvas.Pixels[10][5],
                    canvas.Pixels[8][5],
                    canvas.Pixels[6][5],
                    canvas.Pixels[4][5]
                }
            });
            canvas.Components.Add(new Component
            {
                Id = 2,
                Name = "Ram 2",
                Pixels = new List<Pixel>{
                    canvas.Pixels[18][4],
                    canvas.Pixels[16][4],
                    canvas.Pixels[14][4],
                    canvas.Pixels[12][4],
                    canvas.Pixels[10][4],
                    canvas.Pixels[8][4],
                    canvas.Pixels[6][4],
                    canvas.Pixels[4][4]
                }
            });
            canvas.Components.Add(new Component
            {
                Id = 3,
                Name = "Reservoire",
                Pixels = new List<Pixel>{
                    canvas.Pixels[3][0],
                    canvas.Pixels[5][0],
                    canvas.Pixels[7][0],
                    canvas.Pixels[9][0],
                    canvas.Pixels[11][0],
                    canvas.Pixels[13][0]
                }
            });
            canvas.Components.Add(new Component
            {
                Id = 4,
                Name = "SSD",
                Pixels = new List<Pixel>{
                    canvas.Pixels[3][15],
                    canvas.Pixels[3][14],
                    canvas.Pixels[3][13],
                    canvas.Pixels[3][12],
                    canvas.Pixels[3][11],
                    canvas.Pixels[3][10]
                }
            });
            canvas.Components.Add(new Component
            {
                Id = 5,
                Name = "CPU",
                Pixels = new List<Pixel>{
                    canvas.Pixels[11][9],
                    canvas.Pixels[10][9],
                    canvas.Pixels[9][9],
                    canvas.Pixels[8][9],

                    canvas.Pixels[7][10],
                    canvas.Pixels[7][11],
                    canvas.Pixels[7][12],
                    canvas.Pixels[7][13],
                    canvas.Pixels[7][14],

                    canvas.Pixels[8][15],
                    canvas.Pixels[9][15],
                    canvas.Pixels[10][15],
                    canvas.Pixels[11][15],
                    canvas.Pixels[12][15],

                    canvas.Pixels[13][14],
                    canvas.Pixels[13][13],
                    canvas.Pixels[13][12],
                    canvas.Pixels[13][11],
                    canvas.Pixels[13][10],

                    canvas.Pixels[12][9],
                }
            });

            var movingRainbowEffect = new MovingRainbowEffect(canvas.Width, canvas.Height);
            movingRainbowEffect.Running = true;
            canvas.Layers.Add(movingRainbowEffect);

            var scanningLinesEffect = new ScanningLinesEffect(canvas.Width, canvas.Height);
            scanningLinesEffect.Running = true;
            canvas.Layers.Add(scanningLinesEffect);

            canvas.Update();
        }
    }
}