using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Cruiser
{

    public class VoidSpike : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VoidTouch>(), 160);
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
            Projectile.timeLeft = 520;
            Projectile.extraUpdates = 1;
        }
        public bool setv = true;
        public override void AI()
        {
            Projectile.localAI[1] = float.Lerp(Projectile.localAI[1], 1, 0.1f);
            if (setv)
            {
                setv = false;
                Projectile.velocity *= 0.5f;
            }
            odp.Add(Projectile.Center);
            if (odp.Count > 24)
            {
                odp.RemoveAt(0);
            }

            if (Projectile.timeLeft < 40)
            {
                Projectile.alpha += 255 / 40;
            }
            for (float i = 0; i < 1; i += 0.25f)
            {
                Particle p = new Particle();
                p.alpha = 0.14f;
                p.position = Projectile.Center - Projectile.velocity * (i + 0.42f);
                p.velocity = new Vector2(0.2f, 0).RotatedBy(CEUtils.randomRot());
                VoidParticles.particles.Add(p);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 1.01f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Cruiser/VoidSpike").Value;
            Main.spriteBatch.UseAdditive();
            Texture2D tex = CEUtils.getExtraTex("Glow2");
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(tex.Width / 2, 0, tex.Width / 2, tex.Height), Color.LightBlue, Projectile.rotation, new Vector2(0, tex.Height / 2), Projectile.scale * new Vector2(0.65f * Projectile.localAI[1] * Projectile.velocity.Length(), 0.012f) * Projectile.localAI[1], SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(tex.Width / 2, 0, tex.Width / 2, tex.Height), Color.Blue, Projectile.rotation, new Vector2(0, tex.Height / 2), Projectile.scale * new Vector2(0.65f * Projectile.localAI[1] * Projectile.velocity.Length(), 0.025f) * Projectile.localAI[1], SpriteEffects.None, 0);
            for(float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver4)
            {
                Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition + (i + Main.GlobalTimeWrappedHourly * 16).ToRotationVector2() * 2, null, Color.White, Projectile.rotation, t.Size() / 2, 1, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
            Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, t.Size() / 2, 1, SpriteEffects.None, 0);
            return false;
        }
    }

}