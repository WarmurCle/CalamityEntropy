using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Armor.VoidFaquir
{
    [AutoloadEquip(EquipType.Head)]
    public class VoidFaquirLurkerMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.defense = 30;
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

            player.Calamity().wearingRogueArmor = true;
            player.GetArmorPenetration(DamageClass.Generic) += 20;
            player.Calamity().rogueStealthMax += 1.35f;
            player.Entropy().VFSet = true;
            player.Entropy().VFHelmRogue = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Entropy().rogueVF = true;
            player.GetDamage(CEUtils.RogueDC) += 0.25f;
            player.GetCritChance(CEUtils.RogueDC) += 25;
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
                .AddIngredient(ModContent.ItemType<TwistingNether>(), 8)
                .AddTile(ModContent.TileType<CosmicAnvil>())
                .Register();
        }
    }
}
