using CalamityEntropy.Content.Buffs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class AcropolisTeslaBall : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public List<Vector2> oldPos = new List<Vector2>();
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 1024;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 4;
            Projectile.friendly = false;
        }
        public override void AI()
        {
            Projectile.tileCollide = Projectile.velocity.Length() > 4;
            oldPos.Add(Projectile.Center);
            if (oldPos.Count > 16)
            {
                oldPos.RemoveAt(0);
            }
            if (Projectile.ai[2]++ > 60)
            {
                Projectile.velocity.Y += 0.02f;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<MechanicalTrauma>(), 180);
        }
        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 0)
            {
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, new Color(255, 180, 180), new Vector2(2f, 2f), 0, 0.1f, 0.6f, 16);
                GeneralParticleHandler.SpawnParticle(pulse);
                CEUtils.SetShake(Projectile.Center, 4);
                CEUtils.PlaySound("energyImpact", Main.rand.NextFloat(0.7f, 1.3f), Projectile.Center);
                CEUtils.SpawnExplotionHostile(((int)Projectile.ai[1]).ToNPC().GetSource_FromAI(), Projectile.Center, Projectile.damage, 100);
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            float scale = 1 * Projectile.scale;
            DrawEnergyBall(Projectile.Center, scale, Projectile.Opacity);
            for (int i = 0; i < oldPos.Count; i++)
            {
                float c = (i + 1f) / oldPos.Count;
                DrawEnergyBall(oldPos[i], scale * c, Projectile.Opacity * c);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public static void DrawEnergyBall(Vector2 pos, float size, float alpha)
        {
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(255, 230, 230) * alpha, 0, tex.Size() * 0.5f, size * 0.24f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(255, 40, 40) * alpha, 0, tex.Size() * 0.5f, size * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
        }
    }


}
