using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class PlagueInternalCombustionEngine : ModItem
    {
        public static float ATKSpeed = 0.10f;
        public override void SetDefaults()
        {
            Item.width = 98;
            Item.height = 60;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().plagueEngine = true;
            player.GetAttackSpeed(DamageClass.Melee) += ATKSpeed;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("{AttackSpeed}", ATKSpeed.ToPercent());
        }

        public static void ApplyTrueMeleeEffect(Player player)
        {
            if (player.Entropy().TryHealMeWithCd(3, 1))
            {
                player.Entropy().temporaryArmor += 1f;
            }
        }
    }
}
