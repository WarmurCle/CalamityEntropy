using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Whips
{
    public class WhipOfService : ModItem
    {

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<WhipOfServiceProjectile>(), 24, 2, 4, 36);
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float swingDirection = 0.6f + (0.4f * Main.rand.NextFloat());
            if (Main.rand.NextBool(3))
            {
                swingDirection *= -2.5f;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, swingDirection);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.BlandWhip)
                .AddIngredient(ModContent.ItemType<AncientBoneDust>(), 2)
                .Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
