using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class MagicBookmarkHolder : ModItem, IPriceFromRecipe
    {
        public int AdditionalPrice => 200;
        public static float MAGEDAMAGE = 0.08f;
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 30;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().AdditionalBookmarkSlot += 1;
            player.GetDamage(DamageClass.Magic) += MAGEDAMAGE;
            if (!Main.dedServ)
                player.Entropy().BookmarkHolderSpecialTextures.Add(CEUtils.RequestTex("CalamityEntropy/Content/UI/EntropyBookUI/Extra1"));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[D]", MAGEDAMAGE.ToPercent().ToString());
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 2)
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient(ItemID.RichMahogany, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
