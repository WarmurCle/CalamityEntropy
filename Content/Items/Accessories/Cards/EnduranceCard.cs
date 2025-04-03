using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Fishing.SulphurCatches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.Cards
{
    public class EnduranceCard : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EModPlayer>().enduranceCard = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<HydrothermalCrate>(), 5)
                .AddTile(TileID.WorkBenches).Register();
        }
    }
}
