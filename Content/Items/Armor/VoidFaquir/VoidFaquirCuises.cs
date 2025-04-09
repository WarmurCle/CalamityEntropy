using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.VoidFaquir
{
    [AutoloadEquip(EquipType.Legs)]
    public class VoidFaquirCuises : ModItem, ILocalizedModType
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 32;
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().VFLeg = true;
            player.GetDamage(DamageClass.Generic) += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 12)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 6)
                .AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 3)
                .AddIngredient(ModContent.ItemType<TwistingNether>(), 8).Register();
        }
    }
}
