using System.Threading;
using System.Threading.Tasks;

namespace PcRGB.Model.Render
{
    public delegate void OnRenderedDelegate(Layer outputLayer);

    public class Renderer : Layer
    {
        public int FrameTime { get; set; } = 100;
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

        public Renderer(string name, int width, int height, OnRenderedDelegate onRendered) : base(name, width, height)
        {
            OnRendered = onRendered;
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

            while (Animating)
            {
                Clear();
                Update();
                OnRendered(Render());
                await Task.Delay(FrameTime);
            }
        }
    }
}