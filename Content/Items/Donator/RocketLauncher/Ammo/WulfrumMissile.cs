using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo
{
    public class WulfrumMissile : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Orange;
            Item.ammo = BaseMissileProj.AmmoType;
            Item.damage = 4;
            Item.shoot = ModContent.ProjectileType<WulfrumMissileProj>();
            Item.consumable = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 4;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient(ModContent.ItemType<OsseousRemains>())
		        .AddIngredient<WulfrumMetalScrap>(1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class WulfrumMissileProj : BaseMissileProj
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/RocketLauncher/Ammo/WulfrumMissile";
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.OnFire3, 3 * 60);
        }
        public override float Gravity => 0.7f;
        public override void StickUpdate(NPC target)
        {
            target.AddBuff(BuffID.OnFire3, 3 * 60);
        }
        public override void SpawnParticle(Vector2 vel)
        {
            for (int i = 0; i < 4; i++)
            {
                var d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Smoke);
                d.noGravity = true;
                d.position += Projectile.velocity * (i / 4f) + CEUtils.randomPointInCircle(6);
                d.velocity = Projectile.velocity * 0.2f;
                d.scale = 1f;
            }
        }
    }
}
