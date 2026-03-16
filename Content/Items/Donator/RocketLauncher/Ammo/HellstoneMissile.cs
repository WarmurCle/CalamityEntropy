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
    public class HellstoneMissile : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = BaseMissileProj.AmmoType;
            Item.damage = 13;
            Item.shoot = ModContent.ProjectileType<HellstoneMissileProj>();
            Item.consumable = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddIngredient(ItemID.Bomb, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class HellstoneMissileProj : BaseMissileProj
    {
        public override void SetupStats()
        {
            Projectile.ai[1] += 60;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/HellstoneMissile";
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire3, 3 * 60);
        }
        public override void StickUpdate(NPC target)
        {
            target.AddBuff(BuffID.OnFire3, 3 * 60);
        }
        public override void ExplodeVisual()
        {
            for (int i = 0; i < 36; i++)
            {
                var d = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Flare);
                d.noGravity = true;
                d.scale = 1.5f;
                d.velocity = CEUtils.randomPointInCircle(16);
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            float scale = ExplodeRadius / 40f;
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red * 0.3f, scale * 0.8f, 1, true, BlendState.Additive, 0, 10);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White * 0.3f, scale * 0.5f, 1, true, BlendState.Additive, 0, 10);
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Firebrick * 1.6f, "CalamityMod/Particles/SmokeExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.05f, 24));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Firebrick * 1.6f, "CalamityMod/Particles/SmokeExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.045f, 22));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Firebrick * 1.6f, "CalamityMod/Particles/SmokeExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.04f, 20));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Firebrick * 1.6f, "CalamityMod/Particles/SmokeExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.035f, 18));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Firebrick * 1.6f, "CalamityMod/Particles/SmokeExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.03f, 16));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Firebrick * 1.6f, "CalamityMod/Particles/SmokeExplosion", Vector2.One, CEUtils.randomRot(), 0.005f, scale * 0.025f, 14));

        }
        public override void SpawnParticle(Vector2 vel)
        {
            for (int i = 0; i < 8; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, 0, 0, DustID.Flare);
                d.noGravity = true;
                d.position = Projectile.Center + CEUtils.randomPointInCircle(6) + Projectile.velocity * (i / 8f);
                d.velocity = Projectile.velocity * 0.4f;
                d.scale = 1.15f;
            }
        }
    }
}
