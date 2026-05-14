using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo
{
    public class AerialiteMissile : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = BaseMissileProj.AmmoType;
            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<AerialiteMissileProj>();
            Item.consumable = true;
            Item.DamageType = DamageClass.Ranged;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient(ModContent.ItemType<AerialiteBar>(), 1)
                .AddIngredient(ModContent.ItemType<OsseousRemains>())
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class AerialiteMissileProj : BaseMissileProj
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/AerialiteMissile";
        public override void SetupStats()
        {
            Projectile.ai[1] += 20;
        }
        public override void ExplodeVisual()
        {
            CEUtils.PlaySound("GrassSwordHit1", Main.rand.NextFloat(1.2f, 1.3f), Projectile.Center, 20, 0.4f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            float scale = ExplodeRadius / 40f;
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.SkyBlue * 1.2f, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Gold, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, new Color(234, 240, 210), "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 15));
        }
        public override void SpawnParticle(Vector2 vel)
        {
            GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center, vel * 0.4f, false, 12, Main.rand.NextFloat(0.02f, 0.03f), Color.LightSkyBlue, new Vector2(1.2f, 1f), true, false));
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (Main.myPlayer == Projectile.owner)
            {
                int ptype = ModContent.ProjectileType<AerialiteMissileFeather>();
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * 8, ptype, Projectile.damage / 6, Projectile.knockBack / 4, Projectile.owner, Main.rand.Next(0, i * 16));
                }
            }
        }
    }
    public class AerialiteMissileFeather : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged);
            Projectile.width = Projectile.height = 32;
            Projectile.MaxUpdates = 2;
        }
        public Vector2 targetPos
        {
            get { return new Vector2(Projectile.ai[1], Projectile.ai[2]); }
            set { Projectile.ai[1] = value.X; Projectile.ai[2] = value.Y; }
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                Projectile.rotation = CEUtils.randomRot();
                targetPos = Projectile.Center;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 50)
            {
                NPC target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 1200);
                if (target != null)
                    targetPos = target.Center;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] < 50)
            {
                Projectile.velocity *= 0.96f;
                Projectile.rotation += Projectile.velocity.Length() * Math.Sign(Projectile.velocity.X) * 0.05f;
            }
            else
            {
                if (Projectile.ai[0] == 50)
                {
                    Projectile.velocity += (Projectile.Center - targetPos).normalize() * 8;
                }
                if (Projectile.ai[0] < 68)
                {
                    Projectile.rotation = CEUtils.RotateTowardsAngle(Projectile.rotation, (targetPos - Projectile.Center).ToRotation(), 0.1f, false);
                    Projectile.velocity *= 0.96f;
                }
                if (Projectile.ai[0] == 68)
                {
                    Projectile.rotation = (targetPos - Projectile.Center).ToRotation();
                    Projectile.velocity = (targetPos - Projectile.Center).normalize() * 20;
                }
                if (Projectile.ai[0] >= 68)
                {
                    GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(Projectile.Center, Projectile.velocity * 0.4f, false, 6, Main.rand.NextFloat(0.012f, 0.02f), Color.LightSkyBlue, new Vector2(1f, 1f), true, false));
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] >= 70 ? null : false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("GrassSwordHit1", Main.rand.NextFloat(1.85f, 2f), target.Center, 20, 0.3f);
            float r = Main.GameUpdateCount * 0.384f;
            for (int i = 0; i < 6; i++)
            {
                Vector2 velocity = ((MathHelper.TwoPi * i / 6) - (MathHelper.Pi / 16f) + r).ToRotationVector2() * 10f;
                var sparkle = new CritSpark(target.Center, velocity, Color.White, Color.LightSkyBlue, 0.6f, 20, 0.1f, 3f, Main.rand.NextFloat(0f, 0.01f));
                GeneralParticleHandler.SpawnParticle(sparkle);
            }
        }
    }
}
