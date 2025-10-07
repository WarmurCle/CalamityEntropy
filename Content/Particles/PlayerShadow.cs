using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class PlayerShadow : EParticle
    {
        public override Texture2D Texture => CEUtils.pixelTex;
        public Player plr;
        public override void OnSpawn()
        {
            this.Lifetime = 20;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / (float)this.TimeLeftMax;
        }

        public override void Draw()
        {
            if(plr != null)
            {
                Main.PlayerRenderer.DrawPlayer(Main.Camera, plr, this.Position, this.Rotation, Vector2.Zero, (1 - Opacity * 0.35f), 1);
            }
        }
    }
}
