using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
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
            player.Entropy().moveSpeed += 0.15f;
            player.Entropy().ManaCost -= 0.2f;
            player.GetDamage(DamageClass.Generic) += 0.2f;
            player.GetCritChance(DamageClass.Generic) += 5;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.Entropy().MariviniumSet)
            {
                tooltips.Add(new TooltipLine(Mod, "Armor Bonus", Mod.GetLocalization("MariviniumSet").Value));
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OmegaBlueTentacles>()
                .AddIngredient<WyrmTooth>(4)
                .AddIngredient<AscendantSpiritEssence>(2).AddTile<AbyssalAltarTile>()
                .Register();
        }
    }

}
