namespace PcRGB.Model.EffectLayers
{
    public class DrawLayerEffect : EffectLayer
    {
        public DrawLayerEffect(int width, int height) : base("Draw Layer", width, height) { }

        public override void Update()
        {
            if (!Active) return;
        }
    }
}
