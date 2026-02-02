using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class SinewLash : BaseWhipItem
    {
        public override int TagDamage => 10;
        public override float TagCritChance => 0.06f;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<SinewLashProj>(), 30, 3, 4, 42);
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(player.direction > 0 ? 0.12f : -0.12f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
