using CalamityEntropy.Buffs;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories
{
	public class Godhead : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 22;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EModPlayer>().Godhead = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<DivineGeode>(), 3).
                AddIngredient(ModContent.ItemType<BloodstoneCore>(), 3).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
