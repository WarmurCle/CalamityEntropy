using CalamityEntropy.Content.Items.Donator.RocketLauncher;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher
{
    public class Hope : ModItem
    {
        public static int MaxStick => 2;
        public static int ExplodeRadius => 120;
        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CharredMissleProj>(), BaseMissleProj.AmmoType, singleShotTime: 52, shotVelocity: 20f, hasAutoReuse: true);
            Item.width = 90;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 30;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item61;
            Item.value = Item.buyPrice(silver: 30); 
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().tooltipStyle = 8;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().NameColor = new Color(80, 220, 255);
            Item.Entropy().NameLightColor = Color.Transparent;
        }

        #region R
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8f, 2f);
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AerialiteBar>(15)
                .AddIngredient<BloodSample>(10)
                .AddIngredient<OsseousRemains>(5)
                .AddIngredient(ItemID.Vertebrae, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<AerialiteBar>(15)
                .AddIngredient<RottenMatter>(10)
                .AddIngredient<OsseousRemains>(5)
                .AddIngredient(ItemID.RottenChunk, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, MaxStick, ExplodeRadius);
            
            return false;
        }
    }
}
