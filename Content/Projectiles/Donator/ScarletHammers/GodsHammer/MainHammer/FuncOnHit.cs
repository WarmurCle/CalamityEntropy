using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.Phantasmal;
using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.ScarletParticles;
using CalamityMod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.MainHammer
{
    public partial class GodsHammerProj: BaseHammerClass, ILocalizedModType
    {
        private List<int> ProjID = [];
        public void StealthHit(NPC target, int hitDamage, int targetIndex)
        {
            for (int i = -1; i < 2; i += 2)
            {
                //归一化方向并提供自定义速度
                Vector2 direc = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 spawnVelocity = (direc * 32f).RotatedBy(MathHelper.PiOver4 * i);
                //两把锤子有属于自己的转角
                //不会这里写成了一个终极史山吧？
                float arcAngle = i * (MathHelper.PiOver2 + MathHelper.PiOver4);
                int echo = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, spawnVelocity, ModContent.ProjectileType<PhantasmalHammer>(), Projectile.damage / 2, 0f, Owner.whoAmI, targetIndex, 0f, arcAngle);
                Main.projectile[echo].Calamity().stealthStrike = true;
                ProjID.Add(echo);
            }
        }
        private void PrettySpark(int hitDamage)
        {
            //圆环
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.scale;
            for (int i = 0; i < 36; i++)
            {
                Vector2 dir2 = MathHelper.ToRadians(i * 10f).ToRotationVector2() * Projectile.scale;
                dir2.X /= 3.6f;
                dir2 = dir2.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 pos = Projectile.Center + dir * 12f + dir2 * 18f;
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(pos, dir2 * 5f, Main.rand.NextBool() ? Color.White : Color.HotPink, 40, (3.5f - Math.Abs(18f - i) / 6f), BlendState.Additive);
                shinyOrbParticle.Spawn();
            }
            //从灾厄抄写的锤子特效
            float damageInterpolant = Utils.GetLerpValue(950f, 2000f, hitDamage, true);
            Vector2 splatterDirection = Projectile.velocity * 0.8f;
            for (int i = 0; i < 10; i++)
            {
                int sparkLifetime = Main.rand.Next(55, 70);
                float sparkScale = Main.rand.NextFloat(0.7f, Main.rand.NextFloat(3.3f, 5.5f)) + damageInterpolant * 0.85f;
                Color sparkColor = Color.Lerp(Color.Purple, Color.GhostWhite, Main.rand.NextFloat(0.7f));
                sparkColor = Color.Lerp(sparkColor, Color.HotPink, Main.rand.NextFloat());

                Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.7f) * Main.rand.NextFloat(1.4f, 1.8f);
                sparkVelocity.Y -= 7f;
                SparkParticle spark = new(Projectile.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }

        }
        public void NormalHit(NPC target)
        {
            int dustSets = Main.rand.Next(5, 8);
            int dustRadius = 6;
            Vector2 corner = new(target.Center.X - dustRadius, target.Center.Y - dustRadius);
            for (int i = 0; i < dustSets; ++i)
            {
                float scaleOrb = 1.2f + Main.rand.NextFloat(1f);
                Dust orb = Dust.NewDustDirect(corner, 2 * dustRadius, 2 * dustRadius, DustID.Clentaminator_Purple);
                orb.noGravity = true;
                orb.velocity *= 4f;
                orb.scale = scaleOrb;
                for (int j = 0; j < 6; ++j)
                {
                    float scaleSparkle = 0.8f + Main.rand.NextFloat(1.1f);
                    Dust sparkle = Dust.NewDustDirect(corner, 2 * dustRadius, 2 * dustRadius, DustID.ShadowbeamStaff);
                    sparkle.noGravity = true;
                    float dustSpeed = Main.rand.NextFloat(10f, 18f);
                    sparkle.velocity = Main.rand.NextVector2Unit() * dustSpeed;
                    sparkle.scale = scaleSparkle;
                }
            }
            //生成一点星云射线。
            Projectile.netUpdate = true;
            int laserID = ModContent.ProjectileType<NebulaShot>();
            int laserDamage = Projectile.damage / 2;
            float laserKB = 2.5f;
            int numLasers = 3;
            for (int i = 0; i < numLasers; ++i)
            {
                float startDist = Main.rand.NextFloat(260f, 270f);
                Vector2 startDir = Main.rand.NextVector2Unit();
                Vector2 startPoint = target.Center + startDir * startDist;

                float laserSpeed = Main.rand.NextFloat(15f, 18f);
                Vector2 velocity = startDir * -laserSpeed;

                if (Projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPoint, velocity, laserID, laserDamage, laserKB, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].DamageType = ModContent.GetInstance<RogueDamageClass>();
                        Main.projectile[proj].tileCollide = false;
                        Main.projectile[proj].timeLeft = 30;
                    }
                }
            }

        }
    }
}