using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class JailerWhip : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(JailerWhipDebuff.TagDamage);

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<JailerWhipProjectile>(), 17, 2, 4);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<DemonicBoneAsh>(), 2)
                .AddIngredient(ItemID.Chain, 6)
                .AddIngredient(ItemID.Silk, 4)
                .AddIngredient(ItemID.HellstoneBar, 5)
                .AddTile(TileID.Anvils).Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
