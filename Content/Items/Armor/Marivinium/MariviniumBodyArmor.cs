using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Armor.OmegaBlue;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Marivinium
{
    [AutoloadEquip(EquipType.Body)]
    public class MariviniumBodyArmor : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.defense = 60;
            Item.rare = ModContent.RarityType<AbyssalBlue>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().mariviniumBody = true;
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.GetCritChance(DamageClass.Generic) += 15;
            player.statManaMax2 += 250;
            player.breath = player.breathMax + 91;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OmegaBlueChestplate>()
                .AddIngredient<WyrmTooth>(6)
                .AddIngredient<FadingRunestone>()
                .AddTile<AbyssalAltarTile>()
                .Register();
        }
    }
}
