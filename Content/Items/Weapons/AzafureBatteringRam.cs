using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureBatteringRam : ModItem, IAzafureEnhancable
    {
        public int charge = 0;
        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.crit = 4;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.width = 86;
            Item.height = 28;
            Item.useTime = 46;
            Item.useAnimation = 46;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 10;
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
            CreateRecipe()
                .AddIngredient<HellIndustrialComponents>(4)
                .AddIngredient<DubiousPlating>(10)
                .AddIngredient<AerialiteBar>(5)
                .AddIngredient(ItemID.HellstoneBar, 18)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.zenithWorld)
            {
                tooltips.Add(new TooltipLine(Mod, "Extend Desc", Mod.GetLocalization("BatteringRamZenithText").Value));
            }
        }
    }
}
