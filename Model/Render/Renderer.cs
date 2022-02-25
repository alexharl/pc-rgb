using System.Threading;
using System.Threading.Tasks;

namespace PcRGB.Model.Render
{
    public delegate void OnRenderedDelegate(Layer outputLayer);

    public class Renderer : Layer
    {
        public int FrameTime = 100;
        public bool Animating = true;

        private CancellationToken RenderCancallationToken;
        private CancellationTokenSource RenderCancellationTokenSource;

        private OnRenderedDelegate OnRendered;

        public Renderer(string name, int width, int height, OnRenderedDelegate onRendered) : base(name, width, height)
        {
            OnRendered = onRendered;
        }

        public async Task Animate()
        {
            if (RenderCancellationTokenSource != null && RenderCancallationToken != null && !RenderCancallationToken.IsCancellationRequested)
            {
                // is running
                RenderCancellationTokenSource.Cancel();
                Animating = false;
                return;
            }

            // start
            RenderCancellationTokenSource = new CancellationTokenSource();
            RenderCancallationToken = RenderCancellationTokenSource.Token;
            Animating = true;

            while (!RenderCancallationToken.IsCancellationRequested)
            {
                SetColor(new HSB(0, 0, 0));
                Update();
                OnRendered(Render());
                await Task.Delay(FrameTime);
            }
        }
    }
}