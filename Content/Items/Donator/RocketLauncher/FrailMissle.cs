using CalamityEntropy.Content.Items.Donator.RocketLauncher;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher
{
    public class FrailMissle : ModItem
    {
        public static int MaxStick => 3;
        public static int ExplodeRadius => 120;
        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CharredMissleProj>(), BaseMissleProj.AmmoType, singleShotTime: 70, shotVelocity: 20f, hasAutoReuse: true);
            Item.width = 90;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 90;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item61;
            Item.value = Item.buyPrice(silver: 30);
            Item.rare = ItemRarityID.Green;
            Item.Entropy().tooltipStyle = 8;
            Item.Entropy().strokeColor = new Color(40, 0, 0);
            Item.Entropy().NameColor = new Color(160, 0, 0);
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
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, MaxStick, ExplodeRadius);
            p.ToProj().Entropy().applyBuffs.Add(BuffID.OnFire3);
            return false;
        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PurifiedGel>(10)
                .AddIngredient<BloodOrb>(6)
                .AddIngredient<SparkSpreader>()
                .AddTile(TileID.Anvils)
                .Register();
        }

        
    }
}
