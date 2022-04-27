using core.Model.Graphics;

namespace core.Effects
{
    public class EffectLayer : Layer
    {
        public bool Active = false;

        public EffectLayer(string name, int x, int y, int width, int height) : base(name, x, y, width, height) { }

        public void Activate()
        {
            Active = true;
        }

        public void Stop()
        {
            Active = false;
        }
    }
}