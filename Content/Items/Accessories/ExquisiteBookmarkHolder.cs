using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class ExquisiteBookmarkHolder : ModItem
    {
        public static float MAGESPEED = 0.08f;
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.accessory = true;
            Item.value = Item.buyPrice(0, 8, 42, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().AdditionalBookmarkSlot += 1;
            player.GetAttackSpeed(DamageClass.Magic) += MAGESPEED;
            if (!Main.dedServ)
                player.Entropy().BookmarkHolderSpecialTextures.Add(CEUtils.RequestTex("CalamityEntropy/Content/UI/EntropyBookUI/Extra2"));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[S]", MAGESPEED.ToPercent().ToString());
        }
    }
}
