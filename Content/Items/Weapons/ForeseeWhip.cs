using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class ForeseeWhip : BaseWhipItem
    {
        public override int TagDamage => 8;
        public override float TagDamageMult => 1.12f;
        public override float TagCritChance => 0.1f;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<ForeseeWhipProj>(), 80, 3, 4, 24);
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
        }
    }
}
