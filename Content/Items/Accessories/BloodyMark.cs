using CalamityMod;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class BloodyMark : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 65;
            Item.height = 54;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.longInvince = true;
            player.Calamity().rampartOfDeities = true;
            player.pStone = true;
            player.lifeRegen += 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CrossNecklace).
                AddIngredient(ItemID.CharmofMyths).
                AddIngredient<CoreofCalamity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }    
    }
}
