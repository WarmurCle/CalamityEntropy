using CalamityMod;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories.Oath
{
    public class OathBanner : ModItem
    {
        public static float MoveSpeedDecrease = 0.1f;
        public static float Endurance = 0.07f;
        public static int LifeRegenSec = 1;
        public static float BuffDamageAddition = 0.1f;
        public static int BuffCritAddition = 6;
        public static float BuffMoveSpeedAddition = 0.1f;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[MSD]", MoveSpeedDecrease.ToPercent());
            tooltips.Replace("[DR]", Endurance.ToPercent());
            tooltips.Replace("[REG]", LifeRegenSec);
            tooltips.Replace("[DMG]", BuffDamageAddition.ToPercent());
            tooltips.Replace("[CRIT]", BuffCritAddition);
            tooltips.Replace("[MSA]", BuffMoveSpeedAddition.ToPercent());
        }
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 56;
            Item.accessory = true;
            Item.defense = 10;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().oathBanner = true;
            player.Entropy().oathBannerVisual = !hideVisual;
            player.endurance += Endurance;
            player.lifeRegen += LifeRegenSec * 2;
            player.Entropy().moveSpeed -= MoveSpeedDecrease;
        }
        public override void UpdateVanity(Player player)
        {
            player.Entropy().oathBannerVisual = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PalladiumPike).
                AddIngredient(ItemID.SoulofNight, 8).
                AddIngredient(ItemID.Silk, 12).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.CobaltNaginata).
                AddIngredient(ItemID.SoulofNight, 8).
                AddIngredient(ItemID.Silk, 12).
                Register();
        }
    }
    public class OathofCommand : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += OathBanner.BuffDamageAddition;
            player.GetCritChance(DamageClass.Generic) += OathBanner.BuffCritAddition;
            player.Entropy().moveSpeed += OathBanner.BuffMoveSpeedAddition;
        }
    }
}
