using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using InfernumMode.Common.Graphics.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo
{
    public class HallowedMissile : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = BaseMissileProj.AmmoType;
            Item.damage = 9;
            Item.shoot = ModContent.ProjectileType<HallowedMissileProj>();
            Item.consumable = true;
            Item.DamageType = DamageClass.Ranged;
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient(ItemID.HallowedBar)
                .AddIngredient(ModContent.ItemType<CharredMissile>(), 25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class HallowedMissileProj : BaseMissileProj
    {
        public override float StickDamageAddition => 0.07f;
        public override void SetupStats()
        {
            Projectile.ai[1] += 100;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/HallowedMissile";
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff<MechanicalTrauma>(5 * 60);
        }
        public override void ExplodeVisual()
        {
            CEUtils.PlaySound("metalhit", 0.4f, Projectile.Center, volume: 0.16f);
            CEUtils.PlaySound("explosionbig", 1.1f, Projectile.Center, volume: 0.98f);
            
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            float scale = ExplodeRadius / 40f;
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Gold * 1.2f, "CalamityMod/Particles/DetailedExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.2f, 22));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Gold * 1.2f, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.047f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Gold, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Gold * 0.8f, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 15));
            Vector2 BurstFXDirection = new Vector2(0, 6 * 0.16f).RotatedBy(MathHelper.PiOver4);
            for (int i = 0; i < 16; i++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark = new GlowSparkParticle(Projectile.Center, (BurstFXDirection) * (i + 1f), false, 12, (0.25f - i * 0.02f) * 0.3f, randomColor, new Vector2(2.7f, 1.3f), true);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int k = 0; k < 25; k++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark2 = new GlowSparkParticle(Projectile.Center + Main.rand.NextVector2Circular(30 * 0.16f, 30 * 0.16f), BurstFXDirection * Main.rand.NextFloat(1f, 20.5f), false, Main.rand.Next(40, 50 + 1), Main.rand.NextFloat(0.01f, 0.02f), randomColor, new Vector2(0.3f, 1.6f));
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            for (int i = 0; i < 16; i++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark = new GlowSparkParticle(Projectile.Center, (-BurstFXDirection) * (i + 1f), false, 12, (0.25f - i * 0.02f) * 0.3f, randomColor, new Vector2(2.7f, 1.3f), true);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int k = 0; k < 25; k++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark2 = new GlowSparkParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), -BurstFXDirection * Main.rand.NextFloat(1f, 20.5f), false, Main.rand.Next(40, 50 + 1), Main.rand.NextFloat(0.01f, 0.02f), randomColor, new Vector2(0.3f, 1.6f));
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            Vector2 BurstFXDirection2 = new Vector2(6 * 0.16f, 0).RotatedBy(MathHelper.PiOver4);
            for (int i = 0; i < 16; i++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark = new GlowSparkParticle(Projectile.Center, (BurstFXDirection2) * (i + 1f), false, 12, (0.25f - i * 0.02f) * 0.3f, randomColor, new Vector2(2.7f, 1.3f), true);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int k = 0; k < 25; k++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark2 = new GlowSparkParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), BurstFXDirection2 * Main.rand.NextFloat(1f, 20.5f), false, Main.rand.Next(40, 50 + 1), Main.rand.NextFloat(0.01f, 0.02f), randomColor, new Vector2(0.3f, 1.6f));
                GeneralParticleHandler.SpawnParticle(spark2);
            }
            for (int i = 0; i < 16; i++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark = new GlowSparkParticle(Projectile.Center, (-BurstFXDirection2) * (i + 1f), false, 12, (0.25f - i * 0.02f) * 0.3f, randomColor, new Vector2(2.7f, 1.3f), true);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int k = 0; k < 25; k++)
            {
                var randomColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };

                GlowSparkParticle spark2 = new GlowSparkParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), -BurstFXDirection2 * Main.rand.NextFloat(1f, 20.5f), false, Main.rand.Next(40, 50 + 1), Main.rand.NextFloat(0.01f, 0.02f), randomColor, new Vector2(0.3f, 1.6f));
                GeneralParticleHandler.SpawnParticle(spark2);
            }
        }
        public override void SpawnParticle(Vector2 vel)
        {
            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center - vel * 0.5f, vel * 0.1f, false, 12, 0.024f, Color.Gold, new Vector2(0.8f, 0.9f), false, false));
            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center, vel * 0.1f, false, 12, 0.028f, Color.Gold, new Vector2(1f, 0.7f), false, false));
            CEUtils.AddLight(Projectile.Center, Color.Gold);
        }
    }
}
