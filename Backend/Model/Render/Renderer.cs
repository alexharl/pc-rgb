using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PcRGB.Model.Render
{
    public delegate void OnRenderedDelegate(Layer outputLayer);

    public class Renderer : Layer
    {
        public int FrameTime { get; set; } = 20;
        public bool Animating
        {
            get
            {
                return RenderCancellationTokenSource != null && RenderCancallationToken != null && !RenderCancallationToken.IsCancellationRequested;
            }
            set
            {
                if (!value)
                {
                    RenderCancellationTokenSource.Cancel();
                }
                else
                {
                    RenderCancellationTokenSource = new CancellationTokenSource();
                    RenderCancallationToken = RenderCancellationTokenSource.Token;
                }
            }
        }

        private CancellationToken RenderCancallationToken;
        private CancellationTokenSource RenderCancellationTokenSource;

        private OnRenderedDelegate OnRendered;

        public Renderer(string name, int width, int height, OnRenderedDelegate onRendered) : base(name, 0, 0, width, height)
        {
            OnRendered = onRendered;
        }

        public void Next()
        {
            Clear();
            Update();
            OnRendered(Render());
        }

        public async Task Animate()
        {
            if (Animating)
            {
                // is running
                Animating = false;
                return;
            }

            // start
            Animating = true;

            var sw = new Stopwatch();
            while (Animating)
            {
                sw.Restart();
                Next();

                var nextFrameIn = FrameTime - sw.ElapsedMilliseconds;
                if (nextFrameIn <= 0) nextFrameIn = 1;

                await Task.Delay((int)nextFrameIn);
            }
        }
    }
}