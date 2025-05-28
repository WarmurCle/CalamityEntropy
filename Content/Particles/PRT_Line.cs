using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    internal class PRT_Line : BasePRT, ICELoader
    {
        public Vector2 LineSize;
        private static Asset<Texture2D> PRT_LineCap;
        void ICELoader.LoadAsset() => PRT_LineCap = CEUtils.GetT2DAsset("CalamityEntropy/Content/Particles/PRT_RTSpark");
        void ICELoader.UnLoadData() => PRT_LineCap = null;
        public PRT_Line(Vector2 position, Vector2 lineSize, float scale, Color color, int lifetime)
        {
            Position = position;
            LineSize = lineSize;
            Scale = scale;
            Color = color;
            Lifetime = lifetime;
            Velocity = Vector2.Zero;
            Rotation = 0;
        }

        public override void SetProperty() => PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            float rot = LineSize.ToRotation() + MathHelper.PiOver2;
            Vector2 origin = new Vector2(TexValue.Width / 2f, TexValue.Height);
            Vector2 scale = new Vector2(0.2f, LineSize.Length() / TexValue.Height);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, Color, rot, origin, scale, SpriteEffects.None, 0);
            Texture2D cap = PRT_LineCap.Value;
            scale = new Vector2(Scale, Scale);
            origin = new Vector2(cap.Width / 2f, cap.Height / 2f);
            spriteBatch.Draw(cap, Position - Main.screenPosition, null, Color, rot + MathHelper.Pi, origin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(cap, Position - Main.screenPosition, null, Color, rot + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(cap, Position + LineSize - Main.screenPosition, null, Color, rot, origin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
