using CalamityMod;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class BigShotsWing : ModItem, ISpecialDrawingWing
    {
        public static float HorSpeed = 4f;
        public static float AccMul = 0.5f;
        public static int wTime = 1000;
        public int AnimationTick => 4;
        public int FallingFrame => 2;
        public int MaxFrame => 8;
        public int SlowFallingFrame => 1;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(wTime, HorSpeed, AccMul, false, 20, 2.8f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 12, 25);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", HorSpeed);
            tooltips.Replace("[B]", AccMul);
            tooltips.Replace("[C]", wTime);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().addEquip("BSWing", !hideVisual);
            if (!hideVisual)
            {
                player.Entropy().light += 0.5f;
            }
            bool flag = false;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.IsABoss())
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                player.Entropy().WingTimeMult *= 0.1f;
            }
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().addEquipVisual("BSWing");
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 1.2f;
            maxAscentMultiplier = 1.2f;
            constantAscend = 0.06f;
        }

    }
}