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
    public class AzafureMissle : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = BaseMissleProj.AmmoType;
            Item.damage = 9;
            Item.shoot = ModContent.ProjectileType<AzafureMissleProj>();
            Item.consumable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(5)
                .AddIngredient(ModContent.ItemType<HellIndustrialComponents>())
                .AddIngredient(ItemID.IronBar, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class AzafureMissleProj : BaseMissleProj
    {
        public override float StickDamageAddition => 0.07f;
        public override float StickDamageMult => 0.16f;
        public override void SetupStats()
        {
            Projectile.ai[1] += 50;
            Projectile.ai[0]--;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/AzafureMissle";
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff<MechanicalTrauma>(5 * 60);
        }
        public override void ExplodeVisual()
        {
            CEUtils.PlaySound("metalhit", 0.4f, Projectile.Center, volume: 0.24f);
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
                EParticle.NewParticle(new Smoke() { timeleftmax = 19, Lifetime = 19, scaleEnd = 0 }, Projectile.Center + vel * (i / 8f), CEUtils.randomPointInCircle(0.5f), Color.OrangeRed, Main.rand.NextFloat(0.025f, 0.04f), 0.8f, true, BlendState.Additive, CEUtils.randomRot());
                EParticle.NewParticle(new Smoke() { timeleftmax = 19, Lifetime = 19, scaleEnd = 0 }, Projectile.Center + vel * (i / 8f), CEUtils.randomPointInCircle(0.5f), new Color(255, 160, 160), Main.rand.NextFloat(0.01f, 0.015f), 0.8f, true, BlendState.Additive, CEUtils.randomRot());
            }
        }
    }
}
