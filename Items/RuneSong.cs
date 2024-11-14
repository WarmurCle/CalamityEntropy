using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityEntropy.Projectiles.VoidBlade;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Placeables;
using CalamityEntropy.Projectiles;
namespace CalamityEntropy.Items
{
    public class RuneSong : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 1010;
            Item.crit = 18;
            Item.DamageType = DamageClass.Melee;
            Item.width = 56;
            Item.noUseGraphic = true;
            Item.height = 56;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 12000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.channel = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<RuneSongProj>();
            Item.shootSpeed = 6f;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<RuneSongProj>()] < 1;
        }
        public override void AddRecipes()
        {
        }
    }
}