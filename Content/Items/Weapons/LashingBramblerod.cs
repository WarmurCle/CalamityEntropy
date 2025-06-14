using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class LashingBramblerod : BaseWhipItem
    {
        public override int TagDamage => 10;
        public override float TagCritChance => 0.05f;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Replace("[1]", TagCritChance.ToPercent());
            tooltips.Replace("[2]", SilvaVine.baseDR.ToPercent());
            tooltips.Replace("[3]", SilvaVine.DREachFlower.ToPercent());
            tooltips.Replace("[4]", SilvaVine.RegenPerFlower);
            tooltips.Replace("[5]", SilvaVine.MaxFlowers);
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<LashingBramblerodProjectile>(), 80, 3, 4, 60);
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
    }
}
