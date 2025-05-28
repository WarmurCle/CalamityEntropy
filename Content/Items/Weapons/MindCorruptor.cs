using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class MindCorruptor : BaseWhipItem
    {
        public override int TagDamage => 4;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<MindCorruptorProj>(), 56, 3, 4, 10);
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = Utilities.Util.GetSound("corruptwhip_swing");
        }
        public override bool CanUseItem(Player player)
        {
            Item.UseSound = Utilities.Util.GetSound("corruptwhip_swing", Main.rand.NextFloat(0.6f, 1.4f));
            return true;
        }
    }
}
