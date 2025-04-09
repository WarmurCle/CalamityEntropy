using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class SinewLash : BaseWhipItem
    {
        public override int TagDamage => 5;
        public override float TagCritChance => 0.025f;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<SinewLashProj>(), 46, 3, 4, 42);
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            return true;
        }
    }
}
