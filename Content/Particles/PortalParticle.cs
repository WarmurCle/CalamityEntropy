using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class PortalParticle : EParticle
    {
        public int frame = Main.rand.Next(0, 16);
        public override Texture2D Texture => CEUtils.pixelTex;
        public override void OnSpawn()
        {
            this.Lifetime = 30;
        }
        public int FadingTime = 10;
        public override void AI()
        {
            base.AI();
        }
        public override void Draw()
        {
            float alpha = Lifetime > FadingTime ? 1 : (Lifetime / (float)FadingTime);
            DrawVortex(Position, Color * alpha, Scale);
            DrawVortex(Position, Color.White * alpha, Scale * 0.6f);
        }
        public void DrawVortex(Vector2 pos, Color color, float Size = 1, float glow = 1f)
        {
            Main.spriteBatch.End();
            Effect effect = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/Vortex", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["Center"].SetValue(new Vector2(0.5f, 0.5f));
            effect.Parameters["Strength"].SetValue(22);
            effect.Parameters["AspectRatio"].SetValue(1);
            effect.Parameters["TexOffset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.1f, -Main.GlobalTimeWrappedHourly * 0.07f));
            float fadeOutDistance = 0.06f;
            float fadeOutWidth = 0.3f;
            effect.Parameters["FadeOutDistance"].SetValue(fadeOutDistance);
            effect.Parameters["FadeOutWidth"].SetValue(fadeOutWidth);
            effect.Parameters["enhanceLightAlpha"].SetValue(0.8f);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            effect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(CEUtils.getExtraTex("VoronoiShapes"), pos - Main.screenPosition, null, color, Main.GlobalTimeWrappedHourly * 12, CEUtils.getExtraTex("VoronoiShapes").Size() / 2f, 0.2f * Size, SpriteEffects.None, 0);
            CEUtils.DrawGlow(pos, Color.White * 0.4f * glow, 0.8f * Size * glow);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

        }
    }
}
