﻿using CalamityEntropy.Content.Projectiles.VoidBlade;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class VoidBlade : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.crit = 4;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 100;
            Item.noUseGraphic = true;
            Item.height = 100;
            Item.useTime = 16;
            Item.useAnimation = 0;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 12000;
            Item.rare = ItemRarityID.Gray;
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(20, 26, 92);
            Item.Entropy().tooltipStyle = 4;
            Item.UseSound = null;
            Item.channel = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<VoidBladeProj>();
            Item.shootSpeed = 1f;
            Item.Entropy().NameColor = new Color(60, 80, 140);
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<VoidBladeProj>()] < 1;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Katana, 1);
            recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 5);
            recipe.AddIngredient(ItemID.Silk, 20);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 12);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
