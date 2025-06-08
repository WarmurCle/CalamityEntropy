using CalamityEntropy.Content.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.LuminarisShoots
{
    public class LuminarisVortex : ModProjectile
    {
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 160);
        }
        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = 400;
        }
        public override void AI()
        {
            CEUtils.recordOldPosAndRots(Projectile, ref odp, ref odr, 16);
            Projectile.scale = 1.6f * (1 + ((float)(Math.Cos(Main.GameUpdateCount * 0.12f)) * 0.1f));
            Projectile.rotation += 0.1f;
            Vector2 top = Projectile.Center - Projectile.velocity + CEUtils.randomPointInCircle(Projectile.GetTexture().Width * 0.5f * Projectile.scale * 0.95f);
            Vector2 sparkVelocity2 = Projectile.velocity * 0.25f;
            int sparkLifetime2 = Main.rand.Next(28, 40);
            float sparkScale2 = Main.rand.NextFloat(0.6f, 1.6f);
            Color sparkColor2 = Color.Lerp(Color.SkyBlue, Color.Purple, Main.rand.NextFloat(0, 1));
            LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
            GeneralParticleHandler.SpawnParticle(spark);
            Projectile.velocity *= 1.01f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CEUtils.DrawAfterimage(Projectile.GetTexture(), odp, odr, Projectile.scale);

            Main.EntitySpriteDraw(Projectile.getDrawData(Color.White));

            float orgRot = Projectile.rotation;
            float orgScale = Projectile.scale;
            Projectile.rotation = -Main.GlobalTimeWrappedHourly;
            Projectile.scale *= 0.6f;

            Main.EntitySpriteDraw(Projectile.getDrawData(Color.White));

            Projectile.rotation = orgRot;
            Projectile.scale = orgScale;

            
            return false;
        }
    }

}