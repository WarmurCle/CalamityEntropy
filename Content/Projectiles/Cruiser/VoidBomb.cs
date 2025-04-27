using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Utilities;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Cruiser
{

    public class VoidBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VoidTouch>(), 160);
            Projectile.Kill();
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 120;

        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.rotation);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.rotation = reader.ReadSingle();
        }
        public override void AI()
        {
            if (Projectile.ai[2] == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.ai[2]++;
            }
            Projectile.rotation += (Projectile.whoAmI % 2 == 0 ? 1 : -1) * Projectile.velocity.Length() * 0.01f;
            counter++;
            Projectile.velocity *= 0.97f;
            if(Main.dedServ)
                Utilities.Util.SyncProj(Projectile.whoAmI);
        }
        public float counter = 0;
        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2() * 30, ModContent.ProjectileType<VoidSpike>(), Projectile.damage, 2);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.rotation.ToRotationVector2() * -30, ModContent.ProjectileType<VoidSpike>(), Projectile.damage, 2);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<VoidExplode>(), Projectile.damage, 2, -1, -0.4f, -0.4f).ToProj().Resize(100, 100);
            }
            Utilities.Util.PlaySound("explosion", 1, Projectile.Center, 4);
            CalamityMod.Particles.Particle pulse = new PlasmaExplosion(Projectile.Center, Vector2.Zero, new Color(160, 120, 255), new Vector2(2f, 2f), 0, 0f, 0.036f, 46);
            GeneralParticleHandler.SpawnParticle(pulse);
            CalamityMod.Particles.Particle explosion2 = new DetailedExplosion(Projectile.Center, Vector2.Zero, new Color(180, 156, 255), Vector2.One, Main.rand.NextFloat(-5, 5), 0f, 0.46f, 30);
            GeneralParticleHandler.SpawnParticle(explosion2);
            for (float i = 0; i <= 1; i += 0.05f)
            {
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.White, Color.SkyBlue, i) * 0.8f, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), 0.01f, i * 0.12f, (int)((1.2f - i) * 60)));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D t = Utilities.Util.getExtraTex("a_circle");
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, null, new Color(180, 180, 255) * ((float)counter / 120f) * 0.66f, Projectile.rotation, t.Size() / 2f, new Vector2(12, 0.16f), SpriteEffects.None);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }

}