using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityMod;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items;

public class SignalBlocker : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(0, 0, 0, 20);
    }

    public override void UpdateInventory(Player player)
    {
        if(Item.favorited)
        {
            EModSys.AcropolisDontSpawn = 5;
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient<HellIndustrialComponents>(3).
            AddIngredient<MysteriousCircuitry>().
            AddTile(TileID.WorkBenches).
            Register();
    }
}
