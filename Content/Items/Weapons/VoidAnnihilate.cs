using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class VoidAnnihilate : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 2400;
            Item.crit = 8;
            Item.DamageType = DamageClass.Melee;
            Item.width = 64;
            Item.noUseGraphic = true;
            Item.height = 64;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 12000;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<VoidAnnihilateCharge>();
            Item.shootSpeed = 42f;
            Item.ArmorPenetration = 50;
        }
        public override bool CanUseItem(Player player)
        {
            Item.channel = player.altFunctionUse != 2;
            return true;
        }
        public override void AddRecipes()
        {
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<VoidAnnihilateSpawner>();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

    }
}
