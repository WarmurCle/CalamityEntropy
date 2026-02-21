using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class WindOfUndertaker : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CruiserWhipDebuff.TagDamage);

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<WindOfUndertakerProjectile>(), 220, 2, 8, 28);
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
