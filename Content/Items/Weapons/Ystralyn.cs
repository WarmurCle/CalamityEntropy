﻿using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Ystralyn : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DragonWhipDebuff.TagDamage);
        public static int PhantomDamage = 2400;
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<YstralynProj>(), 1000, 2, 4, 27);
            Item.rare = ModContent.RarityType<AbyssalBlue>();
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.autoReuse = true;
        }


        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.RainbowWhip)
                .AddIngredient(ModContent.ItemType<WyrmTooth>(), 12)
                .AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5)
                .AddTile(ModContent.TileType<AbyssalAltarTile>())
                .Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
