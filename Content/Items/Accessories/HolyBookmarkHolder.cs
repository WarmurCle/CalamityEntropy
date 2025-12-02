using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class HolyBookmarkHolder : ModItem
    {
        public static float MAGESPEED = 0.08f;
        public static float MAGEDAMAGE = 0.1f;
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.accessory = true;
            Item.value = Item.buyPrice(0, 10, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().AdditionalBookmarkSlot += 2;
            player.GetAttackSpeed(DamageClass.Magic) += MAGESPEED;
            player.GetDamage(DamageClass.Magic) += MAGEDAMAGE;
            if (!Main.dedServ) 
                for(int i = 0; i < 2; i++)
                    player.Entropy().BookmarkHolderSpecialTextures.Add(CEUtils.RequestTex("CalamityEntropy/Content/UI/EntropyBookUI/Extra3"));
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[D]", MAGEDAMAGE.ToPercent().ToString());
            tooltips.Replace("[S]", MAGESPEED.ToPercent().ToString());
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<MagicBookmarkHolder>()
                .AddIngredient<ExquisiteBookmarkHolder>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
