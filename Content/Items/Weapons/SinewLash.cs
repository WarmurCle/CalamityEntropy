using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class SinewLash : BaseWhipItem
    {
        public override int TagDamage => 7;
        public override float TagCritChance => 0.06f;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<SinewLashProj>(), 60, 3, 4, 42);
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
    }
}
