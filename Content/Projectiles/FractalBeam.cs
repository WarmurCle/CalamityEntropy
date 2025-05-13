using CalamityEntropy.Content.Dusts;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityEntropy.Content.Particles;

namespace CalamityEntropy.Content.Projectiles
{
    public class FractalBeam : ModProjectile
    {
        public override string Texture => Util.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 16; 
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.velocity.normalize() * 1200, targetHitbox, 16);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity.normalize() * 1200, 16, DelegateMethods.CutTiles);
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0]++;

                Vector2 norl = Projectile.velocity.normalize();
                float sengs = 3;
                var color = Color.SkyBlue;
                
                for (int j = 0; j < 120; j++)
                {
                    var spark = new HeavenfallStar();
                    EParticle.spawnNew(spark, Projectile.Center, norl * (0.1f + j * 0.34f) * sengs, color, Main.rand.NextFloat(0.6f, 1.3f), 1, true, BlendState.Additive, norl.ToRotation(), 24);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}