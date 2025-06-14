using CalamityEntropy.Common;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class VetrasylsEye : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CEKeybinds.VetrasylsEyeBlockHotKey);

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ModContent.RarityType<SkyBlue>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().vetrasylsEye = true;
        }

        public override void AddRecipes()
        {
        }
    }
}
