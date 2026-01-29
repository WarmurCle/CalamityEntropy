using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.AzafureT3
{
    [AutoloadEquip(EquipType.Body)]
    public class AcropolisHeavyArmor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 30;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.defense = 32;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 8f;
            player.maxMinions += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AzafureSteamKnightArmor>()
                .AddIngredient(ItemID.LunarBar, 16)
                .AddIngredient<UnholyEssence>(6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
