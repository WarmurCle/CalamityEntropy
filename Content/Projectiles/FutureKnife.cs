﻿using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Dusts;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace CalamityEntropy.Content.Projectiles
{
    public class FutureKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rogueDC;
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 260;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }
        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.scale = 2.4f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 246 && !Projectile.Calamity().stealthStrike)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 260, 14, 1f);
            }
            if (Main.GameUpdateCount % 2 == 0 || Projectile.Calamity().stealthStrike)
            {
                EParticle.spawnNew(new Particles.RuneParticle(), Projectile.Center, Util.Util.randomRot().ToRotationVector2() * Main.rand.NextFloat(-0.6f, 0.6f), Color.White, 0.8f, 1, true, BlendState.AlphaBlend, 0);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SoulDisorder>(), 300);
            if (Projectile.Calamity().stealthStrike)
            {
                for (float j = 0; j < 360f; j += 60)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<ProphecyRune>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, MathHelper.ToRadians(j), 0, Main.rand.Next(1, 12));
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].Calamity().stealthStrike = true;
                        p.ToProj().netUpdate = true;
                    }
                }
            }
        }
    }


}