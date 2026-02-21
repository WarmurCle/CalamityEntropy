using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class WhipOfService : ModItem
    {

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<WhipOfServiceProjectile>(), 24, 2, 4, 36);
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;

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
