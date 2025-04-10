using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureBatteringRam : ModItem
    {
        public int charge = 0;
        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.crit = 16;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 194;
            Item.height = 42;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 16;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<BatteringRamProj>();
            Item.shootSpeed = 8;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage * 5, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AerialiteBar>(5)
                .AddIngredient<DubiousPlating>(15)
                .AddIngredient<EnergyCore>(2)
                .AddIngredient(ItemID.HellstoneBar, 18)
                .AddIngredient(ItemID.IronBar, 20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}