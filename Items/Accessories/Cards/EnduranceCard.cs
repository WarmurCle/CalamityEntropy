using CalamityEntropy.Buffs;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories.Cards
{
	public class EnduranceCard : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EModPlayer>().enduranceCard = true;
            player.Calamity().defenseDamageRatio *= 0.65;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<HydrothermalCrate>(), 5)
                .AddTile(TileID.WorkBenches).Register();
        }
    }
}
