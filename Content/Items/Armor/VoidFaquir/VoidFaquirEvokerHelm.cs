using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.VoidFaquir
{
    [AutoloadEquip(EquipType.Head)]
    public class VoidFaquirEvokerHelm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 10;
            Item.rare = ModContent.RarityType<VoidPurple>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VoidFaquirBodyArmor>() && legs.type == ModContent.ItemType<VoidFaquirCuises>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.8f;
            player.GetArmorPenetration(DamageClass.Generic) += 20;
            player.maxMinions += 7;
            player.Entropy().VFSet = true;
            player.Entropy().VFHelmSummoner = true;

        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().summonerVF = true;
            player.GetDamage(DamageClass.Summon) += 0.25f;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 14)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 6)
                .AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 3)
                .AddIngredient(ModContent.ItemType<TwistingNether>(), 8).Register();
        }
    }
}
