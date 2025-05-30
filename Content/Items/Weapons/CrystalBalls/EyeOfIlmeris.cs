﻿using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.CrystalBalls
{
    public class EyeOfIlmeris : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.damage = 14;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 1f;
            Item.UseSound = CEUtils.GetSound("soulshine");
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<EyeOfIlmerisHoldout>();
            Item.shootSpeed = 16f;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SeaPrism>(), 5)
                .AddIngredient(ModContent.ItemType<PearlShard>(), 2)
                .AddIngredient(ItemID.Glass, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override bool MagicPrefix()
        {
            return true;
        }
    }
}
