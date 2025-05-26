using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class LashingBramblerod : BaseWhipItem
    {
        public override int TagDamage => 9;
        public override float TagCritChance => 0.1f;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<LashingBramblerodProjectile>(), 80, 3, 4, 28);
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
    }
}
