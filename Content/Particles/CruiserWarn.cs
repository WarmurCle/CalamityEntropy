using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    public class CruiserWarn : BasePRT, IAdditivePRT
    {
        public override string Texture => "CalamityEntropy/Content/Particles/CrLine";
        public override void SetProperty()
        {
            PRTLayersMode = PRTLayersModeEnum.NoDraw;
            this.Lifetime = 30;
        }
        public override void AI()
        {
            this.Opacity = this.Lifetime / 30f;
        }
        void IAdditivePRT.Draw(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.Draw(TexValue, this.Position - Main.screenPosition, null, Color * Opacity
                , Rotation, new Vector2(0, TexValue.Height / 2), Scale, SpriteEffects.None, 0);
        }
    }
}
