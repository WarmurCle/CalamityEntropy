﻿using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class PrisonOfPermafrost : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Magic;
            Item.width = 96;
            Item.noUseGraphic = true;
            Item.height = 96;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.channel = true;
            Item.knockBack = 4;
            Item.value = 145000;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.UseSound = null;
            Item.shoot = ModContent.ProjectileType<PrisonOfPermafrostCircle>();
            Item.shootSpeed = 1f;
            Item.mana = 14;
            Item.useStyle = -1;
            Item.noMelee = true;
            Item.crit = 7;
            Item.Entropy().Legend = true;
            Item.Entropy().tooltipStyle = 1;
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(70, 210, 250);
            Item.Entropy().NameColor = new Color(200, 0, 200);
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
        }
        public override bool MagicPrefix()
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            player.channel = true;
            if (player.ownedProjectileCounts[type] < 1)
            {
                return true;
            }
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<GlacialEmbrace>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 5);
            recipe.AddIngredient(ModContent.ItemType<IceBarrage>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 10);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.Register();
        }
    }
}
