using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class HellIndustrialComponents : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 16;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 4);
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 9999;
        }
    }
}
