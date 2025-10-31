using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class VoidBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 120;
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 8));
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<VoidBarTile>());
            Item.value = Item.sellPrice(gold: 60);
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.maxStack = 9999;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation += new Vector2(-20f * player.direction, 15f * player.gravDir).RotatedBy(player.itemRotation);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.3f * brightness, 0.3f * brightness, 1f * brightness);
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient<VoidOre>(60).
                AddIngredient<VoidScales>().
                AddTile<VoidWellTile>().
                Register();
        }
    }
}
