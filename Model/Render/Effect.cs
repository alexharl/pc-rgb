using System;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class EffectLayer : Layer
    {
        public bool Active = false;

        public EffectLayer(string name, int width, int height) : base(width, height)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
        }

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