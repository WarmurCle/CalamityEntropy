using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.NihTwins
{
    [AutoloadEquip(EquipType.Body)]
    public class VoidEaterBodyArmor : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 42;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.defense = 35;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NihilityFragments>(8)
                .AddIngredient(ModContent.ItemType<Necroplasm>(), 6)
                .AddIngredient(ItemID.LunarBar, 12)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
