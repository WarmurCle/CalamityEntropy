using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo
{
    public class ClusterMissile : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = BaseMissileProj.AmmoType;
            Item.damage = 12;
            Item.shoot = ModContent.ProjectileType<ClusterMissileProj>();
            Item.consumable = true;
            Item.DamageType = DamageClass.Ranged;
        }

        public override void AddRecipes()
        {
            CreateRecipe(15)
                .AddIngredient(ModContent.ItemType<AzafureMissile>(), 15)
                .AddRecipeGroup(CERecipeGroups.AnyOrichalcumBar)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    public class ClusterMissileProj : BaseMissileProj
    {
        public override void SetupStats()
        {
            Projectile.ai[1] += 90;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/ClusterMissile";
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff<MechanicalTrauma>(5 * 60);
            Projectile.Kill();
        }
        public override void ExplodeVisual()
        {
            CEUtils.PlaySound("metalhit", 0.4f, Projectile.Center, volume: 0.2f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            float scale = ExplodeRadius / 40f;
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red * 0.8f, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White * 0.8f, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 15));
        }
        public override void SpawnParticle(Vector2 vel)
        {
            for (int i = 0; i < 8; i++)
            {
                EParticle.NewParticle(new Smoke() { timeleftmax = 19, Lifetime = 19, scaleEnd = 0 }, Projectile.Center + vel * (i / 8f), CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.025f, 0.04f), 0.96f, true, BlendState.Additive, CEUtils.randomRot());
                EParticle.NewParticle(new Smoke() { timeleftmax = 19, Lifetime = 19, scaleEnd = 0 }, Projectile.Center + vel * (i / 8f), CEUtils.randomPointInCircle(0.5f), new Color(255, 160, 160), Main.rand.NextFloat(0.01f, 0.015f), 0.96f, true, BlendState.Additive, CEUtils.randomRot());
            }
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.Entropy().counter > 24)
            {
                Projectile.Kill();
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if(Main.myPlayer == Projectile.owner)
            {
                int mtype = ModContent.ProjectileType<ClusterMissileSmall>()
                    ;
                for(int i = 0; i < 5; i++)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(18, 26), mtype, Projectile.damage / 8, Projectile.knockBack / 3, Projectile.owner, Projectile.ai[0] * 2, Projectile.ai[1] - 90);
                    if (p.ToProj().ModProjectile is BaseMissileProj bmp)
                    {
                        bmp.winding += 0.6f;
                        bmp.Homing += 3.8f;
                        bmp.NoGrav = true;
                    }
                }
            }
        }
    }
    public class ClusterMissileSmall : BaseMissileProj
    {
        public override float StickDamageAddition => 0.03f;
        public override void SetupStats()
        {
            Projectile.ai[1] += 20;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/ClusterMissileSmall";
        public override bool? CanHitNPC(NPC target)
        {
            if(Projectile.Entropy().counter < 4)
                return false;
            return null;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.ArmorPenetration += 32;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff<MechanicalTrauma>(5 * 60);
        }
        public override void ExplodeVisual()
        {
            CEUtils.PlaySound("metalhit", 0.4f, Projectile.Center, volume: 0.1f);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            float scale = ExplodeRadius / 40f;
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red * 0.8f, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White * 0.8f, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 1.4f, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.02f, 15));
        }
        public override void SpawnParticle(Vector2 vel)
        {
            for (int i = 0; i < 2; i++)
            {
                EParticle.NewParticle(new Smoke() { timeleftmax = 19, Lifetime = 19, scaleEnd = 0 }, Projectile.Center + vel * (i / 2f), CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.025f, 0.04f), 0.6f, true, BlendState.Additive, CEUtils.randomRot());
                EParticle.NewParticle(new Smoke() { timeleftmax = 19, Lifetime = 19, scaleEnd = 0 }, Projectile.Center + vel * (i / 2f), CEUtils.randomPointInCircle(0.5f), new Color(255, 160, 160), Main.rand.NextFloat(0.01f, 0.015f), 0.6f, true, BlendState.Additive, CEUtils.randomRot());
            }
        }
    }
}
