using CalamityMod.Walls;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Walls
{
    public class BeewaxWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

        public override void SetDefaults() => Item.DefaultToPlaceableWall(ModContent.WallType<GiantHiveWall>());

        public override void AddRecipes()
        {
            CreateRecipe(40).
            AddIngredient(ItemID.BeeWax).
            AddTile(TileID.HoneyDispenser).
            Register();
        }
    }
}
