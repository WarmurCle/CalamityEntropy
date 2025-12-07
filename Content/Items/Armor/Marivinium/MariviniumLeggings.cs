using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Armor.OmegaBlue;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.Marivinium
{
    [AutoloadEquip(EquipType.Legs)]
    public class MariviniumLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.defense = 56;
            Item.rare = ModContent.RarityType<AbyssalBlue>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().moveSpeed += 0.36f;
            player.Entropy().ManaCost -= 0.2f;
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.GetCritChance(DamageClass.Generic) += 5;

        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OmegaBlueTentacles>()
                .AddIngredient<WyrmTooth>(5)
                .AddIngredient<FadingRunestone>()
                .AddTile<AbyssalAltarTile>()
                .Register();
        }
    }

}
