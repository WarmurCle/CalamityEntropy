using CalamityEntropy.Tiles;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items
{
    public class VoidBar : ModItem
    {


        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 120;
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 13));
        }


        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<VoidBarTile>());
            Item.value = Item.sellPrice(gold: 60);
            Item.rare = ModContent.RarityType<VoidPurple>();
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
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
