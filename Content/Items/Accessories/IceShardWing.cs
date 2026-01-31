using CalamityEntropy.Common;
using CalamityMod;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class IceShardWing : ModItem
    {
        public static float HorSpeed = 7.6f;
        public static float AccMul = 1;
        public static int wTime = 180;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(wTime, HorSpeed, AccMul, false, 20, 2.8f);
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 12, 25);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.Calamity().donorItem = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", HorSpeed);
            tooltips.Replace("[B]", AccMul);
            tooltips.Replace("[C]", wTime);
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.8f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.8f;
            constantAscend = 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SoulofFlight, 20).
                AddIngredient<CryonicBar>(5).
                AddIngredient(ItemID.PurificationPowder, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}