using PcRGB.Model.Extensions;
using PcRGB.Model.Render;

namespace PcRGB.Model.EffectLayers
{
    public class DrawLayerEffect : EffectLayer
    {
        public DrawLayerEffect(int x, int y, int width, int height) : base("Draw Layer", x, y, width, height) { }

        public override void Update()
        {
            if (!Active) return;
            SetColor(new HSB(128, 255, 50, 1));
            PixelAt(0, 0).Color = new HSB(0, 255, 200, 1);
            PixelAt(0, 4).Color = new HSB(64, 255, 200, 1);
            PixelAt(4, 4).Color = new HSB(128, 255, 200, 1);
            PixelAt(4, 0).Color = new HSB(192, 255, 200, 1);
        }
    }
}
