using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class TatteredGun : ModItem
    {

        public override bool RangedPrefix()
        {
            return true;
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 20;
            Item.damage = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1f;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item11;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.StoneBlock, 20).AddTile(TileID.WorkBenches).Register();
        }
    }
}
