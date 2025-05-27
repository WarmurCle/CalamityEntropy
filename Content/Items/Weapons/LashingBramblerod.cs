using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class LashingBramblerod : BaseWhipItem
    {
        public override int TagDamage => 9;
        public override float TagCritChance => 0.05f;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Replace("{CRIT}", TagCritChance.ToPercent());
            tooltips.Replace("{DR}", SilvaVine.baseDR.ToPercent());
            tooltips.Replace("{DR2}", SilvaVine.DREachFlower.ToPercent());
            tooltips.Replace("{LR}", SilvaVine.RegenPerFlower);
            tooltips.Replace("{FC}", SilvaVine.MaxFlowers);
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<LashingBramblerodProjectile>(), 80, 3, 4, 28);
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
    }
}
