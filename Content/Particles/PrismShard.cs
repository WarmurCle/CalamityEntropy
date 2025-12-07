using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class PrismShard : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/PrismShard").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 38;
        }
        public override void AI()
        {
            base.AI();
            if (Lifetime == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    EParticle.spawnNew(new PrismShardSmall() { PixelShader = this.PixelShader }, Position, CEUtils.randomPointInCircle(12), Color.White, 1, 1, true, BlendState.AlphaBlend, CEUtils.randomRot());
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustDirect(Position, 0, 0, DustID.MagicMirror).velocity = CEUtils.randomPointInCircle(10);
                }
                SoundEngine.PlaySound(in SoundID.Item27, Position);
            }
        }
    }
    public class PrismShardSmall : EParticle
    {
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Content/Particles/PrismShardSmall").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 120;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = Lifetime / 120f;
            this.Rotation += Velocity.X * 0.025f;
            this.Velocity += new Vector2(0, 0.36f);
        }
    }
}
