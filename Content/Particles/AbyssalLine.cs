using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class AbyssalLine : BasePRT, IAdditivePRT
    {
        public override string Texture => "CalamityEntropy/Content/Particles/AbyssalLine";
        public float timemax = 50;
        public Color spawnColor = new Color(190, 190, 255);
        public Color endColor = Color.Blue;
        public float xscale = 0;
        public float xdec = 0.87f;
        public float xadd = 3.2f;
        public float lx = 3;
        public override void SetProperty()
        {
            PRTLayersMode = PRTLayersModeEnum.NoDraw;
            Lifetime = 50;
        }
        public override void AI()
        {
            lx *= 0.88f;
            xscale += xadd;
            xadd *= xdec;
        }
        void IAdditivePRT.Draw(SpriteBatch spriteBatch)
        {
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color.Lerp(endColor, spawnColor, (Lifetime / timemax)) * (Lifetime / timemax) * 0.7f, this.Rotation, tex.Size() / 2, new Vector2(0.6f * (xscale + 0.1f), 0.56f * lx) * Scale, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color.Lerp(endColor, spawnColor, (Lifetime / timemax)) * (Lifetime / timemax), this.Rotation, tex.Size() / 2, new Vector2(0.6f * xscale, 0.2f * lx) * Scale, SpriteEffects.None, 0);
        }
    }
}
