using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using core.Model.Cofig;
using core.Model.Extensions;
using core.Model.Layers;

namespace core.Model.Layers
{
    public delegate void LoopDelegate();
    public class Loop
    {
        private CancellationToken RenderCancallationToken;
        private CancellationTokenSource RenderCancellationTokenSource;

        private LoopDelegate OnLooped;

        public int TimePerLoop { get; set; } = 20;

        public Loop(LoopDelegate onLooped)
        {
            OnLooped = onLooped;
        }

        public bool Looping
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

        public async Task StartLoop()
        {
            if (Looping)
            {
                Looping = false;
                return;
            }

            Looping = true;
            var sw = new Stopwatch();

            while (Looping)
            {
                sw.Restart();

                OnLooped();

                var nextLoopIn = TimePerLoop - sw.ElapsedMilliseconds;
                if (nextLoopIn <= 0) nextLoopIn = 1;

                await Task.Delay((int)nextLoopIn);
            }
        }
    }
}