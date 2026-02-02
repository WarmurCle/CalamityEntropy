using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Daylight : BaseWhipItem
    {
        public override int TagDamage => 6;
        public override float TagCritChance => 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<DaylightProjectile>(), 40, 4, 8f, 30);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item130;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.autoReuse = true;
            Item.width = 44;
            Item.height = 38;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddIngredient<EssenceofSunlight>(8)
                .AddIngredient(ItemID.GoldBar, 6)
                .AddIngredient(ItemID.Silk, 6)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddIngredient<EssenceofSunlight>(8)
                .AddIngredient(ItemID.PlatinumBar, 6)
                .AddIngredient(ItemID.Silk, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
