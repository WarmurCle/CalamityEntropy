using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Ammo
{
    public class AnnihilateArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 36;

            Item.damage = 5;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 1.8f;
            Item.value = Item.sellPrice(silver: 92);
            Item.shoot = ModContent.ProjectileType<AnnihilateArrowProjectile>();
            Item.shootSpeed = 2f;
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).AddIngredient(ModContent.ItemType<VoidBar>())
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
