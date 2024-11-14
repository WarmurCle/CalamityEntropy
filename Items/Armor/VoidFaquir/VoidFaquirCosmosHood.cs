using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using System.Collections.Generic;
using System.Security.Policy;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Armor.VoidFaquir
{
    [AutoloadEquip(EquipType.Head)]
    public class VoidFaquirCosmosHood : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 20;
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
            player.GetDamage(DamageClass.Generic) += 0.25f;
            player.GetCritChance(DamageClass.Generic) += 15;
            player.GetArmorPenetration(DamageClass.Generic) += 20;
            player.Entropy().VFSet = true;

            player.GetDamage(DamageClass.Magic) += 0.35f;
            player.GetCritChance(DamageClass.Magic) += 25;
            player.statManaMax2 += 150;
            player.Entropy().VFHelmMagic = true;
            
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().magiVF = true;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VoidBar>(), 14)
                .AddIngredient(ModContent.ItemType<RuinousSoul>(), 6)
                .AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 6)
                .AddIngredient(ModContent.ItemType<TwistingNether>(), 8).Register();
        }
    }
}
